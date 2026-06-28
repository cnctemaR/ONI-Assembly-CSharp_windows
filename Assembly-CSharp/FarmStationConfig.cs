using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FarmStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FarmStation";
		int num = 2;
		int num2 = 3;
		string text2 = "planttender_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ViewMode = SimViewMode.Rooms;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.FarmStation);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FarmStationConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FarmStationConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FarmStationConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		BuildingTemplates.DoPostConfigure(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = FarmStationConfig.MATERIAL_FOR_TINKER;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.capacity = 50f;
		manualDeliveryKG.choreTags = new Tag[] { GameTags.ChoreTypes.Farming };
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
		TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
		tinkerStation.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_planttender_kanim") };
		tinkerStation.inputMaterial = FarmStationConfig.MATERIAL_FOR_TINKER;
		tinkerStation.metalPerTinker = 5f;
		tinkerStation.outputPrefab = FarmStationConfig.TINKER_TOOLS;
		tinkerStation.requiredRolePerk = "CanFarmTinker";
		tinkerStation.choreType = Db.Get().ChoreTypes.FarmingFabricate.IdHash;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Farm.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "FarmStation";

	public static Tag MATERIAL_FOR_TINKER = GameTags.Fertilizer;

	public static Tag TINKER_TOOLS = FarmStationToolsConfig.tag;

	public const float METAL_PER_TINKER = 5f;

	public const string ROLE_TYPE = "Farmer";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
