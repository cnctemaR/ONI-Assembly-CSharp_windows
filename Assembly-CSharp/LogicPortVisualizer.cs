using System;
using UnityEngine;

public class LogicPortVisualizer : ILogicUIElement, IUniformGridObject
{
	public LogicPortVisualizer(bool input, int cell)
	{
		this.input = input;
		this.cell = cell;
	}

	public int GetLogicUICell()
	{
		return this.cell;
	}

	public bool IsLogicInput()
	{
		return this.input;
	}

	public Vector2 PosMin()
	{
		return Grid.CellToPos2D(this.cell);
	}

	public Vector2 PosMax()
	{
		return Grid.CellToPos2D(this.cell);
	}

	private bool input;

	private int cell;
}
