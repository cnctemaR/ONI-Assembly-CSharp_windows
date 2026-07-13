using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class DieselMooConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return null;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public static GameObject CreateMoo(string id, string name, string desc, string anim_file, List<BeckoningMonitor.SongChance> initialSongChances, bool is_baby)
	{
		GameObject gameObject = BaseMooConfig.BaseMoo(id, name, CREATURES.SPECIES.DIESELMOO.DESC, "DieselMooBaseTrait", anim_file, initialSongChances, is_baby, "die_");
		EntityTemplates.ExtendEntityToWildCreature(gameObject, MooTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("DieselMooBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MooTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -MooTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 50f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, MooTuning.STANDARD_LIFESPAN, name, false, false, true));
		BaseMooConfig.SetupBaseDiet(gameObject, DieselMooConfig.POOP_ELEMENT);
		gameObject.AddOrGetDef<BeckoningMonitor.Def>().effectId = "HuskyMooFed";
		MilkProductionMonitor.Def def = gameObject.AddOrGetDef<MilkProductionMonitor.Def>();
		def.effectId = "HuskyMooWellFed";
		def.element = DieselMooConfig.MILK_ELEMENT;
		def.Capacity = 800f;
		gameObject.AddTag(GameTags.OriginalCreature);
		return gameObject;
	}

	public GameObject CreatePrefab()
	{
		return DieselMooConfig.CreateMoo("DieselMoo", CREATURES.SPECIES.DIESELMOO.NAME, CREATURES.SPECIES.DIESELMOO.DESC, "gassy_moo_kanim", MooTuning.DieselSongChances, false);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseMooConfig.OnSpawn(inst);
	}

	public const string ID = "DieselMoo";

	public const string BASE_TRAIT_ID = "DieselMooBaseTrait";

	public static Tag POOP_ELEMENT = SimHashes.CarbonDioxide.CreateTag();

	public static SimHashes MILK_ELEMENT = SimHashes.RefinedLipid;
}
