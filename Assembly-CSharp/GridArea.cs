using System;
using UnityEngine;

public struct GridArea
{
	public Vector2I Min
	{
		get
		{
			return this.min;
		}
	}

	public Vector2I Max
	{
		get
		{
			return this.max;
		}
	}

	public void SetArea(int cell, int width, int height)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = new Vector2I(vector2I.x + width, vector2I.y + height);
		this.SetExtents(vector2I.x, vector2I.y, vector2I2.x, vector2I2.y);
	}

	public void SetExtents(int min_x, int min_y, int max_x, int max_y)
	{
		this.min.x = Math.Max(min_x, 0);
		this.min.y = Math.Max(min_y, 0);
		this.max.x = Math.Min(max_x, Grid.WidthInCells);
		this.max.y = Math.Min(max_y, Grid.HeightInCells);
		this.MinCell = Grid.XYToCell(this.min.x, this.min.y);
		this.MaxCell = Grid.XYToCell(this.max.x, this.max.y);
	}

	public bool Contains(int cell)
	{
		bool flag;
		if (cell >= this.MinCell && cell < this.MaxCell)
		{
			int num = cell % Grid.WidthInCells;
			flag = num >= this.Min.x && num < this.Max.x;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	public bool Contains(int x, int y)
	{
		return x >= this.min.x && x < this.max.x && y >= this.min.y && y < this.max.y;
	}

	public bool Contains(Vector3 pos)
	{
		return (float)this.min.x <= pos.x && pos.x < (float)this.max.x && (float)this.min.y <= pos.y && pos.y <= (float)this.max.y;
	}

	public void RunIfInside(int cell, Action<int> action)
	{
		if (this.Contains(cell))
		{
			action(cell);
		}
	}

	public void Run(Action<int> action)
	{
		for (int i = this.min.y; i < this.max.y; i++)
		{
			for (int j = this.min.x; j < this.max.x; j++)
			{
				int num = Grid.XYToCell(j, i);
				action(num);
			}
		}
	}

	public void RunOnDifference(GridArea subtract_area, Action<int> action)
	{
		for (int i = this.min.y; i < this.max.y; i++)
		{
			for (int j = this.min.x; j < this.max.x; j++)
			{
				if (!subtract_area.Contains(j, i))
				{
					int num = Grid.XYToCell(j, i);
					action(num);
				}
			}
		}
	}

	public int GetCellCount()
	{
		return (this.max.x - this.min.x) * (this.max.y - this.min.y);
	}

	private Vector2I min;

	private Vector2I max;

	private int MinCell;

	private int MaxCell;
}
