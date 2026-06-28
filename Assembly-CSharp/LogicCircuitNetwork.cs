using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FMOD.Studio;
using UnityEngine;

public class LogicCircuitNetwork : UtilityNetwork
{
	public override void AddItem(int cell, object item)
	{
		if (item is LogicWire)
		{
			this.wires.Add((LogicWire)item);
		}
		else if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = (ILogicEventReceiver)item;
			this.receivers.Add(logicEventReceiver);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			this.senders.Add(logicEventSender);
		}
	}

	public override void RemoveItem(int cell, object item)
	{
		if (item is LogicWire)
		{
			this.wires.Remove((LogicWire)item);
		}
		else if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			this.receivers.Remove(logicEventReceiver);
			logicEventReceiver.ReceiveLogicEvent(0);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			this.senders.Remove(logicEventSender);
		}
	}

	public override void ConnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = (ILogicEventReceiver)item;
			logicEventReceiver.OnLogicNetworkConnectionChanged(true);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			logicEventSender.OnLogicNetworkConnectionChanged(true);
		}
	}

	public override void DisconnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			logicEventReceiver.ReceiveLogicEvent(0);
			logicEventReceiver.OnLogicNetworkConnectionChanged(false);
		}
		else if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = item as ILogicEventSender;
			logicEventSender.OnLogicNetworkConnectionChanged(false);
		}
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		this.resetting = true;
		this.previousValue = -1;
		this.outputValue = 0;
		for (int i = 0; i < this.wires.Count; i++)
		{
			LogicWire logicWire = this.wires[i];
			if (logicWire != null)
			{
				int num = Grid.PosToCell(logicWire.transform.position);
				UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
				utilityNetworkGridNode.networkIdx = -1;
				grid[num] = utilityNetworkGridNode;
			}
		}
		this.wires.Clear();
		this.senders.Clear();
		this.receivers.Clear();
		this.resetting = false;
	}

	public void UpdateLogicValue()
	{
		if (!this.resetting)
		{
			this.previousValue = this.outputValue;
			this.outputValue = 0;
			foreach (ILogicEventSender logicEventSender in this.senders)
			{
				int logicValue = logicEventSender.GetLogicValue();
				this.outputValue |= logicValue;
			}
		}
	}

	public void SendLogicEvents(bool force_send)
	{
		if (!this.resetting)
		{
			if (this.outputValue != this.previousValue || force_send)
			{
				foreach (ILogicEventReceiver logicEventReceiver in this.receivers)
				{
					logicEventReceiver.ReceiveLogicEvent(this.outputValue);
				}
				if (!force_send)
				{
					this.TriggerAudio((this.previousValue < 0) ? 0 : this.previousValue);
				}
			}
		}
	}

	private void TriggerAudio(int old_value)
	{
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (old_value != this.outputValue && instance != null && !instance.IsPaused)
		{
			GridArea visibleArea = GridVisibleArea.GetVisibleArea();
			List<LogicWire> list = new List<LogicWire>();
			for (int i = 0; i < this.wires.Count; i++)
			{
				if (visibleArea.Min <= this.wires[i].transform.position && this.wires[i].transform.position <= visibleArea.Max)
				{
					list.Add(this.wires[i]);
				}
			}
			if (list.Count > 0)
			{
				int num = Mathf.CeilToInt((float)(list.Count / 2));
				if (list[num] != null)
				{
					Vector3 position = list[num].transform.position;
					EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Logic_Circuit_Toggle", false), position);
					eventInstance.setParameterValue("wireCount", (float)(this.wires.Count % 24));
					eventInstance.setParameterValue("enabled", (float)this.outputValue);
					KFMOD.EndOneShot(eventInstance);
				}
			}
		}
	}

	public int OutputValue
	{
		get
		{
			return this.outputValue;
		}
	}

	public List<LogicWire> Wires
	{
		get
		{
			return this.wires;
		}
	}

	public ReadOnlyCollection<ILogicEventSender> Senders
	{
		get
		{
			return this.senders.AsReadOnly();
		}
	}

	public ReadOnlyCollection<ILogicEventReceiver> Receivers
	{
		get
		{
			return this.receivers.AsReadOnly();
		}
	}

	private List<LogicWire> wires = new List<LogicWire>();

	private List<ILogicEventReceiver> receivers = new List<ILogicEventReceiver>();

	private List<ILogicEventSender> senders = new List<ILogicEventSender>();

	private int previousValue = -1;

	private int outputValue;

	private bool resetting = false;
}
