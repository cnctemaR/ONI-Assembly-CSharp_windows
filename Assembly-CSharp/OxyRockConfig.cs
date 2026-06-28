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

	public void ConfigurePrefab(GameObject go)
	{
		GeneratedOre.ConfigureAnims(go, "oxyrock_kanim");
		Sublimates sublimates = go.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.OxygenEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.4f, 0f, 1.8f, 1f, this.SublimeElementID);
	}
}
