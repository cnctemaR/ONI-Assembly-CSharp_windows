using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LaunchableRocket : StateMachineComponent<LaunchableRocket.StatesInstance>, IEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.master.parts = AttachableBuilding.GetAttachedNetwork(base.smi.master.GetComponent<AttachableBuilding>());
		base.smi.StartSM();
		int spacecraftID = SpacecraftManager.instance.GetSpacecraftID(this);
		if (spacecraftID == -1)
		{
			Spacecraft spacecraft = new Spacecraft(base.GetComponent<LaunchConditionManager>());
			SpacecraftManager.instance.RegisterSpacecraft(spacecraft);
		}
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	public List<GameObject> parts = new List<GameObject>();

	[Serialize]
	private int takeOffLocation;

	[Serialize]
	private float flightAnimOffset;

	private bool isLanding;

	private float rocketSpeed;

	private GameObject soundSpeakerObject;

	public class StatesInstance : GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.GameInstance
	{
		public StatesInstance(LaunchableRocket master)
			: base(master)
		{
		}

		public bool IsReadyToReturn()
		{
			Spacecraft spacecraftFromLaunchConditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(base.master.GetComponent<LaunchConditionManager>());
			return spacecraftFromLaunchConditionManager.state == Spacecraft.MissionState.WaitingToLand;
		}
	}

	public class States : GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded;
			base.serializable = true;
			this.grounded.EventTransition(GameHashes.LaunchRocket, this.not_grounded.launch_pre, null).Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject gameObject in smi.master.parts)
				{
					if (!(gameObject == null))
					{
						gameObject.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
					}
				}
			});
			this.not_grounded.ToggleTag(GameTags.RocketNotOnGround);
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
				foreach (GameObject gameObject2 in smi.master.GetEngines())
				{
					gameObject2.Trigger(-1358394196, null);
				}
				Game.Instance.Trigger(-1056989049, this);
				foreach (GameObject gameObject3 in smi.master.parts)
				{
					if (!(gameObject3 == null))
					{
						smi.master.takeOffLocation = Grid.PosToCell(smi.master.gameObject);
						gameObject3.Trigger(-1056989049, null);
					}
				}
			}).ScheduleGoTo(3f, this.not_grounded.launch_loop);
			this.not_grounded.launch_loop.EventTransition(GameHashes.ReturnRocket, this.not_grounded.returning, null).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				smi.master.isLanding = false;
				bool flag = true;
				float num = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0f, 10f);
				smi.master.rocketSpeed = num;
				smi.master.flightAnimOffset += dt * num;
				foreach (GameObject gameObject4 in smi.master.parts)
				{
					if (!(gameObject4 == null))
					{
						KBatchedAnimController component = gameObject4.GetComponent<KBatchedAnimController>();
						component.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset = component.PositionIncludingOffset;
						if (smi.master.soundSpeakerObject == null)
						{
							smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
							smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
						}
						smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
						if (Grid.PosToXY(positionIncludingOffset).y > Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize().y)
						{
							gameObject4.GetComponent<RocketModule>().OnSuspend(null);
							gameObject4.GetComponent<KBatchedAnimController>().enabled = false;
							if (gameObject4.gameObject != smi.master.gameObject)
							{
								gameObject4.gameObject.SetActive(false);
							}
						}
						else
						{
							flag = false;
							LaunchableRocket.States.DoWorldDamage(gameObject4, positionIncludingOffset);
						}
					}
				}
				if (flag)
				{
					smi.GoTo(this.not_grounded.space);
				}
			}, UpdateRate.SIM_33ms, false);
			this.not_grounded.space.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject gameObject5 in smi.master.parts)
				{
					if (!(gameObject5 == null))
					{
						gameObject5.GetComponent<KBatchedAnimController>().Offset = Vector3.up * smi.master.flightAnimOffset;
						gameObject5.GetComponent<KBatchedAnimController>().enabled = false;
						if (gameObject5.gameObject != smi.master.gameObject)
						{
							gameObject5.gameObject.SetActive(false);
						}
					}
				}
			}).EventTransition(GameHashes.ReturnRocket, this.not_grounded.returning, (LaunchableRocket.StatesInstance smi) => smi.IsReadyToReturn());
			this.not_grounded.returning.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.isLanding = true;
				smi.master.rocketSpeed = 0f;
				foreach (GameObject gameObject6 in smi.master.parts)
				{
					if (!(gameObject6 == null))
					{
						gameObject6.gameObject.SetActive(true);
					}
				}
			}).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				smi.master.isLanding = true;
				KBatchedAnimController component2 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component2.Offset = Vector3.up * smi.master.flightAnimOffset;
				float num2 = Mathf.Abs(smi.master.gameObject.transform.position.y + component2.Offset.y - (Grid.CellToPos(smi.master.takeOffLocation) + Vector3.down * (Grid.CellSizeInMeters / 2f)).y);
				float num3 = 0.5f;
				float num4 = Mathf.Clamp(num3 * num2, 0f, 10f) * dt;
				smi.master.rocketSpeed = num4;
				smi.master.flightAnimOffset -= num4;
				bool flag2 = true;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				foreach (GameObject gameObject7 in smi.master.parts)
				{
					if (!(gameObject7 == null))
					{
						KBatchedAnimController component3 = gameObject7.GetComponent<KBatchedAnimController>();
						component3.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 positionIncludingOffset2 = component3.PositionIncludingOffset;
						if (Grid.IsValidCell(Grid.PosToCell(gameObject7)))
						{
							gameObject7.GetComponent<KBatchedAnimController>().enabled = true;
						}
						else
						{
							flag2 = false;
						}
						LaunchableRocket.States.DoWorldDamage(gameObject7, positionIncludingOffset2);
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
				int num5 = -1;
				for (int i = 0; i < smi.master.parts.Count; i++)
				{
					GameObject gameObject8 = smi.master.parts[i];
					if (!(gameObject8 == null))
					{
						if (gameObject8 != smi.master.gameObject && gameObject8.GetComponent<RocketEngine>() != null)
						{
							num5 = i;
						}
					}
				}
				if (num5 != -1)
				{
					smi.master.parts[num5].Trigger(-1358394196, null);
				}
			}).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				KBatchedAnimController component4 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component4.Offset = Vector3.up * smi.master.flightAnimOffset;
				float flightAnimOffset = smi.master.flightAnimOffset;
				float num6 = 0.5f;
				float num7 = Mathf.Clamp(num6 * flightAnimOffset, 0f, 10f);
				smi.master.rocketSpeed = num7;
				smi.master.flightAnimOffset -= num7 * dt;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				if (num7 <= 0.0025f && dt != 0f)
				{
					smi.master.GetComponent<KSelectable>().IsSelectable = true;
					foreach (GameObject gameObject9 in smi.master.parts)
					{
						if (!(gameObject9 == null))
						{
							gameObject9.Trigger(238242047, null);
						}
					}
					SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(smi.GetComponent<LaunchConditionManager>()).SetState(Spacecraft.MissionState.Grounded);
					smi.GoTo(this.grounded);
				}
				else
				{
					foreach (GameObject gameObject10 in smi.master.parts)
					{
						if (!(gameObject10 == null))
						{
							KBatchedAnimController component5 = gameObject10.GetComponent<KBatchedAnimController>();
							component5.Offset = Vector3.up * smi.master.flightAnimOffset;
							Vector3 positionIncludingOffset3 = component5.PositionIncludingOffset;
							LaunchableRocket.States.DoWorldDamage(gameObject10, positionIncludingOffset3);
						}
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
						WorldDamage instance = WorldDamage.Instance;
						int num2 = num;
						float num3 = 10000f;
						int num4 = num;
						string text = BUILDINGS.DAMAGESOURCES.ROCKET;
						instance.ApplyDamage(num2, num3, num4, -1, text, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
					}
					else if (Grid.FakeFloor[num])
					{
						GameObject gameObject = Grid.Objects[num, 36];
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

	private class UpdateRocketLandingParameter : LoopingSoundParameterUpdater
	{
		public UpdateRocketLandingParameter()
			: base("rocketLanding")
		{
		}

		public override void Add(LoopingSoundParameterUpdater.Sound sound)
		{
			LaunchableRocket.UpdateRocketLandingParameter.Entry entry = new LaunchableRocket.UpdateRocketLandingParameter.Entry
			{
				rocketModule = sound.transform.GetComponent<RocketModule>(),
				ev = sound.ev,
				parameterIdx = sound.description.GetParameterIdx(base.parameter)
			};
			this.entries.Add(entry);
		}

		public override void Update(float dt)
		{
			foreach (LaunchableRocket.UpdateRocketLandingParameter.Entry entry in this.entries)
			{
				if (!(entry.rocketModule == null))
				{
					LaunchConditionManager conditionManager = entry.rocketModule.conditionManager;
					if (!(conditionManager == null))
					{
						LaunchableRocket component = conditionManager.GetComponent<LaunchableRocket>();
						if (!(component == null))
						{
							if (component.isLanding)
							{
								entry.ev.setParameterValueByIndex(entry.parameterIdx, 1f);
							}
							else
							{
								entry.ev.setParameterValueByIndex(entry.parameterIdx, 0f);
							}
						}
					}
				}
			}
		}

		public override void Remove(LoopingSoundParameterUpdater.Sound sound)
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				if (this.entries[i].ev.handle == sound.ev.handle)
				{
					this.entries.RemoveAt(i);
					break;
				}
			}
		}

		private List<LaunchableRocket.UpdateRocketLandingParameter.Entry> entries = new List<LaunchableRocket.UpdateRocketLandingParameter.Entry>();

		private struct Entry
		{
			public RocketModule rocketModule;

			public EventInstance ev;

			public int parameterIdx;
		}
	}

	private class UpdateRocketSpeedParameter : LoopingSoundParameterUpdater
	{
		public UpdateRocketSpeedParameter()
			: base("rocketSpeed")
		{
		}

		public override void Add(LoopingSoundParameterUpdater.Sound sound)
		{
			LaunchableRocket.UpdateRocketSpeedParameter.Entry entry = new LaunchableRocket.UpdateRocketSpeedParameter.Entry
			{
				rocketModule = sound.transform.GetComponent<RocketModule>(),
				ev = sound.ev,
				parameterIdx = sound.description.GetParameterIdx(base.parameter)
			};
			this.entries.Add(entry);
		}

		public override void Update(float dt)
		{
			foreach (LaunchableRocket.UpdateRocketSpeedParameter.Entry entry in this.entries)
			{
				if (!(entry.rocketModule == null))
				{
					LaunchConditionManager conditionManager = entry.rocketModule.conditionManager;
					if (!(conditionManager == null))
					{
						LaunchableRocket component = conditionManager.GetComponent<LaunchableRocket>();
						if (!(component == null))
						{
							entry.ev.setParameterValueByIndex(entry.parameterIdx, component.rocketSpeed);
						}
					}
				}
			}
		}

		public override void Remove(LoopingSoundParameterUpdater.Sound sound)
		{
			for (int i = 0; i < this.entries.Count; i++)
			{
				if (this.entries[i].ev.handle == sound.ev.handle)
				{
					this.entries.RemoveAt(i);
					break;
				}
			}
		}

		private List<LaunchableRocket.UpdateRocketSpeedParameter.Entry> entries = new List<LaunchableRocket.UpdateRocketSpeedParameter.Entry>();

		private struct Entry
		{
			public RocketModule rocketModule;

			public EventInstance ev;

			public int parameterIdx;
		}
	}
}
