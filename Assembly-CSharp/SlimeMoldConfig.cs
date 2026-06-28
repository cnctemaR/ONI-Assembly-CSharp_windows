using System;
using UnityEngine;

public class SlimeMoldConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.SlimeMold;
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
		GeneratedOre.ConfigureAnims(go, "slime_mold_kanim");
		Sublimates sublimates = go.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(0.025f, 0.125f, 1.8f, 0f, SimHashes.ContaminatedOxygen);
	}
}
