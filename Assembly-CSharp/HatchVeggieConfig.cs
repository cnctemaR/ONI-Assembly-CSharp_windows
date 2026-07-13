using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class HatchVeggieConfig : IEntityConfig
{
	public static GameObject CreateHatch(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, anim_file, "HatchVeggieBaseTrait", is_baby, "veg_"), HatchTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("HatchVeggieBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -HatchTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> list = BaseHatchConfig.VeggieDiet(SimHashes.Carbon.CreateTag(), HatchVeggieConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_3, null, 0f);
		list.AddRange(BaseHatchConfig.FoodDiet(SimHashes.Carbon.CreateTag(), HatchVeggieConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_3, null, 0f));
		return BaseHatchConfig.SetupDiet(gameObject, list, HatchVeggieConfig.CALORIES_PER_KG_OF_ORE, HatchVeggieConfig.MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(HatchVeggieConfig.CreateHatch("HatchVeggie", global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.NAME, global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.DESC, "hatch_kanim", false), this as IHasDlcRestrictions, "HatchVeggieEgg", global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.EGG_NAME, global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.DESC, "egg_hatch_kanim", HatchTuning.EGG_MASS, "HatchVeggieBaby", 60.000004f, 20f, HatchTuning.EGG_CHANCES_VEGGIE, HatchVeggieConfig.EGG_SORT_ORDER, true, false, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "HatchVeggie";

	public const string BASE_TRAIT_ID = "HatchVeggieBaseTrait";

	public const string EGG_ID = "HatchVeggieEgg";

	private const SimHashes EMIT_ELEMENT = SimHashes.Carbon;

	private static float KG_ORE_EATEN_PER_CYCLE = 140f;

	private static float CALORIES_PER_KG_OF_ORE = HatchTuning.STANDARD_CALORIES_PER_CYCLE / HatchVeggieConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 50f;

	public static int EGG_SORT_ORDER = HatchConfig.EGG_SORT_ORDER + 1;
}
