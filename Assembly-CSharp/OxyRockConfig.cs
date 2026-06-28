using System;
using UnityEngine;

public class OxyRockConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.OxyRock;
		}
	}

	public SimHashes SublimeElementID
	{
		get
		{
			return SimHashes.Oxygen;
		}
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateOreEntity(this.ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.OxygenEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.4f, 0f, 1.8f, 1f, this.SublimeElementID);
		return gameObject;
	}
}
