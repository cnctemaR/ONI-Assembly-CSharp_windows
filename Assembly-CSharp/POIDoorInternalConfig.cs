using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class POIDoorInternalConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = POIDoorInternalConfig.ID;
		int num = 1;
		int num2 = 2;
		string text = "door_poi_internal_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none);
		buildingDef.ShowInBuildMenu = false;
		buildingDef.Entombable = false;
		buildingDef.Floodable = false;
		buildingDef.Invincible = true;
		buildingDef.IsFoundation = true;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_poi_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		go.UpdateComponentRequirement<AccessControl>(true);
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 3f;
		go.AddOrGet<BoxCollider2D>();
		Prioritizable.AddRef(go);
		GeneratedBuildings.RegisterLogicPorts(go, POIDoorInternalConfig.INPUT_PORTS);
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		AccessControl component = go.GetComponent<AccessControl>();
		Door component2 = go.GetComponent<Door>();
		component2.hasComplexUserControls = false;
		component.controlEnabled = false;
		go.GetComponent<Deconstructable>().allowDeconstruction = false;
	}

	public static string ID = "POIDoorInternal";

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(Door.OPEN_CLOSE_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_PORT_DESC, false)
	};
}
