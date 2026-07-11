using System;
using UnityEngine;

public class OrnamentReceptacle : SingleEntityReceptacle
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("snapTo_ornament", false);
	}

	protected override void PositionOccupyingObject()
	{
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		component.transform.SetLocalPosition(new Vector3(0f, 0f, -0.1f));
		this.occupyingTracker = base.occupyingObject.AddComponent<KBatchedAnimTracker>();
		this.occupyingTracker.symbol = new HashedString("snapTo_ornament");
		this.occupyingTracker.forceAlwaysVisible = true;
		this.animLink = new KAnimLink(base.GetComponent<KBatchedAnimController>(), component);
	}

	protected override void ClearOccupant()
	{
		if (this.occupyingTracker != null)
		{
			global::UnityEngine.Object.Destroy(this.occupyingTracker);
			this.occupyingTracker = null;
		}
		if (this.animLink != null)
		{
			this.animLink.Unregister();
			this.animLink = null;
		}
		base.ClearOccupant();
	}

	[MyCmpReq]
	private SnapOn snapOn;

	private KBatchedAnimTracker occupyingTracker;

	private KAnimLink animLink;
}
