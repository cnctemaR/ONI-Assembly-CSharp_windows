using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class ParrotFishConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateParrotFish(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseParrotFish.CreatePrefab(id, "ParrotFishBaseTrait", name, desc, anim_file, is_baby, null, 273.15f, 333.15f, 253.15f, 373.15f), ParrotFishTuning.PEN_SIZE_PER_CREATURE, true);
		gameObject.AddTag(GameTags.OriginalCreature);
		return gameObject;
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
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(ParrotFishConfig.CreateParrotFish("ParrotFish", CREATURES.SPECIES.PARROTFISH.NAME, CREATURES.SPECIES.PARROTFISH.DESC, "fish_bioluminescent_kanim", false), this, "ParrotFishEgg", CREATURES.SPECIES.PARROTFISH.EGG_NAME, CREATURES.SPECIES.PARROTFISH.DESC, "egg_bioluminescent_kanim", ParrotFishTuning.EGG_MASS, ParrotFishTuning.EGG_SHELL_RATIO, "ParrotFishBaby", 15.000001f, 5f, ParrotFishTuning.EGG_CHANCES_BASE, 500, true, true, 0.75f, false, false, ParrotFishTuning.EGG_MASS, true);
		gameObject.AddTag(GameTags.OriginalCreature);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.AddOrGet<LoopingSounds>();
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ParrotFish";

	public const string BASE_TRAIT_ID = "ParrotFishBaseTrait";

	public const string EGG_ID = "ParrotFishEgg";

	public const int EGG_SORT_ORDER = 500;
}
