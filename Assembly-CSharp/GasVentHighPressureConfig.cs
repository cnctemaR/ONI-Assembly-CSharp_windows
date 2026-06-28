using System;
using TUNING;
using UnityEngine;

public class GasVentHighPressureConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GasVentHighPressure";
		int num = 1;
		int num2 = 1;
		string text2 = "ventgas_powered_kanim";
		int num3 = 30;
		float num4 = 30f;
		string[] array = new string[] { "RefinedMetal", "Plastic" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, array, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		SoundEventVolumeCache.instance.AddVolume("ventgas_kanim", "GasVent_clunk", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Exhaust>();
		Vent vent = go.AddOrGet<Vent>();
		vent.conduitType = ConduitType.Gas;
		vent.endpointType = Endpoint.Sink;
		vent.overpressureMass = 20f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.ignoreMinMassCheck = true;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		go.AddOrGet<SimpleVent>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			VentController.Instance instance = new VentController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "GasVentHighPressure";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	public const float OVERPRESSURE_MASS = 20f;
}
