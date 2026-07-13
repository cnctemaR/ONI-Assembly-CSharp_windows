using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CircuitSwitch : Switch, IPlayerControlledToggle, ISim33ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<CircuitSwitch>(-905833192, CircuitSwitch.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.CircuitOnToggle;
		int num = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		Wire wire = ((gameObject != null) ? gameObject.GetComponent<Wire>() : null);
		if (wire == null)
		{
			this.wireConnectedGUID = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
		this.AttachWire(wire);
		this.wasOn = this.switchedOn;
		this.UpdateCircuit(true);
		base.GetComponent<KBatchedAnimController>().Play(this.switchedOn ? "on" : "off", KAnim.PlayMode.Once, 1f, 0f);
	}

	protected override void OnCleanUp()
	{
		if (this.attachedWire != null)
		{
			this.UnsubscribeFromWire(this.attachedWire);
		}
		bool switchedOn = this.switchedOn;
		this.switchedOn = true;
		this.UpdateCircuit(false);
		this.switchedOn = switchedOn;
	}

	private void OnCopySettings(object data)
	{
		CircuitSwitch component = ((GameObject)data).GetComponent<CircuitSwitch>();
		if (component != null)
		{
			this.switchedOn = component.switchedOn;
			this.UpdateCircuit(true);
		}
	}

	public bool IsConnected()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		return gameObject != null && gameObject.GetComponent<IDisconnectable>() != null;
	}

	private void CircuitOnToggle(bool on)
	{
		this.UpdateCircuit(true);
	}

	public void AttachWire(Wire wire)
	{
		if (wire == this.attachedWire)
		{
			return;
		}
		if (this.attachedWire != null)
		{
			this.UnsubscribeFromWire(this.attachedWire);
		}
		this.attachedWire = wire;
		if (this.attachedWire != null)
		{
			this.SubscribeToWire(this.attachedWire);
			this.UpdateCircuit(true);
			this.wireConnectedGUID = base.GetComponent<KSelectable>().RemoveStatusItem(this.wireConnectedGUID, false);
			return;
		}
		if (this.wireConnectedGUID == Guid.Empty)
		{
			this.wireConnectedGUID = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
	}

	private void OnWireDestroyed(object data)
	{
		if (this.attachedWire != null)
		{
			this.UnsubscribeFromWire(this.attachedWire);
		}
	}

	private void OnWireStateChanged(object data)
	{
		this.UpdateCircuit(true);
	}

	private void SubscribeToWire(Wire wire)
	{
		this.objectDestroyedHandle = wire.Subscribe(1969584890, new Action<object>(this.OnWireDestroyed));
		this.buildingFullyRepairedHandle = wire.Subscribe(-1735440190, new Action<object>(this.OnWireStateChanged));
		this.buildingBrokenHandle = wire.Subscribe(774203113, new Action<object>(this.OnWireStateChanged));
	}

	private void UnsubscribeFromWire(Wire wire)
	{
		wire.Unsubscribe(ref this.objectDestroyedHandle);
		wire.Unsubscribe(ref this.buildingFullyRepairedHandle);
		wire.Unsubscribe(ref this.buildingBrokenHandle);
	}

	private void UpdateCircuit(bool should_update_anim = true)
	{
		if (this.attachedWire != null)
		{
			if (this.switchedOn)
			{
				this.attachedWire.Connect();
			}
			else
			{
				this.attachedWire.Disconnect();
			}
		}
		if (should_update_anim && this.wasOn != this.switchedOn)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play(this.switchedOn ? "on_pre" : "on_pst", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue(this.switchedOn ? "on" : "off", KAnim.PlayMode.Once, 1f, 0f);
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
		this.wasOn = this.switchedOn;
	}

	public void Sim33ms(float dt)
	{
		if (this.ToggleRequested)
		{
			this.Toggle();
			this.ToggleRequested = false;
			this.GetSelectable().SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
		}
	}

	public void ToggledByPlayer()
	{
		this.Toggle();
	}

	public bool ToggledOn()
	{
		return this.switchedOn;
	}

	public KSelectable GetSelectable()
	{
		return base.GetComponent<KSelectable>();
	}

	public string SideScreenTitleKey
	{
		get
		{
			return "STRINGS.BUILDINGS.PREFABS.SWITCH.SIDESCREEN_TITLE";
		}
	}

	public bool ToggleRequested { get; set; }

	[SerializeField]
	public ObjectLayer objectLayer;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<CircuitSwitch> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CircuitSwitch>(delegate(CircuitSwitch component, object data)
	{
		component.OnCopySettings(data);
	});

	private Wire attachedWire;

	private Guid wireConnectedGUID;

	private bool wasOn;

	private int objectDestroyedHandle = -1;

	private int buildingFullyRepairedHandle = -1;

	private int buildingBrokenHandle = -1;
}
