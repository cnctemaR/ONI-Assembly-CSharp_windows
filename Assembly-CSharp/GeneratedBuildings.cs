using System;
using System.Collections.Generic;
using TUNING;
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
			try
			{
				BuildingConfigManager.Instance.RegisterBuilding(obj as IBuildingConfig);
			}
			catch (Exception ex)
			{
				DebugUtil.LogException(null, "Exception in RegisterBuilding for type " + type2.FullName + " from " + type2.Assembly.GetName().Name, ex);
			}
		}
		foreach (PlanScreen.PlanInfo planInfo in BUILDINGS.PLANORDER)
		{
			planInfo.data.RemoveAll((string prefabId) => Assets.GetBuildingDef(prefabId) == null);
		}
	}

	public static void MakeBuildingAlwaysOperational(GameObject go)
	{
		BuildingDef def = go.GetComponent<BuildingComplete>().Def;
		if (def.LogicInputPorts != null || def.LogicOutputPorts != null)
		{
			global::Debug.LogWarning("Do not call MakeBuildingAlwaysOperational directly if LogicInputPorts or LogicOutputPorts are defined. Instead set BuildingDef.AlwaysOperational = true");
		}
		GeneratedBuildings.MakeBuildingAlwaysOperationalImpl(go);
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

	public static void RegisterSingleLogicInputPort(GameObject go)
	{
		LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0)).ToArray();
		logicPorts.outputPortInfo = null;
	}

	private static void MakeBuildingAlwaysOperationalImpl(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<Operational>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicPorts>());
	}

	public static void InitializeLogicPorts(GameObject go, BuildingDef def)
	{
		if (def.AlwaysOperational)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperationalImpl(go);
		}
		if (def.LogicInputPorts != null)
		{
			go.AddOrGet<LogicPorts>().inputPortInfo = def.LogicInputPorts.ToArray();
		}
		if (def.LogicOutputPorts != null)
		{
			go.AddOrGet<LogicPorts>().outputPortInfo = def.LogicOutputPorts.ToArray();
		}
	}

	public static void InitializeHighEnergyParticlePorts(GameObject go, BuildingDef def)
	{
		if (def.UseHighEnergyParticleInputPort || def.UseHighEnergyParticleOutputPort)
		{
			HighEnergyParticlePort highEnergyParticlePort = go.AddOrGet<HighEnergyParticlePort>();
			highEnergyParticlePort.particleInputOffset = def.HighEnergyParticleInputOffset;
			highEnergyParticlePort.particleOutputOffset = def.HighEnergyParticleOutputOffset;
			highEnergyParticlePort.particleInputEnabled = def.UseHighEnergyParticleInputPort;
			highEnergyParticlePort.particleOutputEnabled = def.UseHighEnergyParticleOutputPort;
		}
	}
}
