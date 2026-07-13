using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ChameleonConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateChameleon(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseChameleonConfig.BaseChameleon(id, name, desc, anim_file, "ChameleonBaseTrait", is_baby, null, 233.15f, 293.15f, 173.15f, 373.15f);
		gameObject = EntityTemplates.ExtendEntityToWildCreature(gameObject, ChameleonTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("ChameleonBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, ChameleonTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -ChameleonTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, (float)ChameleonConfig.LIFESPAN, name, false, false, true));
		Diet diet = new Diet(new Diet.Info[]
		{
			new Diet.Info(new HashSet<Tag> { DewDripConfig.ID.ToTag() }, ChameleonConfig.POOP_ELEMENT, ChameleonConfig.CALORIES_PER_DRIP_EATEN, ChameleonConfig.KG_POOP_PER_DRIP, null, 0f, false, Diet.Info.FoodType.EatSolid, false, null)
		});
		CreatureCalorieMonitor.Def def = gameObject.AddOrGetDef<CreatureCalorieMonitor.Def>();
		def.diet = diet;
		def.minConsumedCaloriesBeforePooping = ChameleonConfig.MIN_POOP_SIZE_IN_CALORIES;
		gameObject.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
		gameObject.AddOrGetDef<SetNavOrientationOnSpawnMonitor.Def>();
		gameObject.AddTag(GameTags.OriginalCreature);
		EntityTemplates.AddSecondaryExcretion(gameObject, SimHashes.ChlorineGas, 0.005f);
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

	public virtual GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFertileCreature(ChameleonConfig.CreateChameleon("Chameleon", CREATURES.SPECIES.CHAMELEON.NAME, CREATURES.SPECIES.CHAMELEON.DESC, "chameleo_kanim", false), this, "ChameleonEgg", CREATURES.SPECIES.CHAMELEON.EGG_NAME, CREATURES.SPECIES.CHAMELEON.DESC, "egg_chameleo_kanim", ChameleonTuning.EGG_MASS, "ChameleonBaby", 0.6f * (float)ChameleonConfig.LIFESPAN, 0.2f * (float)ChameleonConfig.LIFESPAN, ChameleonTuning.EGG_CHANCES_BASE, ChameleonConfig.EGG_SORT_ORDER, true, false, 1f, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Chameleon";

	public const string BASE_TRAIT_ID = "ChameleonBaseTrait";

	public const string EGG_ID = "ChameleonEgg";

	public static Tag POOP_ELEMENT = SimHashes.BleachStone.CreateTag();

	private static float DRIPS_EATEN_PER_CYCLE = 1f;

	private static float CALORIES_PER_DRIP_EATEN = ChameleonTuning.STANDARD_CALORIES_PER_CYCLE / ChameleonConfig.DRIPS_EATEN_PER_CYCLE;

	private static float KG_POOP_PER_DRIP = 10f;

	private static float MIN_POOP_SIZE_IN_KG = 10f;

	private static float MIN_POOP_SIZE_IN_CALORIES = ChameleonConfig.CALORIES_PER_DRIP_EATEN * ChameleonConfig.MIN_POOP_SIZE_IN_KG / ChameleonConfig.KG_POOP_PER_DRIP;

	private static int LIFESPAN = 50;

	public static int EGG_SORT_ORDER = 800;
}
