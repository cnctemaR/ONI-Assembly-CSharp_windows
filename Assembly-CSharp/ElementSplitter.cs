using System;
using UnityEngine;

public struct ElementSplitter
{
	public ElementSplitter(GameObject go)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.kPrefabID = go.GetComponent<KPrefabID>();
		this.onTakeCB = null;
		this.canAbsorbCB = null;
	}

	public PrimaryElement primaryElement;

	public Func<Pickupable, float, Pickupable> onTakeCB;

	public Func<Pickupable, bool> canAbsorbCB;

	public KPrefabID kPrefabID;
}
