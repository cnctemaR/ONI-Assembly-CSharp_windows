using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PowerControlStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "PowerControlStation";
		int num = 2;
		int num2 = 4;
		string text2 = "electricianworkdesk_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ViewMode = SimViewMode.Rooms;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.PowerStation);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, PowerControlStationConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, PowerControlStationConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, PowerControlStationConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		BuildingTemplates.DoPostConfigure(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		storage.showInUI = true;
		storage.storageFilters = new List<Tag> { PowerControlStationConfig.MATERIAL_FOR_TINKER };
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		tinkerStation.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_electricianworkdesk_kanim") };
		tinkerStation.inputMaterial = PowerControlStationConfig.MATERIAL_FOR_TINKER;
		tinkerStation.massPerTinker = 5f;
		tinkerStation.outputPrefab = PowerControlStationConfig.TINKER_TOOLS;
		tinkerStation.requiredRolePerk = PowerControlStationConfig.ROLE_PERK;
		tinkerStation.choreType = Db.Get().ChoreTypes.PowerFabricate.IdHash;
		tinkerStation.useFilteredStorage = true;
		tinkerStation.fetchChoreType = Db.Get().ChoreTypes.PowerFetch.IdHash;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.PowerPlant.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "PowerControlStation";

	public static Tag MATERIAL_FOR_TINKER = GameTags.RefinedMetal;

	public static Tag TINKER_TOOLS = PowerStationToolsConfig.tag;

	public const float MASS_PER_TINKER = 5f;

	public static string ROLE_PERK = "CanPowerTinker";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
