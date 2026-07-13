using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class LightBugBlueConfig : IEntityConfig
{
	public static GameObject CreateLightBug(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseLightBugConfig.BaseLightBug(id, name, desc, anim_file, "LightBugBlueBaseTrait", LIGHT2D.LIGHTBUG_COLOR_BLUE, DECOR.BONUS.TIER6, is_baby, "blu_", "", 0f);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, LightBugTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("LightBugBlueBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, LightBugTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -LightBugTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name, false, false, true));
		gameObject = BaseLightBugConfig.SetupDiet(gameObject, new HashSet<Tag>
		{
			TagManager.Create("SpiceBread"),
			TagManager.Create("Salsa"),
			SimHashes.Phosphorite.CreateTag(),
			SimHashes.Phosphorus.CreateTag()
		}, Tag.Invalid, LightBugBlueConfig.CALORIES_PER_KG_OF_ORE);
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[]
		{
			SimHashes.Phosphorite.CreateTag(),
			SimHashes.Phosphorus.CreateTag(),
			GameTags.Creatures.FlyersLure
		};
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugBlueConfig.CreateLightBug("LightBugBlue", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_BLUE.NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_BLUE.DESC, "lightbug_kanim", false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, this as IHasDlcRestrictions, "LightBugBlueEgg", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_BLUE.EGG_NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.VARIANT_BLUE.DESC, "egg_lightbug_kanim", LightBugTuning.EGG_MASS, "LightBugBlueBaby", 15.000001f, 5f, LightBugTuning.EGG_CHANCES_BLUE, LightBugBlueConfig.EGG_SORT_ORDER, true, false, 1f, false, false, LightBugTuning.EGG_MASS * 0.2f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseLightBugConfig.SetupLoopingSounds(inst);
	}

	public const string ID = "LightBugBlue";

	public const string BASE_TRAIT_ID = "LightBugBlueBaseTrait";

	public const string EGG_ID = "LightBugBlueEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 1f;

	private static float CALORIES_PER_KG_OF_ORE = LightBugTuning.STANDARD_CALORIES_PER_CYCLE / LightBugBlueConfig.KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = LightBugConfig.EGG_SORT_ORDER + 4;
}
