using Godot;
using System;
using System.Collections.Generic;

namespace Nova {

	/// <summary>
	/// General purpose utility functions.
	/// </summary>
	public static class Util {

		/// <summary>
		/// Convert a Vector2 into a Vector3
		/// </summary>
		public static Vector3 ToVector3(this Vector2 vec2, float z = 0) => new Vector3(vec2.x, vec2.y, z);

		/// <summary>
		/// Check if the key event is the current key.
		/// </summary>
		public static bool IsKey(this InputEventKey keyEvent, KeyList key) => keyEvent.Scancode == (uint)key;


		#region Godot Collection to Standard Collection Conversions

		/// <summary>
		/// Convert a Godot array to a standard array.
		/// </summary>
		public static T[] ToArray<T>(this Godot.Collections.Array<T> array) {
			T[] arr = new T[array.Count];
			array.CopyTo(arr, 0);
			return arr;

		}

		/// <summary>
		/// Convert a Godot array to a standard list.
		/// </summary>
		public static List<T> ToList<T>(this Godot.Collections.Array<T> array) {
			return new List<T>(array);
		}

		/// <summary>
		/// Convert a Godot dictionary into a standard dictionary.
		/// </summary>
		public static Dictionary<TKey, TValue> ToDict<TKey, TValue>(this Godot.Collections.Dictionary<TKey, TValue> dict) {
			return new Dictionary<TKey, TValue>(dict);
		}

		#endregion

		#region Node Utilities

		/// <summary>
		/// Call the OnSpawn() method on child nodes.
		/// </summary>
		public static void CallOnSpawn(this Node node) => node.PropagateCall("OnSpawn");

		/// <summary>
		/// Search the scene tree for a single node with the given group. Function throws exception if node cannot be found / cast to given type.
		/// </summary>
		public static T GetNodeInGroup<T>(this SceneTree tree, string name) {
			try {
				return (T)tree.GetNodesInGroup(name)[0];
			} catch (InvalidCastException) {
				GD.PrintErr($"Node '{name}' could not be cast to type {typeof(T).Name} found.");
				throw;
			} catch (IndexOutOfRangeException) {
				GD.PrintErr($"Node '{name}' could not be found.");
				throw;
			}
		}

		/// <summary>
		/// Searches the scene tree for a single node with the given group.
		/// </summary>
		public static Node GetNodeInGroup(this SceneTree tree, string name) {
			try {
				return (Node)tree.GetNodesInGroup(name)[0];
			} catch (IndexOutOfRangeException) {
				GD.PrintErr($"Node '{name}' could not be found.");
				throw;
			}
		}

		#endregion

		#region Color

		/// <summary>
		/// Set the alpha value of a color.
		/// </summary>
		public static Color SetAlpha(Color color, float alpha) {
			color.a = alpha;
			return color;
		}

		/// <summary>
		/// Set the alpha value of a color.
		/// </summary>
		public static void SetAlpha(ref Color color, float alpha) {
			color.a = alpha;
		}

		#endregion

		#region File Handling

		/// <summary>
		/// Enumerate through all objects in a directory.
		/// </summary>
		public static IEnumerable<string> EnumerateDirectory(string rootpath) {
			List<string> ret = new List<string>();
			var dir = new Godot.Directory();
			dir.Open(rootpath);
			dir.ListDirBegin();
			for (string path = dir.GetNext(); !string.IsNullOrEmpty(path); path = dir.GetNext()) {
				if (path[0] != '.') {
					ret.Add(dir.GetCurrentDir() + "/" + path);
				}
			}
			dir.ListDirEnd();
			return ret;
		}

		#endregion

	}

}