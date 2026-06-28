using System;
using UnityEngine;

public class RationBox : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
	}

	protected override void OnSpawn()
	{
		this.OnStorageChange(null);
		this.operational.SetActive(this.operational.IsOperational, false);
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 0.5f, new Action<object>(this.UpdatePreservationStatusItems), null, null, 0f, null);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_frame", "meter_level" });
		this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
	}

	private void OnStorageChange(object data)
	{
		if (this.fetchList == null)
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.StorageLocker, this);
		if (this.meter != null)
		{
			this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / this.storage.capacityKg));
		}
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
		this.handle.Clear();
	}

	private void OnOperationalChanged(object data)
	{
		this.operational.SetActive(this.operational.IsOperational, false);
	}

	private void UpdatePreservationStatusItems(object data)
	{
		Rottable.SetStatusItems(base.GetComponent<KSelectable>(), Rottable.IsRefrigerated(base.gameObject), Rottable.AtmosphereQuality(base.gameObject));
	}

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private TreeFilterable filterable;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private KBatchedAnimController kanim;

	private MeterController meter;

	private FetchList2 fetchList;

	[SerializeField]
	public Color noFilterTint = Color.white;

	[SerializeField]
	public Color filterTint = Color.white;

	private SchedulerHandle handle;
}
