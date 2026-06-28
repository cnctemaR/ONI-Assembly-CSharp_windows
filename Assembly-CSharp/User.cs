using System;

public class User : KMonoBehaviour
{
	public void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		bool flag = status == StateMachine.Status.Success;
		if (flag)
		{
			this.Trigger(58624316, null);
		}
		else
		{
			this.Trigger(1572098533, null);
		}
	}
}
