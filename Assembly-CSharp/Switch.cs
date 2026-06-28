using System;
using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Switch : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	public bool IsSwitchedOn
	{
		get
		{
			return this.switchedOn;
		}
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<bool> OnToggle;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.switchedOn = this.defaultState;
	}

	protected override void OnSpawn()
	{
		this.openToggleIndex = this.openSwitch.SetTarget(this);
		if (this.OnToggle != null)
		{
			this.OnToggle(this.switchedOn);
		}
		if (this.manuallyControlled)
		{
			base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		}
		this.UpdateSwitchStatus();
	}

	public void HandleToggle()
	{
		this.Toggle();
	}

	public bool IsHandlerOn()
	{
		return this.switchedOn;
	}

	private void OnMinionToggle()
	{
		if (!DebugHandler.InstantBuildMode)
		{
			this.openSwitch.Toggle(this.openToggleIndex);
		}
		else
		{
			this.Toggle();
		}
	}

	protected virtual void Toggle()
	{
		this.SetState(!this.switchedOn);
	}

	protected virtual void SetState(bool on)
	{
		if (this.switchedOn != on)
		{
			this.switchedOn = on;
			this.UpdateSwitchStatus();
			if (this.OnToggle != null)
			{
				this.OnToggle(this.switchedOn);
			}
			if (this.manuallyControlled)
			{
				this.userMenu.Refresh();
			}
		}
	}

	protected virtual void OnRefreshUserMenu(object data)
	{
		LocString locString = ((!this.switchedOn) ? BUILDINGS.PREFABS.SWITCH.TURN_ON : BUILDINGS.PREFABS.SWITCH.TURN_OFF);
		this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_power", locString, new global::System.Action(this.OnMinionToggle), global::Action.ToggleEnabled, null, null, null, string.Empty, true), 1f);
	}

	protected void UpdateSwitchStatus()
	{
		StatusItem statusItem = ((!this.switchedOn) ? Db.Get().BuildingStatusItems.SwitchStatusInactive : Db.Get().BuildingStatusItems.SwitchStatusActive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	[SerializeField]
	public bool manuallyControlled = true;

	[SerializeField]
	public bool defaultState = true;

	[Serialize]
	protected bool switchedOn = true;

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpAdd]
	private Toggleable openSwitch;

	private int openToggleIndex;
}
