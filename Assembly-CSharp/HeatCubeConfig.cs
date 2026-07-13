using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class HeatCubeConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateLooseEntity("HeatCube", "Heat Cube", "A cube that holds heat.", 1000f, true, Assets.GetAnim("copper_kanim"), "idle_tallstone", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f, true, SORTORDER.BUILDINGELEMENTS, SimHashes.Diamond, new List<Tag>
		{
			GameTags.MiscPickupable,
			GameTags.IndustrialIngredient
		});
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "HeatCube";
}
