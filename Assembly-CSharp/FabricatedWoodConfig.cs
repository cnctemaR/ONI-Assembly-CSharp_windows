using System;
using UnityEngine;

public class FabricatedWoodConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.FabricatedWood;
		}
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(this.ElementID, null);
		gameObject.GetComponent<KPrefabID>().RemoveTag(GameTags.HideFromSpawnTool);
		return gameObject;
	}

	public const string ID = "FabricatedWood";

	public static readonly Tag TAG = TagManager.Create("FabricatedWood");
}
