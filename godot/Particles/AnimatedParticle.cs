using Godot;

namespace Nova.Particles {

	public class AnimatedParticle : AnimatedSprite, IParticle2D {

		[Export] public PackedScene Offspring { get; set; }
		[Export] public string OffspringSpawnPath { get; set; } = "..";

		public override void _Ready() {
			SetAsToplevel(true);
		}

		void IParticle2D.OnSpawn() { }

		void IParticle2D.OnUpdate(float current, float max) { }

		void IParticle2D.OnDestroy() {
			if (Offspring != null) {
				Node container = GetNodeOrNull(OffspringSpawnPath);
				if (container == null) {
					GD.Print(Name + ": [Error] Invalid offspring spawn path.");
				} else {
					Node offspring = Offspring.Instance();
					container.AddChild(offspring);
					if (offspring is Node2D n2d) {
						n2d.GlobalPosition = GlobalPosition;
					}
					offspring.CallOnSpawn();
				}
			}
		}

		void IParticle2D.SetTexture(Texture texture) { }

		void IParticle2D.SetColor(Color color) => SelfModulate = color;

		void IParticle2D.SetPosition(Vector2 position) => GlobalPosition = position;

		void IParticle2D.SetRotation(float degrees) => RotationDegrees = degrees;

		void IParticle2D.SetVisible(bool visible) => Visible = visible;

		void IParticle2D.Destroy() => QueueFree();

	}

}