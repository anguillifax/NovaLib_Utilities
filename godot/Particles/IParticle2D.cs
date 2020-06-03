using Godot;

namespace Nova.Particles {

	/// <summary>
	/// Interface for all particles manipulated by the particle system.
	/// </summary>
	public interface IParticle2D {

		void SetTexture(Texture texture);
		void SetColor(Color color);
		void SetPosition(Vector2 position);
		void SetRotation(float degrees);
		void SetVisible(bool visible);

		void OnSpawn();
		void OnUpdate(float currentLifetime, float maxLifetime);
		void OnDestroy();
		void Destroy();
	}

}