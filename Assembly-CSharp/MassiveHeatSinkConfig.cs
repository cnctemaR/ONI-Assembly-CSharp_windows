using System;
using TUNING;
using UnityEngine;

public class MassiveHeatSinkConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MassiveHeatSink";
		int num = 4;
		int num2 = 4;
		string text2 = "massiveheatsink_kanim";
		float num3 = 800f;
		int num4 = 100;
		float num5 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num6 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_METALS, num6, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER2, tier2);
		buildingDef.ExhaustKilowattsWhenActive = -16f;
		buildingDef.OperatingKilowatts = -64f;
		buildingDef.Floodable = true;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<MassiveHeatSink>();
		MinimumOperatingTemperature minimumOperatingTemperature = go.AddOrGet<MinimumOperatingTemperature>();
		minimumOperatingTemperature.minimumTemperature = 100f;
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Iron);
		component.Temperature = 294.15f;
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 2f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f)
		};
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "MassiveHeatSink";
}
