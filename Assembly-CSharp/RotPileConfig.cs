using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RotPileConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("RotPile", ITEMS.FOOD.ROTPILE.NAME, ITEMS.FOOD.ROTPILE.DESC, 1f, false, Assets.GetAnim("rotfood_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, SimHashes.Creature, null);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddPrefabTags(new List<Tag> { GameTags.Organics });
		gameObject.UpdateComponentRequirement<EntitySplitter>(true);
		gameObject.UpdateComponentRequirement<OccupyArea>(true);
		gameObject.UpdateComponentRequirement<Modifiers>(true);
		gameObject.UpdateComponentRequirement<RotPile>(true);
		DecorProvider decorProvider = gameObject.AddComponent<DecorProvider>();
		decorProvider.SetValues(DECOR.PENALTY.TIER2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<DecorProvider>().overrideName = ITEMS.FOOD.ROTPILE.NAME;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
