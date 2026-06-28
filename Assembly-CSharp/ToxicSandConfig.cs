using System;
using UnityEngine;

public class ToxicSandConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.ToxicSand;
		}
	}

	public SimHashes SublimeElementID
	{
		get
		{
			return SimHashes.ContaminatedOxygen;
		}
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateOreEntity(this.ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(0.00020000001f, 0.0025000002f, 1.8f, 0.5f, this.SublimeElementID);
		return gameObject;
	}
}
