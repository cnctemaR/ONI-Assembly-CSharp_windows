using System;
using UnityEngine;

public class LogicPortVisualizer : ILogicUIElement, IUniformGridObject
{
	public LogicPortVisualizer(int cell, LogicPortSpriteType sprite_type)
	{
		this.cell = cell;
		this.spriteType = sprite_type;
	}

	public int GetLogicUICell()
	{
		return this.cell;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public LogicPortSpriteType GetLogicPortSpriteType()
	{
		return this.spriteType;
	}

	private int cell;

	private LogicPortSpriteType spriteType;
}
