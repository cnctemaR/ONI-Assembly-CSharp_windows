using System;

public abstract class StateEvent
{
	public StateEvent(string name)
	{
		this.name = name;
		this.debugName = "(Event)" + name;
	}

	public virtual StateEvent.Context Subscribe(StateMachine.Instance smi)
	{
		return new StateEvent.Context(this);
	}

	public virtual void Unsubscribe(StateMachine.Instance smi, StateEvent.Context context)
	{
	}

	public string GetName()
	{
		return this.name;
	}

	public string GetDebugName()
	{
		return this.debugName;
	}

	protected string name;

	private string debugName;

	public struct Context
	{
		public Context(StateEvent state_event)
		{
			this.stateEvent = state_event;
			this.data = 0;
		}

		public StateEvent stateEvent;

		public int data;
	}
}
