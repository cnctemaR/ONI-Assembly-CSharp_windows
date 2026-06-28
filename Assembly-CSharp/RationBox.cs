using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RationBox : KMonoBehaviour, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChange));
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_frame", "meter_level" });
	}

	protected override void OnSpawn()
	{
		this.OnStorageChange(null);
		this.operational.SetActive(this.operational.IsOperational, false);
	}

	private void OnStorageChange(object data)
	{
		if (this.fetchList == null)
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.StorageLocker, this);
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
		if (this.closed)
		{
			return;
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
			this.fetchList.Add(tags, (float)num, false);
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

	[Serialize]
	private bool closed;

	[SerializeField]
	public Color noFilterTint = Color.white;

	[SerializeField]
	public Color filterTint = Color.white;
}
