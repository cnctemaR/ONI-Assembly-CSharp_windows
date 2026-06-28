using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ShockwormConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ShockWorm", global::STRINGS.CREATURES.SPECIES.SHOCKWORM.NAME, global::STRINGS.CREATURES.SPECIES.SHOCKWORM.DESC, 50f, Assets.GetAnim("shockworm_kanim"), "idle", Grid.SceneLayer.Creatures, 1, 2, tier, SimHashes.Creature, null);
		float freezing_ = TemperatureTuning.Freezing_2;
		float freezing_2 = TemperatureTuning.Freezing_1;
		float hot_ = TemperatureTuning.Hot_1;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Hostile, 50f, "FlyerNavGrid", NavType.Hover, 2f, "Meat", 3, true, true, 30f, freezing_2, hot_, freezing_, TemperatureTuning.Hot_2);
		gameObject.UpdateComponentRequirement<Shockworm>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		Weapon weapon = gameObject.AddWeapon(3f, 6f, AttackProperties.DamageType.Standard, AttackProperties.TargetType.AreaOfEffect, 10, 4f);
		weapon.AddEffect("WasAttacked", 1f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
