using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CircuitSwitch : Switch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OnWireStateChangedDelegate = new Action<object>(this.OnWireStateChanged);
		base.OnToggle += this.CircuitOnToggle;
		int num = Grid.PosToCell(base.transform.GetPosition());
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		Wire wire = ((!(gameObject != null)) ? null : gameObject.GetComponent<Wire>());
		if (wire == null)
		{
			this.wireConnectedGUID = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
		this.AttachWire(wire);
		this.wasOn = this.switchedOn;
		this.UpdateCircuit(true);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.Play((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
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
		}
		else if (this.wireConnectedGUID == Guid.Empty)
		{
			this.wireConnectedGUID = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
	}

	private void OnWireDestroyed(object data)
	{
		if (this.attachedWire != null)
		{
			this.attachedWire.Unsubscribe(1969584890, new Action<object>(this.OnWireDestroyed));
		}
	}

	private void OnWireStateChanged(object data)
	{
		this.UpdateCircuit(true);
	}

	private void SubscribeToWire(Wire wire)
	{
		wire.Subscribe(1969584890, new Action<object>(this.OnWireDestroyed));
		wire.Subscribe(-1735440190, this.OnWireStateChangedDelegate);
		wire.Subscribe(774203113, this.OnWireStateChangedDelegate);
	}

	private void UnsubscribeFromWire(Wire wire)
	{
		wire.Unsubscribe(1969584890, new Action<object>(this.OnWireDestroyed));
		wire.Unsubscribe(-1735440190, this.OnWireStateChangedDelegate);
		wire.Unsubscribe(774203113, this.OnWireStateChangedDelegate);
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
			component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
			Game.Instance.userMenu.Refresh(base.gameObject);
		}
		this.wasOn = this.switchedOn;
	}

	[SerializeField]
	public ObjectLayer objectLayer;

	private Wire attachedWire;

	private Guid wireConnectedGUID;

	private bool wasOn;

	private Action<object> OnWireStateChangedDelegate;
}
