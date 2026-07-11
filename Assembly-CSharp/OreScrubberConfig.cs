using System;
using TUNING;
using UnityEngine;

public class OreScrubberConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "OreScrubber";
		int num = 3;
		int num2 = 3;
		string text2 = "orescrubber_kanim";
		int num3 = 30;
		float num4 = 30f;
		string[] array = new string[] { "Metal" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, new float[] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0] }, array, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.UtilityInputOffset = new CellOffset(1, 1);
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.InputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		OreScrubber oreScrubber = go.AddOrGet<OreScrubber>();
		oreScrubber.massConsumedPerUse = 0.07f;
		oreScrubber.consumedElement = SimHashes.ChlorineGas;
		oreScrubber.diseaseRemovalCount = 480000;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.ChlorineGas).tag;
		go.AddOrGet<DirectionControl>();
		OreScrubber.Work work = go.AddOrGet<OreScrubber.Work>();
		work.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_ore_scrubber_kanim") };
		work.workTime = 10.2f;
		work.trackUses = true;
		work.workLayer = Grid.SceneLayer.BuildingUse;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "OreScrubber";

	private const float MASS_PER_USE = 0.07f;

	private const int DISEASE_REMOVAL_COUNT = 480000;

	private const SimHashes CONSUMED_ELEMENT = SimHashes.ChlorineGas;
}
