using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class VistaConfigs : IMultiEntityConfig
{
	public List<GameObject> CreatePrefabs()
	{
		return new List<GameObject>
		{
			this.CreateSetPiece("BeachVista", "farmtile_kanim", 17, 10, "BeachVista", null, "beach_vista_lp"),
			this.CreateSetPiece("ReefVista", "farmtile_kanim", 17, 10, "ReefVista", null, null),
			this.CreateSetPiece("KelpVista", "farmtile_kanim", 17, 10, "KelpVista", null, null),
			this.CreateSetPiece("AbyssVista", "farmtile_kanim", 17, 10, "AbyssVista", null, null)
		};
	}

	private GameObject CreateSetPiece(string ID, string uiAnim, int width, int height, string vistaPrefabID, string spriteId = null, string audioName = null)
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, Strings.Get("STRINGS.ENTITIES.VISTAS." + ID.ToUpperInvariant() + ".NAME"), Strings.Get("STRINGS.ENTITIES.VISTAS." + ID.ToUpperInvariant() + ".DESCRIPTION"), 100f, Assets.GetAnim(uiAnim), "", Grid.SceneLayer.Background, width, height, DECOR.BONUS.TIER2, default(EffectorValues), SimHashes.Creature, null, 293f);
		Vista vista = gameObject.AddComponent<Vista>();
		vista.prefabName = vistaPrefabID;
		vista.sceneLayer = Grid.SceneLayer.Background;
		vista.width = width;
		vista.height = height;
		vista.audioName = audioName;
		gameObject.GetComponent<KSelectable>().IsSelectable = false;
		if (audioName != null)
		{
			gameObject.AddComponent<LoopingSounds>();
		}
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const Grid.SceneLayer SCENE_LAYER = Grid.SceneLayer.Background;

	public const string BEACH_VISTA_ID = "BeachVista";

	public const string REEF_VISTA_ID = "ReefVista";

	public const string KELP_VISTA_ID = "KelpVista";

	public const string ABYSS_VISTA_ID = "AbyssVista";
}
