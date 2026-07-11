using System;
using UnityEngine;

internal class LogicEventSender : ILogicEventSender, ILogicUIElement, ILogicNetworkConnection, IUniformGridObject
{
	public LogicEventSender(HashedString id, int cell, Action<int> on_value_changed, Action<int, bool> on_connection_changed, LogicPortSpriteType sprite_type)
	{
		this.id = id;
		this.cell = cell;
		this.onValueChanged = on_value_changed;
		this.onConnectionChanged = on_connection_changed;
		this.spriteType = sprite_type;
	}

	public HashedString ID
	{
		get
		{
			return this.id;
		}
	}

	public int GetLogicCell()
	{
		return this.cell;
	}

	public int GetLogicValue()
	{
		return this.logicValue;
	}

	public int GetLogicUICell()
	{
		return this.GetLogicCell();
	}

	public LogicPortSpriteType GetLogicPortSpriteType()
	{
		return this.spriteType;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public void SetValue(int value)
	{
		this.logicValue = value;
		this.onValueChanged(value);
	}

	public void LogicTick()
	{
	}

	public void OnLogicNetworkConnectionChanged(bool connected)
	{
		if (this.onConnectionChanged != null)
		{
			this.onConnectionChanged(this.cell, connected);
		}
	}

	private HashedString id;

	private int cell;

	private int logicValue;

	private Action<int> onValueChanged;

	private Action<int, bool> onConnectionChanged;

	private LogicPortSpriteType spriteType;
}
