using Godot;

namespace Nova.Camera {

	public class QuakeShake : Node {

		public static float GlobalScreenshakeFactor => 1f;

		/// <summary>
		/// The output offset from the system.
		/// </summary>
		public Vector2 Offset { get; private set; }

		/// <summary>
		/// The output rotation from the system.
		/// </summary>
		public float Angle { get; private set; }

		[Export] public readonly float GlobalUnitScale = 5f;

		[Export] public readonly OpenSimplexNoise Noise;
		[Export] public readonly float TraumaDecreaseSpeed = 0.5f;
		[Export] public readonly float StopDecreaseSpeed = 3f;
		[Export] public readonly float MaxAngleExtents = 20f;

		/// <summary>
		/// Current "trauma" value of the system. Directly influences strength of shake.
		/// </summary>
		public float Trauma {
			get => _trauma;
			private set => _trauma = Mathf.Clamp(value, 0, 1);
		}
		private float _trauma;

		private float CurrentPower => GlobalScreenshakeFactor * Mathf.Pow(Trauma, 2f);

		private bool stop;

		public void Update(float delta) {

			Trauma -= TraumaDecreaseSpeed * delta;
			if (stop) Trauma -= StopDecreaseSpeed * delta;

			if (Trauma > 0) {

				Vector2 offset = Vector2.Zero;
				offset.x = GetNoise(0f);
				offset.y = GetNoise(10f);
				offset *= CurrentPower * GlobalUnitScale;
				Offset = offset;

				//Logger.Print("Offset: " + Offset.ToFixedString());

				float angle = GetNoise(20f);
				angle *= CurrentPower * MaxAngleExtents;
				Angle = angle;

				//Logger.Print("Angle: " + Angle.ToString("f2"));

			}

		}

		private float GetNoise(float offset) {
			return Noise.GetNoise1d(Time.TotalTime + offset);
		}

		public void AddTrauma(float amount) {
			stop = false;
			Trauma += amount;
		}

		public void Stop() {
			stop = true;
		}

	}

}