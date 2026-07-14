using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class SquidConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateSquid(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseSquidConfig.CreatePrefab(id, "SquidBaseTrait", name, desc, anim_file, is_baby, null, 313.15f, 373.15f, 293.15f, 393.15f), SquidTuning.PEN_SIZE_PER_CREATURE, true);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		gameObject.AddTag(GameTags.OriginalCreature);
		Trait trait = Db.Get().CreateTrait("SquidBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, SquidTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -SquidTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 50f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
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
		GameObject gameObject = SquidConfig.CreateSquid("Squid", CREATURES.SPECIES.SQUID.NAME, CREATURES.SPECIES.SQUID.DESC, "squid_kanim", false);
		gameObject = EntityTemplates.ExtendEntityToFertileCreature(gameObject, this, "SquidEgg", CREATURES.SPECIES.SQUID.EGG_NAME, CREATURES.SPECIES.SQUID.DESC, "egg_squid_kanim", SquidTuning.EGG_MASS, 0f, "SquidBaby", 60.000004f, 20f, SquidTuning.EGG_CHANCES_BASE, 500, true, true, 1f, false, false, SquidTuning.EGG_MASS, true);
		gameObject.AddTag(GameTags.OriginalCreature);
		EggProtectionMonitor.Def def = gameObject.AddOrGetDef<EggProtectionMonitor.Def>();
		def.build = "squid_build_kanim";
		def.defaultFaction = FactionManager.FactionID.Prey;
		def.allyTags = new Tag[] { GameTags.Creatures.SquidFriend };
		def.eggTags = new List<Tag> { "SquidEgg".ToTag() };
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.AddOrGet<LoopingSounds>();
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Squid";

	public const string BASE_TRAIT_ID = "SquidBaseTrait";

	public const string EGG_ID = "SquidEgg";

	public const int EGG_SORT_ORDER = 500;
}
