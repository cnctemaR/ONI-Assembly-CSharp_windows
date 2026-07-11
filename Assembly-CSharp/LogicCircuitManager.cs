using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitManager
{
	public LogicCircuitManager(UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduit_system)
	{
		this.conduitSystem = conduit_system;
		this.elapsedTime = 0f;
	}

	public void Sim200ms(float dt)
	{
		this.Refresh(dt);
	}

	public void RenderEveryTick(float dt)
	{
		this.Refresh(dt);
	}

	private void Refresh(float dt)
	{
		bool isDirty = this.conduitSystem.IsDirty;
		if (isDirty)
		{
			this.conduitSystem.Update();
			this.PropagateSignals(true);
			this.elapsedTime = 0f;
		}
		else if (this.conduitSystem.GetNetworks().Count > 0 && SpeedControlScreen.Instance != null && !SpeedControlScreen.Instance.IsPaused)
		{
			this.elapsedTime += Time.deltaTime;
			while (this.elapsedTime > LogicCircuitManager.ClockTickInterval)
			{
				this.elapsedTime -= LogicCircuitManager.ClockTickInterval;
				this.PropagateSignals(false);
			}
		}
	}

	private void PropagateSignals(bool force_send_events)
	{
		IList<UtilityNetwork> networks = Game.Instance.logicCircuitSystem.GetNetworks();
		foreach (UtilityNetwork utilityNetwork in networks)
		{
			LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork)utilityNetwork;
			logicCircuitNetwork.UpdateLogicValue();
		}
		foreach (UtilityNetwork utilityNetwork2 in networks)
		{
			LogicCircuitNetwork logicCircuitNetwork2 = (LogicCircuitNetwork)utilityNetwork2;
			logicCircuitNetwork2.SendLogicEvents(force_send_events);
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

	public static float ClockTickInterval = 0.1f;

	private float elapsedTime;

	private UtilityNetworkManager<LogicCircuitNetwork, LogicWire> conduitSystem;

	private List<ILogicUIElement> uiVisElements = new List<ILogicUIElement>();

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
