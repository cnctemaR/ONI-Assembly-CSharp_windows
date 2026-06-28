using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class DoorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Door";
		int num = 1;
		int num2 = 2;
		string text2 = "door_internal_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none);
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.HotKey = global::Action.BuildMenuKeyD;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
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
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		AccessControl accessControl = go.UpdateComponentRequirement<AccessControl>(true);
		accessControl.controlEnabled = true;
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 3f;
		go.AddOrGet<BoxCollider2D>();
		Prioritizable.AddRef(go);
		GeneratedBuildings.RegisterLogicPorts(go, DoorConfig.INPUT_PORTS);
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public const string ID = "Door";

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(Door.OPEN_CLOSE_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_PORT_DESC, false)
	};
}
