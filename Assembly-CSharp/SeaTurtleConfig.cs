using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class SeaTurtleConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateSeaTurtle(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseSeaTurtleConfig.CreatePrefab(id, "SeaTurtleBaseTrait", name, desc, anim_file, is_baby, null, 273.15f, 333.15f, 253.15f, 373.15f), SeaTurtleTuning.PEN_SIZE_PER_CREATURE, true);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		Trait trait = Db.Get().CreateTrait("SeaTurtleBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SeaTurtleTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -SeaTurtleTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 50f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
		WellFedShearable.Def def = gameObject.AddOrGetDef<WellFedShearable.Def>();
		def.effectId = "SeaTurtleWellFed";
		def.caloriesPerCycle = SeaTurtleTuning.STANDARD_CALORIES_PER_CYCLE;
		def.growthDurationCycles = SeaTurtleTuning.SCALE_GROWTH_TIME_IN_CYCLES;
		def.dropMass = SeaTurtleTuning.ORE_PER_CYCLE * SeaTurtleTuning.SCALE_GROWTH_TIME_IN_CYCLES;
		def.itemDroppedOnShear = ElementLoader.FindElementByHash(SimHashes.IronOre).tag;
		def.levelCount = 6;
		def.scaleGrowthSymbols = SeaTurtleConfig.SCALE_GROWTH_SYMBOL_NAMES;
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
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(SeaTurtleConfig.CreateSeaTurtle("SeaTurtle", CREATURES.SPECIES.SEATURTLE.NAME, CREATURES.SPECIES.SEATURTLE.DESC, "turtle_kanim", false), this, "SeaTurtleEgg", CREATURES.SPECIES.SEATURTLE.EGG_NAME, CREATURES.SPECIES.SEATURTLE.DESC, "egg_turtle_kanim", SeaTurtleTuning.EGG_MASS, SeaTurtleTuning.EGG_SHELL_RATIO, "SeaTurtleBaby", 60.000004f, 20f, SeaTurtleTuning.EGG_CHANCES_BASE, 500, true, true, 1f, false, false, SeaTurtleTuning.EGG_MASS, true);
		gameObject.AddTag(GameTags.LargeCreature);
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

	public const string ID = "SeaTurtle";

	public const string BASE_TRAIT_ID = "SeaTurtleBaseTrait";

	public const string EGG_ID = "SeaTurtleEgg";

	public const int EGG_SORT_ORDER = 500;

	private static readonly KAnimHashedString[] SCALE_GROWTH_SYMBOL_NAMES = new KAnimHashedString[] { "shell_0", "shell_1", "shell_2", "shell_3", "shell_4" };
}
