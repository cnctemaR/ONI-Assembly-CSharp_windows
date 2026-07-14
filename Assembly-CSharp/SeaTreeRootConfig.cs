using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SeaTreeRootConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "SeaTree";
		string text2 = global::STRINGS.CREATURES.SPECIES.SEATREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SEATREE.DESC;
		float num = 2f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("sea_fairy_plant_kanim"), "grow", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 302.65f);
		string text4 = "SeaTreeOriginal";
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 248.15f, 295.15f, 310.15f, 398.15f, PLANTS.SAFE_ELEMENTS.AllWaters, false, 0f, 0.15f, null, false, false, true, false, false, 2400f, 0f, 2200f, text4, global::STRINGS.CREATURES.SPECIES.SEATREE.NAME);
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		WiltCondition component = gameObject.GetComponent<WiltCondition>();
		component.WiltDelay = 0f;
		component.RecoveryDelay = 0f;
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component2.PrefabID().ToString());
		gameObject.AddOrGet<Traits>();
		Db.Get().traits.Get(text4);
		gameObject.GetComponent<Modifiers>().initialTraits.Add(text4);
		SeaTreeRoot.Def def = gameObject.AddOrGetDef<SeaTreeRoot.Def>();
		def.BRANCH_PREFAB_NAME = "SeaTreeBranch";
		def.MAX_BRANCH_COUNT = 8;
		gameObject.AddOrGet<HarvestDesignatable>();
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.DirtyWater.CreateTag(),
				massConsumptionRate = 0.05f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.ToxicSand.CreateTag(),
				massConsumptionRate = 0.06666667f
			}
		});
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string text5 = "SeaTreeSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.SEATREE.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.SEATREE.DESC;
		KAnimFile anim = Assets.GetAnim("seed_sea_plant_kanim");
		string text8 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.WaterSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text9 = global::STRINGS.CREATURES.SPECIES.SEATREE.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text5, text6, text7, anim, text8, num2, list, receptacleDirection, default(Tag), 12, text9, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, null, "", false), "SeaTree_preview", Assets.GetAnim("sea_fairy_plant_kanim"), "place", 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SeaTree";

	public const string SEED_ID = "SeaTreeSeed";

	public const int MAX_BRANCH_COUNT = 8;

	public const float FERTILIZATION_KG_PER_CYCLE = 40f;

	public const float IRRIGATION_KG_PER_CYCLE = 30f;

	public const float FERTILIZATION_RATE = 0.06666667f;

	public const float IRRIGATION_RATE = 0.05f;

	public const float TEMPERATURE_LETHAL_LOW = 248.15f;

	public const float TEMPERATURE_WARNING_LOW = 295.15f;

	public const float TEMPERATURE_WARNING_HIGH = 310.15f;

	public const float TEMPERATURE_LETHAL_HIGH = 398.15f;
}
