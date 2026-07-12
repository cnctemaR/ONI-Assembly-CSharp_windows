using System;
using KSerialization;
using STRINGS;

public class ConnectionManager : KMonoBehaviour, ISaveLoadable, IToggleHandler
{
	public bool IsConnected
	{
		get
		{
			return this.connected;
		}
		set
		{
			this.connected = value;
			if (this.connectedMeter != null)
			{
				this.connectedMeter.SetPositionPercent(value ? 1f : 0f);
			}
		}
	}

	public bool WaitingForToggle
	{
		get
		{
			return this.toggleQueued;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.toggleIdx = this.toggleable.SetTarget(this);
		base.Subscribe<ConnectionManager>(493375141, ConnectionManager.OnRefreshUserMenuDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.toggleQueued)
		{
			this.OnMenuToggle();
		}
		this.connectedMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_connected_target", "meter_connected", Meter.Offset.NoChange, Grid.SceneLayer.NoLayer, GeothermalVentConfig.CONNECTED_SYMBOLS);
		this.connectedMeter.SetPositionPercent(this.IsConnected ? 1f : 0f);
	}

	public void HandleToggle()
	{
		this.toggleQueued = false;
		Prioritizable.RemoveRef(base.gameObject);
		this.OnToggle();
	}

	private void OnToggle()
	{
		this.IsConnected = !this.IsConnected;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnMenuToggle()
	{
		if (!this.toggleable.IsToggleQueued(this.toggleIdx))
		{
			if (this.IsConnected)
			{
				base.Trigger(2108245096, "BuildingDisabled");
			}
			this.toggleQueued = true;
			Prioritizable.AddRef(base.gameObject);
		}
		else
		{
			this.toggleQueued = false;
			Prioritizable.RemoveRef(base.gameObject);
		}
		this.toggleable.Toggle(this.toggleIdx);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.showButton)
		{
			return;
		}
		bool isConnected = this.IsConnected;
		bool flag = this.toggleable.IsToggleQueued(this.toggleIdx);
		KIconButtonMenu.ButtonInfo buttonInfo;
		if ((isConnected && !flag) || (!isConnected && flag))
		{
			buttonInfo = new KIconButtonMenu.ButtonInfo("action_building_disabled", COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.BUTTONS.DISCONNECT_TITLE, new global::System.Action(this.OnMenuToggle), global::Action.ToggleEnabled, null, null, null, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.BUTTONS.DISCONNECT_TOOLTIP, true);
		}
		else
		{
			buttonInfo = new KIconButtonMenu.ButtonInfo("action_building_disabled", COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.BUTTONS.RECONNECT_TITLE, new global::System.Action(this.OnMenuToggle), global::Action.ToggleEnabled, null, null, null, COLONY_ACHIEVEMENTS.ACTIVATEGEOTHERMALPLANT.BUTTONS.RECONNECT_TOOLTIP, true);
		}
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	bool IToggleHandler.IsHandlerOn()
	{
		return this.IsConnected;
	}

	[MyCmpAdd]
	private ToggleGeothermalVentConnection toggleable;

	[MyCmpGet]
	private GeothermalVent vent;

	private int toggleIdx;

	private MeterController connectedMeter;

	public bool showButton;

	[Serialize]
	private bool connected;

	[Serialize]
	private bool toggleQueued;

	private static readonly EventSystem.IntraObjectHandler<ConnectionManager> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ConnectionManager>(delegate(ConnectionManager component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
