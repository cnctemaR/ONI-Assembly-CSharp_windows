using System;
using UnityEngine;

public interface IOreConfig
{
	SimHashes ElementID { get; }

	SimHashes SublimeElementID { get; }

	void ConfigurePrefab(GameObject go);
}
