using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpaceTreeConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC2;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "SpaceTree";
		string text2 = global::STRINGS.CREATURES.SPECIES.SPACETREE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SPACETREE.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("syrup_tree_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 255f);
		string text4 = "SpaceTreeOriginal";
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 173.15f, 198.15f, 258.15f, 293.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.Snow,
			SimHashes.Vacuum
		}, false, 0f, 0.15f, null, true, false, true, false, false, 2400f, 0f, 12200f, text4, global::STRINGS.CREATURES.SPECIES.SPACETREE.NAME);
		WiltCondition component = gameObject.GetComponent<WiltCondition>();
		component.WiltDelay = 0f;
		component.RecoveryDelay = 0f;
		Modifiers component2 = gameObject.GetComponent<Modifiers>();
		if (gameObject.GetComponent<Traits>() == null)
		{
			gameObject.AddOrGet<Traits>();
			component2.initialTraits.Add(text4);
		}
		Crop.CropVal cropVal = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == SimHashes.SugarWater.CreateTag());
		Klei.AI.Modifier modifier = Db.Get().traits.Get(component2.initialTraits[0]);
		component2.initialAmounts.Add(Db.Get().Amounts.Maturity.Id);
		modifier.Add(new AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute.Id, 4.5f, global::STRINGS.CREATURES.SPECIES.SPACETREE.NAME, false, false, true));
		gameObject.AddOrGet<Crop>().Configure(cropVal);
		KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component3.PrefabID().ToString());
		if (DlcManager.FeaturePlantMutationsEnabled())
		{
			gameObject.AddOrGet<MutantPlant>().SpeciesID = component3.PrefabTag;
			SymbolOverrideControllerUtil.AddToPrefab(gameObject);
		}
		Growing growing = gameObject.AddOrGet<Growing>();
		growing.shouldGrowOld = false;
		growing.maxAge = 2400f;
		gameObject.AddOrGet<HarvestDesignatable>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text5 = "SpaceTreeSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.SPACETREE.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.SPACETREE.DESC;
		KAnimFile anim = Assets.GetAnim("seed_syrup_tree_kanim");
		string text8 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text9 = global::STRINGS.CREATURES.SPECIES.SPACETREE.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text5, text6, text7, anim, text8, num2, list, receptacleDirection, default(Tag), 1, text9, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Snow.CreateTag(),
				massConsumptionRate = 0.16666667f
			}
		});
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "SpaceTree_preview", Assets.GetAnim("syrup_tree_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("meallice_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("meallice_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
		DirectlyEdiblePlant_StorageElement directlyEdiblePlant_StorageElement = gameObject.AddOrGet<DirectlyEdiblePlant_StorageElement>();
		directlyEdiblePlant_StorageElement.tagToConsume = SimHashes.SugarWater.CreateTag();
		directlyEdiblePlant_StorageElement.rateProducedPerCycle = 4f;
		directlyEdiblePlant_StorageElement.storageCapacity = 20f;
		directlyEdiblePlant_StorageElement.edibleCellOffsets = new CellOffset[]
		{
			new CellOffset(-1, 0),
			new CellOffset(1, 0),
			new CellOffset(-1, 1),
			new CellOffset(1, 1)
		};
		DirectlyEdiblePlant_TreeBranches directlyEdiblePlant_TreeBranches = gameObject.AddOrGet<DirectlyEdiblePlant_TreeBranches>();
		directlyEdiblePlant_TreeBranches.overrideCropID = "SpaceTreeBranch";
		directlyEdiblePlant_TreeBranches.MinimumEdibleMaturity = 1f;
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.allowItemRemoval = false;
		storage.showInUI = true;
		storage.capacityKg = 20f;
		storage.SetDefaultStoredItemModifiers(SpaceTreeConfig.storedItemModifiers);
		ConduitDispenser conduitDispenser = gameObject.AddOrGet<ConduitDispenser>();
		conduitDispenser.noBuildingOutputCellOffset = SpaceTreeConfig.OUTPUT_CONDUIT_CELL_OFFSET;
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.SetOnState(false);
		gameObject.AddOrGet<SpaceTreeSyrupHarvestWorkable>();
		UnstableEntombDefense.Def def = gameObject.AddOrGetDef<UnstableEntombDefense.Def>();
		def.defaultAnimName = "shake_trunk";
		def.Cooldown = 5f;
		PlantBranchGrower.Def def2 = gameObject.AddOrGetDef<PlantBranchGrower.Def>();
		def2.BRANCH_OFFSETS = new CellOffset[]
		{
			new CellOffset(-1, 1),
			new CellOffset(-1, 2),
			new CellOffset(0, 2),
			new CellOffset(1, 2),
			new CellOffset(1, 1)
		};
		def2.BRANCH_PREFAB_NAME = "SpaceTreeBranch";
		def2.harvestOnDrown = true;
		def2.propagateHarvestDesignation = false;
		def2.MAX_BRANCH_COUNT = 5;
		SpaceTreePlant.Def def3 = gameObject.AddOrGetDef<SpaceTreePlant.Def>();
		def3.OptimalProductionDuration = 150f;
		def3.OptimalAmountOfBranches = 5;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		EntityCellVisualizer entityCellVisualizer = inst.AddOrGet<EntityCellVisualizer>();
		entityCellVisualizer.AddPort(EntityCellVisualizer.Ports.LiquidOut, SpaceTreeConfig.OUTPUT_CONDUIT_CELL_OFFSET, entityCellVisualizer.Resources.liquidIOColours.output.connected);
	}

	public const string ID = "SpaceTree";

	public const string SEED_ID = "SpaceTreeSeed";

	public const float Temperature_lethal_low = 173.15f;

	public const float Temperature_warning_low = 198.15f;

	public const float Temperature_warning_high = 258.15f;

	public const float Temperature_lethal_high = 293.15f;

	public const float SNOW_RATE = 0.16666667f;

	public const float ENTOMB_DEFENSE_COOLDOWN = 5f;

	public static CellOffset OUTPUT_CONDUIT_CELL_OFFSET = new CellOffset(0, 1);

	public const float TRUNK_GROWTH_DURATION = 2700f;

	public const int MAX_BRANCH_NUMBER = 5;

	public const int OPTIMAL_LUX = 10000;

	public const float MIN_REQUIRED_LIGHT_TO_GROW_BRANCHES = 300f;

	public const float SUGAR_WATER_PRODUCTION_DURATION = 150f;

	public const float SUGAR_WATER_CAPACITY = 20f;

	public const string MANUAL_HARVEST_PRE_ANIM_NAME = "syrup_harvest_trunk_pre";

	public const string MANUAL_HARVEST_LOOP_ANIM_NAME = "syrup_harvest_trunk_loop";

	public const string MANUAL_HARVEST_PST_ANIM_NAME = "syrup_harvest_trunk_pst";

	public const string MANUAL_HARVEST_INTERRUPT_ANIM_NAME = "syrup_harvest_trunk_loop";

	private static readonly List<Storage.StoredItemModifier> storedItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};
}
