using System;
using UnityEngine;

public static class EntityTemplateExtensions
{
	public static DefType AddOrGetDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
	{
		StateMachineController stateMachineController = go.AddOrGet<StateMachineController>();
		DefType defType = stateMachineController.GetDef<DefType>();
		if (defType == null)
		{
			defType = Activator.CreateInstance<DefType>();
			stateMachineController.AddDef(defType);
			defType.Configure(go);
		}
		return defType;
	}

	public static ComponentType AddOrGet<ComponentType>(this GameObject go) where ComponentType : Component
	{
		ComponentType componentType = go.GetComponent<ComponentType>();
		if (componentType == null)
		{
			componentType = go.AddComponent<ComponentType>();
		}
		return componentType;
	}

	public static void RemoveDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
	{
		StateMachineController stateMachineController;
		if (!go.TryGetComponent<StateMachineController>(out stateMachineController))
		{
			return;
		}
		DefType def = stateMachineController.GetDef<DefType>();
		if (def == null)
		{
			return;
		}
		stateMachineController.cmpdef.defs.Remove(def);
	}
}
