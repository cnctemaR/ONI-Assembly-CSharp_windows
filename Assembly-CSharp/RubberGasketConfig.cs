using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RubberGasketConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("RubberGasket", ITEMS.INDUSTRIAL_PRODUCTS.RUBBER_GASKET.NAME, ITEMS.INDUSTRIAL_PRODUCTS.RUBBER_GASKET.DESC, 1f, true, Assets.GetAnim("rubber_gasket_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialProduct,
			GameTags.MiscPickupable,
			GameTags.PedestalDisplayable,
			GameTags.BuildingGasket
		});
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "RubberGasket";

	public static readonly Tag tag = TagManager.Create("RubberGasket");

	public const float MASS = 1f;

	public const float RECIPE_PRODUCTION_MASS = 50f;
}
