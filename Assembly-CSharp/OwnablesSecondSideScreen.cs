using System;
using System.Collections.Generic;
using UnityEngine;

public class OwnablesSecondSideScreen : KScreen
{
	public AssignableSlotInstance Slot { get; private set; }

	public IAssignableIdentity OwnerIdentity { get; private set; }

	public AssignableSlot SlotType
	{
		get
		{
			if (this.Slot != null)
			{
				return this.Slot.slot;
			}
			return null;
		}
	}

	public Assignable CurrentSlotItem
	{
		get
		{
			if (!this.HasItem)
			{
				return null;
			}
			return this.Slot.assignable;
		}
	}

	public bool HasItem
	{
		get
		{
			return this.Slot != null && this.Slot.IsAssigned();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.originalRow.gameObject.SetActive(false);
		MultiToggle multiToggle = this.noneRow;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnNoneRowClicked));
	}

	private void OnNoneRowClicked()
	{
		this.UnassignCurrentItem();
		this.RefreshNoneRow();
	}

	protected override void OnCmpDisable()
	{
		this.SetSlot(null);
		base.OnCmpDisable();
	}

	public void SetSlot(AssignableSlotInstance slot)
	{
		Components.AssignableItems.Unregister(new Action<Assignable>(this.OnNewItemAvailable), new Action<Assignable>(this.OnItemUnregistered));
		this.Slot = slot;
		this.OwnerIdentity = ((slot == null) ? null : slot.assignables.GetComponent<IAssignableIdentity>());
		if (this.Slot != null)
		{
			Components.AssignableItems.Register(new Action<Assignable>(this.OnNewItemAvailable), new Action<Assignable>(this.OnItemUnregistered));
		}
		this.RefreshItemListOptions();
	}

	public void SortRows()
	{
		if (this.itemRows != null)
		{
			OwnablesSecondSideScreenRow ownablesSecondSideScreenRow = null;
			for (int i = 0; i < this.itemRows.Count; i++)
			{
				OwnablesSecondSideScreenRow ownablesSecondSideScreenRow2 = this.itemRows[i];
				if (ownablesSecondSideScreenRow2.item == null || ownablesSecondSideScreenRow2.item.IsAssigned())
				{
					if (ownablesSecondSideScreenRow == null && ownablesSecondSideScreenRow2 != null && ownablesSecondSideScreenRow2.item != null && ownablesSecondSideScreenRow2.item.IsAssigned() && ownablesSecondSideScreenRow2.item == this.CurrentSlotItem)
					{
						ownablesSecondSideScreenRow = ownablesSecondSideScreenRow2;
					}
					else
					{
						ownablesSecondSideScreenRow2.transform.SetAsLastSibling();
					}
				}
			}
			if (ownablesSecondSideScreenRow != null)
			{
				ownablesSecondSideScreenRow.transform.SetAsFirstSibling();
			}
		}
		this.noneRow.transform.SetAsFirstSibling();
	}

	public void RefreshItemListOptions()
	{
		GameObject gameObject = ((this.OwnerIdentity == null) ? null : this.OwnerIdentity.GetOwners()[0].GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
		int worldID = ((this.OwnerIdentity == null) ? 255 : gameObject.GetMyWorldId());
		List<Assignable> list = null;
		int num = 0;
		if (worldID != 255)
		{
			list = Components.AssignableItems.Items.FindAll(delegate(Assignable i)
			{
				bool flag = i.slotID == this.SlotType.Id && i.CanAssignTo(this.OwnerIdentity);
				if (flag && i is Equippable)
				{
					Equippable equippable = i as Equippable;
					GameObject gameObject2 = equippable.gameObject;
					if (equippable.isEquipped)
					{
						gameObject2 = equippable.assignee.GetOwners()[0].GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
					}
					flag = gameObject2.GetMyWorldId() == worldID;
				}
				return flag;
			});
			num = list.Count;
		}
		for (int j = 0; j < Mathf.Max(this.itemRows.Count, num); j++)
		{
			if (list != null && j < list.Count)
			{
				Assignable assignable = list[j];
				if (j >= this.itemRows.Count)
				{
					OwnablesSecondSideScreenRow ownablesSecondSideScreenRow = this.CreateItemRow(assignable);
					this.itemRows.Add(ownablesSecondSideScreenRow);
				}
				OwnablesSecondSideScreenRow ownablesSecondSideScreenRow2 = this.itemRows[j];
				ownablesSecondSideScreenRow2.gameObject.SetActive(true);
				ownablesSecondSideScreenRow2.SetData(this.Slot, assignable);
			}
			else
			{
				OwnablesSecondSideScreenRow ownablesSecondSideScreenRow3 = this.itemRows[j];
				ownablesSecondSideScreenRow3.ClearData();
				ownablesSecondSideScreenRow3.gameObject.SetActive(false);
			}
		}
		this.SortRows();
		this.RefreshNoneRow();
	}

	private void RefreshNoneRow()
	{
		this.noneRow.ChangeState(this.HasItem ? 0 : 1);
	}

	private OwnablesSecondSideScreenRow CreateItemRow(Assignable item)
	{
		OwnablesSecondSideScreenRow component = Util.KInstantiateUI(this.originalRow.gameObject, this.originalRow.transform.parent.gameObject, false).GetComponent<OwnablesSecondSideScreenRow>();
		component.OnRowClicked = (Action<OwnablesSecondSideScreenRow>)Delegate.Combine(component.OnRowClicked, new Action<OwnablesSecondSideScreenRow>(this.OnItemRowClicked));
		component.OnRowItemAssigneeChanged = (Action<OwnablesSecondSideScreenRow>)Delegate.Combine(component.OnRowItemAssigneeChanged, new Action<OwnablesSecondSideScreenRow>(this.OnItemRowAsigneeChanged));
		component.OnRowItemDestroyed = (Action<OwnablesSecondSideScreenRow>)Delegate.Combine(component.OnRowItemDestroyed, new Action<OwnablesSecondSideScreenRow>(this.OnItemDestroyed));
		return component;
	}

	private void OnItemDestroyed(OwnablesSecondSideScreenRow correspondingItemRow)
	{
		correspondingItemRow.ClearData();
		correspondingItemRow.gameObject.SetActive(false);
	}

	private void OnItemRowAsigneeChanged(OwnablesSecondSideScreenRow correspondingItemRow)
	{
		correspondingItemRow.Refresh();
		this.SortRows();
		this.RefreshNoneRow();
	}

	private void OnItemRowClicked(OwnablesSecondSideScreenRow rowClicked)
	{
		Assignable item = rowClicked.item;
		bool flag = item.IsAssigned() && item.assignee is AssignmentGroup;
		bool flag2 = item.IsAssigned() && item.IsAssignedTo(this.OwnerIdentity) && !flag && this.Slot.IsAssigned() && this.Slot.assignable == item;
		if (item.IsAssigned())
		{
			item.Unassign();
		}
		if (!flag2)
		{
			item.Assign(this.OwnerIdentity, this.Slot);
		}
		rowClicked.Refresh();
		this.RefreshNoneRow();
	}

	private void UnassignCurrentItem()
	{
		if (this.Slot != null)
		{
			this.Slot.Unassign(true);
			this.RefreshItemListOptions();
		}
	}

	private void OnNewItemAvailable(Assignable item)
	{
		if (this.Slot != null && item.slotID == this.SlotType.Id)
		{
			this.RefreshItemListOptions();
		}
	}

	private void OnItemUnregistered(Assignable item)
	{
		if (this.Slot != null && item.slotID == this.SlotType.Id)
		{
			this.RefreshItemListOptions();
		}
	}

	public MultiToggle noneRow;

	public OwnablesSecondSideScreenRow originalRow;

	public global::System.Action OnScreenDeactivated;

	private List<OwnablesSecondSideScreenRow> itemRows = new List<OwnablesSecondSideScreenRow>();
}
