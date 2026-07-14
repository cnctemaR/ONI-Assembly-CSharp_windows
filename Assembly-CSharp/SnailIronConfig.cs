using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class SnailIronConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateSnail(string id, string name, string desc, string anim_file, bool is_baby)
	{
		return EntityTemplates.ExtendEntityToWildCreature(BaseSnailConfig.CreatePrefab(id, "SnailIronBaseTrait", name, desc, anim_file, is_baby, "iron_", 333.15f, 388.15f, 308.15f, 413.15f, "SnailIronShell"), SnailTuning.PEN_SIZE_PER_CREATURE, true);
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(SnailIronConfig.CreateSnail("SnailIron", CREATURES.SPECIES.SNAIL.VARIANT_IRON.NAME, CREATURES.SPECIES.SNAIL.VARIANT_IRON.DESC, "snail_kanim", false), this, "SnailIronEgg", CREATURES.SPECIES.SNAIL.VARIANT_IRON.EGG_NAME, CREATURES.SPECIES.SNAIL.VARIANT_IRON.DESC, "egg_snail_kanim", SnailTuning.EGG_MASS, "SnailIronBaby", 15.000001f, 5f, SnailTuning.EGG_CHANCES_IRON, 700, true, false, 1f, false);
		Diet diet = new Diet(BaseSnailConfig.SulfurToObsidianDiet());
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = SnailTuning.CALORIES_PER_KG_OF_ORE * SnailTuning.MIN_POOP_SIZE_IN_KG;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.AddOrGet<LoopingSounds>();
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("beached_limpetgrowth", false);
		BaseSnailConfig.OnSpawn(inst);
	}

	public const string ID = "SnailIron";

	public const string BASE_TRAIT_ID = "SnailIronBaseTrait";

	public const string EGG_ID = "SnailIronEgg";

	public const int EGG_SORT_ORDER = 700;
}
