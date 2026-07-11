using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidConduitInboxConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SolidConduitInbox";
		int num = 1;
		int num2 = 2;
		string text2 = "conveyorin_kanim";
		int num3 = 100;
		float num4 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.OutputConduitType = ConduitType.Solid;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduitInbox");
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SolidConduitInboxConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SolidConduitInboxConfig.INPUT_PORTS);
		Constructable component = go.GetComponent<Constructable>();
		component.choreTags = GameTags.ChoreTypes.ConveyorChores;
		component.requiredRolePerk = RoleManager.rolePerks.ConveyorBuild.id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SolidConduitInboxConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<Automatable>();
		List<Tag> list = new List<Tag>();
		list.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
		list.AddRange(STORAGEFILTERS.FOOD);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = list;
		storage.allowItemRemoval = false;
		storage.onlyTransferFromLowerPriority = true;
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<SolidConduitInbox>();
		go.AddOrGet<SolidConduitDispenser>();
	}

	public const string ID = "SolidConduitInbox";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
