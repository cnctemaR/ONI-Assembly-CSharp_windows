using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class MoleConfig : IEntityConfig
{
	public static GameObject CreateMole(string id, string name, string desc, string anim_file, bool is_baby = false)
	{
		GameObject gameObject = BaseMoleConfig.BaseMole(id, name, global::STRINGS.CREATURES.SPECIES.MOLE.DESC, "MoleBaseTrait", anim_file, is_baby);
		gameObject.AddTag(GameTags.Creatures.Digger);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MoleTuning.PEN_SIZE_PER_CREATURE, 100f);
		Trait trait = Db.Get().CreateTrait("MoleBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MoleTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -MoleTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> list = BaseMoleConfig.SimpleOreDiet(new List<Tag>
		{
			SimHashes.Regolith.CreateTag(),
			SimHashes.Dirt.CreateTag(),
			SimHashes.IronOre.CreateTag()
		}, MoleConfig.CALORIES_PER_KG_OF_DIRT, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL);
		Diet diet = new Diet(list.ToArray());
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minPoopSizeInCalories = MoleConfig.MIN_POOP_SIZE_IN_CALORIES;
		SolidConsumerMonitor.Def def2 = gameObject.AddOrGetDef<SolidConsumerMonitor.Def>();
		def2.diet = diet;
		OvercrowdingMonitor.Def def3 = gameObject.AddOrGetDef<OvercrowdingMonitor.Def>();
		def3.spaceRequiredPerCreature = 0;
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = MoleConfig.CreateMole("Mole", global::STRINGS.CREATURES.SPECIES.MOLE.NAME, global::STRINGS.CREATURES.SPECIES.MOLE.DESC, "driller_kanim", false);
		GameObject gameObject2 = gameObject;
		string text = "MoleEgg";
		string text2 = global::STRINGS.CREATURES.SPECIES.MOLE.EGG_NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.MOLE.DESC;
		string text4 = "egg_driller_kanim";
		float egg_MASS = MoleTuning.EGG_MASS;
		string text5 = "MoleBaby";
		float num = 60.000004f;
		float num2 = 20f;
		int egg_SORT_ORDER = MoleConfig.EGG_SORT_ORDER;
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject2, text, text2, text3, text4, egg_MASS, text5, num, num2, MoleTuning.EGG_CHANCES_BASE, egg_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		MoleConfig.SetSpawnNavType(inst);
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
				KBatchedAnimController component2 = inst.GetComponent<KBatchedAnimController>();
				component2.SetSceneLayer(Grid.SceneLayer.FXFront);
			}
			else
			{
				KBatchedAnimController component3 = inst.GetComponent<KBatchedAnimController>();
				component3.SetSceneLayer(Grid.SceneLayer.Creatures);
			}
		}
	}

	public const string ID = "Mole";

	public const string BASE_TRAIT_ID = "MoleBaseTrait";

	public const string EGG_ID = "MoleEgg";

	private static float MIN_POOP_SIZE_IN_CALORIES = 2400000f;

	private static float CALORIES_PER_KG_OF_DIRT = 1000f;

	public static int EGG_SORT_ORDER = 800;
}
