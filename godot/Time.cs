using Godot;
using Nova;
using System;
using System.Collections.Generic;

namespace Nova {

	/// <summary>
	/// Global-level timing information.
	/// </summary>
	public static class Time {

		/// <summary>
		/// Get the current exact time of the engine in seconds.
		/// </summary>
		public static float EngineTime => OS.GetTicksMsec() / 1000f;

		/// <summary>
		/// Get the total time of the process loop, accounting for pauses.
		/// </summary>
		public static float TotalTime => (float)_totalTime;
		private static double _totalTime;

		/// <summary>
		/// Get the total time of the physics process loop, accounting for pauses.
		/// </summary>
		public static float TotalTimePhysics => (float)_totalTimePhysics;
		private static double _totalTimePhysics;

		/// <summary>
		/// Call this once every _Process()
		/// </summary>
		internal static void UpdateTimeProcess(float delta) => _totalTime += delta;

		/// <summary>
		/// Call this once every _PhysicsProcess()
		/// </summary>
		internal static void UpdateTimeProcessPhysics(float delta) => _totalTimePhysics += delta;

	}

}