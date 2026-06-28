using System;
using System.Collections.Generic;

public class OffsetWalkerTable
{
	public OffsetWalkerTable(CellOffset[][] offset_table)
	{
		this.offsetTable = offset_table;
		this.CreateDeltaTable(2, 2, this.offsetTable);
	}

	private void CreateDeltaTable(int x_range, int y_range, CellOffset[][] offset_table)
	{
		this.rangeX = x_range;
		this.rangeY = y_range;
		this.deltaTable = new CellOffset[x_range * 2 + 1][][][];
		for (int i = 0; i < this.deltaTable.Length; i++)
		{
			this.deltaTable[i] = new CellOffset[y_range * 2 + 1][][];
			for (int j = 0; j < this.deltaTable[i].Length; j++)
			{
				CellOffset cellOffset = new CellOffset(i - x_range, j - y_range);
				this.deltaTable[i][j] = OffsetWalkerTable.CreateDeltaTableEntry(cellOffset, offset_table);
			}
		}
	}

	private static CellOffset[][] CreateDeltaTableEntry(CellOffset table_offset, CellOffset[][] offset_table)
	{
		List<CellOffset[]> list = new List<CellOffset[]>();
		foreach (CellOffset[] array in offset_table)
		{
			CellOffset cellOffset = new CellOffset(array[0].x + table_offset.x, array[0].y + table_offset.y);
			bool flag = false;
			foreach (CellOffset[] array2 in offset_table)
			{
				if (array2[0].Equals(cellOffset) && array2.Length == 1)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(array);
			}
		}
		return list.ToArray();
	}

	public CellOffset[][] GetDeltaOffsets(int cell, int previous_cell)
	{
		if (!Grid.IsValidCell(cell) || !Grid.IsValidCell(previous_cell))
		{
			return this.offsetTable;
		}
		CellOffset offset = Grid.GetOffset(previous_cell, cell);
		int num = offset.x + this.rangeX;
		if (num < 0 || num >= this.deltaTable.Length)
		{
			return this.offsetTable;
		}
		int num2 = offset.y + this.rangeY;
		if (num2 < 0 || num2 >= this.deltaTable[num].Length)
		{
			return this.offsetTable;
		}
		return this.deltaTable[num][num2];
	}

	private CellOffset[][] offsetTable;

	private CellOffset[][][][] deltaTable;

	private int rangeX;

	private int rangeY;
}
