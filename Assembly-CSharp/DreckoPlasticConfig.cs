using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class DreckoPlasticConfig : IEntityConfig
{
	public static GameObject CreateDrecko(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseDreckoConfig.BaseDrecko(id, name, desc, anim_file, "DreckoPlasticBaseTrait", is_baby, null, 298.15f, 333.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, DreckoTuning.PEN_SIZE_PER_CREATURE, 150f);
		Trait trait = Db.Get().CreateTrait("DreckoPlasticBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DreckoTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -DreckoTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name, false, false, true));
		TagBits tagBits = default(TagBits);
		tagBits.SetTag("BasicSingleHarvestPlant".ToTag());
		tagBits.SetTag("PrickleFlower".ToTag());
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(tagBits, DreckoPlasticConfig.POOP_ELEMENT, DreckoPlasticConfig.CALORIES_PER_DAY_OF_PLANT_EATEN, DreckoPlasticConfig.KG_POOP_PER_DAY_OF_PLANT, null, 0f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = DreckoPlasticConfig.MIN_POOP_SIZE_IN_CALORIES;
		ScaleGrowthMonitor.Def def2 = gameObject.AddOrGetDef<ScaleGrowthMonitor.Def>();
		def2.defaultGrowthRate = 1f / DreckoPlasticConfig.SCALE_GROWTH_TIME_IN_CYCLES / 600f;
		def2.dropMass = DreckoPlasticConfig.PLASTIC_PER_CYCLE * DreckoPlasticConfig.SCALE_GROWTH_TIME_IN_CYCLES;
		def2.itemDroppedOnShear = DreckoPlasticConfig.EMIT_ELEMENT;
		def2.levelCount = 6;
		def2.targetAtmosphere = SimHashes.Hydrogen;
		SolidConsumerMonitor.Def def3 = gameObject.AddOrGetDef<SolidConsumerMonitor.Def>();
		def3.diet = diet;
		return gameObject;
	}

	public virtual GameObject CreatePrefab()
	{
		GameObject gameObject = DreckoPlasticConfig.CreateDrecko("DreckoPlastic", CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.NAME, CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.DESC, "drecko_kanim", false);
		GameObject gameObject2 = gameObject;
		string text = "DreckoPlasticEgg";
		string text2 = CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.EGG_NAME;
		string text3 = CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.DESC;
		string text4 = "egg_drecko_kanim";
		float egg_MASS = DreckoTuning.EGG_MASS;
		string text5 = "DreckoPlasticBaby";
		float num = 90f;
		float num2 = 30f;
		int egg_SORT_ORDER = DreckoPlasticConfig.EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject2, text, text2, text3, text4, egg_MASS, text5, num, num2, DreckoTuning.EGG_CHANCES_PLASTIC, egg_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DreckoPlastic";

	public const string BASE_TRAIT_ID = "DreckoPlasticBaseTrait";

	public const string EGG_ID = "DreckoPlasticEgg";

	public static Tag POOP_ELEMENT = SimHashes.Phosphorite.CreateTag();

	public static Tag EMIT_ELEMENT = SimHashes.Polypropylene.CreateTag();

	private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 3f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = DreckoTuning.STANDARD_CALORIES_PER_CYCLE / DreckoPlasticConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 3f;

	private static float MIN_POOP_SIZE_IN_KG = 1.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = DreckoPlasticConfig.CALORIES_PER_DAY_OF_PLANT_EATEN * DreckoPlasticConfig.MIN_POOP_SIZE_IN_KG / DreckoPlasticConfig.KG_POOP_PER_DAY_OF_PLANT;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 3f;

	public static float PLASTIC_PER_CYCLE = 50f;

	public static int EGG_SORT_ORDER = 800;
}
