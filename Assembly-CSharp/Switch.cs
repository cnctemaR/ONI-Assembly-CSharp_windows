using System;
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
			base.Subscribe<Switch>(493375141, Switch.OnRefreshUserMenuDelegate);
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
			return;
		}
		this.Toggle();
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
				Game.Instance.userMenu.Refresh(base.gameObject);
			}
		}
	}

	protected virtual void OnRefreshUserMenu(object data)
	{
		LocString locString = (this.switchedOn ? BUILDINGS.PREFABS.SWITCH.TURN_OFF : BUILDINGS.PREFABS.SWITCH.TURN_ON);
		LocString locString2 = (this.switchedOn ? BUILDINGS.PREFABS.SWITCH.TURN_OFF_TOOLTIP : BUILDINGS.PREFABS.SWITCH.TURN_ON_TOOLTIP);
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_power", locString, new global::System.Action(this.OnMinionToggle), global::Action.ToggleEnabled, null, null, null, locString2, true), 1f);
	}

	protected virtual void UpdateSwitchStatus()
	{
		StatusItem statusItem = (this.switchedOn ? Db.Get().BuildingStatusItems.SwitchStatusActive : Db.Get().BuildingStatusItems.SwitchStatusInactive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	[SerializeField]
	public bool manuallyControlled = true;

	[SerializeField]
	public bool defaultState = true;

	[Serialize]
	protected bool switchedOn = true;

	[MyCmpAdd]
	private Toggleable openSwitch;

	private int openToggleIndex;

	private static readonly EventSystem.IntraObjectHandler<Switch> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Switch>(delegate(Switch component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
