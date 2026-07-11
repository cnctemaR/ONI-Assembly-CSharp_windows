using System;
using STRINGS;
using UnityEngine;

public class MopPlacerConfig : CommonPlacerConfig, IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = base.CreatePrefab(MopPlacerConfig.ID, MISC.PLACERS.MOPPLACER.NAME, Assets.instance.mopPlacerAssets.material);
		gameObject.AddTag(GameTags.NotConversationTopic);
		Moppable moppable = gameObject.AddOrGet<Moppable>();
		moppable.synchronizeAnims = false;
		moppable.amountMoppedPerTick = 20f;
		gameObject.AddOrGet<Cancellable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "MopPlacer";

	[Serializable]
	public class MopPlacerAssets
	{
		public Material material;
	}
}
