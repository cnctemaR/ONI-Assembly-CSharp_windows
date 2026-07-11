using System;

public class NavTable
{
	public NavTable(int cell_count)
	{
		this.ValidCells = new short[cell_count];
		this.NavTypeMasks = new short[10];
		for (short num = 0; num < 10; num += 1)
		{
			this.NavTypeMasks[(int)num] = (short)(1 << (int)num);
		}
	}

	public bool IsValid(int cell, NavType nav_type = NavType.Floor)
	{
		if (Grid.IsValidCell(cell))
		{
			short num = this.NavTypeMasks[(int)nav_type];
			return (num & this.ValidCells[cell]) != 0;
		}
		return false;
	}

	public void SetValid(int cell, NavType nav_type, bool is_valid)
	{
		short num = this.NavTypeMasks[(int)nav_type];
		short num2 = this.ValidCells[cell];
		bool flag = (num2 & num) != 0;
		if (flag != is_valid)
		{
			if (is_valid)
			{
				this.ValidCells[cell] = num | num2;
			}
			else
			{
				this.ValidCells[cell] = ~num & num2;
			}
			if (this.OnValidCellChanged != null)
			{
				this.OnValidCellChanged(cell, nav_type);
			}
		}
	}

	public Action<int, NavType> OnValidCellChanged;

	private short[] NavTypeMasks;

	private short[] ValidCells;
}
