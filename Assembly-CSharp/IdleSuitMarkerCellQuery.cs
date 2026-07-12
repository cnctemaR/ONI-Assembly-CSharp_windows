using System;

public class IdleSuitMarkerCellQuery : PathFinderQuery
{
	public IdleSuitMarkerCellQuery(bool is_rotated, int marker_x)
	{
		this.targetCell = Grid.InvalidCell;
		this.isRotated = is_rotated;
		this.markerX = marker_x;
	}

	public override bool IsMatch(int cell, int parent_cell, int cost)
	{
		if (!Grid.PreventIdleTraversal[cell] && Grid.CellToXY(cell).x < this.markerX != this.isRotated)
		{
			this.targetCell = cell;
		}
		return this.targetCell != Grid.InvalidCell;
	}

	public override int GetResultCell()
	{
		return this.targetCell;
	}

	private int targetCell;

	private bool isRotated;

	private int markerX;
}
