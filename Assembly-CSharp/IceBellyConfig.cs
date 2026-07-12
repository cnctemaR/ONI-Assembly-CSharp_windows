using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[EntityConfigOrder(1)]
public class IceBellyConfig : IEntityConfig
{
	public static GameObject CreateIceBelly(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseBellyConfig.BaseBelly(id, name, desc, anim_file, "IceBellyBaseTrait", is_baby, null), MooTuning.PEN_SIZE_PER_CREATURE);
		gameObject.AddOrGet<WarmBlooded>().BaseGenerationKW = 1.3f;
		Trait trait = Db.Get().CreateTrait("IceBellyBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, BellyTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -BellyTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 200f, name, false, false, true));
		string text = "PollenGerms";
		List<Diet.Info> list = BaseBellyConfig.BasicDiet("CarrotPlant", "IceBellyPoop", IceBellyConfig.CALORIES_PER_UNIT_EATEN / IceBellyConfig.CONSUMABLE_PLANT_MATURITY_LEVELS, 67.474f / IceBellyConfig.CONSUMABLE_PLANT_MATURITY_LEVELS, text, 1000f);
		list.Add(new Diet.Info(new HashSet<Tag> { CarrotConfig.ID }, "IceBellyPoop", IceBellyConfig.CALORIES_PER_UNIT_EATEN / 1f, 67.474f, text, 1000f, false, false, true));
		WellFedShearable.Def def = gameObject.AddOrGetDef<WellFedShearable.Def>();
		def.effectId = "IceBellyWellFed";
		def.caloriesPerCycle = BellyTuning.STANDARD_CALORIES_PER_CYCLE;
		def.growthDurationCycles = IceBellyConfig.SCALE_GROWTH_TIME_IN_CYCLES;
		def.dropMass = IceBellyConfig.FIBER_PER_CYCLE * IceBellyConfig.SCALE_GROWTH_TIME_IN_CYCLES;
		def.itemDroppedOnShear = IceBellyConfig.SCALE_GROWTH_EMIT_ELEMENT;
		def.levelCount = 6;
		GameObject gameObject2 = BaseBellyConfig.SetupDiet(gameObject, list, IceBellyConfig.CALORIES_PER_UNIT_EATEN, 1f);
		gameObject2.AddTag(GameTags.OriginalCreature);
		return gameObject2;
	}

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(IceBellyConfig.CreateIceBelly("IceBelly", global::STRINGS.CREATURES.SPECIES.ICEBELLY.NAME, global::STRINGS.CREATURES.SPECIES.ICEBELLY.DESC, "ice_belly_kanim", false), "IceBellyEgg", global::STRINGS.CREATURES.SPECIES.ICEBELLY.EGG_NAME, global::STRINGS.CREATURES.SPECIES.ICEBELLY.DESC, "egg_icebelly_kanim", 8f, "IceBellyBaby", 120.00001f, 40f, BellyTuning.EGG_CHANCES_BASE, this.GetDlcIds(), IceBellyConfig.EGG_SORT_ORDER, true, false, true, 1f, false);
		gameObject.AddTag(GameTags.LargeCreature);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "IceBelly";

	public const string BASE_TRAIT_ID = "IceBellyBaseTrait";

	public const string EGG_ID = "IceBellyEgg";

	public const int GERMS_EMMITED_PER_KG_POOPED = 1000;

	public static Tag SCALE_GROWTH_EMIT_ELEMENT = BasicFabricConfig.ID;

	public static float SCALE_INITIAL_GROWTH_PCT = 0.25f;

	public static float SCALE_GROWTH_TIME_IN_CYCLES = 10f;

	public static float FIBER_PER_CYCLE = 0.5f;

	private static float CALORIES_PER_UNIT_EATEN = FOOD.FOOD_TYPES.CARROT.CaloriesPerUnit;

	public static float CONSUMABLE_PLANT_MATURITY_LEVELS = CROPS.CROP_TYPES.Find((Crop.CropVal m) => m.cropId == CarrotConfig.ID).cropDuration / 600f;

	private const float CONSUMED_MASS_TO_POOP_MASS_MULTIPLIER = 67.474f;

	private const float MIN_POOP_SIZE_IN_KG = 1f;

	public static int EGG_SORT_ORDER = 0;
}
