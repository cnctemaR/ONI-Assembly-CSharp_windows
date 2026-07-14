using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class SeaHorseConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateSeaHorse(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseSeaHorseConfig.CreatePrefab(id, "SeaHorseBaseTrait", name, desc, anim_file, is_baby, null, 273.15f, 333.15f, 253.15f, 373.15f), SeaHorseTuning.PEN_SIZE_PER_CREATURE, true);
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
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(SeaHorseConfig.CreateSeaHorse("SeaHorse", CREATURES.SPECIES.SEAHORSE.NAME, CREATURES.SPECIES.SEAHORSE.DESC, "seahorse_kanim", false), this, "SeaHorseEgg", CREATURES.SPECIES.SEAHORSE.EGG_NAME, CREATURES.SPECIES.SEAHORSE.DESC, "egg_seahorse_kanim", SeaHorseTuning.EGG_MASS, "SeaHorseBaby", 60.000004f, 20f, SeaHorseTuning.EGG_CHANCES_BASE, 600, true, true, 0.75f, false);
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

	public const string ID = "SeaHorse";

	public const string BASE_TRAIT_ID = "SeaHorseBaseTrait";

	public const string EGG_ID = "SeaHorseEgg";

	public const int EGG_SORT_ORDER = 600;
}
