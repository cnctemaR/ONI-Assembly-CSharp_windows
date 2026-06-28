using System;
using UnityEngine;

public class AlgaeConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.Algae;
		}
	}

	public SimHashes SublimeElementID
	{
		get
		{
			return SimHashes.Vacuum;
		}
	}

	public void ConfigurePrefab(GameObject go)
	{
		GeneratedOre.ConfigureAnims(go, "algae_kanim");
		KPrefabID kprefabID = go.AddOrGet<KPrefabID>();
		kprefabID.AddTag(GameTags.Life);
	}
}
