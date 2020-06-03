using Godot;
using System;
using System.Collections.Generic;

namespace Nova {

	/// <summary>
	/// Draws a line behind the node that disappears over time.
	/// </summary>
	public class TrailLine2D : Line2D {

		[Export] public Node2D Target { get; set; }

		/// <summary>
		/// New points are continuously added while Emitting is true.
		/// <para>If Emitting is set to true after being disabled, the last trail will be cleared.</para>
		/// </summary>
		[Export]
		public bool Emitting {
			get => _emitting;
			set {
				if (!_emitting && value) {
					trail.Clear();
					trailTimes.Clear();
				}
				_emitting = value;
			}
		}
		private bool _emitting;

		[Export] public readonly float PointDistance = 2f;
		[Export] public readonly float FadeTime = 5f;
		[Export] public readonly int BaseCapacity = 100;

		private readonly Queue<float> trailTimes;
		private readonly Queue<Vector2> trail;
		private Vector2 lastPosition;

		public TrailLine2D() {
			trailTimes = new Queue<float>(BaseCapacity);
			trail = new Queue<Vector2>(BaseCapacity);
		}

		public override void _Ready() {
			if (Target == null) {
				if (GetParent() is Node2D n) {
					Target = n;
				} else {
					throw new Exception($"[{Name}] No assigned target. Could not fall back parent Node2D.");
				}
			}
			SetAsToplevel(true);
		}

		public override void _Process(float delta) {

			bool dirty = false;

			// Remove points older than fade time
			while (trail.Count > 0 && Time.TotalTime - trailTimes.Peek() > FadeTime) {
				trailTimes.Dequeue();
				trail.Dequeue();
				dirty = true;
			}

			if (Emitting) {

				// Add new points when player has moved significantly
				if (Target.GlobalPosition.DistanceSquaredTo(lastPosition) > PointDistance * PointDistance) {
					lastPosition = Target.GlobalPosition;
					trail.Enqueue(Target.GlobalPosition);
					trailTimes.Enqueue(Time.TotalTime);
				}

				// Append current exact position and write points to Line2D
				Vector2[] points = new Vector2[trail.Count + 1];
				trail.CopyTo(points, 0);
				points[points.Length - 1] = Target.GlobalPosition;
				Points = points;

			} else if (dirty) {

				// If emission is disabled, only update any removed points
				Points = trail.ToArray();

			}

		}

	}

}