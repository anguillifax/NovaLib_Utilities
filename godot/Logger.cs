using Godot;

namespace Nova {

	/// <summary>
	/// General purpose wrapper for logging. Prepends time of messages.
	/// </summary>
	public static class Logger {

		private const string IndentTimeFormat = "[{0,8:f2}s] ";
		private static string CurTimeString { get; set; }
		private static string CurSplitString { get; set; }

		/// <summary>
		/// Controls whether or not messages are written to the console. Use SetEnabled() to change states.
		/// </summary>
		public static bool Enabled { get; private set; } = true;

		/// <summary>
		/// Enable or disable the writing of text to the console.
		/// </summary>
		public static void SetEnabled(bool enabled) {
			Enabled = enabled;
			if (Enabled) {
				GD.Print("Enabled logging.");
			} else {
				GD.Print("Disabled logging.");
			}
		}

		/// <summary>
		/// Print a formatted string to the console.
		/// </summary>
		public static void PrintFormat(string format, params object[] args) {
			Write(string.Format(format, args));
		}

		/// <summary>
		/// Print a string representation of an object to the console.
		/// </summary>
		public static void Print(object message) {
			Write(message.ToString());
		}

		/// <summary>
		/// Print an empty line.
		/// </summary>
		public static void Print() {
			if (!Enabled) return;
			GD.Print(' ');
		}

		private static void Write(string msg) {
			if (!Enabled) return;
			string m = CurTimeString + msg.Replace("\n", CurSplitString);
			GD.Print(m);
		}

		/// <summary>
		/// Set the current time of all subsequent logs. Call at the satrt of every update period.
		/// </summary>
		public static void Timestamp() {
			CurTimeString = string.Format(IndentTimeFormat, Time.EngineTime);
			CurSplitString = "\n" + new string(' ', CurTimeString.Length);
		}

	}

}