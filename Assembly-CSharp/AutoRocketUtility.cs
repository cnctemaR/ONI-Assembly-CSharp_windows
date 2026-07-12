using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoRocketUtility
{
	public static void StartAutoRocket(LaunchPad selectedPad)
	{
		selectedPad.StartCoroutine(AutoRocketUtility.AutoRocketRoutine(selectedPad));
	}

	private static IEnumerator AutoRocketRoutine(LaunchPad selectedPad)
	{
		GameObject gameObject = AutoRocketUtility.AddEngine(selectedPad);
		GameObject oxidizerTank = AutoRocketUtility.AddOxidizerTank(gameObject);
		yield return SequenceUtil.WaitForEndOfFrame;
		AutoRocketUtility.AddOxidizer(oxidizerTank);
		GameObject gameObject2 = AutoRocketUtility.AddPassengerModule(oxidizerTank);
		AutoRocketUtility.AddDrillCone(AutoRocketUtility.AddSolidStorageModule(gameObject2));
		PassengerRocketModule passengerModule = gameObject2.GetComponent<PassengerRocketModule>();
		ClustercraftExteriorDoor exteriorDoor = passengerModule.GetComponent<ClustercraftExteriorDoor>();
		int max = 100;
		while (exteriorDoor.GetInteriorDoor() == null && max > 0)
		{
			int num = max;
			max = num - 1;
			yield return SequenceUtil.WaitForEndOfFrame;
		}
		WorldContainer interiorWorld = passengerModule.GetComponent<RocketModuleCluster>().CraftInterface.GetInteriorWorld();
		RocketControlStation rocketControlStation = Components.RocketControlStations.GetWorldItems(interiorWorld.id, false)[0];
		GameObject minion = AutoRocketUtility.AddPilot(rocketControlStation);
		AutoRocketUtility.AddOxygen(rocketControlStation);
		yield return SequenceUtil.WaitForEndOfFrame;
		AutoRocketUtility.AssignCrew(minion, passengerModule);
		yield break;
	}

	private static GameObject AddEngine(LaunchPad selectedPad)
	{
		BuildingDef buildingDef = Assets.GetBuildingDef("KeroseneEngineClusterSmall");
		List<Tag> list = new List<Tag> { SimHashes.Steel.CreateTag() };
		GameObject gameObject = selectedPad.AddBaseModule(buildingDef, list);
		Element element = ElementLoader.GetElement(gameObject.GetComponent<RocketEngineCluster>().fuelTag);
		Storage component = gameObject.GetComponent<Storage>();
		if (element.IsGas)
		{
			component.AddGasChunk(element.id, component.Capacity(), element.defaultValues.temperature, byte.MaxValue, 0, false, true);
			return gameObject;
		}
		if (element.IsLiquid)
		{
			component.AddLiquid(element.id, component.Capacity(), element.defaultValues.temperature, byte.MaxValue, 0, false, true);
			return gameObject;
		}
		if (element.IsSolid)
		{
			component.AddOre(element.id, component.Capacity(), element.defaultValues.temperature, byte.MaxValue, 0, false, true);
		}
		return gameObject;
	}

	private static GameObject AddPassengerModule(GameObject baseModule)
	{
		ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
		BuildingDef buildingDef = Assets.GetBuildingDef("HabitatModuleMedium");
		List<Tag> list = new List<Tag> { SimHashes.Cuprite.CreateTag() };
		return component.AddModule(buildingDef, list);
	}

	private static GameObject AddSolidStorageModule(GameObject baseModule)
	{
		ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
		BuildingDef buildingDef = Assets.GetBuildingDef("SolidCargoBaySmall");
		List<Tag> list = new List<Tag> { SimHashes.Steel.CreateTag() };
		return component.AddModule(buildingDef, list);
	}

	private static GameObject AddDrillCone(GameObject baseModule)
	{
		ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
		BuildingDef buildingDef = Assets.GetBuildingDef("NoseconeHarvest");
		List<Tag> list = new List<Tag>
		{
			SimHashes.Steel.CreateTag(),
			SimHashes.Polypropylene.CreateTag()
		};
		GameObject gameObject = component.AddModule(buildingDef, list);
		gameObject.GetComponent<Storage>().AddOre(SimHashes.Diamond, 1000f, 273f, byte.MaxValue, 0, false, true);
		return gameObject;
	}

	private static GameObject AddOxidizerTank(GameObject baseModule)
	{
		ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
		BuildingDef buildingDef = Assets.GetBuildingDef("SmallOxidizerTank");
		List<Tag> list = new List<Tag> { SimHashes.Cuprite.CreateTag() };
		return component.AddModule(buildingDef, list);
	}

	private static void AddOxidizer(GameObject oxidizerTank)
	{
		SimHashes simHashes = SimHashes.OxyRock;
		Element element = ElementLoader.FindElementByHash(simHashes);
		DiscoveredResources.Instance.Discover(element.tag, element.GetMaterialCategoryTag());
		oxidizerTank.GetComponent<OxidizerTank>().DEBUG_FillTank(simHashes);
	}

	private static GameObject AddPilot(RocketControlStation station)
	{
		Vector3 position = station.transform.position;
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(vector);
		gameObject.SetActive(true);
		new MinionStartingStats(false, null, null).Apply(gameObject);
		MinionResume component = gameObject.GetComponent<MinionResume>();
		if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
		{
			component.ForceAddSkillPoint();
		}
		string id = Db.Get().Skills.RocketPiloting1.Id;
		MinionResume.SkillMasteryConditions[] skillMasteryConditions = component.GetSkillMasteryConditions(id);
		bool flag = component.CanMasterSkill(skillMasteryConditions);
		if (component != null && !component.HasMasteredSkill(id) && flag)
		{
			component.MasterSkill(id);
		}
		return gameObject;
	}

	private static void AddOxygen(RocketControlStation station)
	{
		SimMessages.ReplaceElement(Grid.PosToCell(station.transform.position + Vector3.up * 2f), SimHashes.OxyRock, CellEventLogger.Instance.DebugTool, 1000f, 273f, byte.MaxValue, 0, -1);
	}

	private static void AssignCrew(GameObject minion, PassengerRocketModule passengerModule)
	{
		for (int i = 0; i < Components.MinionAssignablesProxy.Count; i++)
		{
			if (Components.MinionAssignablesProxy[i].GetTargetGameObject() == minion)
			{
				passengerModule.GetComponent<AssignmentGroupController>().SetMember(Components.MinionAssignablesProxy[i], true);
				break;
			}
		}
		passengerModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Request);
	}

	private static void SetDestination(CraftModuleInterface craftModuleInterface, PassengerRocketModule passengerModule)
	{
		craftModuleInterface.GetComponent<ClusterDestinationSelector>().SetDestination(passengerModule.GetMyWorldLocation() + AxialI.NORTHEAST);
	}
}
