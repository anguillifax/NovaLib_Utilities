using Godot;
using System;
using System.Collections.Generic;

namespace Nova.Particles {

	/// <summary>
	/// Custom particle system.
	/// </summary>
	public class ParticleSystem : Node2D {

		public enum EmissionShapeMode {
			Point, Circle, Rectangle
		}

		public enum InitVelocityMode {
			Constant, PointRadial, Radial, CustomHandler
		}

		public class LifetimePair {
			public float current;
			public float max;
			public float Progress => current / max;

			public LifetimePair(float current, float max) {
				this.current = current;
				this.max = max;
			}
		}

		#region Editor Properties

		// General
		[Export] public string _0 { get; set; } = "";
		[Export] public PackedScene ParticlePrefab { get; set; }
		[Export] public bool PoolParticles { get; set; } = true;
		[Export] public int InitialCapacity { get; set; } = 100;
		[Export] public bool RunOnReady { get; set; } = true;
		[Export] public bool PixelSnap { get; set; }
		[Export] public Texture FallbackTexture { get; set; }
		[Export] public Color BaseColor { get; set; } = Colors.White;

		// Emission Shape
		[Export] public string _1 { get; set; } = "";
		[Export] public EmissionShapeMode EmissionShape { get; set; }
		[Export] public float EmissionRadius { get; set; } = 1;
		[Export] public Vector2 EmissionExtents { get; set; } = Vector2.One;

		// Emission Over Time
		[Export] public bool EmitOverTime { get; set; } = true;
		[Export]
		public float ParticlesPerSecond {
			get => 1 / secondsPerParticle;
			set {
				if (value > 0) secondsPerParticle = 1 / value;
			}
		}

		// Emission Over Distance
		[Export] public bool EmitOverDistance { get; set; }
		[Export] public float DistancePerParticle { get; set; } = 1;

		// Burst Emission
		[Export] public bool BurstOnReady { get; set; }
		[Export] public Vector2 SingleBurstAmount { get; set; }
		[Export] public Godot.Collections.Array<Vector2> BurstDelays { get; set; } = new Godot.Collections.Array<Vector2>();
		[Export] public Godot.Collections.Array<Vector2> BurstAmounts { get; set; } = new Godot.Collections.Array<Vector2>();

		// Particle Properties
		[Export] public string _2 { get; set; } = "";
		[Export] public Vector2 InitLifetime { get; set; } = new Vector2(1, 2);
		[Export] public InitVelocityMode InitVelocity { get; set; }
		[Export] public Vector2 InitVelocityParameter { get; set; }
		[Export] public float LinearDamp { get; set; }
		[Export] public float LinearDampTargetFactor { get; set; }
		[Export] public Vector2 Acceleration { get; set; }
		[Export] public Gradient ColorOverLifetime { get; set; }

		#endregion

		#region Data

		public Func<Vector2> CustomInitVelFunc { get; set; }

		// Component lists
		private List<IParticle2D> particles;
		private List<LifetimePair> lifetimes;
		private List<Vector2> positions;
		private List<Color> colors;
		private List<Vector2> velocities;
		private List<Vector2> initVelocities;

		// General Data
		private float deltatime;
		private readonly Stack<IParticle2D> pooledParticles = new Stack<IParticle2D>();
		public bool Emitting { get; private set; }
		private int ParticleCount => particles.Count;

		private float timeAccumulator;
		private float secondsPerParticle = 1;

		private float distanceAccumulator;
		private Vector2 lastPosition;

		private int BurstCount => BurstDelays.Count;
		private float[] burstAccumulators;

		#endregion

		#region Setup

		public override void _Ready() {
			if (ParticlePrefab == null) {
				GD.Print(Name + ": [Error] No particle prefab has been set.");
			}

			particles = new List<IParticle2D>(InitialCapacity);
			lifetimes = new List<LifetimePair>();
			positions = new List<Vector2>(InitialCapacity);
			colors = new List<Color>(InitialCapacity);
			velocities = new List<Vector2>(InitialCapacity);
			initVelocities = new List<Vector2>();

			lastPosition = GlobalPosition;

			InitBursts();

			if (RunOnReady) StartEmitting(BurstOnReady);
		}

		private void InitBursts() {
			// Make input the same length
			if (BurstDelays.Count != BurstAmounts.Count) {
				GD.Print(Name + ": [Warning] Mismatched burst lifetimes and burst counts. Truncating to shortest array.");
				while (BurstDelays.Count > BurstAmounts.Count) BurstDelays.RemoveAt(BurstDelays.Count - 1);
				while (BurstAmounts.Count > BurstDelays.Count) BurstAmounts.RemoveAt(BurstAmounts.Count - 1);
			}

			// Fix illegal delay arguments
			for (int i = 0; i < BurstCount; i++) {
				Vector2 times = BurstDelays[i];
				if (times.y <= 0) times.y = 0.1f;
				if (times.x <= 0) times.x = 0.1f;
				BurstDelays[i] = times;
			}

			// Set initial values in accumulators
			burstAccumulators = new float[BurstCount];
			for (int i = 0; i < BurstCount; i++) {
				burstAccumulators[i] = LerpRandom(BurstDelays[i]);
			}
		}

		#endregion

		#region Start / Stop

		public void StartEmitting(bool doSingleBurst = true) {
			Emitting = true;

			// Spawn immediate burst
			if (doSingleBurst) {
				int count = (int)LerpRandom(SingleBurstAmount);
				for (int i = 0; i < count; i++) CreateParticle();
			}
		}

		public void StopEmitting() {
			Emitting = false;
		}

		#endregion

		#region Core Update

		private void UpdateEmission() {
			// Create particles over time
			if (EmitOverTime) {
				timeAccumulator += deltatime;
				while (timeAccumulator >= secondsPerParticle) {
					CreateParticle();
					timeAccumulator -= secondsPerParticle;
				}
			} else {
				timeAccumulator = 0;
			}

			// Create particles over distance
			if (EmitOverDistance) {
				distanceAccumulator += (GlobalPosition - lastPosition).Length();
				lastPosition = GlobalPosition;
				while (distanceAccumulator > DistancePerParticle) {
					CreateParticle();
					distanceAccumulator -= DistancePerParticle;
				}
			} else {
				distanceAccumulator = 0;
			}

			// Trigger bursts
			for (int i = 0; i < BurstCount; i++) {
				burstAccumulators[i] -= deltatime;
				if (burstAccumulators[i] < 0) {
					int count = (int)LerpRandom(BurstAmounts[i]);
					for (int j = 0; j < count; j++) CreateParticle();
					burstAccumulators[i] += LerpRandom(BurstDelays[i]);
				}
			}
		}

		private void UpdateLifetimes() {
			// Diminish lifetimes
			lifetimes.ForEach(x => x.current += deltatime);

			// Kill particles with no lifetime remaining
			for (int i = 0; i < ParticleCount; i++) {
				if (lifetimes[i].Progress > 1) {
					DestroyParticle(i);
					--i;
				}
			}
		}

		public override void _Process(float delta) {
			deltatime = delta;
			UpdateLifetimes();
			UpdateParticles();
			if (Emitting) UpdateEmission();
		}

		#endregion

		#region Creation / Destruction / Updating

		private void CreateParticle() {

			if (ParticlePrefab == null) return;

			// Create the sprite
			IParticle2D particle;
			if (PoolParticles && pooledParticles.Count > 0) {
				particle = pooledParticles.Pop();
			} else {
				Node n = ParticlePrefab.Instance();
				if (n is IParticle2D p) {
					AddChild(n);
					particle = p;
				} else {
					GD.Print(Name + ": Prefab does not implement IParticle2D");
					return;
				}
			}
			particle.SetTexture(FallbackTexture);
			particles.Add(particle);

			// Set lifetime
			lifetimes.Add(new LifetimePair(0, LerpRandom(InitLifetime)));

			// Set position
			Vector2 position = GlobalPosition;
			switch (EmissionShape) {
				case EmissionShapeMode.Circle:
					position += RandomUtil.InsideCircle(EmissionRadius);
					break;
				case EmissionShapeMode.Rectangle:
					position += RandomUtil.InsideExtents(EmissionExtents);
					break;
			}
			particle.SetPosition(position);
			positions.Add(position);

			// Set color
			Color color = BaseColor;
			if (ColorOverLifetime != null) {
				color = ColorOverLifetime.Interpolate(0);
			}
			particle.SetColor(color);
			colors.Add(color);

			// Set initial velocity
			Vector2 initVel = Vector2.Zero;
			if (InitVelocity == InitVelocityMode.Constant) {
				initVel = InitVelocityParameter;
			} else if (InitVelocity == InitVelocityMode.PointRadial) {
				Vector2 dir = RandomUtil.Unit();
				float pow = LerpRandom(InitVelocityParameter);
				initVel = pow * dir;
			} else if (InitVelocity == InitVelocityMode.Radial) {
				Vector2 delta = position - GlobalPosition;
				Vector2 dir = delta.LengthSquared() == 0 ? Vector2.Up : delta.Normalized();
				float pow = delta.Length() * LerpRandom(InitVelocityParameter);
				initVel = pow * dir;
			} else if (InitVelocity == InitVelocityMode.CustomHandler) {
				if (CustomInitVelFunc != null) initVel = CustomInitVelFunc();
			}
			initVelocities.Add(initVel);
			velocities.Add(initVel);

			// Finalize setup
			particle.OnSpawn();
			particle.SetVisible(true);
		}

		private void DestroyParticle(int index) {
			IParticle2D particle = particles[index];
			particle.OnDestroy();
			if (PoolParticles) {
				pooledParticles.Push(particle);
				particle.SetVisible(false);
			} else {
				particle.Destroy();
			}

			particles.RemoveAt(index);
			lifetimes.RemoveAt(index);
			positions.RemoveAt(index);
			colors.RemoveAt(index);
			velocities.RemoveAt(index);
			initVelocities.RemoveAt(index);
		}

		private void UpdateParticles() {

			// Update positions
			for (int i = 0; i < ParticleCount; i++) {
				Vector2 vel = velocities[i];
				vel = vel.MoveToward(initVelocities[i] * LinearDampTargetFactor, LinearDamp * deltatime);
				vel += Acceleration * deltatime;
				velocities[i] = vel;
				positions[i] += vel * deltatime;
				particles[i].SetPosition(PixelSnap ? positions[i].Round() : positions[i]);
			}

			// Update color
			if (ColorOverLifetime != null) {
				// Use gradient
				for (int i = 0; i < ParticleCount; i++) {
					particles[i].SetColor(ColorOverLifetime.Interpolate(lifetimes[i].Progress));
				}
			} else {
				// Apply base color
				for (int i = 0; i < ParticleCount; i++) {
					particles[i].SetColor(colors[i]);
				}
			}

			// Raise update event
			for (int i = 0; i < ParticleCount; i++) {
				particles[i].OnUpdate(lifetimes[i].current, lifetimes[i].max);
			}
		}

		#endregion

		#region Utility

		private float LerpRandom(Vector2 v) {
			return Mathf.Lerp(v.x, v.y, RandomUtil.Float01());
		}

		#endregion

	}

}