using System;

public class NavTable
{
	public NavTable(int cell_count)
	{
		this.ValidCells = new bool[cell_count * 9];
	}

	public bool IsValid(int cell, NavType nav_type = NavType.Floor)
	{
		return Grid.IsValidCell(cell) && this.ValidCells[(int)((byte)(cell * 9) + nav_type)];
	}

	public void SetValid(int cell, NavType nav_type, bool is_valid)
	{
		int num = (int)((byte)(cell * 9) + nav_type);
		bool flag = this.ValidCells[num];
		if (flag != is_valid)
		{
			this.ValidCells[num] = is_valid;
			if (this.OnValidCellChanged != null)
			{
				this.OnValidCellChanged(cell, nav_type);
			}
		}
	}

	public Action<int, NavType> OnValidCellChanged;

	private bool[] ValidCells;
}
