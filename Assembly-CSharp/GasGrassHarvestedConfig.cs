using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GasGrassHarvestedConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GasGrassHarvested";
		string text2 = CREATURES.SPECIES.GASGRASS.NAME;
		string text3 = CREATURES.SPECIES.GASGRASS.DESC;
		float num = 1f;
		bool flag = false;
		KAnimFile anim = Assets.GetAnim("harvested_gassygrass_kanim");
		string text4 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE;
		float num2 = 0.25f;
		float num3 = 0.25f;
		bool flag2 = true;
		List<Tag> list = new List<Tag> { GameTags.Other };
		GameObject gameObject = EntityTemplates.CreateLooseEntity(text, text2, text3, num, flag, anim, text4, sceneLayer, collisionShape, num2, num3, flag2, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GasGrassHarvested";
}
