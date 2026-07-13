using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClosestPickupableSensor<T> : Sensor where T : Component
{
	public ClosestPickupableSensor(Sensors sensors, Tag itemSearchTag, bool shouldStartActive)
		: base(sensors, shouldStartActive)
	{
		this.navigator = base.GetComponent<Navigator>();
		this.consumableConsumer = base.GetComponent<ConsumableConsumer>();
		this.storage = base.GetComponent<Storage>();
		this.itemSearchTag = itemSearchTag;
	}

	public T GetItem()
	{
		return this.item;
	}

	public int GetItemNavCost()
	{
		if (!(this.item == null))
		{
			return this.itemNavCost;
		}
		return int.MaxValue;
	}

	public virtual HashSet<Tag> GetForbbidenTags()
	{
		if (!(this.consumableConsumer == null))
		{
			return this.consumableConsumer.forbiddenTagSet;
		}
		return new HashSet<Tag>(0);
	}

	public override void Update()
	{
		HashSet<Tag> forbbidenTags = this.GetForbbidenTags();
		int maxValue = int.MaxValue;
		Pickupable pickupable = this.FindClosestPickupable(this.storage, forbbidenTags, out maxValue, this.itemSearchTag, this.requiredTags);
		bool flag = this.itemInReachButNotPermitted;
		T t = default(T);
		bool flag2 = false;
		if (pickupable != null)
		{
			t = pickupable.GetComponent<T>();
			flag2 = true;
			flag = false;
		}
		else
		{
			int num;
			flag = this.FindClosestPickupable(this.storage, new HashSet<Tag>(), out num, this.itemSearchTag, this.requiredTags) != null;
		}
		if (t != this.item || this.isThereAnyItemAvailable != flag2)
		{
			this.item = t;
			this.itemNavCost = maxValue;
			this.isThereAnyItemAvailable = flag2;
			this.itemInReachButNotPermitted = flag;
			this.ItemChanged();
		}
	}

	public Pickupable FindClosestPickupable(Storage destination, HashSet<Tag> exclude_tags, out int cost, Tag categoryTag, Tag[] otherRequiredTags = null)
	{
		ICollection<Pickupable> pickupables = base.gameObject.GetMyWorld().worldInventory.GetPickupables(categoryTag, false);
		if (pickupables == null)
		{
			cost = int.MaxValue;
			return null;
		}
		if (otherRequiredTags == null)
		{
			otherRequiredTags = new Tag[] { categoryTag };
		}
		Pickupable pickupable = null;
		int num = int.MaxValue;
		foreach (Pickupable pickupable2 in pickupables)
		{
			if (FetchManager.IsFetchablePickup_Exclude(pickupable2.KPrefabID, pickupable2.storage, pickupable2.UnreservedFetchAmount, exclude_tags, otherRequiredTags, destination))
			{
				int navigationCost = pickupable2.GetNavigationCost(this.navigator, pickupable2.cachedCell);
				if (navigationCost != -1 && navigationCost < num)
				{
					pickupable = pickupable2;
					num = navigationCost;
				}
			}
		}
		cost = num;
		return pickupable;
	}

	public virtual void ItemChanged()
	{
		Action<T> onItemChanged = this.OnItemChanged;
		if (onItemChanged == null)
		{
			return;
		}
		onItemChanged(this.item);
	}

	public Action<T> OnItemChanged;

	protected T item;

	protected int itemNavCost = int.MaxValue;

	protected Tag itemSearchTag;

	protected Tag[] requiredTags;

	protected bool isThereAnyItemAvailable;

	protected bool itemInReachButNotPermitted;

	private Navigator navigator;

	protected ConsumableConsumer consumableConsumer;

	private Storage storage;
}
