using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GroundTrapConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CreatureGroundTrap", 2, 2, "critter_trap_ground_kanim", 10, 10f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort("TRAP_HAS_PREY_STATUS_PORT", new CellOffset(1, 0), global::STRINGS.BUILDINGS.PREFABS.REUSABLETRAP.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.REUSABLETRAP.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.REUSABLETRAP.LOGIC_PORT_INACTIVE, false, false) };
		buildingDef.AudioCategory = "Metal";
		buildingDef.Floodable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Prioritizable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<ArmTrapWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_critter_trap_ground_kanim") };
		go.AddOrGet<Operational>();
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = true;
		storage.SetDefaultStoredItemModifiers(GroundTrapConfig.StoredItemModifiers);
		storage.sendOnStoreOnSpawn = true;
		TrapTrigger trapTrigger = go.AddOrGet<TrapTrigger>();
		trapTrigger.trappableCreatures = new Tag[]
		{
			GameTags.Creatures.Walker,
			GameTags.Creatures.Hoverer,
			GameTags.Creatures.Swimmer
		};
		trapTrigger.trappedOffset = new Vector2(0.5f, 0f);
		go.AddOrGetDef<ReusableTrap.Def>().OUTPUT_LOGIC_PORT_ID = "TRAP_HAS_PREY_STATUS_PORT";
		LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
		logicPorts.inputPortInfo = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0)).ToArray();
		logicPorts.outputPortInfo = null;
		go.AddOrGet<LogicOperationalController>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "CreatureGroundTrap";

	public const string OUTPUT_LOGIC_PORT_ID = "TRAP_HAS_PREY_STATUS_PORT";

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>();
}
