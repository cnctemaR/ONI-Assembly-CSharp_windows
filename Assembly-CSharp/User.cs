using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/User")]
public class User : KMonoBehaviour
{
	public void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (status == StateMachine.Status.Success)
		{
			base.Trigger(58624316, null);
			return;
		}
		base.Trigger(1572098533, null);
	}
}
