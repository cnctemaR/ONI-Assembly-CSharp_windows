using System;
using UnityEngine;

public struct OreSizeVisualizerData
{
	public OreSizeVisualizerData(GameObject go)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.onMassChangedCB = null;
	}

	public PrimaryElement primaryElement;

	public Action<object> onMassChangedCB;
}
