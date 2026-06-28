using System;
using TUNING;
using UnityEngine;

public class CompostConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Compost";
		int num = 2;
		int num2 = 2;
		string text2 = "compost_kanim";
		float num3 = 400f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER3, none);
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.Overheatable = false;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		SoundEventVolumeCache.instance.AddVolume("anim_interacts_compost_kanim", "Compost_shovel_in", NOISE_POLLUTION.NOISY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("anim_interacts_compost_kanim", "Compost_shovel_out", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 2000f;
		Compost compost = go.AddOrGet<Compost>();
		compost.emitHash = SimHashes.Fertilizer;
		compost.emitMassThreshold = 10f;
		compost.simulatedInternalTemperature = 348.15f;
		CompostWorkable compostWorkable = go.AddOrGet<CompostWorkable>();
		compostWorkable.workTime = 20f;
		compostWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_compost_kanim") };
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(GameTags.Compostable, 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.1f, SimHashes.Fertilizer, 348.15f, true, 0.5f, 1f, false, 1f, byte.MaxValue, 0)
		};
		elementConverter.conversionInterval = 1f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTags.Compostable;
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 100f;
		manualDeliveryKG.minimumMass = 1f;
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "Compost";

	public const float SAND_INPUT_PER_SECOND = 0.1f;

	public const float FERTILIZER_OUTPUT_PER_SECOND = 0.1f;

	public const float FERTILIZER_OUTPUT_TEMP = 348.15f;
}
