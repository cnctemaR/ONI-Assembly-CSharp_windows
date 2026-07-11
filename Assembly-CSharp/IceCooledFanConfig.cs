using System;
using TUNING;
using UnityEngine;

public class IceCooledFanConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "IceCooledFan";
		int num = 2;
		int num2 = 2;
		string text2 = "fanice_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = -this.COOLING_RATE;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Temperature.ID;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddComponent<Storage>();
		storage.capacityKg = 50f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 50f;
		storage2.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		MinimumOperatingTemperature minimumOperatingTemperature = go.AddOrGet<MinimumOperatingTemperature>();
		minimumOperatingTemperature.minimumTemperature = 273.15f;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		IceCooledFan iceCooledFan = go.AddOrGet<IceCooledFan>();
		iceCooledFan.coolingRate = this.COOLING_RATE;
		iceCooledFan.targetTemperature = this.TARGET_TEMPERATURE;
		iceCooledFan.iceStorage = storage;
		iceCooledFan.liquidStorage = storage2;
		iceCooledFan.minCooledTemperature = 278.15f;
		iceCooledFan.minEnvironmentMass = 0.25f;
		iceCooledFan.minCoolingRange = new Vector2I(-2, 0);
		iceCooledFan.maxCoolingRange = new Vector2I(2, 4);
		iceCooledFan.consumptionTag = GameTags.IceOre;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.IceOre;
		manualDeliveryKG.capacity = this.ICE_CAPACITY;
		manualDeliveryKG.refillMass = this.ICE_CAPACITY * 0.2f;
		manualDeliveryKG.minimumMass = 10f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		IceCooledFanWorkable iceCooledFanWorkable = go.AddOrGet<IceCooledFanWorkable>();
		iceCooledFanWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_icefan_kanim") };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "IceCooledFan";

	private float COOLING_RATE = 32f;

	private float TARGET_TEMPERATURE = 278.15f;

	private float ICE_CAPACITY = 50f;
}
