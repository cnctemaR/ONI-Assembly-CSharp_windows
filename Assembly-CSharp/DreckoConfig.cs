using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class DreckoConfig : IEntityConfig
{
	public static GameObject CreateDrecko(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseDreckoConfig.BaseDrecko(id, name, desc, anim_file, "DreckoBaseTrait", is_baby, "fbr_", 308.15f, 363.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, DreckoTuning.PEN_SIZE_PER_CREATURE, 150f);
		Trait trait = Db.Get().CreateTrait("DreckoBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DreckoTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -DreckoTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name, false, false, true));
		TagBits tagBits = default(TagBits);
		tagBits.SetTag("SpiceVine".ToTag());
		tagBits.SetTag(SwampLilyConfig.ID.ToTag());
		tagBits.SetTag("BasicSingleHarvestPlant".ToTag());
		Diet.Info[] array = new Diet.Info[]
		{
			new Diet.Info(tagBits, DreckoConfig.POOP_ELEMENT, DreckoConfig.CALORIES_PER_DAY_OF_PLANT_EATEN, DreckoConfig.KG_POOP_PER_DAY_OF_PLANT, null, 0f)
		};
		Diet diet = new Diet(array);
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = DreckoConfig.MIN_POOP_SIZE_IN_CALORIES;
		SolidConsumerMonitor.Def def2 = gameObject.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		ScaleGrowthMonitor.Def def3 = gameObject.AddOrGetDef<ScaleGrowthMonitor.Def>();
		def3.defaultGrowthRate = 1f / DreckoConfig.SCALE_GROWTH_TIME_IN_CYCLES / 600f;
		def3.dropMass = DreckoConfig.FIBER_PER_CYCLE * DreckoConfig.SCALE_GROWTH_TIME_IN_CYCLES;
		def3.itemDroppedOnShear = DreckoConfig.EMIT_ELEMENT;
		def3.levelCount = 6;
		def3.targetAtmosphere = SimHashes.Hydrogen;
		return gameObject;
	}

	public virtual GameObject CreatePrefab()
	{
		GameObject gameObject = DreckoConfig.CreateDrecko("Drecko", CREATURES.SPECIES.DRECKO.NAME, CREATURES.SPECIES.DRECKO.DESC, "drecko_kanim", false);
		GameObject gameObject2 = gameObject;
		string text = "DreckoEgg";
		string text2 = CREATURES.SPECIES.DRECKO.EGG_NAME;
		string text3 = CREATURES.SPECIES.DRECKO.DESC;
		string text4 = "egg_drecko_kanim";
		string text5 = "DreckoBaby";
		int egg_SORT_ORDER = DreckoConfig.EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject2, text, text2, text3, text4, text5, DreckoTuning.EGG_CHANCES_BASE, egg_SORT_ORDER, true, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Drecko";

	public const string BASE_TRAIT_ID = "DreckoBaseTrait";

	public const string EGG_ID = "DreckoEgg";

	public static Tag POOP_ELEMENT = SimHashes.Phosphorite.CreateTag();

	public static Tag EMIT_ELEMENT = BasicFabricConfig.ID;

	private static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 2f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = DreckoTuning.STANDARD_CALORIES_PER_CYCLE / DreckoConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 0.8f;

	private static float MIN_POOP_SIZE_IN_KG = 1.5f;

	private static float MIN_POOP_SIZE_IN_CALORIES = DreckoConfig.CALORIES_PER_DAY_OF_PLANT_EATEN * DreckoConfig.MIN_POOP_SIZE_IN_KG / DreckoConfig.KG_POOP_PER_DAY_OF_PLANT;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 8f;

	public static float FIBER_PER_CYCLE = 0.25f;

	public static int EGG_SORT_ORDER = 800;
}
