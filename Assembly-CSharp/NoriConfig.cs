using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class NoriConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Nori", global::STRINGS.ITEMS.FOOD.NORI.NAME, global::STRINGS.ITEMS.FOOD.NORI.DESC, 1f, false, Assets.GetAnim("nori_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true, 0, SimHashes.Creature, new List<Tag> { GameTags.PedestalDisplayable });
		gameObject.AddOrGet<EntitySplitter>();
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.NORI);
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Nori";

	public const float KCAL_PER_UNIT = 400000f;
}
