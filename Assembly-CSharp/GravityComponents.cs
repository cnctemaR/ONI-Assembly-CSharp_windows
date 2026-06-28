using System;
using UnityEngine;

public class GravityComponents : KGameObjectComponentManager<GravityComponent>
{
	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity, global::System.Action on_landed = null)
	{
		return base.Add(go, new GravityComponent(go.transform, on_landed, initial_velocity));
	}

	public override void FixedUpdate(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			GravityComponent gravityComponent = this.data[i];
			if (gravityComponent.elapsedTime >= 0f)
			{
				Vector2 velocity = gravityComponent.Velocity;
				Vector3 position = gravityComponent.transform.GetPosition();
				Vector2 vector = position;
				Vector2 vector2 = vector + velocity * dt;
				Vector2 vector3 = vector2;
				vector3.y -= gravityComponent.offset;
				int num = Grid.PosToCell(vector3);
				if (Grid.IsValidCell(num))
				{
					if (velocity.sqrMagnitude > 0.2f)
					{
						int num2 = Grid.PosToCell(vector);
						if (!Grid.Element[num2].IsLiquid && Grid.Element[num].IsLiquid)
						{
							AmbienceType ambience = Grid.Element[num].substance.GetAmbience();
							if (ambience != AmbienceType.None)
							{
								string text = Sounds.Instance.OreSplashSoundsMigrated[(int)ambience];
								if (CameraController.Instance != null && CameraController.Instance.IsAudibleSound(vector2, text))
								{
									SoundEvent.PlayOneShot(text, vector2);
								}
							}
						}
					}
					if (Grid.Solid[num])
					{
						vector2.y = Grid.CellToPosCBC(Grid.CellAbove(num), Grid.SceneLayer.Move).y + gravityComponent.offset;
						gravityComponent.initialVelocity.x = 0f;
						gravityComponent.elapsedTime = -1f;
						gravityComponent.transform.SetPosition(new Vector3(vector2.x, vector2.y, position.z));
						this.data[i] = gravityComponent;
						EventSystem.Trigger(gravityComponent.transform.gameObject, 1188683690, velocity);
						if (gravityComponent.onLanded != null)
						{
							gravityComponent.onLanded();
						}
					}
					else
					{
						gravityComponent.transform.SetPosition(new Vector3(vector2.x, vector2.y, position.z));
						gravityComponent.elapsedTime += dt;
						this.data[i] = gravityComponent;
					}
				}
			}
		}
	}
}
