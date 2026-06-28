using System;
using Klei.AI;
using UnityEngine;

public class BuildingConfigManager : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		BuildingConfigManager.Instance = this;
		this.baseTemplate = new GameObject("BuildingTemplate");
		this.baseTemplate.SetActive(false);
		this.baseTemplate.AddComponent<KPrefabID>();
		this.baseTemplate.AddComponent<KSelectable>();
		this.baseTemplate.AddComponent<Modifiers>();
		this.baseTemplate.AddComponent<PrimaryElement>();
		this.baseTemplate.AddComponent<BuildingComplete>();
		this.baseTemplate.AddComponent<ChoreProvider>();
		this.baseTemplate.AddComponent<Notifier>();
		this.baseTemplate.AddComponent<StateMachineController>();
		this.baseTemplate.AddComponent<Deconstructable>();
		this.baseTemplate.AddComponent<UserMenu>();
		this.baseTemplate.AddComponent<SaveLoadRoot>();
		this.baseTemplate.AddComponent<OccupyArea>();
		this.baseTemplate.AddComponent<DecorProvider>();
		this.baseTemplate.AddComponent<Operational>();
		this.baseTemplate.AddComponent<BuildingEnabledButton>();
		this.baseTemplate.AddComponent<RequiresFoundation>();
		this.baseTemplate.AddComponent<Prioritizable>();
	}

	public void RegisterBuilding(IBuildingConfig config)
	{
		BuildingDef buildingDef = config.CreateBuildingDef();
		GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.baseTemplate);
		gameObject.name = buildingDef.PrefabID;
		gameObject.transform.parent = SceneOrganizer.Instance.GetFolder(Folder.GlobalDoNotDestroy).transform;
		gameObject.GetComponent<Building>().Def = buildingDef;
		gameObject.GetComponent<OccupyArea>().OccupiedCellsOffsets = buildingDef.PlacementOffsets;
		buildingDef.BuildingTemplate = gameObject;
		config.ConfigureBuildingTemplate(gameObject);
		buildingDef.BuildingComplete = BuildingLoader.Instance.CreateBuildingComplete(buildingDef);
		bool flag = true;
		for (int i = 0; i < this.NonBuildableBuildings.Length; i++)
		{
			if (buildingDef.PrefabID == this.NonBuildableBuildings[i])
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			buildingDef.BuildingUnderConstruction = BuildingLoader.Instance.CreateBuildingUnderConstruction(buildingDef, false);
			buildingDef.BuildingUnderRelocation = BuildingLoader.Instance.CreateBuildingUnderConstruction(buildingDef, true);
			buildingDef.BuildingPreview = BuildingLoader.Instance.CreateBuildingPreview(buildingDef);
			buildingDef.BuildingPackage = BuildingLoader.Instance.CreateBuildingPackage(buildingDef);
		}
		buildingDef.PostProcess();
		config.DoPostConfigureComplete(buildingDef.BuildingComplete);
		if (flag)
		{
			config.DoPostConfigurePreview(buildingDef, buildingDef.BuildingPreview);
			config.DoPostConfigureUnderConstruction(buildingDef.BuildingUnderConstruction);
		}
		Assets.AddBuildingDef(buildingDef);
	}

	public static BuildingConfigManager Instance;

	private GameObject baseTemplate;

	private string[] NonBuildableBuildings = new string[] { "Headquarters", "POIBunkerExteriorDoor" };
}
