using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class PufferFishConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreatePufferFish(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BasePufferFish.CreatePrefab(id, "PufferFishBaseTrait", name, desc, anim_file, is_baby, null, 273.15f, 333.15f, 253.15f, 373.15f), PufferFishTuning.PEN_SIZE_PER_CREATURE, true);
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
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(PufferFishConfig.CreatePufferFish("PufferFish", CREATURES.SPECIES.PUFFERFISH.NAME, CREATURES.SPECIES.PUFFERFISH.DESC, "blowfish_kanim", false), this, "PufferFishEgg", CREATURES.SPECIES.PUFFERFISH.EGG_NAME, CREATURES.SPECIES.PUFFERFISH.DESC, "egg_blowfish_kanim", PufferFishTuning.EGG_MASS, "PufferFishBaby", 15.000001f, 5f, PufferFishTuning.EGG_CHANCES_BASE, 500, true, true, 1f, false);
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

	public const string ID = "PufferFish";

	public const string BASE_TRAIT_ID = "PufferFishBaseTrait";

	public const string EGG_ID = "PufferFishEgg";

	public const int EGG_SORT_ORDER = 500;
}
