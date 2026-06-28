using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class AirborneCreatureLureConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AirborneCreatureLure", 1, 3, "airbornecreaturetrap_kanim", 400f, 10, 10f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		CreatureLure creatureLure = go.UpdateComponentRequirement<CreatureLure>(true);
		creatureLure.lurePoints = new CellOffset[]
		{
			new CellOffset(0, 5),
			new CellOffset(-1, 4),
			new CellOffset(0, 4),
			new CellOffset(1, 4),
			new CellOffset(-2, 3),
			new CellOffset(-1, 3),
			new CellOffset(0, 3),
			new CellOffset(1, 3),
			new CellOffset(2, 3),
			new CellOffset(-1, 2),
			new CellOffset(0, 2),
			new CellOffset(1, 2),
			new CellOffset(0, 1)
		};
		creatureLure.baitStorage = go.UpdateComponentRequirement<Storage>(true);
		creatureLure.baitTypes = new List<Tag>
		{
			GameTags.SlimeMold,
			GameTags.Phosphorite
		};
		creatureLure.baitStorage.storageFilters = creatureLure.baitTypes;
		creatureLure.baitStorage.allowItemRemoval = false;
		creatureLure.baitStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		go.UpdateComponentRequirement<Operational>(true);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, AirborneCreatureLureConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, AirborneCreatureLureConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, AirborneCreatureLureConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
	}

	public const string ID = "AirborneCreatureLure";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
