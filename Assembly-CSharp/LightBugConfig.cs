using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("LightBug", global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME, global::STRINGS.CREATURES.SPECIES.LIGHTBUG.DESC, 50f, Assets.GetAnim("lightbug_kanim"), "idle", Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		float freezing_ = TemperatureTuning.Freezing_2;
		float freezing_2 = TemperatureTuning.Freezing_1;
		float hot_ = TemperatureTuning.Hot_1;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject, FactionManager.FactionID.Prey, 5f, "FlyerNavGrid1x1", NavType.Hover, 2f, "Meat", 0, true, true, 30f, freezing_2, hot_, freezing_, TemperatureTuning.Hot_2);
		gameObject.UpdateComponentRequirement<LightBug>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		Light2D light2D = gameObject.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.LIGHTBUG_COLOR;
		light2D.Intensity = 0.5f;
		light2D.Range = 5f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
		light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
		SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "Shockworm_attack_arc", NOISE_POLLUTION.CREATURES.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "LightBug";
}
