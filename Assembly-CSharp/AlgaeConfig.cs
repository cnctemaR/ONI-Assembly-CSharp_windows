using System;
using System.Collections.Generic;
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

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(this.ElementID, new List<Tag> { GameTags.Life });
		DissolvingAlgae dissolvingAlgae = gameObject.AddOrGet<DissolvingAlgae>();
		dissolvingAlgae.emitRange = 1;
		dissolvingAlgae.emitCount = 1000;
		return gameObject;
	}
}
