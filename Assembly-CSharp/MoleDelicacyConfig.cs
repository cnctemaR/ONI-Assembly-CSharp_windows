using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MoleDelicacyConfig : IEntityConfig
{
	public static GameObject CreateMole(string id, string name, string desc, string anim_file, bool is_baby = false)
	{
		GameObject gameObject = BaseMoleConfig.BaseMole(id, name, desc, "MoleDelicacyBaseTrait", anim_file, is_baby, 173.15f, 373.15f, 73.149994f, 773.15f, "del_", 5);
		gameObject.AddTag(GameTags.Creatures.Digger);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MoleTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("MoleDelicacyBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MoleTuning.DELICACY_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -MoleTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		Diet diet = new Diet(BaseMoleConfig.SimpleOreDiet(new List<Tag>
		{
			SimHashes.Regolith.CreateTag(),
			SimHashes.Dirt.CreateTag(),
			SimHashes.IronOre.CreateTag()
		}, MoleDelicacyConfig.CALORIES_PER_KG_OF_DIRT, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL).ToArray());
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = MoleDelicacyConfig.MIN_POOP_SIZE_IN_CALORIES;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
		gameObject.AddOrGet<LoopingSounds>();
		if (!is_baby)
		{
			ElementGrowthMonitor.Def def2 = gameObject.AddOrGetDef<ElementGrowthMonitor.Def>();
			def2.defaultGrowthRate = 1f / MoleDelicacyConfig.GINGER_GROWTH_TIME_IN_CYCLES / 600f;
			def2.dropMass = MoleDelicacyConfig.GINGER_PER_CYCLE * MoleDelicacyConfig.GINGER_GROWTH_TIME_IN_CYCLES;
			def2.itemDroppedOnShear = MoleDelicacyConfig.SHEAR_DROP_ELEMENT;
			def2.levelCount = 5;
			def2.minTemperature = MoleDelicacyConfig.MIN_GROWTH_TEMPERATURE;
			def2.maxTemperature = MoleDelicacyConfig.MAX_GROWTH_TEMPERATURE;
		}
		else
		{
			gameObject.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.ElementGrowth.Id);
		}
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = MoleDelicacyConfig.CreateMole("MoleDelicacy", global::STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.NAME, global::STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.DESC, "driller_kanim", false);
		string text = "MoleDelicacyEgg";
		string text2 = global::STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.EGG_NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.MOLE.VARIANT_DELICACY.DESC;
		string text4 = "egg_driller_kanim";
		float egg_MASS = MoleTuning.EGG_MASS;
		string text5 = "MoleDelicacyBaby";
		float num = 60.000004f;
		float num2 = 20f;
		int egg_SORT_ORDER = MoleDelicacyConfig.EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject, text, text2, text3, text4, egg_MASS, text5, num, num2, MoleTuning.EGG_CHANCES_DELICACY, this.GetDlcIds(), egg_SORT_ORDER, true, false, true, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		MoleDelicacyConfig.SetSpawnNavType(inst);
	}

	public static void SetSpawnNavType(GameObject inst)
	{
		int num = Grid.PosToCell(inst);
		Navigator component = inst.GetComponent<Navigator>();
		if (component != null)
		{
			if (Grid.IsSolidCell(num))
			{
				component.SetCurrentNavType(NavType.Solid);
				inst.transform.SetPosition(Grid.CellToPosCBC(num, Grid.SceneLayer.FXFront));
				inst.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.FXFront);
				return;
			}
			inst.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
		}
	}

	public const string ID = "MoleDelicacy";

	public const string BASE_TRAIT_ID = "MoleDelicacyBaseTrait";

	public const string EGG_ID = "MoleDelicacyEgg";

	private static float MIN_POOP_SIZE_IN_CALORIES = 2400000f;

	private static float CALORIES_PER_KG_OF_DIRT = 1000f;

	public static int EGG_SORT_ORDER = 800;

	public static float GINGER_GROWTH_TIME_IN_CYCLES = 8f;

	public static float GINGER_PER_CYCLE = 1f;

	public static Tag SHEAR_DROP_ELEMENT = GingerConfig.ID;

	public static float MIN_GROWTH_TEMPERATURE = 343.15f;

	public static float MAX_GROWTH_TEMPERATURE = 353.15f;

	public static float EGG_CHANCES_TEMPERATURE_MIN = 333.15f;

	public static float EGG_CHANCES_TEMPERATURE_MAX = 373.15f;
}
