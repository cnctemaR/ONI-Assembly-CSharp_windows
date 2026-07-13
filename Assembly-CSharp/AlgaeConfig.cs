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
		return EntityTemplates.CreateSolidOreEntity(this.ElementID, new List<Tag> { GameTags.Life });
	}
}
