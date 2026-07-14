using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SnailShellConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("SnailShell", ITEMS.INDUSTRIAL_PRODUCTS.SNAIL_SHELL.NAME, ITEMS.INDUSTRIAL_PRODUCTS.SNAIL_SHELL.DESC, 1f, false, Assets.GetAnim("snail_lime_shell_kanim"), "object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.4f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.Organics,
			GameTags.MoltShell
		});
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SnailShell";

	public const float MASS = 10f;
}
