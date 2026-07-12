using System;
using KSerialization;
using UnityEngine;

public class PedestalArtifactSpawner : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.artifactSpawned)
		{
			return;
		}
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ArtifactSelector.Instance.GetUniqueArtifactID()), base.transform.position);
		gameObject.SetActive(true);
		gameObject.AddTag(GameTags.TerrestrialArtifact);
		this.storage.Store(gameObject, false, false, true, false);
		this.receptacle.ForceDeposit(gameObject);
		this.artifactSpawned = true;
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private SingleEntityReceptacle receptacle;

	[Serialize]
	private bool artifactSpawned;
}
