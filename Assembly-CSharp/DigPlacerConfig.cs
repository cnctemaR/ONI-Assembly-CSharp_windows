using System;
using STRINGS;
using UnityEngine;

public class DigPlacerConfig : CommonPlacerConfig, IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = base.CreatePrefab(DigPlacerConfig.ID, MISC.PLACERS.DIGPLACER.NAME, Assets.instance.digPlacerAssets.materials[0]);
		Diggable diggable = gameObject.AddOrGet<Diggable>();
		diggable.workLayer = Grid.SceneLayer.BuildingUse;
		diggable.workTime = 5f;
		diggable.synchronizeAnims = false;
		diggable.workAnims = new HashedString[]
		{
			new HashedString("place"),
			new HashedString("release")
		};
		diggable.workPstAnim = new HashedString("working_pst");
		diggable.materials = Assets.instance.digPlacerAssets.materials;
		diggable.materialDisplay = gameObject.GetComponentInChildren<MeshRenderer>(true);
		gameObject.AddOrGet<CancellableDig>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "DigPlacer";

	[Serializable]
	public class DigPlacerAssets
	{
		public Material[] materials;
	}
}
