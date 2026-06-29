using System;
using STRINGS;
using UnityEngine;

public class PacuConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = BasePacuConfig.CreatePrefab("Pacu", "PacuBaseTrait", CREATURES.SPECIES.PACU.NAME, CREATURES.SPECIES.PACU.DESC, null, 273.15f, 333.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject, "PacuEgg", CREATURES.SPECIES.PACU.EGG_NAME, CREATURES.SPECIES.PACU.DESC, "egg_pacu_kanim", "Pacu", PacuTuning.EGG_CHANCES_BASE, PacuConfig.EGG_SORT_ORDER, false, true);
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.AddOrGet<LoopingSounds>();
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Pacu";

	public const string BASE_TRAIT_ID = "PacuBaseTrait";

	public const string EGG_ID = "PacuEgg";

	public static int EGG_SORT_ORDER = 500;
}
