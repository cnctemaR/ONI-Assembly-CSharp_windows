using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class SquirrelHugConfig : IEntityConfig
{
	public static GameObject CreateSquirrelHug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseSquirrelConfig.BaseSquirrel(id, name, desc, anim_file, "SquirrelHugBaseTrait", is_baby, "hug_", true);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, SquirrelTuning.PEN_SIZE_PER_CREATURE_HUG);
		gameObject.AddOrGet<DecorProvider>().SetValues(DECOR.BONUS.TIER3);
		Trait trait = Db.Get().CreateTrait("SquirrelHugBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SquirrelTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		Diet.Info[] array = BaseSquirrelConfig.BasicDiet(SimHashes.Dirt.CreateTag(), SquirrelHugConfig.CALORIES_PER_DAY_OF_PLANT_EATEN, SquirrelHugConfig.KG_POOP_PER_DAY_OF_PLANT, null, 0f);
		gameObject = BaseSquirrelConfig.SetupDiet(gameObject, array, SquirrelHugConfig.MIN_POOP_SIZE_KG);
		if (!is_baby)
		{
			gameObject.AddOrGetDef<HugMonitor.Def>();
		}
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = SquirrelHugConfig.CreateSquirrelHug("SquirrelHug", global::STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.NAME, global::STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.DESC, "squirrel_kanim", false);
		string text = "SquirrelHugEgg";
		string text2 = global::STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.EGG_NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.DESC;
		string text4 = "egg_squirrel_kanim";
		float egg_MASS = SquirrelTuning.EGG_MASS;
		string text5 = "SquirrelHugBaby";
		float num = 60.000004f;
		float num2 = 20f;
		List<FertilityMonitor.BreedingChance> egg_CHANCES_HUG = SquirrelTuning.EGG_CHANCES_HUG;
		int egg_SORT_ORDER = SquirrelHugConfig.EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject, text, text2, text3, text4, egg_MASS, text5, num, num2, egg_CHANCES_HUG, this.GetDlcIds(), egg_SORT_ORDER, true, false, true, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SquirrelHug";

	public const string BASE_TRAIT_ID = "SquirrelHugBaseTrait";

	public const string EGG_ID = "SquirrelHugEgg";

	public const float OXYGEN_RATE = 0.023437504f;

	public const float BABY_OXYGEN_RATE = 0.011718752f;

	private const SimHashes EMIT_ELEMENT = SimHashes.Dirt;

	public static float DAYS_PLANT_GROWTH_EATEN_PER_CYCLE = 0.5f;

	private static float CALORIES_PER_DAY_OF_PLANT_EATEN = SquirrelTuning.STANDARD_CALORIES_PER_CYCLE / SquirrelHugConfig.DAYS_PLANT_GROWTH_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DAY_OF_PLANT = 25f;

	private static float MIN_POOP_SIZE_KG = 40f;

	public static int EGG_SORT_ORDER = 0;
}
