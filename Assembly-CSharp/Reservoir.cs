using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Reservoir : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.log = new LoggerFS("Reservoir");
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(this, Meter.Offset.Infront, new string[] { "meter_fill", "meter_OL" });
		Storage component = base.GetComponent<Storage>();
		component.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
		this.OnStorageChange(null);
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	private void OnStorageChange(object data)
	{
		Storage component = base.GetComponent<Storage>();
		if (this.fetchList == null)
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}
		this.meter.SetPositionPercent(Mathf.Clamp01(component.MassStored() / component.capacityKg));
	}

	private void OnFilterChanged(Tag[] tags)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		bool flag = tags != null && tags.Length != 0;
		component.TintColour = ((!flag) ? this.noFilterTint : this.filterTint);
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Filter changed");
			this.fetchList = null;
		}
		BuildingEnabledButton component2 = base.GetComponent<BuildingEnabledButton>();
		if (component2 != null && !component2.IsEnabled)
		{
			return;
		}
		Storage component3 = base.GetComponent<Storage>();
		int num = (int)component3.RemainingCapacity();
		if (num <= 0)
		{
			return;
		}
		if (flag)
		{
			this.fetchList = new FetchList2(component3, Db.Get().ChoreTypes.Fetch, null);
			this.fetchList.ShowStatusItem = false;
			this.fetchList.Add(tags, null, (float)num, FetchOrder2.OperationalRequirement.None);
			this.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
		}
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoStorageFilterSet, !flag, this);
	}

	private void OnFetchComplete()
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	protected override void OnCleanUp()
	{
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("OnCleanUp");
		}
	}

	private void OnToggleClosed(object data)
	{
		BuildingEnabledButton component = base.GetComponent<BuildingEnabledButton>();
		bool flag = component != null && !component.IsEnabled;
		if (flag)
		{
			if (this.fetchList != null)
			{
				this.fetchList.Cancel("Toggle closed");
				this.fetchList = null;
			}
		}
		else if (this.fetchList == null)
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}
		base.GetComponent<UserMenu>().Refresh();
	}

	[MyCmpAdd]
	private TreeFilterable filterable;

	private MeterController meter;

	private FetchList2 fetchList;

	private LoggerFS log;

	[SerializeField]
	public Color noFilterTint = Color.white;

	[SerializeField]
	public Color filterTint = Color.white;
}
