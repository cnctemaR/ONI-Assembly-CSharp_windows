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

	public void ConfigurePrefab(GameObject go)
	{
		GeneratedOre.ConfigureAnims(go, "toxic_sand_kanim");
		Sublimates sublimates = go.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(0.00020000001f, 0.0025000002f, 1.8f, 0.5f, this.SublimeElementID);
	}
}
