using System;
using System.Collections.Generic;
using UnityEngine;

public static class StateMachineControllerExtensions
{
	public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(this Component cmp) where StateMachineInstanceType : class
	{
		return cmp.gameObject.GetSMI<StateMachineInstanceType>();
	}

	public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(this GameObject go) where StateMachineInstanceType : class
	{
		StateMachineController component = go.GetComponent<StateMachineController>();
		StateMachineInstanceType stateMachineInstanceType;
		if (component != null)
		{
			stateMachineInstanceType = component.GetSMI<StateMachineInstanceType>();
		}
		else
		{
			stateMachineInstanceType = (StateMachineInstanceType)((object)null);
		}
		return stateMachineInstanceType;
	}

	public static List<StateMachineInstanceType> GetAllSMI<StateMachineInstanceType>(this Component cmp) where StateMachineInstanceType : class
	{
		return cmp.gameObject.GetAllSMI<StateMachineInstanceType>();
	}

	public static List<StateMachineInstanceType> GetAllSMI<StateMachineInstanceType>(this GameObject go) where StateMachineInstanceType : class
	{
		StateMachineController component = go.GetComponent<StateMachineController>();
		List<StateMachineInstanceType> list;
		if (component != null)
		{
			list = component.GetAllSMI<StateMachineInstanceType>();
		}
		else
		{
			list = new List<StateMachineInstanceType>();
		}
		return list;
	}
}
