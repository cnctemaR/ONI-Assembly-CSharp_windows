using System;
using UnityEngine;

public struct GravityComponent
{
	public GravityComponent(Transform transform, global::System.Action on_landed)
	{
		this = new GravityComponent(transform, on_landed, Vector2.zero);
	}

	public GravityComponent(Transform transform, global::System.Action on_landed, Vector2 initial_velocity)
	{
		this.transform = transform;
		this.fallStartTime = Time.time;
		this.initialVelocity = initial_velocity;
		this.onLanded = on_landed;
		Collider2D component = transform.GetComponent<Collider2D>();
		if (component != null)
		{
			this.offset = transform.position.y - component.bounds.min.y;
		}
		else
		{
			this.offset = 0f;
		}
	}

	public Vector2 Velocity
	{
		get
		{
			float num = Time.time - this.fallStartTime;
			Vector2 vector = new Vector2(this.initialVelocity.x, this.initialVelocity.y + GravityComponent.Acceleration * num);
			return vector;
		}
	}

	public void FixedUpdate(float dt)
	{
		Vector2 velocity = this.Velocity;
		Vector3 position = this.transform.position;
		Vector2 vector = position;
		Vector2 vector2 = vector + velocity * dt;
		this.transform.SetPosition(new Vector3(vector2.x, vector2.y, position.z));
		Vector2 vector3 = vector2;
		vector3.y -= this.offset;
		vector.y -= this.offset;
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
				vector2.y = Grid.CellToPosCBC(Grid.CellAbove(num), Grid.SceneLayer.Move).y + this.offset;
				this.initialVelocity.x = 0f;
				this.transform.SetPosition(new Vector3(vector2.x, vector2.y, position.z));
				EventSystem.Trigger(this.transform.gameObject, 1188683690, velocity);
				if (this.onLanded != null)
				{
					this.onLanded();
				}
			}
		}
	}

	private static float Acceleration = -9.8f;

	private Transform transform;

	private float fallStartTime;

	private Vector2 initialVelocity;

	private float offset;

	private global::System.Action onLanded;
}
