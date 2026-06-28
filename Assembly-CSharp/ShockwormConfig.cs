using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ShockwormConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "ShockWorm";
		string text2 = global::STRINGS.CREATURES.SPECIES.SHOCKWORM.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SHOCKWORM.DESC;
		float num = 50f;
		KAnimFile anim = Assets.GetAnim("shockworm_kanim");
		string text4 = "idle";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, Grid.SceneLayer.Creatures, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Hostile;
		text4 = null;
		text3 = "FlyerNavGrid1x2";
		NavType navType = NavType.Hover;
		num = 2f;
		text2 = "Meat";
		int num2 = 3;
		float freezing_ = global::TUNING.CREATURES.TEMPERATURE.FREEZING_1;
		float hot_ = global::TUNING.CREATURES.TEMPERATURE.HOT_1;
		float hot_2 = global::TUNING.CREATURES.TEMPERATURE.HOT_2;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, text4, text3, navType, 32, num, text2, num2, true, true, 30f, freezing_, hot_, global::TUNING.CREATURES.TEMPERATURE.FREEZING_2, hot_2);
		gameObject.AddOrGet<LoopingSounds>();
		Weapon weapon = gameObject.AddWeapon(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f);
		weapon.AddEffect("WasAttacked", 1f);
		SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "Shockworm_attack_arc", NOISE_POLLUTION.CREATURES.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ShockWorm";
}
