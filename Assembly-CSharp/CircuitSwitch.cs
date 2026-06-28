using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class CircuitSwitch : Switch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.CircuitOnToggle;
		int num = Grid.PosToCell(base.transform.position);
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		Wire wire = ((!(gameObject != null)) ? null : gameObject.GetComponent<Wire>());
		if (wire == null)
		{
			this.wireConnectedGUID = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
		}
		this.AttachWire(wire);
		this.wasOn = this.switchedOn;
		this.UpdateCircuit();
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
		this.UpdateCircuit();
		this.switchedOn = switchedOn;
	}

	public bool IsConnected()
	{
		int num = Grid.PosToCell(base.transform.position);
		GameObject gameObject = Grid.Objects[num, (int)this.objectLayer];
		return gameObject != null && gameObject.GetComponent<IDisconnectable>() != null;
	}

	private void CircuitOnToggle(bool on)
	{
		this.UpdateCircuit();
	}

	public void AttachWire(Wire wire)
	{
		if (!(wire == this.attachedWire))
		{
			if (this.attachedWire != null)
			{
				this.UnsubscribeFromWire(this.attachedWire);
			}
			this.attachedWire = wire;
			if (this.attachedWire != null)
			{
				this.SubscribeToWire(this.attachedWire);
				this.UpdateCircuit();
				this.wireConnectedGUID = base.GetComponent<KSelectable>().RemoveStatusItem(this.wireConnectedGUID, false);
			}
			else if (this.wireConnectedGUID == Guid.Empty)
			{
				this.wireConnectedGUID = base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
			}
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
		this.UpdateCircuit();
	}

	private void SubscribeToWire(Wire wire)
	{
		wire.Subscribe(1969584890, new Action<object>(this.OnWireDestroyed));
		wire.Subscribe(-1735440190, new Action<object>(this.OnWireStateChanged));
		wire.Subscribe(774203113, new Action<object>(this.OnWireStateChanged));
	}

	private void UnsubscribeFromWire(Wire wire)
	{
		wire.Unsubscribe(1969584890, new Action<object>(this.OnWireDestroyed));
		wire.Unsubscribe(-1735440190, new Action<object>(this.OnWireStateChanged));
		wire.Unsubscribe(774203113, new Action<object>(this.OnWireStateChanged));
	}

	private void UpdateCircuit()
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
		if (this.wasOn != this.switchedOn)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.Play((!this.switchedOn) ? "on_pst" : "on_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue((!this.switchedOn) ? "off" : "on", KAnim.PlayMode.Once, 1f, 0f);
			this.userMenu.Refresh();
		}
		this.wasOn = this.switchedOn;
	}

	[SerializeField]
	public ObjectLayer objectLayer;

	private Wire attachedWire = null;

	private Guid wireConnectedGUID;

	private bool wasOn;
}
