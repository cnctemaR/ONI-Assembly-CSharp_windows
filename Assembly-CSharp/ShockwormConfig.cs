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
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("shockworm_kanim"), "idle", Grid.SceneLayer.Creatures, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		FactionManager.FactionID factionID = FactionManager.FactionID.Hostile;
		string text4 = null;
		string text5 = "FlyerNavGrid1x2";
		NavType navType = NavType.Hover;
		int num2 = 32;
		float num3 = 2f;
		string text6 = "Meat";
		int num4 = 3;
		bool flag = true;
		bool flag2 = true;
		float freezing_ = global::TUNING.CREATURES.TEMPERATURE.FREEZING_2;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, factionID, text4, text5, navType, num2, num3, text6, num4, flag, flag2, global::TUNING.CREATURES.TEMPERATURE.FREEZING_1, global::TUNING.CREATURES.TEMPERATURE.HOT_1, freezing_, global::TUNING.CREATURES.TEMPERATURE.HOT_2);
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddWeapon(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f).AddEffect("WasAttacked", 1f);
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
