using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugOrangeConfig : IEntityConfig
{
	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugOrangeBaseTrait", LIGHT2D.LIGHTBUG_COLOR_ORANGE, DECOR.BONUS.TIER6, is_baby, "org_");
		EntityTemplates.ExtendEntityToWildCreature(gameObject, LightBugTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("LightBugOrangeBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -LightBugTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		return BaseLightBugConfig.SetupDiet(gameObject, new HashSet<Tag>
		{
			TagManager.Create(MushroomConfig.ID),
			TagManager.Create("FriedMushroom"),
			TagManager.Create("GrilledPrickleFruit"),
			SimHashes.Phosphorite.CreateTag()
		}, Tag.Invalid, LightBugOrangeConfig.CALORIES_PER_KG_OF_ORE);
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugOrangeConfig.CreateLightBug("LightBugOrange", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.DESC, "lightbug_kanim", false);
		string text = "LightBugOrangeEgg";
		string text2 = global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.EGG_NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.DESC;
		string text4 = "egg_lightbug_kanim";
		float egg_MASS = LightBugTuning.EGG_MASS;
		string text5 = "LightBugOrangeBaby";
		float num = 15.000001f;
		float num2 = 5f;
		List<FertilityMonitor.BreedingChance> egg_CHANCES_ORANGE = LightBugTuning.EGG_CHANCES_ORANGE;
		int egg_SORT_ORDER = LightBugOrangeConfig.EGG_SORT_ORDER;
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, text, text2, text3, text4, egg_MASS, text5, num, num2, egg_CHANCES_ORANGE, this.GetDlcIds(), egg_SORT_ORDER, true, false, true, 1f, false);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseLightBugConfig.SetupLoopingSounds(inst);
	}

	public const string ID = "LightBugOrange";

	public const string BASE_TRAIT_ID = "LightBugOrangeBaseTrait";

	public const string EGG_ID = "LightBugOrangeEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 0.25f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / LightBugOrangeConfig.KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = LightBugConfig.EGG_SORT_ORDER + 1;
}
