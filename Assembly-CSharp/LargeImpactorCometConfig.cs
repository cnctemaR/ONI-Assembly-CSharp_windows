using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LargeImpactorCometConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return new string[] { "DLC4_ID" };
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(LargeImpactorCometConfig.ID, UI.SPACEDESTINATIONS.COMETS.ROCKCOMET.NAME, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<LoopingSounds>();
		LargeComet largeComet = gameObject.AddOrGet<LargeComet>();
		largeComet.impactSound = "Meteor_Large_Impact";
		largeComet.flyingSoundID = 2;
		largeComet.additionalAnimFiles.Add(new KeyValuePair<string, string>("asteroid_wind_kanim", "wind_loop"));
		largeComet.additionalAnimFiles.Add(new KeyValuePair<string, string>("asteroid_flame_inner_kanim", "flame_loop"));
		largeComet.mainAnimFile = new KeyValuePair<string, string>("asteroid_001_kanim", "idle");
		PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
		primaryElement.SetElement(SimHashes.Regolith, true);
		primaryElement.Temperature = 20000f;
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("asteroid_flame_outer_kanim") };
		kbatchedAnimController.isMovable = true;
		kbatchedAnimController.initialAnim = "flame_loop";
		kbatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kbatchedAnimController.animScale = 0.2f;
		kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
		gameObject.AddOrGet<KCircleCollider2D>().radius = 0.5f;
		gameObject.AddTag(GameTags.Comet);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
		LargeComet largeComet = go.AddOrGet<LargeComet>();
		largeComet.additionalAnimFiles.Add(new KeyValuePair<string, string>("asteroid_wind_kanim", "wind_loop"));
		largeComet.additionalAnimFiles.Add(new KeyValuePair<string, string>("asteroid_flame_inner_kanim", "flame_loop"));
		largeComet.mainAnimFile = new KeyValuePair<string, string>("asteroid_001_kanim", "idle");
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "LargeImpactorComet";

	private const SimHashes element = SimHashes.Regolith;

	private const int ADDED_CELLS = 6;
}
