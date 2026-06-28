using System;
using UnityEngine;

public static class StateMachineControllerExtensions
{
	public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(this Component cmp) where StateMachineInstanceType : StateMachine.Instance
	{
		return cmp.gameObject.GetSMI<StateMachineInstanceType>();
	}

	public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(this GameObject go) where StateMachineInstanceType : StateMachine.Instance
	{
		StateMachineController component = go.GetComponent<StateMachineController>();
		if (component != null)
		{
			return component.GetSMI<StateMachineInstanceType>();
		}
		return (StateMachineInstanceType)((object)null);
	}
}
