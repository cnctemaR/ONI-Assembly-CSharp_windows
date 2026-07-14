using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class UrchinConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("Urchin", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.URCHIN.NAME, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.URCHIN.DESC, 100f, true, Assets.GetAnim("urchin_pod_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.4f, 0.4f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.PedestalDisplayable,
			GameTags.IndustrialProduct
		});
		gameObject.GetComponent<KCollider2D>().offset = new Vector2(0f, 0.05f);
		gameObject.AddOrGet<OccupyArea>().SetCellOffsets(EntityTemplates.GenerateOffsets(1, 1));
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(DECOR.PENALTY.TIER1);
		decorProvider.overrideName = global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.URCHIN.NAME;
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Urchin";
}
