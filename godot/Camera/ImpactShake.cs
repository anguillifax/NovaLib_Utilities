using Godot;

namespace Nova.Camera {

	/// <summary>
	/// Tool for impact shakes.
	/// <para>All values are based in the range [0, 1] then scaled to the output strength.</para>
	/// </summary>
	public class ImpactShake : Node {

		public static float GlobalScreenshakeFactor => 1f;

		[Export] public readonly float GlobalUnitScale = 5f;

		[Export] public readonly Curve DampingCurve;

		[Export] public readonly float Mag2DurScale = 0.4f; // 'a' term in ax^n + b.
		[Export] public readonly float Mag2DurPower = 0.5f; // 'n' term in ax^n + b.
		[Export] public readonly float Mag2DurIntercept = 0.21f; // 'b' term in ax^n + b.

		[Export] public readonly float SineCycles = 2;
		[Export] public readonly float StopRecenterSpeed = 3f;


		/// <summary>
		/// The output offset from the system.
		/// </summary>
		public Vector2 Offset { get; private set; }


		private readonly SimpleTimer timer = new SimpleTimer();
		private Vector2 direction;
		private float magnitude;
		private bool stopFlag;


		/// <summary>
		/// Start a new impact shake with given magnitude and direction.
		/// </summary>
		public void Start(float mag, Vector2 dir) {
			if (GlobalScreenshakeFactor == 0) return;

			if (mag < 0) {
				dir = -dir;
				mag = -mag;
			}

			magnitude = GlobalScreenshakeFactor * mag;
			direction = dir;

			stopFlag = false;
			timer.Duration = Mag2DurScale * Mathf.Pow(magnitude, Mag2DurPower) + Mag2DurIntercept;

			timer.Start();
		}

		/// <summary>
		/// Calculate the new shake offsets. Call once per frame.
		/// </summary>
		public void Update(float delta) {

			if (!stopFlag && GlobalScreenshakeFactor > 0) {

				timer.Update(delta);

				if (timer.Running && SineCycles > 0) {
					float power = magnitude * DampingCurve.InterpolateBaked(timer.Progress) * Mathf.Sin(timer.Progress * 2 * Mathf.Pi * SineCycles);
					Offset =  GlobalUnitScale * direction * power;
				} else {
					Offset = Vector2.Zero;
				}

			} else {
				Offset = Offset.MoveToward(Vector2.Zero, StopRecenterSpeed * GlobalUnitScale * delta);
			}

		}

		/// <summary>
		/// Quickly suppress any remaining shake.
		/// </summary>
		public void Stop() {
			stopFlag = true;
		}

	}

}