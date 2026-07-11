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
			return;
		}
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = (ILogicEventReceiver)item;
			this.receivers.Add(logicEventReceiver);
			return;
		}
		if (item is ILogicEventSender)
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
			return;
		}
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			this.receivers.Remove(logicEventReceiver);
			return;
		}
		if (item is ILogicEventSender)
		{
			ILogicEventSender logicEventSender = (ILogicEventSender)item;
			this.senders.Remove(logicEventSender);
		}
	}

	public override void ConnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			((ILogicEventReceiver)item).OnLogicNetworkConnectionChanged(true);
			return;
		}
		if (item is ILogicEventSender)
		{
			((ILogicEventSender)item).OnLogicNetworkConnectionChanged(true);
		}
	}

	public override void DisconnectItem(int cell, object item)
	{
		if (item is ILogicEventReceiver)
		{
			ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
			logicEventReceiver.ReceiveLogicEvent(0);
			logicEventReceiver.OnLogicNetworkConnectionChanged(false);
			return;
		}
		if (item is ILogicEventSender)
		{
			(item as ILogicEventSender).OnLogicNetworkConnectionChanged(false);
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
				int num = Grid.PosToCell(logicWire.transform.GetPosition());
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
		if (this.resetting)
		{
			return;
		}
		this.previousValue = this.outputValue;
		this.outputValue = 0;
		foreach (ILogicEventSender logicEventSender in this.senders)
		{
			logicEventSender.LogicTick();
		}
		foreach (ILogicEventSender logicEventSender2 in this.senders)
		{
			int logicValue = logicEventSender2.GetLogicValue();
			this.outputValue |= logicValue;
		}
	}

	public void SendLogicEvents(bool force_send, int id)
	{
		if (this.resetting)
		{
			return;
		}
		if (this.outputValue != this.previousValue || force_send)
		{
			foreach (ILogicEventReceiver logicEventReceiver in this.receivers)
			{
				logicEventReceiver.ReceiveLogicEvent(this.outputValue);
			}
			if (!force_send)
			{
				this.TriggerAudio((this.previousValue >= 0) ? this.previousValue : 0, id);
			}
		}
	}

	private void TriggerAudio(int old_value, int id)
	{
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (old_value != this.outputValue && instance != null && !instance.IsPaused)
		{
			GridArea visibleArea = GridVisibleArea.GetVisibleArea();
			List<LogicWire> list = new List<LogicWire>();
			for (int i = 0; i < this.wires.Count; i++)
			{
				if (visibleArea.Min <= this.wires[i].transform.GetPosition() && this.wires[i].transform.GetPosition() <= visibleArea.Max)
				{
					list.Add(this.wires[i]);
				}
			}
			if (list.Count > 0)
			{
				int num = Mathf.CeilToInt((float)(list.Count / 2));
				if (list[num] != null)
				{
					Vector3 position = list[num].transform.GetPosition();
					position.z = 0f;
					string text = "Logic_Circuit_Toggle";
					LogicCircuitNetwork.LogicSoundPair logicSoundPair = new LogicCircuitNetwork.LogicSoundPair();
					if (!LogicCircuitNetwork.logicSoundRegister.ContainsKey(id))
					{
						LogicCircuitNetwork.logicSoundRegister.Add(id, logicSoundPair);
					}
					else
					{
						logicSoundPair.playedIndex = LogicCircuitNetwork.logicSoundRegister[id].playedIndex;
						logicSoundPair.lastPlayed = LogicCircuitNetwork.logicSoundRegister[id].lastPlayed;
					}
					if (logicSoundPair.playedIndex < 2)
					{
						LogicCircuitNetwork.logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
					}
					else
					{
						LogicCircuitNetwork.logicSoundRegister[id].playedIndex = 0;
						LogicCircuitNetwork.logicSoundRegister[id].lastPlayed = Time.time;
					}
					float num2 = (Time.time - logicSoundPair.lastPlayed) / 3f;
					EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound(text, false), position, 1f);
					eventInstance.setParameterValue("logic_volumeModifer", num2);
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

	private bool resetting;

	public static float logicSoundLastPlayedTime = 0f;

	public static Dictionary<int, LogicCircuitNetwork.LogicSoundPair> logicSoundRegister = new Dictionary<int, LogicCircuitNetwork.LogicSoundPair>();

	public class LogicSoundPair
	{
		public int playedIndex;

		public float lastPlayed;
	}
}
