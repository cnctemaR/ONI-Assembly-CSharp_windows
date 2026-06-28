using System;

public struct Extents
{
	public Extents(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public Extents(int cell, int radius)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		this.x = num - radius;
		this.y = num2 - radius;
		this.width = radius * 2 + 1;
		this.height = radius * 2 + 1;
	}

	public Extents(int center_x, int center_y, int radius)
	{
		this.x = center_x - radius;
		this.y = center_y - radius;
		this.width = radius * 2 + 1;
		this.height = radius * 2 + 1;
	}

	public Extents(int cell, CellOffset[] offsets)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = num;
		int num4 = num2;
		foreach (CellOffset cellOffset in offsets)
		{
			int num5 = 0;
			int num6 = 0;
			int num7 = Grid.OffsetCell(cell, cellOffset);
			Grid.CellToXY(num7, out num5, out num6);
			num = Math.Min(num, num5);
			num2 = Math.Min(num2, num6);
			num3 = Math.Max(num3, num5);
			num4 = Math.Max(num4, num6);
		}
		this.x = num;
		this.y = num2;
		this.width = num3 - num + 1;
		this.height = num4 - num2 + 1;
	}

	public Extents(int cell, CellOffset[][] offset_table)
	{
		int num = 0;
		int num2 = 0;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = num;
		int num4 = num2;
		foreach (CellOffset[] array in offset_table)
		{
			int num5 = 0;
			int num6 = 0;
			int num7 = Grid.OffsetCell(cell, array[0]);
			Grid.CellToXY(num7, out num5, out num6);
			num = Math.Min(num, num5);
			num2 = Math.Min(num2, num6);
			num3 = Math.Max(num3, num5);
			num4 = Math.Max(num4, num6);
		}
		this.x = num;
		this.y = num2;
		this.width = num3 - num + 1;
		this.height = num4 - num2 + 1;
	}

	public static Extents OneCell(int cell)
	{
		int num;
		int num2;
		Grid.CellToXY(cell, out num, out num2);
		return new Extents(num, num2, 1, 1);
	}

	public bool Contains(Vector2I pos)
	{
		return this.x <= pos.x && pos.x < this.x + this.width && this.y <= pos.y && pos.y < this.y + this.height;
	}

	public int x;

	public int y;

	public int width;

	public int height;
}
