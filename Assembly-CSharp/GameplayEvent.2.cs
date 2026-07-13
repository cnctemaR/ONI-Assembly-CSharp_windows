using System;

public abstract class GameplayEvent<StateMachineInstanceType> : GameplayEvent where StateMachineInstanceType : StateMachine.Instance
{
	public GameplayEvent(string id, int priority = 0, int importance = 0, string[] requiredDlcIds = null, string[] forbiddenDlcIds = null)
		: base(id, priority, importance, requiredDlcIds, forbiddenDlcIds)
	{
	}
}
