using Godot;
using Nova;
using System;
using System.Collections.Generic;

namespace Nova {

	internal class DebugDrawNode : Node {

		private static List<(Line2D line, float time)> AllLines { get; } = new List<(Line2D line, float time)>();

		public override void _Ready() {
			DebugDraw.Node = this;
		}

		public override void _Process(float delta) {

			for (int i = 0; i < AllLines.Count; i++) {
				if (AllLines[i].time < Time.EngineTime) {
					AllLines[i].line.QueueFree();
					AllLines.RemoveAt(i);
					--i;
				}
			}

		}

		internal void Add(Line2D line, float duration) {
			DebugDraw.SpawnTarget.AddChild(line);
			AllLines.Add((line, Time.EngineTime + duration));
		}

	}

	/// <summary>
	/// Quickly draw debug information.
	/// </summary>
	public static class DebugDraw {

		internal static DebugDrawNode Node { get; set; }

		/// <summary>
		/// Node to spawn lines under.
		/// </summary>
		public static Node SpawnTarget { get; set; }

		/// <summary>
		/// Draw a line between two points in global space.
		/// </summary>
		public static void DrawLine(Vector2 pos1, Vector2 pos2, Color color, float duration) {
			if (CheckSetup()) return;

			Line2D line = new Line2D {
				Points = new Vector2[] { pos1, pos2 },
				DefaultColor = color,
				Width = .5f
			};
			line.SetAsToplevel(true);

			Node.Add(line, duration);
		}

		/// <summary>
		/// Draw a ray in global space.
		/// </summary>
		public static void DrawRay(Vector2 origin, Vector2 direction, Color color, float duration) {
			DrawLine(origin, origin + direction, color, duration);
		}

		/// <summary>
		/// Check if the class has been fully set up. Returns true if drawing code cannot proceed.
		/// </summary>
		private static bool CheckSetup() {
			if (Node == null || SpawnTarget == null) {
				GD.PrintErr("[ERROR] DebugDraw has not been fully configured.");
				return true;
			}
			return false;
		}

	}

}