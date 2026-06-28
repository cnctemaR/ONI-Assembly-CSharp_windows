using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Switch : KMonoBehaviour, ISaveLoadable, IToggleHandler, IEffectDescriptor
{
	public event Action<bool> OnToggle;

	public bool IsSwitchedOn
	{
		get
		{
			return this.switchedOn;
		}
	}

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
			this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
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
		this.switchedOn = !this.switchedOn;
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

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESMANUALOPERATION, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESMANUALOPERATION, Descriptor.DescriptorType.Requirement);
		list.Add(descriptor);
		return list;
	}

	[SerializeField]
	public bool manuallyControlled = true;

	[SerializeField]
	public bool defaultState = true;

	[SerializeField]
	public ObjectLayer objectLayer;

	[Serialize]
	protected bool switchedOn = true;

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpAdd]
	private Toggleable openSwitch;

	private int openToggleIndex;
}
