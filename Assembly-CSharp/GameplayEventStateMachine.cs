using System;
using UnityEngine;

public abstract class GameplayEventStateMachine<StateMachineType, StateMachineInstanceType, MasterType, SecondMasterType> : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType> where StateMachineType : GameplayEventStateMachine<StateMachineType, StateMachineInstanceType, MasterType, SecondMasterType> where StateMachineInstanceType : GameplayEventStateMachine<StateMachineType, StateMachineInstanceType, MasterType, SecondMasterType>.GameplayEventStateMachineInstance where MasterType : IStateMachineTarget where SecondMasterType : GameplayEvent<StateMachineInstanceType>
{
	public void MonitorStart(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.TargetParameter target, StateMachineInstanceType smi)
	{
		GameObject gameObject = target.Get(smi);
		if (gameObject != null)
		{
			gameObject.Trigger(-1660384580, smi.eventInstance);
		}
	}

	public void MonitorChanged(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.TargetParameter target, StateMachineInstanceType smi)
	{
		GameObject gameObject = target.Get(smi);
		if (gameObject != null)
		{
			gameObject.Trigger(-1122598290, smi.eventInstance);
		}
	}

	public void MonitorStop(StateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.TargetParameter target, StateMachineInstanceType smi)
	{
		GameObject gameObject = target.Get(smi);
		if (gameObject != null)
		{
			gameObject.Trigger(-828272459, smi.eventInstance);
		}
	}

	public virtual EventInfoData GenerateEventPopupData(StateMachineInstanceType smi)
	{
		return null;
	}

	public class GameplayEventStateMachineInstance : GameStateMachine<StateMachineType, StateMachineInstanceType, MasterType, object>.GameInstance
	{
		public GameplayEventStateMachineInstance(MasterType master, GameplayEventInstance eventInstance, SecondMasterType gameplayEvent)
			: base(master)
		{
			this.gameplayEvent = gameplayEvent;
			this.eventInstance = eventInstance;
			eventInstance.GetEventPopupData = () => base.smi.sm.GenerateEventPopupData(base.smi);
			this.serializationSuffix = gameplayEvent.Id;
		}

		public GameplayEventInstance eventInstance;

		public SecondMasterType gameplayEvent;
	}
}
