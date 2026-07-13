using System;
using UnityEngine;

public struct OreSizeVisualizerData
{
	public OreSizeVisualizerData(GameObject go)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.tierSetType = OreSizeVisualizerComponents.TiersSetType.Ores;
		this.absorbHandle = -1;
		this.splitFromChunkHandle = -1;
	}

	public PrimaryElement primaryElement;

	public OreSizeVisualizerComponents.TiersSetType tierSetType;

	public int absorbHandle;

	public int splitFromChunkHandle;
}
