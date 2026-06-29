using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PacuTropicalConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = BasePacuConfig.CreatePrefab("PacuTropical", "PacuTropicalBaseTrait", global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.NAME, global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.DESC, "trp_", 303.15f, 353.15f);
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(PacuTropicalConfig.DECOR);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject, "PacuTropicalEgg", global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.EGG_NAME, global::STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.DESC, "egg_pacu_kanim", "PacuTropical", PacuTuning.EGG_CHANCES_TROPICAL, PacuTropicalConfig.EGG_SORT_ORDER, false, true);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PacuTropical";

	public const string BASE_TRAIT_ID = "PacuTropicalBaseTrait";

	public const string EGG_ID = "PacuTropicalEgg";

	public static EffectorValues DECOR = global::TUNING.BUILDINGS.DECOR.BONUS.TIER4;

	public static int EGG_SORT_ORDER = PacuConfig.EGG_SORT_ORDER + 2;
}
