using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GasketConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return null;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("PlasticGasket", ITEMS.INDUSTRIAL_PRODUCTS.PLASTIC_GASKET.NAME, ITEMS.INDUSTRIAL_PRODUCTS.PLASTIC_GASKET.DESC, 1f, true, Assets.GetAnim("gasket_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, 0, SimHashes.Creature, new List<Tag>
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

	public const string ID = "PlasticGasket";

	public static readonly Tag tag = TagManager.Create("PlasticGasket");

	public const float MASS = 1f;

	public const float RECIPE_PRODUCTION_MASS = 50f;
}
