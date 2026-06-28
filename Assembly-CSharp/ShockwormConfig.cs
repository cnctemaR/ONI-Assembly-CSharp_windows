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
		num = 50f;
		text4 = "FlyerNavGrid1x2";
		NavType navType = NavType.Hover;
		float num2 = 2f;
		text3 = "Meat";
		int num3 = 3;
		float freezing_ = TemperatureTuning.Freezing_2;
		float freezing_2 = TemperatureTuning.Freezing_1;
		float hot_ = TemperatureTuning.Hot_1;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, num, text4, navType, num2, text3, num3, true, true, 30f, freezing_2, hot_, freezing_, TemperatureTuning.Hot_2);
		gameObject.UpdateComponentRequirement<Shockworm>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
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
