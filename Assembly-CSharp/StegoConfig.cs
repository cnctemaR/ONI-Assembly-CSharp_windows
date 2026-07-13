using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class StegoConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateStego(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BaseStegoConfig.BaseStego(id, name, desc, anim_file, "StegoBaseTrait", is_baby, null), StegoTuning.PEN_SIZE_PER_CREATURE);
		Trait trait = Db.Get().CreateTrait("StegoBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StegoTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -StegoTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 50f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 200f, name, false, false, true));
		GameObject gameObject2 = BaseStegoConfig.SetupDiet(gameObject, BaseStegoConfig.StandardDiets(), StegoTuning.CALORIES_PER_UNIT_EATEN, StegoTuning.MIN_POOP_SIZE_IN_KG);
		gameObject2.AddTag(GameTags.OriginalCreature);
		return gameObject2;
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
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(StegoConfig.CreateStego("Stego", CREATURES.SPECIES.STEGO.NAME, CREATURES.SPECIES.STEGO.DESC, "stego_kanim", false), this, "StegoEgg", CREATURES.SPECIES.STEGO.EGG_NAME, CREATURES.SPECIES.STEGO.DESC, "egg_stego_kanim", 8f, "StegoBaby", 120.00001f, 40f, StegoTuning.EGG_CHANCES_BASE, StegoConfig.EGG_SORT_ORDER, true, false, 1f, false);
		gameObject.AddTag(GameTags.LargeCreature);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Stego";

	public const string BASE_TRAIT_ID = "StegoBaseTrait";

	public const string EGG_ID = "StegoEgg";

	public static int EGG_SORT_ORDER;

	public List<Emote> StegoEmotes = new List<Emote> { Db.Get().Emotes.Critter.Roar };
}
