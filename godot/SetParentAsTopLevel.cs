using Godot;

namespace Nova {

	public class SetParentAsTopLevel : Node {

		public override void _Ready() {
			if (GetParent() is Node2D n2d) {
				n2d.SetAsToplevel(true);
			}
		}

	}

}