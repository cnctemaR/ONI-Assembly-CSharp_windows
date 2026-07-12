using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocket : StateMachineComponent<LaunchableRocket.StatesInstance>, ILaunchableRocket
{
	public LaunchableRocketRegisterType registerType
	{
		get
		{
			return LaunchableRocketRegisterType.Spacecraft;
		}
	}

	public GameObject LaunchableGameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	public float rocketSpeed { get; private set; }

	public bool isLanding { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.master.parts = AttachableBuilding.GetAttachedNetwork(base.smi.master.GetComponent<AttachableBuilding>());
		if (SpacecraftManager.instance.GetSpacecraftID(this) == -1)
		{
			Spacecraft spacecraft = new Spacecraft(base.GetComponent<LaunchConditionManager>());
			spacecraft.GenerateName();
			SpacecraftManager.instance.RegisterSpacecraft(spacecraft);
			base.gameObject.AddOrGet<RocketLaunchConditionVisualizerEffect>();
		}
		base.smi.StartSM();
	}

	public List<GameObject> GetEngines()
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in this.parts)
		{
			if (gameObject.GetComponent<RocketEngine>())
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	protected override void OnCleanUp()
	{
		SpacecraftManager.instance.UnregisterSpacecraft(base.GetComponent<LaunchConditionManager>());
		base.OnCleanUp();
	}

	public List<GameObject> parts = new List<GameObject>();

	[Serialize]
	private int takeOffLocation;

	[Serialize]
	private float flightAnimOffset;

	private GameObject soundSpeakerObject;

	public class StatesInstance : GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.GameInstance
	{
		public StatesInstance(LaunchableRocket master)
			: base(master)
		{
		}

		public bool IsMissionState(Spacecraft.MissionState state)
		{
			return SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).state == state;
		}

		public void SetMissionState(Spacecraft.MissionState state)
		{
			SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>()).SetState(state);
		}
	}

	public class States : GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.grounded.ToggleTag(GameTags.RocketOnGround).Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				foreach (GameObject gameObject in smi.master.parts)
				{
					if (!(gameObject == null))
					{
						gameObject.AddTag(GameTags.RocketOnGround);
					}
				}
			}).Exit(delegate(LaunchableRocket.StatesInstance smi)
			{
				foreach (GameObject gameObject2 in smi.master.parts)
				{
					if (!(gameObject2 == null))
					{
						gameObject2.RemoveTag(GameTags.RocketOnGround);
					}
				}
			})
				.EventTransition(GameHashes.DoLaunchRocket, this.not_grounded.launch_pre, null)
				.Enter(delegate(LaunchableRocket.StatesInstance smi)
				{
					smi.master.rocketSpeed = 0f;
					foreach (GameObject gameObject3 in smi.master.parts)
					{
						if (!(gameObject3 == null))
						{
							gameObject3.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
						}
					}
					smi.SetMissionState(Spacecraft.MissionState.Grounded);
				});
			this.not_grounded.ToggleTag(GameTags.RocketNotOnGround).Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				foreach (GameObject gameObject4 in smi.master.parts)
				{
					if (!(gameObject4 == null))
					{
						gameObject4.AddTag(GameTags.RocketNotOnGround);
					}
				}
			}).Exit(delegate(LaunchableRocket.StatesInstance smi)
			{
				foreach (GameObject gameObject5 in smi.master.parts)
				{
					if (!(gameObject5 == null))
					{
						gameObject5.RemoveTag(GameTags.RocketNotOnGround);
					}
				}
			});
			this.not_grounded.launch_pre.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.isLanding = false;
				smi.master.rocketSpeed = 0f;
				smi.master.parts = AttachableBuilding.GetAttachedNetwork(smi.master.GetComponent<AttachableBuilding>());
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				foreach (GameObject gameObject6 in smi.master.GetEngines())
				{
					gameObject6.Trigger(-1358394196, null);
				}
				Game.Instance.Trigger(-1277991738, smi.gameObject);
				foreach (GameObject gameObject7 in smi.master.parts)
				{
					if (!(gameObject7 == null))
					{
						smi.master.takeOffLocation = Grid.PosToCell(smi.master.gameObject);
						gameObject7.Trigger(-1277991738, null);
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Launching);
			}).ScheduleGoTo(5f, this.not_grounded.launch_loop);
			this.not_grounded.launch_loop.EventTransition(GameHashes.DoReturnRocket, this.not_grounded.returning, null).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				smi.master.isLanding = false;
				bool flag = true;
				float num = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0f, 10f);
				smi.master.rocketSpeed = num;
				smi.master.flightAnimOffset += dt * num;
				foreach (GameObject gameObject8 in smi.master.parts)
				{
					if (!(gameObject8 == null))
					{
						KBatchedAnimController component = gameObject8.GetComponent<KBatchedAnimController>();
						component.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset = component.PositionIncludingOffset;
						if (smi.master.soundSpeakerObject == null)
						{
							smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
							smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
						}
						smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
						if (Grid.PosToXY(positionIncludingOffset).y > Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize().y + 20)
						{
							gameObject8.GetComponent<KBatchedAnimController>().enabled = false;
						}
						else
						{
							flag = false;
							LaunchableRocket.States.DoWorldDamage(gameObject8, positionIncludingOffset);
						}
					}
				}
				if (flag)
				{
					smi.GoTo(this.not_grounded.space);
				}
			}, UpdateRate.SIM_33ms, false).Exit(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.gameObject.GetMyWorld().RevealSurface();
			});
			this.not_grounded.space.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject gameObject9 in smi.master.parts)
				{
					if (!(gameObject9 == null))
					{
						gameObject9.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
						gameObject9.GetComponent<KBatchedAnimController>().enabled = false;
					}
				}
				smi.SetMissionState(Spacecraft.MissionState.Underway);
			}).EventTransition(GameHashes.DoReturnRocket, this.not_grounded.returning, (LaunchableRocket.StatesInstance smi) => smi.IsMissionState(Spacecraft.MissionState.WaitingToLand));
			this.not_grounded.returning.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.isLanding = true;
				smi.master.rocketSpeed = 0f;
				smi.SetMissionState(Spacecraft.MissionState.Landing);
			}).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				smi.master.isLanding = true;
				KBatchedAnimController component2 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component2.Offset = Vector3.up * smi.master.flightAnimOffset;
				float num2 = Mathf.Abs(smi.master.gameObject.transform.position.y + component2.Offset.y - (Grid.CellToPos(smi.master.takeOffLocation) + Vector3.down * (Grid.CellSizeInMeters / 2f)).y);
				float num3 = Mathf.Clamp(0.5f * num2, 0f, 10f) * dt;
				smi.master.rocketSpeed = num3;
				smi.master.flightAnimOffset -= num3;
				bool flag2 = true;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				foreach (GameObject gameObject10 in smi.master.parts)
				{
					if (!(gameObject10 == null))
					{
						KBatchedAnimController component3 = gameObject10.GetComponent<KBatchedAnimController>();
						component3.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset2 = component3.PositionIncludingOffset;
						if (Grid.IsValidCell(Grid.PosToCell(gameObject10)))
						{
							gameObject10.GetComponent<KBatchedAnimController>().enabled = true;
						}
						else
						{
							flag2 = false;
						}
						LaunchableRocket.States.DoWorldDamage(gameObject10, positionIncludingOffset2);
					}
				}
				if (flag2)
				{
					smi.GoTo(this.not_grounded.landing_loop);
				}
			}, UpdateRate.SIM_33ms, false);
			this.not_grounded.landing_loop.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.isLanding = true;
				int num4 = -1;
				for (int i = 0; i < smi.master.parts.Count; i++)
				{
					GameObject gameObject11 = smi.master.parts[i];
					if (!(gameObject11 == null) && gameObject11 != smi.master.gameObject && gameObject11.GetComponent<RocketEngine>() != null)
					{
						num4 = i;
					}
				}
				if (num4 != -1)
				{
					smi.master.parts[num4].Trigger(-1358394196, null);
				}
			}).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				smi.master.gameObject.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
				float flightAnimOffset = smi.master.flightAnimOffset;
				float num5 = Mathf.Clamp(0.5f * flightAnimOffset, 0f, 10f);
				smi.master.rocketSpeed = num5;
				smi.master.flightAnimOffset -= num5 * dt;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				if (num5 <= 0.0025f && dt != 0f)
				{
					smi.master.GetComponent<KSelectable>().IsSelectable = true;
					Game.Instance.Trigger(-887025858, smi.gameObject);
					foreach (GameObject gameObject12 in smi.master.parts)
					{
						if (!(gameObject12 == null))
						{
							gameObject12.Trigger(-887025858, null);
						}
					}
					smi.GoTo(this.grounded);
					return;
				}
				foreach (GameObject gameObject13 in smi.master.parts)
				{
					if (!(gameObject13 == null))
					{
						KBatchedAnimController component4 = gameObject13.GetComponent<KBatchedAnimController>();
						component4.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset3 = component4.PositionIncludingOffset;
						LaunchableRocket.States.DoWorldDamage(gameObject13, positionIncludingOffset3);
					}
				}
			}, UpdateRate.SIM_33ms, false);
		}

		private static void DoWorldDamage(GameObject part, Vector3 apparentPosition)
		{
			OccupyArea component = part.GetComponent<OccupyArea>();
			component.UpdateOccupiedArea();
			foreach (CellOffset cellOffset in component.OccupiedCellsOffsets)
			{
				int num = Grid.OffsetCell(Grid.PosToCell(apparentPosition), cellOffset);
				if (Grid.IsValidCell(num))
				{
					if (Grid.Solid[num])
					{
						WorldDamage.Instance.ApplyDamage(num, 10000f, num, BUILDINGS.DAMAGESOURCES.ROCKET, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
					}
					else if (Grid.FakeFloor[num])
					{
						GameObject gameObject = Grid.Objects[num, 39];
						if (gameObject != null)
						{
							BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
							if (component2 != null)
							{
								gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
								{
									damage = component2.MaxHitPoints,
									source = BUILDINGS.DAMAGESOURCES.ROCKET,
									popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET
								});
							}
						}
					}
				}
			}
		}

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State grounded;

		public LaunchableRocket.States.NotGroundedStates not_grounded;

		public class NotGroundedStates : GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State
		{
			public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State launch_pre;

			public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State space;

			public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State launch_loop;

			public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State returning;

			public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State landing_loop;
		}
	}
}
