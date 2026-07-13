using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class ButterflyConfig : IEntityConfig, IHasDlcRestrictions
{
	public static GameObject CreateButterfly(string id, string name, string desc, string anim_file)
	{
		GameObject gameObject = BaseButterflyConfig.BaseButterfly(id, name, desc, anim_file, "ButterflyBaseTrait", null);
		gameObject.AddOrGetDef<AgeMonitor.Def>();
		gameObject.AddOrGetDef<FixedCapturableMonitor.Def>();
		Trait trait = Db.Get().CreateTrait("ButterflyBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 5f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 5f, name, false, false, true));
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
		GameObject gameObject = ButterflyConfig.CreateButterfly("Butterfly", CREATURES.SPECIES.BUTTERFLY.NAME, CREATURES.SPECIES.BUTTERFLY.DESC, "pollinator_kanim");
		gameObject.AddTag(GameTags.Creatures.Pollinator);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Butterfly";

	public const string BASE_TRAIT_ID = "ButterflyBaseTrait";
}
