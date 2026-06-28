using System;
using UnityEngine;

public class BaseArea : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		BaseArea.Instance = this;
		this.cells = new BaseArea.Cell[Grid.CellCount];
		this.pendingCells = new int[Grid.CellCount * 2];
	}

	public bool IsInsideBase(int cell)
	{
		return this.cells != null && Grid.IsValidCell(cell) && this.cells[cell].lastUpdateFrame == this.lastUpdateFrame;
	}

	private void UpdateCell(int cell, int update_frame)
	{
		if (this.cells[cell].lastUpdateFrame == update_frame)
		{
			return;
		}
		this.cells[cell] = new BaseArea.Cell
		{
			lastUpdateFrame = update_frame
		};
		this.AddCell(Grid.CellRight(cell), update_frame);
		this.AddCell(Grid.CellLeft(cell), update_frame);
		this.AddCell(Grid.CellAbove(cell), update_frame);
		this.AddCell(Grid.CellBelow(cell), update_frame);
	}

	private void AddCell(int cell, int update_frame)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		if (Grid.Solid[cell])
		{
			return;
		}
		if (this.cells[cell].lastUpdateFrame == update_frame)
		{
			return;
		}
		this.pendingCells[this.pendingCellCount++] = cell;
	}

	private void Update()
	{
		GameObject gameObject = ((Components.Telepads.Count <= 0) ? null : Components.Telepads[0].gameObject);
		if (gameObject == null)
		{
			return;
		}
		int frameCount = Time.frameCount;
		int num = Grid.PosToCell(gameObject.transform.position);
		this.pendingCellCount = 0;
		this.AddCell(num, frameCount);
		this.AddCell(Grid.CellRight(num), frameCount);
		this.AddCell(Grid.CellLeft(num), frameCount);
		this.AddCell(Grid.CellAbove(num), frameCount);
		this.AddCell(Grid.CellBelow(num), frameCount);
		for (int i = 0; i < this.pendingCellCount; i++)
		{
			this.UpdateCell(this.pendingCells[i], frameCount);
		}
		this.lastUpdateFrame = frameCount;
	}

	public static BaseArea Instance;

	private BaseArea.Cell[] cells;

	private int lastUpdateFrame;

	private int[] pendingCells;

	private int pendingCellCount;

	public struct Cell
	{
		public int lastUpdateFrame;
	}
}
