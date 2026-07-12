using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class PlantBlightEvent : GameplayEvent<PlantBlightEvent.StatesInstance>
	{
		public PlantBlightEvent(string id, string targetPlantPrefab, float infectionDuration, float incubationDuration)
			: base(id, 0, 0)
		{
			this.targetPlantPrefab = targetPlantPrefab;
			this.infectionDuration = infectionDuration;
			this.incubationDuration = incubationDuration;
			this.title = GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.NAME;
			this.description = GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new PlantBlightEvent.StatesInstance(manager, eventInstance, this);
		}

		private const float BLIGHT_DISTANCE = 6f;

		public string targetPlantPrefab;

		public float infectionDuration;

		public float incubationDuration;

		public class States : GameplayEventStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, PlantBlightEvent>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = this.planning;
				base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
				this.planning.Enter(delegate(PlantBlightEvent.StatesInstance smi)
				{
					smi.InfectAPlant(true);
				}).GoTo(this.running);
				this.running.ToggleNotification((PlantBlightEvent.StatesInstance smi) => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi), null)).EventHandlerTransition(GameHashes.Uprooted, this.finished, new Func<PlantBlightEvent.StatesInstance, object, bool>(this.NoBlightedPlants)).DefaultState(this.running.waiting)
					.OnSignal(this.doFinish, this.finished);
				this.running.waiting.ParamTransition<float>(this.nextInfection, this.running.infect, (PlantBlightEvent.StatesInstance smi, float p) => p <= 0f).Update(delegate(PlantBlightEvent.StatesInstance smi, float dt)
				{
					this.nextInfection.Delta(-dt, smi);
				}, UpdateRate.SIM_4000ms, false);
				this.running.infect.Enter(delegate(PlantBlightEvent.StatesInstance smi)
				{
					smi.InfectAPlant(false);
				}).GoTo(this.running.waiting);
				this.finished.DoNotification((PlantBlightEvent.StatesInstance smi) => this.CreateSuccessNotification(smi, this.GenerateEventPopupData(smi))).ReturnSuccess();
			}

			public override EventInfoData GenerateEventPopupData(PlantBlightEvent.StatesInstance smi)
			{
				EventInfoData eventInfoData = new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName);
				string text = smi.gameplayEvent.targetPlantPrefab.ToTag().ProperName();
				eventInfoData.location = GAMEPLAY_EVENTS.LOCATIONS.COLONY_WIDE;
				eventInfoData.whenDescription = GAMEPLAY_EVENTS.TIMES.NOW;
				eventInfoData.SetTextParameter("plant", text);
				return eventInfoData;
			}

			private Notification CreateSuccessNotification(PlantBlightEvent.StatesInstance smi, EventInfoData eventInfoData)
			{
				string plantName = smi.gameplayEvent.targetPlantPrefab.ToTag().ProperName();
				return new Notification(GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.SUCCESS.Replace("{plant}", plantName), NotificationType.Neutral, (List<Notification> list, object data) => GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.SUCCESS_TOOLTIP.Replace("{plant}", plantName), null, true, 0f, null, null, null, true, false, false);
			}

			private bool NoBlightedPlants(PlantBlightEvent.StatesInstance smi, object obj)
			{
				GameObject gameObject = (GameObject)obj;
				if (!gameObject.HasTag(GameTags.Blighted))
				{
					return true;
				}
				foreach (Crop crop in Components.Crops.Items.FindAll((Crop p) => p.name == smi.gameplayEvent.targetPlantPrefab))
				{
					if (!(gameObject.gameObject == crop.gameObject) && crop.HasTag(GameTags.Blighted))
					{
						return false;
					}
				}
				return true;
			}

			public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State planning;

			public PlantBlightEvent.States.RunningStates running;

			public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State finished;

			public StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.Signal doFinish;

			public StateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.FloatParameter nextInfection;

			public class RunningStates : GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State
			{
				public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State waiting;

				public GameStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, object>.State infect;
			}
		}

		public class StatesInstance : GameplayEventStateMachine<PlantBlightEvent.States, PlantBlightEvent.StatesInstance, GameplayEventManager, PlantBlightEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, PlantBlightEvent blightEvent)
				: base(master, eventInstance, blightEvent)
			{
				this.startTime = Time.time;
			}

			public void InfectAPlant(bool initialInfection)
			{
				if (Time.time - this.startTime > base.smi.gameplayEvent.infectionDuration)
				{
					base.sm.doFinish.Trigger(base.smi);
					return;
				}
				List<Crop> list = Components.Crops.Items.FindAll((Crop p) => p.name == base.smi.gameplayEvent.targetPlantPrefab);
				if (list.Count == 0)
				{
					base.sm.doFinish.Trigger(base.smi);
					return;
				}
				if (list.Count > 0)
				{
					List<Crop> list2 = new List<Crop>();
					List<Crop> list3 = new List<Crop>();
					foreach (Crop crop in list)
					{
						if (crop.HasTag(GameTags.Blighted))
						{
							list2.Add(crop);
						}
						else
						{
							list3.Add(crop);
						}
					}
					if (list2.Count == 0)
					{
						if (initialInfection)
						{
							Crop crop2 = list3[global::UnityEngine.Random.Range(0, list3.Count)];
							global::Debug.Log("Blighting a random plant", crop2);
							crop2.GetComponent<BlightVulnerable>().MakeBlighted();
						}
						else
						{
							base.sm.doFinish.Trigger(base.smi);
						}
					}
					else if (list3.Count > 0)
					{
						Crop crop3 = list2[global::UnityEngine.Random.Range(0, list2.Count)];
						global::Debug.Log("Spreading blight from a plant", crop3);
						list3.Shuffle<Crop>();
						foreach (Crop crop4 in list3)
						{
							if ((crop4.transform.GetPosition() - crop3.transform.GetPosition()).magnitude < 6f)
							{
								crop4.GetComponent<BlightVulnerable>().MakeBlighted();
								break;
							}
						}
					}
				}
				base.sm.nextInfection.Set(base.smi.gameplayEvent.incubationDuration, this, false);
			}

			[Serialize]
			private float startTime;
		}
	}
}
