using System;
using UnityEngine;

public class OffsetTracker
{
	public virtual CellOffset[] GetOffsets(int current_cell)
	{
		if (current_cell != this.previousCell)
		{
			this.UpdateCell(this.previousCell, current_cell);
			this.previousCell = current_cell;
		}
		if (this.offsets == null)
		{
			this.UpdateOffsets(this.previousCell);
		}
		return this.offsets;
	}

	public void ForceRefresh()
	{
		int num = this.previousCell;
		this.previousCell = Grid.InvalidCell;
		this.Refresh(num);
	}

	public void Refresh(int cell)
	{
		this.GetOffsets(cell);
	}

	protected virtual void UpdateCell(int previous_cell, int current_cell)
	{
	}

	protected virtual void UpdateOffsets(int current_cell)
	{
	}

	public virtual void Clear()
	{
	}

	public virtual void DebugDrawExtents()
	{
	}

	public void DebugDrawOffsets(int cell)
	{
		foreach (CellOffset cellOffset in this.GetOffsets(cell))
		{
			int num = Grid.OffsetCell(cell, cellOffset);
			Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
			Gizmos.DrawCube(Grid.CellToPosCCC(num, Grid.SceneLayer.Move), new Vector3(1f, 1f, 1f));
		}
	}

	public static bool isExecutingWithinJob;

	protected CellOffset[] offsets;

	private int previousCell = Grid.InvalidCell;
}
