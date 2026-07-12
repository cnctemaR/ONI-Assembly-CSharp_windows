using System;

public abstract class GameplayEvent<StateMachineInstanceType> : GameplayEvent where StateMachineInstanceType : StateMachine.Instance
{
	public GameplayEvent(string id, int priority = 0, int importance = 0)
		: base(id, priority, importance)
	{
	}
}
