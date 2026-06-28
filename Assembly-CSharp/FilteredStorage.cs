using System;
using UnityEngine;

public class FilteredStorage
{
	public FilteredStorage(KMonoBehaviour root, Tag[] forbidden_tags, Color32 filter_tint, Color32 no_filter_tint, IUserControlledCapacity capacity_control)
	{
		this.root = root;
		this.forbiddenTags = forbidden_tags;
		this.filterTint = filter_tint;
		this.noFilterTint = no_filter_tint;
		this.capacityControl = capacity_control;
		root.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		this.filterable = root.FindOrAdd<TreeFilterable>();
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		this.storage = root.GetComponent<Storage>();
		Storage storage = this.storage;
		storage.onPriorityChanged = (global::System.Action)Delegate.Combine(storage.onPriorityChanged, new global::System.Action(delegate
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}));
		if (FilteredStorage.capacityStatusItem == null)
		{
			FilteredStorage.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.Regions, SimViewMode.None, true, 2046);
			FilteredStorage.capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				FilteredStorage filteredStorage = (FilteredStorage)data;
				string text = Util.FormatWholeNumber(filteredStorage.storage.MassStored());
				float num = filteredStorage.storage.capacityKg;
				IUserControlledCapacity component = filteredStorage.root.GetComponent<IUserControlledCapacity>();
				if (component != null)
				{
					num = Mathf.Min(component.UserMaxCapacity, num);
				}
				string text2 = Util.FormatWholeNumber(num);
				str = str.Replace("{Stored}", text);
				str = str.Replace("{Capacity}", text2);
				return str;
			};
			FilteredStorage.noFilterStatusItem = new StatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.Regions, SimViewMode.None, true, 2046);
		}
		root.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, FilteredStorage.capacityStatusItem, this);
	}

	private void CreateMeter()
	{
		this.meter = new MeterController(this.root.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_frame", "meter_level" });
		this.meter.SetFilterByAnim(false);
	}

	public void CleanUp()
	{
		if (this.filterable != null)
		{
			TreeFilterable treeFilterable = this.filterable;
			treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Remove(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		}
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Parent destroyed");
		}
	}

	public void FilterChanged()
	{
		if (this.meter == null)
		{
			this.CreateMeter();
		}
		this.OnFilterChanged(this.filterable.GetTags());
		this.UpdateMeter();
	}

	private void OnStorageChanged(object data)
	{
		if (this.fetchList == null)
		{
			this.OnFilterChanged(this.filterable.GetTags());
		}
		this.UpdateMeter();
	}

	private void UpdateMeter()
	{
		if (this.meter != null)
		{
			float maxCapacity = this.GetMaxCapacity();
			this.meter.SetPositionPercent(Mathf.Clamp01(this.storage.MassStored() / maxCapacity));
		}
	}

	private void OnFetchComplete()
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	private float GetMaxCapacity()
	{
		float num = this.storage.capacityKg;
		if (this.capacityControl != null)
		{
			num = Mathf.Min(num, this.capacityControl.UserMaxCapacity);
		}
		return num;
	}

	private void OnFilterChanged(Tag[] tags)
	{
		KBatchedAnimController component = this.root.GetComponent<KBatchedAnimController>();
		bool flag = tags != null && tags.Length != 0;
		component.TintColour = ((!flag) ? this.noFilterTint : this.filterTint);
		if (this.fetchList != null)
		{
			this.fetchList.Cancel(string.Empty);
			this.fetchList = null;
		}
		float maxCapacity = this.GetMaxCapacity();
		float num = this.storage.MassStored();
		float num2 = Mathf.Max(0f, maxCapacity - num);
		int num3 = (int)num2;
		if (num3 <= 0)
		{
			return;
		}
		if (flag)
		{
			this.fetchList = new FetchList2(this.storage);
			this.fetchList.ShowStatusItem = false;
			this.fetchList.Add(tags, this.forbiddenTags, (float)num3, FetchOrder2.OperationalRequirement.None);
			this.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
		}
		this.root.GetComponent<KSelectable>().ToggleStatusItem(FilteredStorage.noFilterStatusItem, !flag, this);
	}

	public void SetEnabled(bool enabled)
	{
		if (enabled)
		{
			if (this.fetchList == null)
			{
				this.OnFilterChanged(this.filterable.GetTags());
			}
		}
		else if (this.fetchList != null)
		{
			this.fetchList.Cancel("Toggle closed");
			this.fetchList = null;
		}
	}

	private KMonoBehaviour root;

	private FetchList2 fetchList;

	private IUserControlledCapacity capacityControl;

	private TreeFilterable filterable;

	private Storage storage;

	private MeterController meter;

	private Color32 filterTint;

	private Color32 noFilterTint;

	private Tag[] forbiddenTags;

	private static StatusItem capacityStatusItem;

	private static StatusItem noFilterStatusItem;
}
