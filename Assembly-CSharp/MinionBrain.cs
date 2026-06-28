using System;
using System.Collections.Generic;
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
		Storage component = base.GetComponent<Storage>();
		component.defaultStoredItemModifers = MinionBrain.MinionStoredItemModifiers;
		this.accessControlNavMask = new AccessControlNavMask(base.gameObject, int.MaxValue);
		this.Navigator.AddMask(this.accessControlNavMask);
		this.Navigator.AddMask(new NavigationFeatureMask(base.gameObject));
		this.Subscribe(-1697596308, new Action<object>(this.AnimTrackStoredItem));
	}

	public void SetMaxNavCost(int max_path_cost)
	{
		this.accessControlNavMask.SetMaxPathCost(max_path_cost);
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
		if (component.AnimFiles != null && component.AnimFiles.Count > 0 && component.AnimFiles[0] != null)
		{
			KBatchedAnimTracker kbatchedAnimTracker = go.AddComponent<KBatchedAnimTracker>();
			kbatchedAnimTracker.useTargetPoint = false;
			kbatchedAnimTracker.filterByAnim = false;
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

	private void FixedUpdate()
	{
		FallMonitor.Instance smi = this.GetSMI<FallMonitor.Instance>();
		if (smi != null)
		{
			smi.FixedUpdate();
		}
	}

	[MyCmpReq]
	public Navigator Navigator;

	[MyCmpGet]
	public OxygenBreather OxygenBreather;

	private AccessControlNavMask accessControlNavMask;

	private static readonly List<Storage.StoredItemModifier> MinionStoredItemModifiers = new List<Storage.StoredItemModifier>();
}
