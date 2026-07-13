using System;
using TUNING;
using UnityEngine;

public class CampfireConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC2;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "Campfire";
		int num = 1;
		int num2 = 2;
		string text2 = "campfire_small_kanim";
		int num3 = 100;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, CampfireConfig.DECOR_ON, none, 0.1f);
		buildingDef.Floodable = true;
		buildingDef.Entombable = true;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.DefaultAnimState = "on";
		buildingDef.OverheatTemperature = 10000f;
		buildingDef.Overheatable = false;
		buildingDef.POIUnlockable = true;
		buildingDef.ShowInBuildMenu = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(RoomConstraints.ConstraintTags.WarmingStation, false);
		component.AddTag(RoomConstraints.ConstraintTags.Decoration, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 45f;
		storage.showInUI = true;
		storage.allowItemRemoval = false;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.capacity = 45f;
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = CampfireConfig.FUEL_TAG;
		manualDeliveryKG.refillMass = 18f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		manualDeliveryKG.MinimumMass = 0.025f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(CampfireConfig.FUEL_TAG, 0.025f, true)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.004f, SimHashes.CarbonDioxide, 303.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0, true)
		};
		this.AddVisualizer(go);
		Operational operational = go.AddOrGet<Operational>();
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.Range = 6f;
		light2D.Color = new Color(0.8f, 0.6f, 0f, 1f);
		light2D.Lux = 450;
		Campfire.Def def = go.AddOrGetDef<Campfire.Def>();
		def.fuelTag = CampfireConfig.FUEL_TAG;
		def.initialFuelMass = 5f;
		WarmthProvider.Def def2 = go.AddOrGetDef<WarmthProvider.Def>();
		def2.RangeMax = new Vector2I(4, 3);
		def2.RangeMin = new Vector2I(-4, 0);
		go.AddOrGetDef<ColdImmunityProvider.Def>().range = new CellOffset[][]
		{
			new CellOffset[]
			{
				new CellOffset(-1, 0),
				new CellOffset(1, 0)
			},
			new CellOffset[]
			{
				new CellOffset(0, 0)
			}
		};
		DirectVolumeHeater directVolumeHeater = go.AddOrGet<DirectVolumeHeater>();
		directVolumeHeater.operational = operational;
		directVolumeHeater.DTUs = 20000f;
		directVolumeHeater.width = 9;
		directVolumeHeater.height = 4;
		directVolumeHeater.maximumExternalTemperature = 343.15f;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		this.AddVisualizer(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		this.AddVisualizer(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	private void AddVisualizer(GameObject go)
	{
		RangeVisualizer rangeVisualizer = go.AddOrGet<RangeVisualizer>();
		rangeVisualizer.RangeMax = new Vector2I(4, 3);
		rangeVisualizer.RangeMin = new Vector2I(-4, 0);
		rangeVisualizer.BlockingTileVisible = false;
		go.AddOrGet<EntityCellVisualizer>().AddPort(EntityCellVisualizer.Ports.HeatSource, default(CellOffset));
	}

	public const string ID = "Campfire";

	public const int RANGE_X = 4;

	public const int RANGE_Y = 3;

	public static Tag FUEL_TAG = "BuildingWood";

	public const float FUEL_CONSUMPTION_RATE = 0.025f;

	public const float FUEL_CONSTRUCTION_MASS = 5f;

	public const float FUEL_CAPACITY = 45f;

	public const float EXHAUST_RATE = 0.004f;

	public const SimHashes EXHAUST_TAG = SimHashes.CarbonDioxide;

	private const float EXHAUST_TEMPERATURE = 303.15f;

	public static readonly EffectorValues DECOR_ON = BUILDINGS.DECOR.BONUS.TIER3;

	public static readonly EffectorValues DECOR_OFF = BUILDINGS.DECOR.NONE;
}
