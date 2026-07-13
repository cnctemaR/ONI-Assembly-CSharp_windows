using System;
using UnityEngine;

public class TallowConfig : IOreConfig
{
	public SimHashes ElementID
	{
		get
		{
			return SimHashes.Tallow;
		}
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateSolidOreEntity(this.ElementID, null);
	}

	public const string ID = "Tallow";

	public static readonly Tag TAG = TagManager.Create("Tallow");
}
