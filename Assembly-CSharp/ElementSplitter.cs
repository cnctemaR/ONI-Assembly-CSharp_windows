using System;
using UnityEngine;

public struct ElementSplitter
{
	public ElementSplitter(GameObject go)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.onTakeCB = null;
		this.canAbsorbCB = null;
	}

	public PrimaryElement primaryElement;

	public Func<float, Pickupable> onTakeCB;

	public Func<Pickupable, bool> canAbsorbCB;
}
