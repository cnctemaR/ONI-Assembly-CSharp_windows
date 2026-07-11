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
		public StatesInstance(LaunchableRocket smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.grounded;
			base.serializable = true;
			this.grounded.EventTransition(GameHashes.LaunchRocket, this.launch_pre, null).Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.rocketSpeed = 0f;
				foreach (GameObject gameObject in smi.master.parts)
				{
					if (!(gameObject == null))
					{
						if (gameObject.GetComponent<CommandModule>() != null)
						{
							gameObject.GetComponent<CommandModule>().SetSuspended(false);
						}
						gameObject.GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
					}
				}
			});
			this.launch_pre.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.isLanding = false;
				smi.master.rocketSpeed = 0f;
				smi.master.parts = AttachableBuilding.GetAttachedNetwork(smi.master.GetComponent<AttachableBuilding>());
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				int num = -1;
				for (int i = 0; i < smi.master.parts.Count; i++)
				{
					GameObject gameObject2 = smi.master.parts[i];
					if (!(gameObject2 == null))
					{
						if (gameObject2 != smi.master.gameObject && gameObject2.GetComponent<RocketEngine>() != null)
						{
							num = i;
						}
					}
				}
				if (num != -1)
				{
					smi.master.parts[num].Trigger(-1358394196, null);
				}
				foreach (GameObject gameObject3 in smi.master.parts)
				{
					if (!(gameObject3 == null))
					{
						smi.master.takeOffLocation = Grid.PosToCell(smi.master.gameObject);
						gameObject3.Trigger(-1056989049, null);
					}
				}
			}).ScheduleGoTo(3f, this.launch_loop);
			this.launch_loop.EventTransition(GameHashes.ReturnRocket, this.returning, null).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				smi.master.isLanding = false;
				bool flag = true;
				float num2 = Mathf.Clamp(Mathf.Pow(smi.timeinstate / 5f, 4f), 0f, 10f);
				smi.master.rocketSpeed = num2;
				smi.master.flightAnimOffset += dt * num2;
				foreach (GameObject gameObject4 in smi.master.parts)
				{
					if (!(gameObject4 == null))
					{
						KBatchedAnimController component = gameObject4.GetComponent<KBatchedAnimController>();
						component.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 vector = gameObject4.transform.position + component.Offset;
						if (smi.master.soundSpeakerObject == null)
						{
							smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
							smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
						}
						smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
						if (Grid.PosToXY(vector).y > Singleton<KBatchedAnimUpdater>.Instance.GetVisibleSize().y)
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
							OccupyArea component2 = gameObject4.GetComponent<OccupyArea>();
							component2.UpdateOccupiedArea();
							foreach (CellOffset cellOffset in component2.OccupiedCellsOffsets)
							{
								int num3 = Grid.OffsetCell(Grid.PosToCell(vector), cellOffset);
								if (Grid.IsValidCell(num3) && Grid.Solid[num3])
								{
									WorldDamage instance = WorldDamage.Instance;
									int num4 = num3;
									float num5 = 10000f;
									int num6 = num3;
									string text = BUILDINGS.DAMAGESOURCES.ROCKET;
									instance.ApplyDamage(num4, num5, num6, -1, text, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
								}
							}
						}
					}
				}
				if (flag)
				{
					smi.GoTo(this.space);
				}
			}, UpdateRate.SIM_33ms, false);
			this.space.Enter(delegate(LaunchableRocket.StatesInstance smi)
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
			}).EventTransition(GameHashes.ReturnRocket, this.returning, null);
			this.returning.Enter(delegate(LaunchableRocket.StatesInstance smi)
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
				KBatchedAnimController component3 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component3.Offset = Vector3.up * smi.master.flightAnimOffset;
				float num7 = Mathf.Abs(smi.master.gameObject.transform.position.y + component3.Offset.y - (Grid.CellToPos(smi.master.takeOffLocation) + Vector3.down * (Grid.CellSizeInMeters / 2f)).y);
				float num8 = 0.5f;
				float num9 = Mathf.Clamp(num8 * num7, 0f, 10f) * dt;
				smi.master.rocketSpeed = num9;
				smi.master.flightAnimOffset -= num9;
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
						KBatchedAnimController component4 = gameObject7.GetComponent<KBatchedAnimController>();
						component4.Offset = Vector3.up * smi.master.flightAnimOffset;
						Vector3 vector2 = gameObject7.transform.position + component4.Offset;
						if (Grid.IsValidCell(Grid.PosToCell(gameObject7)))
						{
							gameObject7.GetComponent<KBatchedAnimController>().enabled = true;
						}
						else
						{
							flag2 = false;
						}
						OccupyArea component5 = gameObject7.GetComponent<OccupyArea>();
						component5.UpdateOccupiedArea();
						foreach (CellOffset cellOffset2 in component5.OccupiedCellsOffsets)
						{
							int num10 = Grid.OffsetCell(Grid.PosToCell(vector2), cellOffset2);
							if (Grid.IsValidCell(num10) && Grid.Solid[num10])
							{
								WorldDamage instance2 = WorldDamage.Instance;
								int num11 = num10;
								float num12 = 10000f;
								int num13 = num10;
								string text2 = BUILDINGS.DAMAGESOURCES.ROCKET;
								instance2.ApplyDamage(num11, num12, num13, -1, text2, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
							}
						}
					}
				}
				if (flag2)
				{
					smi.GoTo(this.landing_loop);
				}
			}, UpdateRate.SIM_33ms, false);
			this.landing_loop.Enter(delegate(LaunchableRocket.StatesInstance smi)
			{
				smi.master.isLanding = true;
				int num14 = -1;
				for (int l = 0; l < smi.master.parts.Count; l++)
				{
					GameObject gameObject8 = smi.master.parts[l];
					if (!(gameObject8 == null))
					{
						if (gameObject8 != smi.master.gameObject && gameObject8.GetComponent<RocketEngine>() != null)
						{
							num14 = l;
						}
					}
				}
				if (num14 != -1)
				{
					smi.master.parts[num14].Trigger(-1358394196, null);
				}
			}).Update(delegate(LaunchableRocket.StatesInstance smi, float dt)
			{
				KBatchedAnimController component6 = smi.master.gameObject.GetComponent<KBatchedAnimController>();
				component6.Offset = Vector3.up * smi.master.flightAnimOffset;
				float flightAnimOffset = smi.master.flightAnimOffset;
				float num15 = 0.5f;
				float num16 = Mathf.Clamp(num15 * flightAnimOffset, 0f, 10f);
				smi.master.rocketSpeed = num16;
				smi.master.flightAnimOffset -= num16 * dt;
				if (smi.master.soundSpeakerObject == null)
				{
					smi.master.soundSpeakerObject = new GameObject("rocketSpeaker");
					smi.master.soundSpeakerObject.transform.SetParent(smi.master.gameObject.transform);
				}
				smi.master.soundSpeakerObject.transform.SetLocalPosition(smi.master.flightAnimOffset * Vector3.up);
				if (num16 <= 0.0025f && dt != 0f)
				{
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
							KBatchedAnimController component7 = gameObject10.GetComponent<KBatchedAnimController>();
							component7.Offset = Vector3.up * smi.master.flightAnimOffset;
							Vector3 vector3 = gameObject10.transform.position + component7.Offset;
							OccupyArea component8 = gameObject10.GetComponent<OccupyArea>();
							component8.UpdateOccupiedArea();
							foreach (CellOffset cellOffset3 in component8.OccupiedCellsOffsets)
							{
								int num17 = Grid.OffsetCell(Grid.PosToCell(vector3), cellOffset3);
								if (Grid.IsValidCell(num17) && Grid.Solid[num17])
								{
									WorldDamage instance3 = WorldDamage.Instance;
									int num18 = num17;
									float num19 = 10000f;
									int num20 = num17;
									string text3 = BUILDINGS.DAMAGESOURCES.ROCKET;
									instance3.ApplyDamage(num18, num19, num20, -1, text3, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.ROCKET);
								}
							}
						}
					}
				}
			}, UpdateRate.SIM_33ms, false);
		}

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State grounded;

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State launch_pre;

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State space;

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State launch_loop;

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State returning;

		public GameStateMachine<LaunchableRocket.States, LaunchableRocket.StatesInstance, LaunchableRocket, object>.State landing_loop;
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
