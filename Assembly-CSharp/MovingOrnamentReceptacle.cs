using System;
using UnityEngine;

public class MovingOrnamentReceptacle : OrnamentReceptacle, ISim1000ms
{
	protected override void OnPrefabInit()
	{
		this.prefabID = base.GetComponent<KPrefabID>();
		base.OnPrefabInit();
		base.Subscribe(144050788, new Action<object>(this.OnRoomUpdate));
		this.UpdateCavity();
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

	public void Sim1000ms(float dt)
	{
		this.UpdateCavity();
	}

	private void OnRoomUpdate(object roomInfo)
	{
		if (roomInfo == null)
		{
			this.UpdateCavity();
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		base.Unsubscribe(144050788, new Action<object>(this.OnRoomUpdate));
		this.UnregisterFromLastCavity();
	}

	public void UpdateCavity()
	{
		int num = Grid.PosToCell(base.gameObject);
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
		if (this.lastCavity != cavityForCell)
		{
			this.UnregisterFromLastCavity();
			if (cavityForCell != null)
			{
				cavityForCell.AddEntity(this.prefabID);
				Game.Instance.roomProber.UpdateRoom(cavityForCell);
			}
			this.lastCavity = cavityForCell;
		}
	}

	private void UnregisterFromLastCavity()
	{
		if (this.lastCavity != null)
		{
			this.lastCavity.RemoveFromCavity(this.prefabID, this.lastCavity.otherEntities);
			Game.Instance.roomProber.UpdateRoom(this.lastCavity);
		}
		this.lastCavity = null;
	}

	[MyCmpReq]
	private SnapOn snapOn;

	private Navigator navigator;

	private KPrefabID prefabID;

	private KBatchedAnimTracker occupyingTracker;

	private KAnimLink animLink;

	private CavityInfo lastCavity;
}
