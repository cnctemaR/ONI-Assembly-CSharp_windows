using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class LightBugConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "LightBug";
		string text2 = global::STRINGS.CREATURES.SPECIES.LIGHTBUG.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.LIGHTBUG.DESC;
		float num = 50f;
		KAnimFile anim = Assets.GetAnim("lightbug_kanim");
		string text4 = "idle";
		EffectorValues tier = DECOR.BONUS.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, Grid.SceneLayer.Creatures, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		FactionManager.FactionID factionID = FactionManager.FactionID.Prey;
		num = 5f;
		text4 = "FlyerNavGrid1x1";
		NavType navType = NavType.Hover;
		float num2 = 2f;
		text3 = "Meat";
		int num3 = 0;
		float freezing_ = global::TUNING.CREATURES.TEMPERATURE.FREEZING_1;
		float hot_ = global::TUNING.CREATURES.TEMPERATURE.HOT_1;
		float hot_2 = global::TUNING.CREATURES.TEMPERATURE.HOT_2;
		EntityTemplates.ExtendEntityToBasicCreature(gameObject2, factionID, num, text4, navType, 32, num2, text3, num3, true, true, 30f, freezing_, hot_, global::TUNING.CREATURES.TEMPERATURE.FREEZING_2, hot_2);
		gameObject.UpdateComponentRequirement<LightBug>(true);
		gameObject.UpdateComponentRequirement<LoopingSounds>(true);
		Light2D light2D = gameObject.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.LIGHTBUG_COLOR;
		light2D.Range = 5f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
		light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
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
