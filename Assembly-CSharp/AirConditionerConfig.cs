using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AirConditionerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "AirConditioner";
		int num = 2;
		int num2 = 2;
		string text2 = "airconditioner_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ThermalConductivity = 5f;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
		airConditioner.temperatureDelta = -14f;
		airConditioner.maxEnvironmentDelta = -50f;
		go.AddOrGet<KBatchedAnimHeatPostProcessingEffect>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}

	public const string ID = "AirConditioner";
}
