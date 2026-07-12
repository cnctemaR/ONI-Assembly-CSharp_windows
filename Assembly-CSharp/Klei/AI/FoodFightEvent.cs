using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class FoodFightEvent : GameplayEvent<FoodFightEvent.StatesInstance>
	{
		public FoodFightEvent()
			: base("FoodFight", 0, 0)
		{
			this.title = GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.NAME;
			this.description = GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new FoodFightEvent.StatesInstance(manager, eventInstance, this);
		}

		public const float FUTURE_TIME = 60f;

		public const float DURATION = 60f;

		public class StatesInstance : GameplayEventStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, FoodFightEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, FoodFightEvent foodEvent)
				: base(master, eventInstance, foodEvent)
			{
			}

			public void CreateChores(FoodFightEvent.StatesInstance smi)
			{
				this.chores = new List<FoodFightChore>();
				List<Room> list = Game.Instance.roomProber.rooms.FindAll((Room match) => match.roomType == Db.Get().RoomTypes.MessHall || match.roomType == Db.Get().RoomTypes.GreatHall);
				if (list == null || list.Count == 0)
				{
					return;
				}
				List<GameObject> buildingsOnFloor = list[global::UnityEngine.Random.Range(0, list.Count)].GetBuildingsOnFloor();
				for (int i = 0; i < Math.Min(Components.LiveMinionIdentities.Count, buildingsOnFloor.Count); i++)
				{
					IStateMachineTarget stateMachineTarget = Components.LiveMinionIdentities[i];
					GameObject gameObject = buildingsOnFloor[global::UnityEngine.Random.Range(0, buildingsOnFloor.Count)];
					GameObject locator = ChoreHelpers.CreateLocator("FoodFightLocator", gameObject.transform.position);
					FoodFightChore foodFightChore = new FoodFightChore(stateMachineTarget, locator);
					buildingsOnFloor.Remove(gameObject);
					FoodFightChore foodFightChore2 = foodFightChore;
					foodFightChore2.onExit = (Action<Chore>)Delegate.Combine(foodFightChore2.onExit, new Action<Chore>(delegate(Chore data)
					{
						Util.KDestroyGameObject(locator);
					}));
					this.chores.Add(foodFightChore);
				}
			}

			public void ClearChores()
			{
				if (this.chores != null)
				{
					for (int i = this.chores.Count - 1; i >= 0; i--)
					{
						if (this.chores[i] != null)
						{
							this.chores[i].Cancel("end");
						}
					}
				}
				this.chores = null;
			}

			public List<FoodFightChore> chores;
		}

		public class States : GameplayEventStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, FoodFightEvent>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = this.planning;
				base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
				this.root.Exit(delegate(FoodFightEvent.StatesInstance smi)
				{
					smi.ClearChores();
				});
				this.planning.ToggleNotification((FoodFightEvent.StatesInstance smi) => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi), null));
				this.warmup.ToggleNotification((FoodFightEvent.StatesInstance smi) => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi), null));
				this.warmup.wait.ScheduleGoTo(60f, this.warmup.start);
				this.warmup.start.Enter(delegate(FoodFightEvent.StatesInstance smi)
				{
					smi.CreateChores(smi);
				}).Update(delegate(FoodFightEvent.StatesInstance smi, float data)
				{
					int num = 0;
					foreach (FoodFightChore foodFightChore in smi.chores)
					{
						if (foodFightChore.smi.IsInsideState(foodFightChore.smi.sm.waitForParticipants))
						{
							num++;
						}
					}
					if (num >= smi.chores.Count || smi.timeinstate > 30f)
					{
						foreach (FoodFightChore foodFightChore2 in smi.chores)
						{
							foodFightChore2.gameObject.Trigger(-2043101269, null);
						}
						smi.GoTo(this.partying);
					}
				}, UpdateRate.RENDER_1000ms, false);
				this.partying.ToggleNotification((FoodFightEvent.StatesInstance smi) => new Notification(GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.UNDERWAY, NotificationType.Good, (List<Notification> a, object b) => GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.UNDERWAY_TOOLTIP, null, true, 0f, null, null, null, true, false)).ScheduleGoTo(60f, this.ending);
				this.ending.ReturnSuccess();
				this.canceled.DoNotification((FoodFightEvent.StatesInstance smi) => GameplayEventManager.CreateStandardCancelledNotification(this.GenerateEventPopupData(smi))).Enter(delegate(FoodFightEvent.StatesInstance smi)
				{
					foreach (object obj in Components.LiveMinionIdentities)
					{
						((MinionIdentity)obj).GetComponent<Effects>().Add("NoFunAllowed", true);
					}
				}).ReturnFailure();
			}

			public override EventInfoData GenerateEventPopupData(FoodFightEvent.StatesInstance smi)
			{
				EventInfoData eventInfoData = new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName);
				eventInfoData.location = GAMEPLAY_EVENTS.LOCATIONS.PRINTING_POD;
				eventInfoData.whenDescription = string.Format(GAMEPLAY_EVENTS.TIMES.IN_CYCLES, 0.1f);
				eventInfoData.AddOption(GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.ACCEPT_OPTION_NAME, null).callback = delegate
				{
					smi.GoTo(smi.sm.warmup.wait);
				};
				eventInfoData.AddOption(GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.REJECT_OPTION_NAME, null).callback = delegate
				{
					smi.GoTo(smi.sm.canceled);
				};
				return eventInfoData;
			}

			public GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State planning;

			public FoodFightEvent.States.WarmupStates warmup;

			public GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State partying;

			public GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State ending;

			public GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State canceled;

			public class WarmupStates : GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State
			{
				public GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State wait;

				public GameStateMachine<FoodFightEvent.States, FoodFightEvent.StatesInstance, GameplayEventManager, object>.State start;
			}
		}
	}
}
