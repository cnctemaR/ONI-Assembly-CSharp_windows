using System;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Clearable : Workable, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		this.Subscribe(2127324410, new EventSystem.EventHandler(this.OnCancel));
		this.Subscribe(856640610, new EventSystem.EventHandler(this.OnStore));
		this.Subscribe(-2064133523, new EventSystem.EventHandler(this.OnAbsorb));
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Clearing;
		Components.Clearables.Add(this);
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.isMarkedForClear)
		{
			this.MarkForClear(true);
		}
	}

	private void OnStore(object data)
	{
		this.CancelClearing();
	}

	private void OnCancel(object data)
	{
		for (ObjectLayerListItem objectLayerListItem = this.pickupable.objectLayerListItem; objectLayerListItem != null; objectLayerListItem = objectLayerListItem.nextItem)
		{
			if (objectLayerListItem.gameObject != null)
			{
				objectLayerListItem.gameObject.GetComponent<Clearable>().CancelClearing();
			}
		}
	}

	public void CancelClearing()
	{
		if (this.isMarkedForClear)
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingClear);
			this.isMarkedForClear = false;
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Garbage);
			if (this.chore != null)
			{
				this.chore.Cancel("Clearing canceled");
				this.chore = null;
			}
		}
	}

	public void MarkForClear(bool force = false)
	{
		if (!this.isClearable)
		{
			return;
		}
		if ((!this.isMarkedForClear || force) && !this.pickupable.IsEntombed && this.chore == null)
		{
			if (this.pickupable.storage == null)
			{
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.PendingClear, this);
			}
			this.chore = new ClearChore(Db.Get().ChoreTypes.Transport, base.GetComponent<Pickupable>(), null, true, null, null, null);
			base.GetComponent<KPrefabID>().AddTag(GameTags.Garbage);
		}
		this.isMarkedForClear = true;
	}

	private void OnClickClear()
	{
		this.MarkForClear(false);
	}

	private void OnClickCancel()
	{
		this.CancelClearing();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.isClearable || base.GetComponent<Health>() != null)
		{
			return;
		}
		if (!this.isMarkedForClear)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.CLEAR.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.CLEAR.NAME, new global::System.Action(this.OnClickClear), global::Action.NumActions, null, null, null, null, text));
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = UI.USERMENUACTIONS.CLEAR.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.CLEAR.NAME_OFF, new global::System.Action(this.OnClickCancel), global::Action.NumActions, null, null, null, null, text));
		}
	}

	private void OnAbsorb(object data)
	{
		GameObject gameObject = data as GameObject;
		if (gameObject != null)
		{
			Clearable component = gameObject.GetComponent<Clearable>();
			if (component != null && component.isMarkedForClear)
			{
				this.MarkForClear(false);
			}
		}
	}

	public bool IsMarkedForClear()
	{
		return this.isMarkedForClear;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Clearables.Remove(this);
	}

	public void SetIsClearable(bool is_clearable)
	{
		this.isClearable = is_clearable;
	}

	[MyCmpReq]
	private Pickupable pickupable;

	[MyCmpAdd]
	private UserMenu userMenu;

	private Chore chore;

	[Serialize]
	private bool isMarkedForClear;

	private bool isClearable = true;
}
