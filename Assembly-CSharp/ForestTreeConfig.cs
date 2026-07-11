using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ForestTreeConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "ForestTree";
		string text2 = global::STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC;
		float num = 2f;
		KAnimFile kanimFile = Assets.GetAnim("tree_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		List<Tag> list = new List<Tag>();
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.Building, 1, 2, tier, default(EffectorValues), SimHashes.Creature, list, 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 288.15f, 313.15f, 448.15f, null, true, 0f, 0.15f, "WoodLog", true, true, true, false, 2400f);
		gameObject.AddOrGet<BuddingTrunk>();
		gameObject.UpdateComponentRequirement<Harvestable>(false);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.11666667f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.016666668f
			}
		});
		gameObject.AddComponent<StandardCropPlant>();
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, -1);
		BuddingTrunk buddingTrunk = gameObject.AddOrGet<BuddingTrunk>();
		buddingTrunk.budPrefabID = "ForestTreeBranch";
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		text4 = "ForestTreeSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.DESC;
		kanimFile = Assets.GetAnim("seed_tree_kanim");
		list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.WOOD_TREE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "ForestTree_preview", Assets.GetAnim("tree_kanim"), "place", 3, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ForestTree";

	public const string SEED_ID = "ForestTreeSeed";

	public const float FERTILIZATION_RATE = 0.016666668f;

	public const float WATER_RATE = 0.11666667f;

	public const float BRANCH_GROWTH_TIME = 2100f;

	public const int NUM_BRANCHES = 7;
}
