using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugConfig : IEntityConfig
{
	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugBaseTrait", LIGHT2D.LIGHTBUG_COLOR, DECOR.BONUS.TIER4, is_baby, null);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, LightBugTuning.PEN_SIZE_PER_CREATURE, 25f);
		Trait trait = Db.Get().CreateTrait("LightBugBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -LightBugTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		return BaseLightBugConfig.SetupDiet(gameObject, new HashSet<Tag>
		{
			TagManager.Create(PrickleFruitConfig.ID),
			TagManager.Create("GrilledPrickleFruit"),
			SimHashes.Phosphorite.CreateTag()
		}, Tag.Invalid, LightBugConfig.CALORIES_PER_KG_OF_ORE);
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugConfig.CreateLightBug("LightBug", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.DESC, "lightbug_kanim", false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "LightBugEgg", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.EGG_NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugBaby", 15.000001f, 5f, LightBugTuning.EGG_CHANCES_BASE, LightBugConfig.EGG_SORT_ORDER, true, false, true, 1f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseLightBugConfig.SetupLoopingSounds(inst);
	}

	public const string ID = "LightBug";

	public const string BASE_TRAIT_ID = "LightBugBaseTrait";

	public const string EGG_ID = "LightBugEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 0.166f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / LightBugConfig.KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = 100;
}
