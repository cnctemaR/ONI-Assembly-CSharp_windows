using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RemoteWorkTerminalConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = RemoteWorkTerminalConfig.ID;
		int num = 3;
		int num2 = 3;
		string text = "remote_work_terminal_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddComponent<RemoteWorkTerminal>().workTime = float.PositiveInfinity;
		go.AddComponent<RemoteWorkTerminalSM>();
		go.AddOrGet<Operational>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 100f;
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Insulate
		});
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = RemoteWorkTerminalConfig.INPUT_MATERIAL;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.ResearchFetch.IdHash;
		manualDeliveryKG.operationalRequirement = Operational.State.Functional;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(RemoteWorkTerminalConfig.INPUT_MATERIAL, 0.013333334f, true)
		};
		elementConverter.showDescriptors = false;
		go.AddOrGet<ElementConverterOperationalRequirement>();
		Prioritizable.AddRef(go);
	}

	public override string[] GetRequiredDlcIds()
	{
		return new string[] { "DLC3_ID" };
	}

	public static string ID = "RemoteWorkTerminal";

	public static readonly Tag INPUT_MATERIAL = DatabankHelper.TAG;

	public const float INPUT_CAPACITY = 10f;

	public const float INPUT_CONSUMPTION_RATE_PER_S = 0.013333334f;

	public const float INPUT_REFILL_RATIO = 0.5f;
}
