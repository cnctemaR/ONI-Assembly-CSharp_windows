using System;
using UnityEngine;

public class BleachStoneConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.BleachStone;
		}
	}

	public SimHashes SublimeElementID
	{
		get
		{
			return SimHashes.ChlorineGas;
		}
	}

	public void ConfigurePrefab(GameObject go)
	{
		GeneratedOre.ConfigureAnims(go, "bleach_stone_kanim");
		Sublimates sublimates = go.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.BleachStoneEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.00020000001f, 0.0025000002f, 1.8f, 0.5f, this.SublimeElementID);
	}
}
