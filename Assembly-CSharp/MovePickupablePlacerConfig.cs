using System;
using STRINGS;
using UnityEngine;

public class MovePickupablePlacerConfig : CommonPlacerConfig, IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = base.CreatePrefab(MovePickupablePlacerConfig.ID, MISC.PLACERS.MOVEPICKUPABLEPLACER.NAME, Assets.instance.movePickupToPlacerAssets.material);
		gameObject.AddOrGet<CancellableMove>();
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.showUnreachableStatus = true;
		gameObject.AddOrGet<Approachable>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddTag(GameTags.NotConversationTopic);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "MovePickupablePlacer";

	[Serializable]
	public class MovePickupablePlacerAssets
	{
		public Material material;
	}
}
