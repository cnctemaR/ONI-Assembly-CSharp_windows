using System;
using ProcGen;
using UnityEngine;

public class MinionBrain : Brain
{
	public bool IsCellClear(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 0];
		bool flag = gameObject != null && base.gameObject != gameObject && !gameObject.GetComponent<Navigator>().IsMoving();
		return (gameObject == null && !Grid.Reserved[cell]) || !flag;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.accessControlNavMask = new AccessControlNavMask(base.gameObject);
		this.Navigator.AddMask(this.accessControlNavMask);
		this.Navigator.AddMask(new NavigationFeatureMask(base.gameObject));
		this.Navigator.AddMask(new TravelTubeNavMask(base.gameObject));
		base.Subscribe(-1697596308, new Action<object>(this.AnimTrackStoredItem));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage component = base.GetComponent<Storage>();
		foreach (GameObject gameObject in component.items)
		{
			this.AddAnimTracker(gameObject);
		}
	}

	private void AnimTrackStoredItem(object data)
	{
		Storage component = base.GetComponent<Storage>();
		GameObject gameObject = (GameObject)data;
		this.RemoveTracker(gameObject);
		if (component.items.Contains(gameObject))
		{
			this.AddAnimTracker(gameObject);
		}
	}

	private void AddAnimTracker(GameObject go)
	{
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (component == null)
		{
			return;
		}
		if (component.AnimFiles != null && component.AnimFiles.Length > 0 && component.AnimFiles[0] != null && component.GetComponent<Pickupable>().trackOnPickup)
		{
			KBatchedAnimTracker kbatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.useTargetPoint = false;
			kbatchedAnimTracker.fadeOut = false;
			kbatchedAnimTracker.symbol = new HashedString("snapTo_chest");
			kbatchedAnimTracker.forceAlwaysVisible = true;
		}
	}

	private void RemoveTracker(GameObject go)
	{
		KBatchedAnimTracker component = go.GetComponent<KBatchedAnimTracker>();
		if (component != null)
		{
			global::UnityEngine.Object.DestroyObject(component);
		}
	}

	public override void UpdateBrain()
	{
		base.UpdateBrain();
		if (Game.Instance == null)
		{
			return;
		}
		if (!Game.Instance.savedInfo.discoveredSurface)
		{
			int num = Grid.PosToCell(base.gameObject);
			SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num);
			if (subWorldZoneType == SubWorld.ZoneType.Space)
			{
				Game.Instance.savedInfo.discoveredSurface = true;
				Vector3 position = base.gameObject.transform.GetPosition();
				DiscoveredSpaceMessage discoveredSpaceMessage = new DiscoveredSpaceMessage(position);
				Messenger.Instance.QueueMessage(discoveredSpaceMessage);
			}
		}
	}

	[MyCmpReq]
	public Navigator Navigator;

	[MyCmpGet]
	public OxygenBreather OxygenBreather;

	private AccessControlNavMask accessControlNavMask;
}
