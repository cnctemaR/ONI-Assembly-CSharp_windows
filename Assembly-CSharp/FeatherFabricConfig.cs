using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class FeatherFabricConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(FeatherFabricConfig.ID, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.FEATHER_FABRIC.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.FEATHER_FABRIC.DESC, 1f, true, Assets.GetAnim("feather_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, true, SORTORDER.BUILDINGELEMENTS + BasicFabricTuning.SORTORDER, SimHashes.Creature, new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.BuildingFiber,
			GameTags.PedestalDisplayable
		});
		gameObject.AddOrGet<EntitySplitter>();
		KBoxCollider2D kboxCollider2D = gameObject.AddOrGet<KBoxCollider2D>();
		kboxCollider2D.offset = new Vector2f(0f, 0.3f);
		kboxCollider2D.size = new Vector2f(0.8f, 0.8f);
		gameObject.AddOrGet<PrefabAttributeModifiers>().AddAttributeDescriptor(this.decorModifier);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "FeatherFabric";

	private AttributeModifier decorModifier = new AttributeModifier("Decor", 0.1f, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME, true, false, true);
}
