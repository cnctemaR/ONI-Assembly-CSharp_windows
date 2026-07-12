using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class StaterpillarLiquidConfig : IEntityConfig
{
	public static GameObject CreateStaterpillarLiquid(string id, string name, string desc, string anim_file, bool is_baby)
	{
		InhaleStates.Def def = new InhaleStates.Def
		{
			behaviourTag = GameTags.Creatures.WantsToStore,
			inhaleAnimPre = "liquid_consume_pre",
			inhaleAnimLoop = "liquid_consume_loop",
			inhaleAnimPst = "liquid_consume_pst",
			useStorage = true,
			alwaysPlayPstAnim = true,
			inhaleTime = StaterpillarLiquidConfig.INHALE_TIME,
			storageStatusItem = Db.Get().CreatureStatusItems.LookingForLiquid
		};
		GameObject gameObject = BaseStaterpillarConfig.BaseStaterpillar(id, name, desc, anim_file, "StaterpillarLiquidBaseTrait", is_baby, ObjectLayer.LiquidConduit, StaterpillarLiquidConnectorConfig.ID, GameTags.Unbreathable, "wtr_", 263.15f, 313.15f, 173.15f, 373.15f, def);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, global::TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3);
		if (!is_baby)
		{
			GasAndLiquidConsumerMonitor.Def def2 = gameObject.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
			def2.behaviourTag = GameTags.Creatures.WantsToStore;
			def2.consumableElementTag = GameTags.Liquid;
			def2.transitionTag = new Tag[] { GameTags.Creature };
			def2.minCooldown = StaterpillarLiquidConfig.COOLDOWN_MIN;
			def2.maxCooldown = StaterpillarLiquidConfig.COOLDOWN_MAX;
			def2.consumptionRate = StaterpillarLiquidConfig.CONSUMPTION_RATE;
		}
		Trait trait = Db.Get().CreateTrait("StaterpillarLiquidBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StaterpillarTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		List<Diet.Info> list = new List<Diet.Info>();
		list.AddRange(BaseStaterpillarConfig.RawMetalDiet(SimHashes.Hydrogen.CreateTag(), StaterpillarLiquidConfig.CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, null, 0f));
		list.AddRange(BaseStaterpillarConfig.RefinedMetalDiet(SimHashes.Hydrogen.CreateTag(), StaterpillarLiquidConfig.CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, null, 0f));
		gameObject = BaseStaterpillarConfig.SetupDiet(gameObject, list);
		Storage storage = gameObject.AddComponent<Storage>();
		storage.capacityKg = StaterpillarLiquidConfig.STORAGE_CAPACITY;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		return gameObject;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public virtual GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(StaterpillarLiquidConfig.CreateStaterpillarLiquid("StaterpillarLiquid", global::STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.NAME, global::STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.DESC, "caterpillar_kanim", false), "StaterpillarLiquidEgg", global::STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.EGG_NAME, global::STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.DESC, "egg_caterpillar_kanim", StaterpillarTuning.EGG_MASS, "StaterpillarLiquidBaby", 60.000004f, 20f, StaterpillarTuning.EGG_CHANCES_LIQUID, this.GetDlcIds(), 2, true, false, true, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
		KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("electric_bolt_c_bloom", false);
		component.SetSymbolVisiblity("gulp", false);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "StaterpillarLiquid";

	public const string BASE_TRAIT_ID = "StaterpillarLiquidBaseTrait";

	public const string EGG_ID = "StaterpillarLiquidEgg";

	public const int EGG_SORT_ORDER = 2;

	private static float KG_ORE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_PER_KG_OF_ORE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / StaterpillarLiquidConfig.KG_ORE_EATEN_PER_CYCLE;

	private static float STORAGE_CAPACITY = 1000f;

	private static float COOLDOWN_MIN = 20f;

	private static float COOLDOWN_MAX = 40f;

	private static float CONSUMPTION_RATE = 10f;

	private static float INHALE_TIME = 6f;
}
