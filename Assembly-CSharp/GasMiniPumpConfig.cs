using System;
using TUNING;
using UnityEngine;

public class GasMiniPumpConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GasMiniPump";
		int num = 1;
		int num2 = 2;
		string text2 = "minigaspump_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] plastics = MATERIALS.PLASTICS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, plastics, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = true;
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, "GasMiniPump");
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<Pump>();
		go.AddOrGet<Storage>().capacityKg = 0.1f;
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 0.05f;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showInStatusPanel = false;
		elementConsumer.consumptionRadius = 2;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Gas;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
		go.AddOrGetDef<OperationalController.Def>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
	}

	public const string ID = "GasMiniPump";
}
