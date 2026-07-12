using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PinkRockCarvedConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("PinkRockCarved", global::STRINGS.CREATURES.SPECIES.PINKROCKCARVED.NAME, global::STRINGS.CREATURES.SPECIES.PINKROCKCARVED.DESC, 1f, true, Assets.GetAnim("pinkrock_decor_kanim"), "idle", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.5f, 0.5f, true, 0, SimHashes.Creature, new List<Tag>
		{
			GameTags.RareMaterials,
			GameTags.MiscPickupable,
			GameTags.PedestalDisplayable,
			GameTags.Experimental
		});
		gameObject.AddOrGet<OccupyArea>();
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(global::TUNING.BUILDINGS.DECOR.BONUS.TIER1);
		decorProvider.overrideName = gameObject.GetProperName();
		Light2D light2D = gameObject.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.PINKROCK_COLOR;
		light2D.Color = LIGHT2D.PINKROCK_COLOR;
		light2D.Range = 3f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.PINKROCK_DIRECTION;
		light2D.Offset = LIGHT2D.PINKROCK_OFFSET;
		light2D.shape = global::LightShape.Circle;
		light2D.drawOverlay = true;
		gameObject.GetComponent<KCircleCollider2D>().offset = new Vector2(0f, 0.25f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PinkRockCarved";
}
