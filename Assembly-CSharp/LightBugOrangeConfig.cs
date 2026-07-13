using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class LightBugOrangeConfig : IEntityConfig
{
	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugOrangeBaseTrait", LIGHT2D.LIGHTBUG_COLOR_ORANGE, DECOR.BONUS.TIER6, is_baby, "org_", "", 0f);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, LightBugTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("LightBugOrangeBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -LightBugTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		HashSet<Tag> hashSet = new HashSet<Tag>();
		hashSet.Add(TagManager.Create(MushroomConfig.ID));
		hashSet.Add(TagManager.Create("FriedMushroom"));
		hashSet.Add(TagManager.Create("GrilledPrickleFruit"));
		if (DlcManager.IsContentSubscribed("DLC2_ID"))
		{
			hashSet.Add(TagManager.Create("CookedPikeapple"));
		}
		hashSet.Add(SimHashes.Phosphorite.CreateTag());
		return BaseLightBugConfig.SetupDiet(gameObject, hashSet, Tag.Invalid, LightBugOrangeConfig.CALORIES_PER_KG_OF_ORE);
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugOrangeConfig.CreateLightBug("LightBugOrange", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.DESC, "lightbug_kanim", false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, this as IHasDlcRestrictions, "LightBugOrangeEgg", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.EGG_NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_ORANGE.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugOrangeBaby", 15.000001f, 5f, LightBugTuning.EGG_CHANCES_ORANGE, LightBugOrangeConfig.EGG_SORT_ORDER, true, false, 1f, false, false, LightBugTuning.EGG_MASS * 0.2f);
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
