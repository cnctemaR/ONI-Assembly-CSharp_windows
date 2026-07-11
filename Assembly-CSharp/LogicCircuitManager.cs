using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitManager
{
	public LogicCircuitManager(UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduit_system)
	{
		this.conduitSystem = conduit_system;
		this.timeSinceBridgeRefresh = 0f;
		this.elapsedTime = 0f;
		for (int i = 0; i < 2; i++)
		{
			this.bridgeGroups[i] = new List<LogicUtilityNetworkLink>();
		}
	}

	public void RenderEveryTick(float dt)
	{
		this.Refresh(dt);
	}

	private void Refresh(float dt)
	{
		if (this.conduitSystem.IsDirty)
		{
			this.conduitSystem.Update();
			LogicCircuitNetwork.logicSoundRegister.Clear();
			this.PropagateSignals(true);
			this.elapsedTime = 0f;
		}
		else if (this.conduitSystem.GetNetworks().Count > 0 && SpeedControlScreen.Instance != null && !SpeedControlScreen.Instance.IsPaused)
		{
			this.elapsedTime += dt;
			this.timeSinceBridgeRefresh += dt;
			while (this.elapsedTime > LogicCircuitManager.ClockTickInterval)
			{
				this.elapsedTime -= LogicCircuitManager.ClockTickInterval;
				this.PropagateSignals(false);
			}
			if (this.timeSinceBridgeRefresh > LogicCircuitManager.BridgeRefreshInterval)
			{
				this.UpdateCircuitBridgeLists();
				this.timeSinceBridgeRefresh = 0f;
			}
		}
		foreach (UtilityNetwork utilityNetwork in Game.Instance.logicCircuitSystem.GetNetworks())
		{
			LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)utilityNetwork;
			this.CheckCircuitOverloaded(dt, logicCircuitNetwork.id, logicCircuitNetwork.GetBitsUsed());
		}
	}

	private void PropagateSignals(bool force_send_events)
	{
		IList<UtilityNetwork> networks = Game.Instance.logicCircuitSystem.GetNetworks();
		foreach (UtilityNetwork utilityNetwork in networks)
		{
			((LogicCircuitNetwork)utilityNetwork).UpdateLogicValue();
		}
		foreach (UtilityNetwork utilityNetwork2 in networks)
		{
			LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)utilityNetwork2;
			logicCircuitNetwork.SendLogicEvents(force_send_events, logicCircuitNetwork.id);
		}
	}

	public LogicCircuitNetwork GetNetworkForCell(int cell)
	{
		return this.conduitSystem.GetNetworkForCell(cell) as LogicCircuitNetwork;
	}

	public void AddVisElem(ILogicUIElement elem)
	{
		this.uiVisElements.Add(elem);
		if (this.onElemAdded != null)
		{
			this.onElemAdded(elem);
		}
	}

	public void RemoveVisElem(ILogicUIElement elem)
	{
		if (this.onElemRemoved != null)
		{
			this.onElemRemoved(elem);
		}
		this.uiVisElements.Remove(elem);
	}

	public ReadOnlyCollection<ILogicUIElement> GetVisElements()
	{
		return this.uiVisElements.AsReadOnly();
	}

	public static void ToggleNoWireConnected(bool show_missing_wire, GameObject go)
	{
		go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoLogicWireConnected, show_missing_wire, null);
	}

	private void CheckCircuitOverloaded(float dt, int id, int bits_used)
	{
		UtilityNetwork networkByID = Game.Instance.logicCircuitSystem.GetNetworkByID(id);
		if (networkByID != null)
		{
			LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)networkByID;
			if (logicCircuitNetwork != null)
			{
				logicCircuitNetwork.UpdateOverloadTime(dt, bits_used);
			}
		}
	}

	public void Connect(LogicUtilityNetworkLink bridge)
	{
		this.bridgeGroups[(int)bridge.bitDepth].Add(bridge);
	}

	public void Disconnect(LogicUtilityNetworkLink bridge)
	{
		this.bridgeGroups[(int)bridge.bitDepth].Remove(bridge);
	}

	private void UpdateCircuitBridgeLists()
	{
		foreach (UtilityNetwork utilityNetwork in Game.Instance.logicCircuitSystem.GetNetworks())
		{
			LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)utilityNetwork;
			if (this.updateEvenBridgeGroups)
			{
				if (logicCircuitNetwork.id % 2 == 0)
				{
					logicCircuitNetwork.UpdateRelevantBridges(this.bridgeGroups);
				}
			}
			else if (logicCircuitNetwork.id % 2 == 1)
			{
				logicCircuitNetwork.UpdateRelevantBridges(this.bridgeGroups);
			}
		}
		this.updateEvenBridgeGroups = !this.updateEvenBridgeGroups;
	}

	public static float ClockTickInterval = 0.1f;

	private float elapsedTime;

	private UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduitSystem;

	private List<ILogicUIElement> uiVisElements = new List<ILogicUIElement>();

	public static float BridgeRefreshInterval = 1f;

	private List<LogicUtilityNetworkLink>[] bridgeGroups = new List<LogicUtilityNetworkLink>[2];

	private bool updateEvenBridgeGroups;

	private float timeSinceBridgeRefresh;

	public Action<ILogicUIElement> onElemAdded;

	public Action<ILogicUIElement> onElemRemoved;

	private struct Signal
	{
		public Signal(int cell, int value)
		{
			this.cell = cell;
			this.value = value;
		}

		public int cell;

		public int value;
	}
}
