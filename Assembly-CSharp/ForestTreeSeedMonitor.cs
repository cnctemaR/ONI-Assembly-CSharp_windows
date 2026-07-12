using System;
using KSerialization;
using UnityEngine;

public class ForestTreeSeedMonitor : KMonoBehaviour
{
	public bool ExtraSeedAvailable
	{
		get
		{
			return this.hasExtraSeedAvailable;
		}
	}

	public void ExtractExtraSeed()
	{
		if (!this.hasExtraSeedAvailable)
		{
			return;
		}
		this.hasExtraSeedAvailable = false;
		Vector3 position = base.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
		Util.KInstantiate(Assets.GetPrefab("ForestTreeSeed"), position).SetActive(true);
	}

	public void TryRollNewSeed()
	{
		if (!this.hasExtraSeedAvailable && global::UnityEngine.Random.Range(0, 100) < 5)
		{
			this.hasExtraSeedAvailable = true;
		}
	}

	[Serialize]
	private bool hasExtraSeedAvailable;
}
