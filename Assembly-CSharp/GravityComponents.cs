using System;
using FMODUnity;
using UnityEngine;

public class GravityComponents : KGameObjectComponentManager<GravityComponent>
{
	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity, global::System.Action on_landed = null)
	{
		bool flag = false;
		KPrefabID component = go.GetComponent<KPrefabID>();
		if (component != null)
		{
			flag = component.HasAnyTags(GravityComponents.LANDS_ON_FAKEFLOOR);
		}
		bool flag2 = go.GetComponent<MinionIdentity>() != null;
		return base.Add(go, new GravityComponent(go.transform, on_landed, initial_velocity, flag, flag2));
	}

	public override void FixedUpdate(float dt)
	{
		GravityComponents.Tuning tuning = TuningData<GravityComponents.Tuning>.Get();
		float num = tuning.maxVelocity * tuning.maxVelocity;
		for (int i = 0; i < this.data.Count; i++)
		{
			GravityComponent gravityComponent = this.data[i];
			if (gravityComponent.elapsedTime >= 0f && !(gravityComponent.transform == null) && !base.IsInCleanupList(gravityComponent.transform.gameObject))
			{
				Vector3 position = gravityComponent.transform.GetPosition();
				Vector2 vector = position;
				Vector2 vector2 = new Vector2(gravityComponent.velocity.x, gravityComponent.velocity.y + -9.8f * dt);
				float sqrMagnitude = vector2.sqrMagnitude;
				if (sqrMagnitude > num)
				{
					vector2 *= tuning.maxVelocity / Mathf.Sqrt(sqrMagnitude);
				}
				int num2 = Grid.PosToCell(vector);
				bool flag = Grid.IsVisiblyInLiquid(vector - new Vector2(0f, gravityComponent.bottomYOffset));
				if (flag)
				{
					flag = true;
					float num3 = (float)(gravityComponent.transform.GetInstanceID() % 1000) / 1000f * 0.25f;
					float num4 = tuning.maxVelocityInLiquid + num3 * tuning.maxVelocityInLiquid;
					if (sqrMagnitude > num4 * num4)
					{
						float num5 = Mathf.Sqrt(sqrMagnitude);
						vector2 = vector2 / num5 * Mathf.Lerp(num5, num3, dt * (5f + 5f * num3));
					}
				}
				gravityComponent.velocity = vector2;
				gravityComponent.elapsedTime += dt;
				Vector2 vector3 = vector + vector2 * dt;
				Vector2 vector4 = vector3;
				vector4.y = vector3.y - gravityComponent.bottomYOffset;
				bool flag2 = Grid.IsVisiblyInLiquid(vector3 + new Vector2(0f, gravityComponent.bottomYOffset));
				if (!flag && flag2)
				{
					KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("splash_step_kanim", new Vector3(vector3.x, vector3.y, 0f) + new Vector3(-0.38f, 0.75f, -0.1f), null, false, Grid.SceneLayer.FXFront, false);
					kbatchedAnimController.Play("fx1", KAnim.PlayMode.Once, 1f, 0f);
					kbatchedAnimController.destroyOnAnimComplete = true;
				}
				bool flag3 = false;
				int num6 = Grid.PosToCell(vector4);
				if (Grid.IsValidCell(num6))
				{
					if (vector2.sqrMagnitude > 0.2f && Grid.IsValidCell(num2) && !Grid.Element[num2].IsLiquid && Grid.Element[num6].IsLiquid)
					{
						AmbienceType ambience = Grid.Element[num6].substance.GetAmbience();
						if (ambience != AmbienceType.None)
						{
							EventReference eventReference = Sounds.Instance.OreSplashSoundsMigrated[(int)ambience];
							if (CameraController.Instance != null && CameraController.Instance.IsAudibleSound(vector3, eventReference))
							{
								SoundEvent.PlayOneShot(eventReference, vector3, 1f);
							}
						}
					}
					bool flag4 = Grid.Solid[num6];
					if (!flag4 && gravityComponent.landOnFakeFloors && Grid.FakeFloor[num6])
					{
						Navigator component = gravityComponent.transform.GetComponent<Navigator>();
						if (component)
						{
							flag4 = component.NavGrid.NavTable.IsValid(num6, NavType.Floor);
							if (!flag4)
							{
								int num7 = Grid.CellAbove(num6);
								flag4 = component.NavGrid.NavTable.IsValid(num7, NavType.Hover);
							}
						}
					}
					if (flag4)
					{
						Vector3 vector5 = Grid.CellToPosCBC(Grid.CellAbove(num6), Grid.SceneLayer.Move);
						vector3.y = vector5.y + gravityComponent.bottomYOffset;
						gravityComponent.velocity.x = 0f;
						flag3 = true;
					}
					else
					{
						Vector2 vector6 = vector3;
						vector6.x -= gravityComponent.extents.x;
						int num8 = Grid.PosToCell(vector6);
						if (Grid.IsValidCell(num8) && Grid.Solid[num8])
						{
							vector3.x = Mathf.Floor(vector3.x - gravityComponent.extents.x) + (1f + gravityComponent.extents.x);
							gravityComponent.velocity.x = -0.1f * gravityComponent.velocity.x;
						}
						else
						{
							Vector3 vector7 = vector3;
							vector7.x += gravityComponent.extents.x;
							int num9 = Grid.PosToCell(vector7);
							if (Grid.IsValidCell(num9) && Grid.Solid[num9])
							{
								vector3.x = Mathf.Floor(vector3.x + gravityComponent.extents.x) - gravityComponent.extents.x;
								gravityComponent.velocity.x = -0.1f * gravityComponent.velocity.x;
							}
						}
					}
				}
				this.data[i] = gravityComponent;
				int num10 = Grid.PosToCell(vector3);
				if (gravityComponent.mayLeaveWorld || !Grid.IsValidCell(num2) || Grid.WorldIdx[num2] == 255 || Grid.IsValidCellInWorld(num10, (int)Grid.WorldIdx[num2]))
				{
					gravityComponent.transform.SetPosition(new Vector3(vector3.x, vector3.y, position.z));
					if (flag3)
					{
						gravityComponent.transform.gameObject.Trigger(1188683690, vector2);
						if (gravityComponent.onLanded != null)
						{
							gravityComponent.onLanded();
						}
					}
				}
			}
		}
	}

	private const float Acceleration = -9.8f;

	private static Tag[] LANDS_ON_FAKEFLOOR = new Tag[]
	{
		GameTags.Minion,
		GameTags.Creatures.Walker,
		GameTags.Creatures.Hoverer
	};

	public class Tuning : TuningData<GravityComponents.Tuning>
	{
		public float maxVelocity;

		public float maxVelocityInLiquid;
	}
}
