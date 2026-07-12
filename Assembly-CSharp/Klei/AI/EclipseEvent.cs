using System;
using STRINGS;

namespace Klei.AI
{
	public class EclipseEvent : GameplayEvent<EclipseEvent.StatesInstance>
	{
		public EclipseEvent()
			: base("EclipseEvent", 0, 0)
		{
			this.title = GAMEPLAY_EVENTS.EVENT_TYPES.ECLIPSE.NAME;
			this.description = GAMEPLAY_EVENTS.EVENT_TYPES.ECLIPSE.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new EclipseEvent.StatesInstance(manager, eventInstance, this);
		}

		public const string ID = "EclipseEvent";

		public const float duration = 30f;

		public class StatesInstance : GameplayEventStateMachine<EclipseEvent.States, EclipseEvent.StatesInstance, GameplayEventManager, EclipseEvent>.GameplayEventStateMachineInstance
		{
			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, EclipseEvent eclipseEvent)
				: base(master, eventInstance, eclipseEvent)
			{
			}
		}

		public class States : GameplayEventStateMachine<EclipseEvent.States, EclipseEvent.StatesInstance, GameplayEventManager, EclipseEvent>
		{
			public override void InitializeStates(out StateMachine.BaseState default_state)
			{
				default_state = this.planning;
				base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
				this.planning.GoTo(this.eclipse);
				this.eclipse.ToggleNotification((EclipseEvent.StatesInstance smi) => EventInfoScreen.CreateNotification(this.GenerateEventPopupData(smi), null)).Enter(delegate(EclipseEvent.StatesInstance smi)
				{
					TimeOfDay.Instance.SetEclipse(true);
				}).Exit(delegate(EclipseEvent.StatesInstance smi)
				{
					TimeOfDay.Instance.SetEclipse(false);
				})
					.ScheduleGoTo(30f, this.finished);
				this.finished.ReturnSuccess();
			}

			public override EventInfoData GenerateEventPopupData(EclipseEvent.StatesInstance smi)
			{
				return new EventInfoData(smi.gameplayEvent.title, smi.gameplayEvent.description, smi.gameplayEvent.animFileName)
				{
					location = GAMEPLAY_EVENTS.LOCATIONS.SUN,
					whenDescription = GAMEPLAY_EVENTS.TIMES.NOW
				};
			}

			public GameStateMachine<EclipseEvent.States, EclipseEvent.StatesInstance, GameplayEventManager, object>.State planning;

			public GameStateMachine<EclipseEvent.States, EclipseEvent.StatesInstance, GameplayEventManager, object>.State eclipse;

			public GameStateMachine<EclipseEvent.States, EclipseEvent.StatesInstance, GameplayEventManager, object>.State finished;
		}
	}
}
