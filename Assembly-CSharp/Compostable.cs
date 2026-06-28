using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Compostable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.isMarkedForCompost)
		{
			this.MarkForCompost(true);
		}
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.Subscribe(856640610, new Action<object>(this.OnStore));
	}

	private void MarkForCompost(bool force = false)
	{
		if (!this.isMarkedForCompost || force)
		{
			this.RefreshStatusItem();
			base.GetComponent<KPrefabID>().AddTag(GameTags.ToxicSand);
			base.GetComponent<KPrefabID>().AddTag(GameTags.MarkedForCompost);
			this.isMarkedForCompost = true;
			Storage storage = base.GetComponent<Pickupable>().storage;
			if (storage != null)
			{
				storage.Drop(base.gameObject);
			}
		}
	}

	private void CancelCompost()
	{
		if (this.isMarkedForCompost)
		{
			this.RefreshStatusItem();
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.ToxicSand);
			base.GetComponent<KPrefabID>().RemoveTag(GameTags.MarkedForCompost);
			this.isMarkedForCompost = false;
		}
	}

	private void OnToggleCompost()
	{
		if (!this.isMarkedForCompost)
		{
			Pickupable component = base.GetComponent<Pickupable>();
			if (component.storage != null)
			{
				component.storage.Drop(base.gameObject);
			}
			Pickupable pickupable = EntitySplitter.Split(component, component.TotalAmount, this.compostPrefab);
			if (pickupable != null)
			{
				pickupable.GetComponent<Compostable>().MarkForCompost(false);
				UIScheduler.Instance.Schedule("SelectCompostObject", 0f, new Action<object>(this.SelectCompostObject), pickupable, null);
			}
		}
		else
		{
			Pickupable component2 = base.GetComponent<Pickupable>();
			Pickupable pickupable2 = EntitySplitter.Split(component2, component2.TotalAmount, this.originalPrefab);
			UIScheduler.Instance.Schedule("SelectCompostObject", 0f, new Action<object>(this.SelectCompostObject), pickupable2, null);
		}
	}

	private void RefreshStatusItem()
	{
		KSelectable component = base.GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompost, false);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage, false);
		if (this.isMarkedForCompost)
		{
			if (base.GetComponent<Pickupable>() != null && base.GetComponent<Pickupable>().storage == null)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompost, null);
			}
			else
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage, null);
			}
		}
	}

	private void SelectCompostObject(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (pickupable != null)
		{
			SelectTool.Instance.Select(pickupable.GetComponent<KSelectable>(), true);
		}
	}

	private void OnStore(object data)
	{
		this.RefreshStatusItem();
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.isMarkedForCompost)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.COMPOST.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.COMPOST.NAME, new global::System.Action(this.OnToggleCompost), global::Action.NumActions, null, null, null, text, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = UI.USERMENUACTIONS.COMPOST.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.COMPOST.NAME_OFF, new global::System.Action(this.OnToggleCompost), global::Action.NumActions, null, null, null, text, true), 1f);
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	[Serialize]
	private bool isMarkedForCompost;

	public GameObject originalPrefab;

	public GameObject compostPrefab;
}
