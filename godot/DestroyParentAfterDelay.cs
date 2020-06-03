using Godot;

namespace Nova {

	public class DestroyParentAfterDelay : Node {

		[Export] public float Delay { get; set; } = 1;

		public override void _Ready() {
			Timer t = new Timer();
			AddChild(t);
			t.OneShot = true;
			t.Connect("timeout", GetParent(), "queue_free");
			t.Start(Delay);
		}

	}

}