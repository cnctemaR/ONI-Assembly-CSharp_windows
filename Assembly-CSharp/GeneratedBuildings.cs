using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GeneratedBuildings
{
	public static void LoadGeneratedBuildings()
	{
		Type typeFromHandle = typeof(IBuildingConfig);
		Assembly assembly = Assembly.GetAssembly(typeof(TileConfig));
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
			{
				object obj = Activator.CreateInstance(type);
				BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
			}
		}
	}

	public static void MakeBuildingAlwaysOperational(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<Operational>());
	}

	public static void RemoveLoopingSounds(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LoopingSounds>());
	}

	public static void MakeBuildableAnywhere(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<RequiresFoundation>());
	}

	public static void RegisterWithOverlay(HashSet<Tag> overlay_tags, string id)
	{
		overlay_tags.Add(new Tag(id));
		overlay_tags.Add(new Tag(id + "UnderConstruction"));
	}
}
