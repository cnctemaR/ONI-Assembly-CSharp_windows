using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class PrehistoricPacuConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreatePrehistoricPacu(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = EntityTemplates.ExtendEntityToWildCreature(BasePrehistoricPacuConfig.CreatePrefab(id, "PrehistoricPacuBaseTrait", name, desc, anim_file, is_baby, null, 273.15f, 333.15f, 253.15f, 373.15f), PrehistoricPacuTuning.PEN_SIZE_PER_CREATURE, false);
		EntityTemplates.CreateAndRegisterBaggedCreature(gameObject, true, true, false);
		Trait trait = Db.Get().CreateTrait("PrehistoricPacuBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PrehistoricPacuTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -PrehistoricPacuTuning.STANDARD_CALORIES_PER_CYCLE / 600f, UI.TOOLTIPS.BASE_VALUE, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 50f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name, false, false, true));
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
		GameObject gameObject = EntityTemplates.ExtendEntityToFertileCreature(PrehistoricPacuConfig.CreatePrehistoricPacu("PrehistoricPacu", CREATURES.SPECIES.PREHISTORICPACU.NAME, CREATURES.SPECIES.PREHISTORICPACU.DESC, "paculacanth_kanim", false), this, "PrehistoricPacuEgg", CREATURES.SPECIES.PREHISTORICPACU.EGG_NAME, CREATURES.SPECIES.PREHISTORICPACU.DESC, "egg_paculacanth_kanim", PrehistoricPacuTuning.EGG_MASS, "PrehistoricPacuBaby", 60.000004f, 20f, PrehistoricPacuTuning.EGG_CHANCES_BASE, 500, false, true, 0.75f, false);
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

	public const string ID = "PrehistoricPacu";

	public const string BASE_TRAIT_ID = "PrehistoricPacuBaseTrait";

	public const string EGG_ID = "PrehistoricPacuEgg";

	public const int EGG_SORT_ORDER = 500;
}
