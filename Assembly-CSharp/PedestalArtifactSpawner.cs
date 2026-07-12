using System;
using KSerialization;
using UnityEngine;

public class PedestalArtifactSpawner : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (GameObject gameObject in this.storage.items)
		{
			if (ArtifactSelector.Instance.GetArtifactType(gameObject.name) == ArtifactType.Terrestrial)
			{
				gameObject.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, true);
			}
		}
		if (this.artifactSpawned)
		{
			return;
		}
		GameObject gameObject2 = Util.KInstantiate(Assets.GetPrefab(ArtifactSelector.Instance.GetUniqueArtifactID(ArtifactType.Terrestrial)), base.transform.position);
		gameObject2.SetActive(true);
		gameObject2.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, true);
		this.storage.Store(gameObject2, false, false, true, false);
		this.receptacle.ForceDeposit(gameObject2);
		this.artifactSpawned = true;
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SingleEntityReceptacle receptacle;

	[Serialize]
	private bool artifactSpawned;
}
