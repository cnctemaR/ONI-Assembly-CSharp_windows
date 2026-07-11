using System;
using UnityEngine;

public class MouthAnimation : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(MouthAnimation.ID, MouthAnimation.ID, false);
		gameObject.AddOrGet<KBatchedAnimController>().AnimFiles = new KAnimFile[] { Assets.GetAnim("anim_mouth_flap_kanim") };
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "MouthAnimation";
}
