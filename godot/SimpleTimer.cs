using Godot;

namespace Nova {

	/// <summary>
	/// Represents a simple countdown timer that is updated manually.
	/// </summary>
	public class SimpleTimer {

		/// <summary>
		/// Amount of time remaining in the timer. Once it reaches 0, the timer is finished.
		/// </summary>
		public float Current { get; set; }

		/// <summary>
		/// Total time to count down from.
		/// </summary>
		public float Duration { get; set; }

		/// <summary>
		/// True if timer has completed.
		/// </summary>
		public bool Done => Current <= 0;

		/// <summary>
		/// True if timer is still counting down.
		/// </summary>
		public bool Running => Current > 0;

		/// <summary>
		/// A normalized value from [0, 1] of the time remaining. Starts at 0 and ends at 1.
		/// <para>If Duration is 0, this function throws a division by 0 error.</para>
		/// </summary>
		public float Progress => 1 - Mathf.Clamp(Current / Duration, 0, 1);

		/// <summary>
		/// Create a new timer. Starts with running property true.
		/// </summary>
		public SimpleTimer(float duration = 0) {
			Duration = duration;
			Current = Duration;
		}

		//public SimpleTimer(SimpleTimer other) :
		//	this(other.Duration) {
		//	if (ReferenceEquals(other, this)) GD.Print("Warning, creating SimpleTimer from copy of self");
		//}

		/// <summary>
		/// Reset the time remaining back to full.
		/// </summary>
		public void Start() {
			Current = Duration;
		}

		/// <summary>
		/// Immediately mark the timer as complete.
		/// </summary>
		public void Stop() {
			Current = 0;
		}

		/// <summary>
		/// Updates the amount of time remaining in the timer. Call repeatedly once every update period.
		/// </summary>
		public void Update(float delta) {
			if (Current >= 0) Current -= delta;
		}

	}

}