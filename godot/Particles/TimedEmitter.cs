using Godot;

namespace Nova.Particles {

	/// <summary>
	/// Emits particles for a given duration before disabling emission then destroying node.
	/// </summary>
	public class TimedEmitter : Node2D {

		[Export] public int ParticleSystemIndex { get; set; }

		[Export]
		public float EmitDuration {
			get => emit.Duration;
			set => emit.Duration = value;
		}
		[Export]
		public float DestroyTime {
			get => destroy.Duration;
			set => destroy.Duration = value;
		}

		[Export] public bool EmitterAsTopLevel { get; set; } = true;

		private readonly SimpleTimer emit = new SimpleTimer();
		private readonly SimpleTimer destroy = new SimpleTimer();
		private bool hasEmit;
		private bool hasDestroy;
		private ParticleSystem system;

		public override void _Ready() {
			emit.Start();
			destroy.Start();
			system = GetChildOrNull<ParticleSystem>(ParticleSystemIndex);
			system?.SetAsToplevel(EmitterAsTopLevel);
		}

		public void OnSpawn() {
			if (system != null) {
				system.GlobalPosition = GlobalPosition;
				system.StartEmitting();
			}
		}

		public override void _Process(float delta) {
			emit.Update(delta);
			if (!hasEmit && emit.Done) {
				system?.StopEmitting();
				hasEmit = true;
			}

			destroy.Update(delta);
			if (!hasDestroy && destroy.Done) {
				hasDestroy = true;
				QueueFree();
			}
		}

	}

}