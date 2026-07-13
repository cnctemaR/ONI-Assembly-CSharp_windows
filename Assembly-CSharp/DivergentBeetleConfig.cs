using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class DivergentBeetleConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateDivergentBeetle(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseDivergentConfig.BaseDivergent(id, name, desc, 50f, anim_file, is_baby ? null : "critter_build_kanim", "DivergentBeetleBaseTrait", is_baby, 8f, null, "DivergentCropTended", 1, true, "critter_emotes_kanim"), DivergentTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("DivergentBeetleBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DivergentTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -DivergentTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		List<Diet.Info> list = BaseDivergentConfig.BasicSulfurDiet(SimHashes.Sucrose.CreateTag(), DivergentBeetleConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f);
		GameObject gameObject2 = BaseDivergentConfig.SetupDiet(gameObject, list, DivergentBeetleConfig.CALORIES_PER_KG_OF_ORE, DivergentBeetleConfig.MIN_POOP_SIZE_IN_KG);
		gameObject2.AddTag(GameTags.OriginalCreature);
		return gameObject2;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(DivergentBeetleConfig.CreateDivergentBeetle("DivergentBeetle", global::STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.NAME, global::STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.DESC, "critter_kanim", false), this, "DivergentBeetleEgg", global::STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.EGG_NAME, global::STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.DESC, "egg_critter_kanim", DivergentTuning.EGG_MASS, "DivergentBeetleBaby", 45f, 15f, DivergentTuning.EGG_CHANCES_BEETLE, DivergentBeetleConfig.EGG_SORT_ORDER, true, false, 1f, false);
		gameObject.AddTag(GameTags.Creatures.Pollinator);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DivergentBeetle";

	public const string BASE_TRAIT_ID = "DivergentBeetleBaseTrait";

	public const string EGG_ID = "DivergentBeetleEgg";

	private const float LIFESPAN = 75f;

	private const SimHashes EMIT_ELEMENT = SimHashes.Sucrose;

	private static float KG_ORE_EATEN_PER_CYCLE = 20f;

	private static float CALORIES_PER_KG_OF_ORE = DivergentTuning.STANDARD_CALORIES_PER_CYCLE / DivergentBeetleConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 4f;

	public static int EGG_SORT_ORDER = 0;
}
