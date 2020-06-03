using Godot;
using System;
using System.Collections.Generic;

namespace Nova {

	/// <summary>
	/// Handy general functions for mathematic calculations.
	/// </summary>
	public static class Calc {

		public const float Deg2Rad = Mathf.Pi / 180f;
		public const float Rad2Deg = 180f / Mathf.Pi;

		/// <summary>
		/// Return an equivalent angle between (-180, 180].
		/// </summary>
		public static float SimplifyAngle(float angle) {
			while (angle > 180) angle -= 360;
			while (angle <= -180) angle += 360;
			return angle;
		}

		/// <summary>
		/// Move toward the nearest angle, wrapping around as necessary.
		/// </summary>
		public static float MoveTowardDegrees(float from, float to, float delta) {
			float dir = SimplifyAngle(to - from);
			float maxDelta = Math.Min(Math.Abs(dir), Math.Abs(delta));
			if (dir >= 0) {
				from += maxDelta;
			} else {
				from -= maxDelta;
			}
			return SimplifyAngle(from);
		}

		/// <summary>
		/// Round a number to the nearest boundary.
		/// </summary>
		public static float Round(float amount, float boundary) {
			return boundary * Mathf.Round(amount / boundary);
		}

		/// <summary>
		/// Round to an integer boundary.
		/// </summary>
		public static int RoundToInt(float amount, int boundary) {
			return (int)(boundary * Mathf.Round(amount / boundary));
		}

		/// <summary>
		/// Calculate integer direction from raw value.
		/// </summary>
		public static int AsIntDirection(float raw, float deadzone = 0.01f) {
			if (raw < -deadzone) {
				return -1;
			} else if (raw > deadzone) {
				return 1;
			} else {
				return 0;
			}
		}

	}

}