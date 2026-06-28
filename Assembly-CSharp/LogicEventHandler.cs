using System;
using FMOD.Studio;
using UnityEngine;

internal class LogicEventHandler : ILogicEventReceiver, ILogicUIElement, ILogicNetworkConnection, IUniformGridObject
{
	public LogicEventHandler(int cell, Action<int> on_value_changed, Action<int, bool> on_connection_changed)
	{
		this.cell = cell;
		this.onValueChanged = on_value_changed;
		this.onConnectionChanged = on_connection_changed;
	}

	public void ReceiveLogicEvent(int value)
	{
		this.TriggerAudio(value);
		this.value = value;
		this.onValueChanged(value);
	}

	public int Value
	{
		get
		{
			return this.value;
		}
	}

	public int GetLogicUICell()
	{
		return this.cell;
	}

	public bool IsLogicInput()
	{
		return true;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public int GetLogicCell()
	{
		return this.cell;
	}

	private void TriggerAudio(int new_value)
	{
		LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(this.cell);
		SpeedControlScreen instance = SpeedControlScreen.Instance;
		if (networkForCell != null && new_value != this.value && instance != null && !instance.IsPaused)
		{
			EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Logic_Building_Toggle", false), Grid.CellToPos(this.cell));
			eventInstance.setParameterValue("wireCount", (float)(networkForCell.Wires.Count % 24));
			eventInstance.setParameterValue("enabled", (float)new_value);
			KFMOD.EndOneShot(eventInstance);
		}
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
		if (this.onConnectionChanged != null)
		{
			this.onConnectionChanged(this.cell, connected);
		}
	}

	private int cell;

	private int value;

	private Action<int> onValueChanged;

	private Action<int, bool> onConnectionChanged;
}
