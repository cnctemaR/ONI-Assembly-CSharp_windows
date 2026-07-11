using System;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class Clearable : Workable, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.Subscribe(2127324410, new Action<object>(this.OnCancel));
		base.Subscribe(856640610, new Action<object>(this.OnStore));
		base.Subscribe(-2064133523, new Action<object>(this.OnAbsorb));
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(-1617557748, new Action<object>(this.OnEquipped));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Clearing;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable component = base.GetComponent<Prioritizable>();
		if (component != null && base.GetComponent<Health>() == null)
		{
			component.showIcon = false;
		}
		if (this.isMarkedForClear)
		{
			if (this.HasTag(GameTags.Stored))
			{
				this.isMarkedForClear = false;
			}
			else
			{
				this.MarkForClear(true);
			}
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
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingClear, false);
			this.isMarkedForClear = false;
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.Garbage);
			if (this.chore != null)
			{
				this.chore.Cancel("Clearing canceled");
				this.chore = null;
			}
			Prioritizable.RemoveRef(base.gameObject);
		}
	}

	public void MarkForClear(bool force = false)
	{
		if (!this.isClearable)
		{
			return;
		}
		if ((!this.isMarkedForClear || force) && !this.pickupable.IsEntombed && this.chore == null && !this.HasTag(GameTags.Stored))
		{
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.PendingClear, this);
			Prioritizable.AddRef(base.gameObject);
			this.chore = new ClearChore(Db.Get().ChoreTypes.Transport, base.GetComponent<Pickupable>(), null, true, null, null, null);
			base.GetComponent<KPrefabID>().AddTag(GameTags.Garbage);
			this.isMarkedForClear = true;
		}
	}

	private void OnClickClear()
	{
		this.MarkForClear(false);
	}

	private void OnClickCancel()
	{
		this.CancelClearing();
	}

	private void OnEquipped(object data)
	{
		this.CancelClearing();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.isClearable || base.GetComponent<Health>() != null || this.HasTag(GameTags.Stored))
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (this.isMarkedForClear)
		{
			string text = "action_move_to_storage";
			string text2 = UI.USERMENUACTIONS.CLEAR.NAME_OFF;
			global::System.Action action = new global::System.Action(this.OnClickCancel);
			string text3 = UI.USERMENUACTIONS.CLEAR.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_move_to_storage";
			string text2 = UI.USERMENUACTIONS.CLEAR.NAME;
			global::System.Action action = new global::System.Action(this.OnClickClear);
			string text = UI.USERMENUACTIONS.CLEAR.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			Clearable component = pickupable.GetComponent<Clearable>();
			if (component != null && component.isMarkedForClear)
			{
				this.MarkForClear(false);
			}
		}
	}

	[MyCmpReq]
	private Pickupable pickupable;

	private Chore chore;

	[Serialize]
	private bool isMarkedForClear;

	public bool isClearable = true;
}
