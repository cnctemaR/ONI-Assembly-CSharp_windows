using System;
using UnityEngine;

public class DeployingPioneerLanderFXConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("DeployingPioneerLanderFX", "DeployingPioneerLanderFX", false);
		ClusterFXEntity clusterFXEntity = gameObject.AddOrGet<ClusterFXEntity>();
		clusterFXEntity.kAnimName = "pioneer01_kanim";
		clusterFXEntity.animName = "landing";
		clusterFXEntity.animPlayMode = KAnim.PlayMode.Loop;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DeployingPioneerLanderFX";
}
