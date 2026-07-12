using System;
using System.Collections.Generic;
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
		int num3 = 30;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Tile;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none, 1f);
		buildingDef.Entombable = true;
		buildingDef.Floodable = false;
		buildingDef.IsFoundation = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.ForegroundLayer = Grid.SceneLayer.InteriorWall;
		buildingDef.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public static List<LogicPorts.Port> CreateSingleInputPortList(CellOffset offset)
	{
		return new List<LogicPorts.Port> { LogicPorts.Port.InputPort(Door.OPEN_CLOSE_PORT_ID, offset, global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN, global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_INACTIVE, false, false) };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		door.doorOpeningSoundEventName = "Open_DoorInternal";
		door.doorClosingSoundEventName = "Close_DoorInternal";
		go.AddOrGet<AccessControl>().controlEnabled = true;
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Door;
		go.AddOrGet<Workable>().workTime = 3f;
		go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
		go.AddOrGet<ZoneTile>();
		go.AddOrGet<KBoxCollider2D>();
		Prioritizable.AddRef(go);
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		go.AddTag(GameTags.NoCreatureIdling);
	}

	public const string ID = "Door";
}
