using System;
using UnityEngine;

public struct OreSizeVisualizerData
{
	public OreSizeVisualizerData(GameObject go)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.onMassChangedCB = null;
		this.tierSetType = OreSizeVisualizerComponents.TiersSetType.Ores;
	}

	public PrimaryElement primaryElement;

	public Action<object> onMassChangedCB;

	public OreSizeVisualizerComponents.TiersSetType tierSetType;
}
