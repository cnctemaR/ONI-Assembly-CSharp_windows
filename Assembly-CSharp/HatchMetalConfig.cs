using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class HatchMetalConfig : IEntityConfig
{
	public static HashSet<Tag> METAL_ORE_TAGS
	{
		get
		{
			HashSet<Tag> hashSet = new HashSet<Tag>
			{
				SimHashes.Cuprite.CreateTag(),
				SimHashes.GoldAmalgam.CreateTag(),
				SimHashes.IronOre.CreateTag(),
				SimHashes.Wolframite.CreateTag(),
				SimHashes.AluminumOre.CreateTag()
			};
			if (DlcManager.IsExpansion1Active())
			{
				hashSet.Add(SimHashes.Cobaltite.CreateTag());
			}
			return hashSet;
		}
	}

	public static GameObject CreateHatch(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, anim_file, "HatchMetalBaseTrait", is_baby, "mtl_"), HatchTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("HatchMetalBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -HatchTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 400f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> list = BaseHatchConfig.MetalDiet(GameTags.Metal, HatchMetalConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0f);
		return BaseHatchConfig.SetupDiet(gameObject, list, HatchMetalConfig.CALORIES_PER_KG_OF_ORE, HatchMetalConfig.MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(HatchMetalConfig.CreateHatch("HatchMetal", global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.NAME, global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.DESC, "hatch_kanim", false), this as IHasDlcRestrictions, "HatchMetalEgg", global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.EGG_NAME, global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.DESC, "egg_hatch_kanim", HatchTuning.EGG_MASS, "HatchMetalBaby", 60.000004f, 20f, HatchTuning.EGG_CHANCES_METAL, HatchMetalConfig.EGG_SORT_ORDER, true, false, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "HatchMetal";

	public const string BASE_TRAIT_ID = "HatchMetalBaseTrait";

	public const string EGG_ID = "HatchMetalEgg";

	private static float KG_ORE_EATEN_PER_CYCLE = 100f;

	private static float CALORIES_PER_KG_OF_ORE = HatchTuning.STANDARD_CALORIES_PER_CYCLE / HatchMetalConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 10f;

	public static int EGG_SORT_ORDER = HatchConfig.EGG_SORT_ORDER + 3;
}
