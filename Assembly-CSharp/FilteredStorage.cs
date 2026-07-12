using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FilteredStorage
{
	public void SetHasMeter(bool has_meter)
	{
		this.hasMeter = has_meter;
	}

	public FilteredStorage(KMonoBehaviour root, Tag[] forbidden_tags, IUserControlledCapacity capacity_control, bool use_logic_meter, ChoreType fetch_chore_type)
	{
		this.root = root;
		this.forbiddenTags = forbidden_tags;
		this.capacityControl = capacity_control;
		this.useLogicMeter = use_logic_meter;
		this.choreType = fetch_chore_type;
		root.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		root.Subscribe(-543130682, new Action<object>(this.OnUserSettingsChanged));
		this.filterable = root.FindOrAdd<TreeFilterable>();
		TreeFilterable treeFilterable = this.filterable;
		treeFilterable.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
		this.storage = root.GetComponent<Storage>();
		this.storage.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		this.storage.Subscribe(-1852328367, new Action<object>(this.OnFunctionalChanged));
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	private void CreateMeter()
	{
		if (!this.hasMeter)
		{
			return;
		}
		this.meter = new MeterController(this.root.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_frame", "meter_level" });
	}

	private void CreateLogicMeter()
	{
		if (!this.hasMeter)
		{
			return;
		}
		this.logicMeter = new MeterController(this.root.GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
	}

	public void CleanUp()
	{
		if (this.filterable != null)
		{
			TreeFilterable treeFilterable = this.filterable;
			treeFilterable.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Remove(treeFilterable.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
		}
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("Parent destroyed");
		}
	}

	public void FilterChanged()
	{
		if (this.hasMeter)
		{
			if (this.meter == null)
			{
				this.CreateMeter();
			}
			if (this.logicMeter == null && this.useLogicMeter)
			{
				this.CreateLogicMeter();
			}
		}
		this.OnFilterChanged(this.filterable.GetTags());
		this.UpdateMeter();
	}

	private void OnUserSettingsChanged(object data)
	{
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

	private void OnFunctionalChanged(object data)
	{
		this.OnFilterChanged(this.filterable.GetTags());
	}

	private void UpdateMeter()
	{
		float maxCapacityMinusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
		float num = Mathf.Clamp01(this.GetAmountStored() / maxCapacityMinusStorageMargin);
		if (this.meter != null)
		{
			this.meter.SetPositionPercent(num);
		}
	}

	public bool IsFull()
	{
		float maxCapacityMinusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
		float num = Mathf.Clamp01(this.GetAmountStored() / maxCapacityMinusStorageMargin);
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

	private float GetMaxCapacityMinusStorageMargin()
	{
		return this.GetMaxCapacity() - this.storage.storageFullMargin;
	}

	private float GetAmountStored()
	{
		float num = this.storage.MassStored();
		if (this.capacityControl != null)
		{
			num = this.capacityControl.AmountStored;
		}
		return num;
	}

	private bool IsFunctional()
	{
		Operational component = this.storage.GetComponent<Operational>();
		return component == null || component.IsFunctional;
	}

	private void OnFilterChanged(HashSet<Tag> tags)
	{
		bool flag = tags != null && tags.Count != 0;
		if (this.fetchList != null)
		{
			this.fetchList.Cancel("");
			this.fetchList = null;
		}
		float maxCapacityMinusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
		float amountStored = this.GetAmountStored();
		float num = Mathf.Max(0f, maxCapacityMinusStorageMargin - amountStored);
		if (num > 0f && flag && this.IsFunctional())
		{
			num = Mathf.Max(0f, this.GetMaxCapacity() - amountStored);
			this.fetchList = new FetchList2(this.storage, this.choreType);
			this.fetchList.ShowStatusItem = false;
			this.fetchList.Add(tags, this.forbiddenTags, num, Operational.State.Functional);
			this.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
		}
	}

	public void SetLogicMeter(bool on)
	{
		if (this.logicMeter != null)
		{
			this.logicMeter.SetPositionPercent(on ? 1f : 0f);
		}
	}

	public void AddForbiddenTag(Tag forbidden_tag)
	{
		if (this.forbiddenTags == null)
		{
			this.forbiddenTags = new Tag[0];
		}
		if (!this.forbiddenTags.Contains(forbidden_tag))
		{
			this.forbiddenTags = this.forbiddenTags.Append(forbidden_tag);
			this.OnFilterChanged(this.filterable.GetTags());
		}
	}

	public void RemoveForbiddenTag(Tag forbidden_tag)
	{
		if (this.forbiddenTags != null)
		{
			List<Tag> list = new List<Tag>(this.forbiddenTags);
			list.Remove(forbidden_tag);
			this.forbiddenTags = list.ToArray();
			this.OnFilterChanged(this.filterable.GetTags());
		}
	}

	public static readonly HashedString FULL_PORT_ID = "FULL";

	private KMonoBehaviour root;

	private FetchList2 fetchList;

	private IUserControlledCapacity capacityControl;

	private TreeFilterable filterable;

	private Storage storage;

	private MeterController meter;

	private MeterController logicMeter;

	private Tag[] forbiddenTags;

	private bool hasMeter = true;

	private bool useLogicMeter;

	private ChoreType choreType;
}
