using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftAlphaConfig : IEntityConfig
{
	public static GameObject CreatePuftAlpha(string id, string name, string desc, string anim_file, bool is_baby)
	{
		string text = "alp_";
		GameObject gameObject = BasePuftConfig.BasePuft(id, name, desc, "PuftAlphaBaseTrait", anim_file, is_baby, text);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, PuftTuning.PEN_SIZE_PER_CREATURE, 75f);
		Trait trait = Db.Get().CreateTrait("PuftAlphaBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -PuftTuning.STANDARD_CALORIES_PER_CYCLE / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		gameObject = BasePuftConfig.SetupDiet(gameObject, SimHashes.ContaminatedOxygen.CreateTag(), SimHashes.SlimeMold.CreateTag(), PuftAlphaConfig.CALORIES_PER_KG_OF_ORE, global::TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_2, "SlimeLung", 1000f, PuftAlphaConfig.MIN_POOP_SIZE_IN_KG);
		DiseaseSourceVisualizer diseaseSourceVisualizer = gameObject.AddOrGet<DiseaseSourceVisualizer>();
		diseaseSourceVisualizer.alwaysShowDisease = "SlimeLung";
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftAlphaConfig.CreatePuftAlpha("PuftAlpha", global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.NAME, global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.DESC, "puft_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(gameObject, "PuftAlphaEgg", global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.EGG_NAME, global::STRINGS.CREATURES.SPECIES.PUFT.VARIANT_ALPHA.DESC, "egg_puft_kanim", "PuftAlphaBaby", PuftTuning.EGG_CHANCES_ALPHA, PuftAlphaConfig.EGG_SORT_ORDER, true, false);
	}

	public void OnPrefabInit(GameObject inst)
	{
		KBatchedAnimController component = inst.GetComponent<KBatchedAnimController>();
		component.animScale *= 1.1f;
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}

	public const string ID = "PuftAlpha";

	public const string BASE_TRAIT_ID = "PuftAlphaBaseTrait";

	public const string EGG_ID = "PuftAlphaEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ContaminatedOxygen;

	public const SimHashes EMIT_ELEMENT = SimHashes.SlimeMold;

	public const string EMIT_DISEASE = "SlimeLung";

	public const float EMIT_DISEASE_PER_KG = 1000f;

	private static float KG_ORE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / PuftAlphaConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 5f;

	public static int EGG_SORT_ORDER = PuftConfig.EGG_SORT_ORDER + 1;
}
