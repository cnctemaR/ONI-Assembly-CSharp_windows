using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class RaptorConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateRaptor(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseRaptorConfig.BaseRaptor(id, name, desc, anim_file, "RaptorBaseTrait", is_baby, null);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, global::TUNING.CREATURES.SPACE_REQUIREMENTS.TIER4);
		Trait trait = Db.Get().CreateTrait("RaptorBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, RaptorTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -RaptorTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 50f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 200f, name, false, false, true));
		gameObject = BaseRaptorConfig.SetupDiet(gameObject, BaseRaptorConfig.StandardDiets());
		WellFedShearable.Def def = gameObject.AddOrGetDef<WellFedShearable.Def>();
		def.effectId = "RaptorWellFed";
		def.scaleGrowthSymbols = new KAnimHashedString[] { "body_feathers", "tail_feather" };
		def.caloriesPerCycle = RaptorTuning.STANDARD_CALORIES_PER_CYCLE;
		def.growthDurationCycles = RaptorConfig.SCALE_GROWTH_TIME_IN_CYCLES;
		def.dropMass = RaptorConfig.FIBER_PER_CYCLE * RaptorConfig.SCALE_GROWTH_TIME_IN_CYCLES;
		def.itemDroppedOnShear = RaptorConfig.SCALE_GROWTH_EMIT_ELEMENT;
		def.levelCount = 2;
		gameObject.AddTag(GameTags.OriginalCreature);
		return gameObject;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(RaptorConfig.CreateRaptor("Raptor", global::STRINGS.CREATURES.SPECIES.RAPTOR.NAME, global::STRINGS.CREATURES.SPECIES.RAPTOR.DESC, "raptor_kanim", false), this, "RaptorEgg", global::STRINGS.CREATURES.SPECIES.RAPTOR.EGG_NAME, global::STRINGS.CREATURES.SPECIES.RAPTOR.DESC, "egg_raptor_kanim", 8f, "RaptorBaby", 120.00001f, 40f, RaptorTuning.EGG_CHANCES_BASE, RaptorConfig.EGG_SORT_ORDER, true, false, 1f, false);
		gameObject.AddTag(GameTags.LargeCreature);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Raptor";

	public const string BASE_TRAIT_ID = "RaptorBaseTrait";

	public const string EGG_ID = "RaptorEgg";

	public static int EGG_SORT_ORDER = 0;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 4f;

	public static float SCALE_INITIAL_GROWTH_PCT = 0.9f;

	public static float FIBER_PER_CYCLE = 1f;

	public static Tag SCALE_GROWTH_EMIT_ELEMENT = FeatherFabricConfig.ID;

	public static KAnimHashedString[] SCALE_SYMBOLS = new KAnimHashedString[] { "scale_0", "scale_1", "scale_2" };

	public List<Emote> RaptorEmotes = new List<Emote>
	{
		Db.Get().Emotes.Critter.Roar,
		Db.Get().Emotes.Critter.RaptorSignal
	};
}
