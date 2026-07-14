using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SnailIronShellConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("SnailIronShell", ITEMS.INDUSTRIAL_PRODUCTS.SNAIL_IRON_SHELL.NAME, ITEMS.INDUSTRIAL_PRODUCTS.SNAIL_IRON_SHELL.DESC, 1f, false, Assets.GetAnim("snail_iron_shell_kanim"), "object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.4f, true, 0, SimHashes.Creature, new List<Tag> { GameTags.Organics });
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

	public const string ID = "SnailIronShell";

	public const float MASS = 10f;
}
