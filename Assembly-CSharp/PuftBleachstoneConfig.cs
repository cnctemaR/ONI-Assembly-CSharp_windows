using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftBleachstoneConfig : IEntityConfig
{
	public static GameObject CreatePuftBleachstone(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BasePuftConfig.BasePuft(id, name, desc, "PuftBleachstoneBaseTrait", anim_file, is_baby, "anti_", 273.15f, 333.15f, 223.15f, 373.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, PuftTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("PuftBleachstoneBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -PuftTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		gameObject = BasePuftConfig.SetupDiet(gameObject, SimHashes.ChlorineGas.CreateTag(), SimHashes.BleachStone.CreateTag(), PuftBleachstoneConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_2, null, 0f, PuftBleachstoneConfig.MIN_POOP_SIZE_IN_KG);
		gameObject.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[]
		{
			SimHashes.BleachStone.CreateTag(),
			GameTags.Creatures.FlyersLure
		};
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftBleachstoneConfig.CreatePuftBleachstone("PuftBleachstone", global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.NAME, global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.DESC, "puft_kanim", false);
		string text = "PuftBleachstoneEgg";
		string text2 = global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.EGG_NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.DESC;
		string text4 = "egg_puft_kanim";
		float egg_MASS = PuftTuning.EGG_MASS;
		string text5 = "PuftBleachstoneBaby";
		float num = 45f;
		float num2 = 15f;
		List<FertilityMonitor.BreedingChance> egg_CHANCES_BLEACHSTONE = PuftTuning.EGG_CHANCES_BLEACHSTONE;
		int egg_SORT_ORDER = PuftBleachstoneConfig.EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject, text, text2, text3, text4, egg_MASS, text5, num, num2, egg_CHANCES_BLEACHSTONE, this.GetDlcIds(), egg_SORT_ORDER, true, false, true, 1f, false);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}

	public const string ID = "PuftBleachstone";

	public const string BASE_TRAIT_ID = "PuftBleachstoneBaseTrait";

	public const string EGG_ID = "PuftBleachstoneEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ChlorineGas;

	public const SimHashes EMIT_ELEMENT = SimHashes.BleachStone;

	private static float KG_ORE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / PuftBleachstoneConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 15f;

	public static int EGG_SORT_ORDER = PuftConfig.EGG_SORT_ORDER + 3;
}
