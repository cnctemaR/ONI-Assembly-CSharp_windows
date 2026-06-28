using System;
using TUNING;
using UnityEngine;

public class SteamTurbineConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SteamTurbine";
		int num = 3;
		int num2 = 1;
		string text2 = "steamturbine_kanim";
		float num3 = 25f;
		int num4 = 30;
		float num5 = 60f;
		string[] array = new string[] { "Plastic", "Metal" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0]
		}, array, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.NONE, none);
		buildingDef.GeneratorWattageRating = 8000f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileFront;
		buildingDef.Deprecated = true;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.OverheatTemperature = 2273.15f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Turbine turbine = go.AddOrGet<Turbine>();
		turbine.srcElem = SimHashes.Steam;
		turbine.srcMinTemp = 473.15f;
		turbine.destTempDelta = -50f;
		turbine.pumpKGRate = 5f;
		turbine.srcMinMass = turbine.pumpKGRate;
		turbine.destMaxMass = turbine.pumpKGRate * 3f;
		turbine.minEmitMass = 15f;
		turbine.maxRPM = 5000f;
		turbine.rpmAcceleration = turbine.maxRPM / 30f;
		turbine.rpmDeceleration = turbine.maxRPM / 20f;
		turbine.minGenerationRPM = 4000f;
		go.AddOrGet<Generator>();
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.setLiquidImpermeable = false;
		simCellOccupier.setGasImpermeable = false;
		simCellOccupier.strengthMultiplier = 1f;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "SteamTurbine";
}
