using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class GlassDeerConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateGlassDeer(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseDeerConfig.BaseDeer(id, name, desc, anim_file, "GlassDeerBaseTrait", is_baby, "gla_"), DeerTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("GlassDeerBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, 1000000f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -166.66667f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		GameObject gameObject2 = BaseDeerConfig.SetupDiet(gameObject, new List<Diet.Info>
		{
			BaseDeerConfig.CreateDietInfo("HardSkinBerryPlant", SimHashes.Dirt.CreateTag(), GlassDeerConfig.HARD_SKIN_CALORIES_PER_KG, 8.333334f, null, 0f),
			new Diet.Info(new HashSet<Tag> { "HardSkinBerry" }, SimHashes.Dirt.CreateTag(), GlassDeerConfig.CONSUMABLE_PLANT_MATURITY_LEVELS * GlassDeerConfig.HARD_SKIN_CALORIES_PER_KG / 1f, 25.000002f, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null),
			BaseDeerConfig.CreateDietInfo("PrickleFlower", SimHashes.Dirt.CreateTag(), GlassDeerConfig.BRISTLE_CALORIES_PER_KG / 2f, 8.333334f, null, 0f),
			new Diet.Info(new HashSet<Tag> { PrickleFruitConfig.ID }, SimHashes.Dirt.CreateTag(), GlassDeerConfig.CONSUMABLE_PLANT_MATURITY_LEVELS * GlassDeerConfig.BRISTLE_CALORIES_PER_KG / 1f, 50.000004f, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null),
			new Diet.Info(new HashSet<Tag> { SimHashes.Katairite.CreateTag() }, SimHashes.Dirt.CreateTag(), 5000f, 0.5f, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		}.ToArray(), 1f);
		WellFedShearable.Def def = gameObject2.AddOrGetDef<WellFedShearable.Def>();
		def.effectId = "GlassDeerWellFed";
		def.caloriesPerCycle = 100000f;
		def.growthDurationCycles = 6f;
		def.dropMass = 60f;
		def.requiredDiet = SimHashes.Katairite.CreateTag();
		def.itemDroppedOnShear = SimHashes.Glass.CreateTag();
		def.levelCount = 6;
		return gameObject2;
	}

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
		return EntityTemplates.ExtendEntityToFertileCreature(GlassDeerConfig.CreateGlassDeer("GlassDeer", global::STRINGS.CREATURES.SPECIES.GLASSDEER.NAME, global::STRINGS.CREATURES.SPECIES.GLASSDEER.DESC, "ice_floof_kanim", false), this, "GlassDeerEgg", global::STRINGS.CREATURES.SPECIES.GLASSDEER.EGG_NAME, global::STRINGS.CREATURES.SPECIES.GLASSDEER.DESC, "egg_ice_floof_kanim", DeerTuning.EGG_MASS, "GlassDeerBaby", 60.000004f, 20f, DeerTuning.EGG_CHANCES_GLASS, GlassDeerConfig.EGG_SORT_ORDER, true, false, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GlassDeer";

	public const string BASE_TRAIT_ID = "GlassDeerBaseTrait";

	public const string EGG_ID = "GlassDeerEgg";

	public static int EGG_SORT_ORDER = 0;

	public const SimHashes CONSUMED_ELEMENT = SimHashes.Katairite;

	public const SimHashes POOP_ELEMENT = SimHashes.Dirt;

	public const SimHashes SHEAR_ELEMENT = SimHashes.Glass;

	public const float ANTLER_GROWTH_TIME_IN_CYCLES = 6f;

	public const float ANTLER_STARTING_GROWTH_PCT = 0.5f;

	public const float ANTLER_MATERIAL_MASS_PER_CYCLE = 10f;

	public const float ANTLER_MASS_PER_ANTLER = 60f;

	public const float CALORIES_PER_PLANT_BITE = 100000f;

	public const float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 0.2f;

	public static float CONSUMABLE_PLANT_MATURITY_LEVELS = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == "HardSkinBerry").cropDuration / 600f;

	public static float KG_PLANT_EATEN_A_DAY = 0.2f * GlassDeerConfig.CONSUMABLE_PLANT_MATURITY_LEVELS;

	public static float HARD_SKIN_CALORIES_PER_KG = 100000f / GlassDeerConfig.KG_PLANT_EATEN_A_DAY;

	public static float BRISTLE_CALORIES_PER_KG = GlassDeerConfig.HARD_SKIN_CALORIES_PER_KG * 2f;

	public const float CALORIES_PER_BITE = 100000f;

	public const float KG_SOLIDS_EATEN_A_DAY = 20f;

	public const float CALORIES_PER_SOLID_KG = 5000f;

	public const float MIN_KG_CONSUMED_BEFORE_POOPING = 1f;

	public const float POOP_MASS_CONVERSION_MULTIPLIER = 0.5f;

	public const float POOP_MASS_PLANT_CONVERSION_MULTIPLIER = 8.333334f;
}
