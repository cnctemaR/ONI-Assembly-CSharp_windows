using System;
using UnityEngine;

public class Refrigerator : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	protected override void OnSpawn()
	{
		this.operational.SetActive(this.operational.IsOperational, false);
		base.GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_frame", "meter_level" });
		this.meter.SetFilterByAnim(false);
		foreach (GameObject gameObject in this.storage.items)
		{
			this.SetItemRefrigerated(gameObject, false);
		}
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.StorageLocker, this);
		this.OnStorageChange(null);
		this.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
	}

	private void PreventStoredItemRotting(object data)
	{
		Storage component = base.GetComponent<Storage>();
		GameObject gameObject = (GameObject)data;
		bool flag = component.items.Contains(gameObject);
		this.SetItemRefrigerated(gameObject, flag);
	}

	private void SetItemRefrigerated(GameObject go, bool enabled)
	{
		if (go == null)
		{
			return;
		}
		if (enabled)
		{
			go.Trigger(-511823844, null);
		}
		else
		{
			go.Trigger(411746901, null);
		}
	}

	private void OnStorageChange(object data)
	{
		if (this.fetchList == null)
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}
		this.PreventStoredItemRotting(data);
		this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
	}

	private void OnFetchComplete()
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	private void OnFilterChanged(Tag[] tags)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		bool flag = tags != null && tags.Length != 0;
		component.TintColour = ((!flag) ? this.noFilterTint : this.filterTint);
		if (this.fetchList != null)
		{
			this.fetchList.Cancel(string.Empty);
			this.fetchList = null;
		}
		int num = (int)this.storage.RemainingCapacity();
		if (num <= 0)
		{
			return;
		}
		if (flag)
		{
			this.fetchList = new FetchList2(this.storage);
			this.fetchList.ShowStatusItem = false;
			this.fetchList.Add(tags, (float)num, FetchOrder2.OperationalRequirement.None);
			this.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Refrigerator destroyed.");
		}
	}

	private void OnOperationalChanged(object data)
	{
		this.operational.SetActive(this.operational.IsOperational, false);
	}

	public bool IsActive()
	{
		return this.operational.IsActive;
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private TreeFilterable filterable;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private KBatchedAnimController kanim;

	[MyCmpReq]
	private KSelectable selectable;

	private FetchList2 fetchList;

	private MeterController meter;

	[SerializeField]
	public Color noFilterTint = Color.white;

	[SerializeField]
	public Color filterTint = Color.white;
}
