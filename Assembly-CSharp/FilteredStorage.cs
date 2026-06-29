using System;
using UnityEngine;

public class FilteredStorage
{
	public FilteredStorage(KMonoBehaviour root, Tag[] forbidden_tags, Color32 filter_tint, Color32 no_filter_tint, IUserControlledCapacity capacity_control, bool use_logic_meter, ChoreType fetch_chore_type)
	{
		this.root = root;
		this.forbiddenTags = forbidden_tags;
		this.filterTint = filter_tint;
		this.noFilterTint = no_filter_tint;
		this.capacityControl = capacity_control;
		this.useLogicMeter = use_logic_meter;
		this.choreType = fetch_chore_type;
		root.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		this.filterable = root.FindOrAdd<TreeFilterable>();
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		this.storage = root.GetComponent<Storage>();
		this.storage.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		if (FilteredStorage.capacityStatusItem == null)
		{
			FilteredStorage.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			FilteredStorage.capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				FilteredStorage filteredStorage = (FilteredStorage)data;
				string text = Util.FormatWholeNumber(Mathf.Floor(filteredStorage.storage.MassStored()));
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
			FilteredStorage.noFilterStatusItem = new StatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, false, SimViewMode.None, true, 63486);
		}
		root.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, FilteredStorage.capacityStatusItem, this);
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	private void CreateMeter()
	{
		this.meter = new MeterController(this.root.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_frame", "meter_level" });
	}

	private void CreateLogicMeter()
	{
		this.logicMeter = new MeterController(this.root.GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, new string[0]);
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
		if (this.logicMeter == null && this.useLogicMeter)
		{
			this.CreateLogicMeter();
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
		float maxCapacity = this.GetMaxCapacity();
		float num = Mathf.Clamp01(this.storage.MassStored() / maxCapacity);
		if (this.meter != null)
		{
			this.meter.SetPositionPercent(num);
		}
	}

	public bool IsFull()
	{
		float maxCapacity = this.GetMaxCapacity();
		float num = Mathf.Clamp01(this.storage.MassStored() / maxCapacity);
		if (this.meter != null)
		{
			this.meter.SetPositionPercent(num);
		}
		return num >= 1f;
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
		if (num2 < 0.001f)
		{
			return;
		}
		if (flag)
		{
			this.fetchList = new FetchList2(this.storage, this.choreType, null);
			this.fetchList.ShowStatusItem = false;
			this.fetchList.Add(tags, this.forbiddenTags, num2, FetchOrder2.OperationalRequirement.None);
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

	public void SetLogicMeter(bool on)
	{
		if (this.logicMeter != null)
		{
			this.logicMeter.SetPositionPercent((!on) ? 0f : 1f);
		}
	}

	private const float MIN_FETCH_AMT = 0.001f;

	public static readonly HashedString FULL_PORT_ID = "FULL";

	private KMonoBehaviour root;

	private FetchList2 fetchList;

	private IUserControlledCapacity capacityControl;

	private TreeFilterable filterable;

	private Storage storage;

	private MeterController meter;

	private MeterController logicMeter;

	private Color32 filterTint;

	private Color32 noFilterTint;

	private Tag[] forbiddenTags;

	private bool useLogicMeter;

	private static StatusItem capacityStatusItem;

	private static StatusItem noFilterStatusItem;

	private ChoreType choreType;
}
