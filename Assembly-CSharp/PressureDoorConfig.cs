using System;
using TUNING;
using UnityEngine;

public class PressureDoorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "PressureDoor";
		int num = 1;
		int num2 = 2;
		string text2 = "door_external_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 60f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.HotKey = global::Action.BuildMenuKeyE;
		SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Open_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_external_kanim", "Close_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, DoorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, DoorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Door door = go.UpdateComponentRequirement<Door>(true);
		door.hasComplexUserControls = true;
		door.unpoweredAnimSpeed = 1f;
		go.UpdateComponentRequirement<AccessControl>(true);
		go.UpdateComponentRequirement<BoxCollider2D>(true);
		Prioritizable.AddRef(go);
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
		GeneratedBuildings.RegisterLogicPorts(go, DoorConfig.INPUT_PORTS);
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		BuildingTemplates.DoPostConfigure(go);
		AccessControl component = go.GetComponent<AccessControl>();
		component.controlEnabled = true;
	}

	public const string ID = "PressureDoor";
}
