using System;

public class CellVisibility
{
	public CellVisibility()
	{
		Grid.GetVisibleExtents(out this.MinX, out this.MinY, out this.MaxX, out this.MaxY);
	}

	public bool IsVisible(int cell)
	{
		int num = Grid.CellColumn(cell);
		bool flag;
		if (num < this.MinX || num > this.MaxX)
		{
			flag = false;
		}
		else
		{
			int num2 = Grid.CellRow(cell);
			flag = num2 >= this.MinY && num2 <= this.MaxY;
		}
		return flag;
	}

	private int MinX;

	private int MinY;

	private int MaxX;

	private int MaxY;
}
