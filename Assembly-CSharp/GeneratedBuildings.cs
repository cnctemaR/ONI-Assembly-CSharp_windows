using System;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedBuildings
{
	public static void LoadGeneratedBuildings(List<Type> types)
	{
		Type typeFromHandle = typeof(IBuildingConfig);
		List<Type> list = new List<Type>();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				list.Add(type);
			}
		}
		foreach (Type type2 in list)
		{
			object obj = Activator.CreateInstance(type2);
			BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
		}
	}

	public static void MakeBuildingAlwaysOperational(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<Operational>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicPorts>());
	}

	public static void RemoveLoopingSounds(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LoopingSounds>());
	}

	public static void RemoveDefaultLogicPorts(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicPorts>());
	}

	public static void RegisterWithOverlay(HashSet<Tag> overlay_tags, string id)
	{
		overlay_tags.Add(new Tag(id));
		overlay_tags.Add(new Tag(id + "UnderConstruction"));
	}

	public static void RegisterLogicPorts(GameObject go, LogicPorts.Port[] inputs, LogicPorts.Port[] outputs)
	{
		LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = inputs;
		logicPorts.outputPortInfo = outputs;
	}

	public static void RegisterLogicPorts(GameObject go, LogicPorts.Port[] inputs)
	{
		LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = inputs;
		logicPorts.outputPortInfo = null;
	}

	public static void RegisterLogicPorts(GameObject go, LogicPorts.Port output)
	{
		LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = null;
		logicPorts.outputPortInfo = new LogicPorts.Port[] { output };
	}
}
