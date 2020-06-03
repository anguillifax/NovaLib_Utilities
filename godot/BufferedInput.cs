using Godot;

namespace Nova {

	/// <summary>
	/// Buffer an input down event.
	/// </summary>
	public class BufferedInput {

		private uint timestamp;

		/// <summary>
		/// Create a new buffer starting in the cleared state.
		/// </summary>
		public BufferedInput() {
			Clear();
		}

		/// <summary>
		/// Set the buffer to the current time.
		/// </summary>
		public void Set() => timestamp = OS.GetTicksMsec();

		/// <summary>
		/// Check if buffer is in given window. Does not consume.
		/// </summary>
		public bool Get(float maxBuffer) => (OS.GetTicksMsec() - timestamp) / 1000f < maxBuffer;

		/// <summary>
		/// Consume the buffer.
		/// </summary>
		public void Clear() => timestamp = uint.MaxValue;

	}

}