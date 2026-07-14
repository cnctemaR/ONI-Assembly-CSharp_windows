using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CaviarConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Caviar", ITEMS.FOOD.CAVIAR.NAME, ITEMS.FOOD.CAVIAR.DESC, 1f, false, Assets.GetAnim("caviar_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.32f, 0.32f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.Other,
			GameTags.PedestalDisplayable
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Caviar";

	public static readonly Tag TAG = "Caviar".ToTag();
}
