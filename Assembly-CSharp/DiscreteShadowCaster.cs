using System;
using System.Collections.Generic;

public static class DiscreteShadowCaster
{
	public static void GetVisibleCells(int cell, List<int> visiblePoints, int range, LightShape shape)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		if (shape == LightShape.Circle)
		{
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.N_NW, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.N_NE, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.E_NE, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.E_SE, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.S_SE, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.S_SW, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.W_SW, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.W_NW, 1.0, 0.0, visiblePoints);
		}
		else if (shape == LightShape.Cone)
		{
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.S_SE, 1.0, 0.0, visiblePoints);
			DiscreteShadowCaster.ScanOctant(vector2I, range, 1, DiscreteShadowCaster.Octant.S_SW, 1.0, 0.0, visiblePoints);
		}
	}

	private static bool DoesOcclude(int x, int y)
	{
		int num = Grid.XYToCell(x, y);
		return Grid.Solid[num] && Grid.Element[num].IsSolid;
	}

	private static void ScanOctant(Vector2I cellPos, int range, int depth, DiscreteShadowCaster.Octant octant, double startSlope, double endSlope, List<int> visiblePoints)
	{
		int num = range * range;
		int num2 = 0;
		int num3 = 0;
		switch (octant)
		{
		case DiscreteShadowCaster.Octant.N_NW:
			num3 = cellPos.y + depth;
			if (num3 >= Grid.HeightInCells)
			{
				return;
			}
			num2 = cellPos.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 < 0)
			{
				num2 = 0;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) <= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num2 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num3) && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num3 - 1))
						{
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false), visiblePoints);
						}
					}
					else
					{
						if (num2 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num2 - 1, num3))
						{
							startSlope = -DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2, num3 - 1))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num2++;
			}
			num2--;
			break;
		case DiscreteShadowCaster.Octant.N_NE:
			num3 = cellPos.y + depth;
			if (num3 >= Grid.HeightInCells)
			{
				return;
			}
			num2 = cellPos.x + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 >= Grid.WidthInCells)
			{
				num2 = Grid.WidthInCells - 1;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) >= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num2 + 1 < Grid.HeightInCells && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num3) && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num3 - 1))
						{
							double slope = DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false);
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope, visiblePoints);
						}
					}
					else
					{
						if (num2 + 1 < Grid.HeightInCells && DiscreteShadowCaster.DoesOcclude(num2 + 1, num3))
						{
							startSlope = DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2, num3 - 1))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num2--;
			}
			num2++;
			break;
		case DiscreteShadowCaster.Octant.E_NE:
			num2 = cellPos.x + depth;
			if (num2 >= Grid.WidthInCells)
			{
				return;
			}
			num3 = cellPos.y + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 >= Grid.HeightInCells)
			{
				num3 = Grid.HeightInCells - 1;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) >= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num3 + 1 < Grid.HeightInCells && !DiscreteShadowCaster.DoesOcclude(num2, num3 + 1) && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num3 + 1))
						{
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 + 1 < Grid.HeightInCells && DiscreteShadowCaster.DoesOcclude(num2, num3 + 1))
						{
							startSlope = DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2 - 1, num3))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num3--;
			}
			num3++;
			break;
		case DiscreteShadowCaster.Octant.E_SE:
			num2 = cellPos.x + depth;
			if (num2 >= Grid.WidthInCells)
			{
				return;
			}
			num3 = cellPos.y - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 < 0)
			{
				num3 = 0;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) <= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num3 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num2, num3 - 1) && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num3 - 1))
						{
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num2, num3 - 1))
						{
							startSlope = -DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2 - 1, num3))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num3++;
			}
			num3--;
			break;
		case DiscreteShadowCaster.Octant.S_SE:
			num3 = cellPos.y - depth;
			if (num3 < 0)
			{
				return;
			}
			num2 = cellPos.x + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 >= Grid.WidthInCells)
			{
				num2 = Grid.WidthInCells - 1;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) <= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num2 + 1 < Grid.WidthInCells && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num3) && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num3 + 1))
						{
							double slope2 = DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope2, visiblePoints);
						}
					}
					else
					{
						if (num2 + 1 < Grid.WidthInCells && DiscreteShadowCaster.DoesOcclude(num2 + 1, num3))
						{
							startSlope = -DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2, num3 + 1))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num2--;
			}
			num2++;
			break;
		case DiscreteShadowCaster.Octant.S_SW:
			num3 = cellPos.y - depth;
			if (num3 < 0)
			{
				return;
			}
			num2 = cellPos.x - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num2 < 0)
			{
				num2 = 0;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, false) >= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num2 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num3) && !DiscreteShadowCaster.DoesOcclude(num2 - 1, num3 + 1))
						{
							double slope3 = DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, false);
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, slope3, visiblePoints);
						}
					}
					else
					{
						if (num2 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num2 - 1, num3))
						{
							startSlope = DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, false);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2, num3 + 1))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num2++;
			}
			num2--;
			break;
		case DiscreteShadowCaster.Octant.W_SW:
			num2 = cellPos.x - depth;
			if (num2 < 0)
			{
				return;
			}
			num3 = cellPos.y - Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 < 0)
			{
				num3 = 0;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) >= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num3 - 1 >= 0 && !DiscreteShadowCaster.DoesOcclude(num2, num3 - 1) && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num3 - 1))
						{
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 - 1 >= 0 && DiscreteShadowCaster.DoesOcclude(num2, num3 - 1))
						{
							startSlope = DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 - 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2 + 1, num3))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num3++;
			}
			num3--;
			break;
		case DiscreteShadowCaster.Octant.W_NW:
			num2 = cellPos.x - depth;
			if (num2 < 0)
			{
				return;
			}
			num3 = cellPos.y + Convert.ToInt32(startSlope * Convert.ToDouble(depth));
			if (num3 >= Grid.HeightInCells)
			{
				num3 = Grid.HeightInCells - 1;
			}
			while (DiscreteShadowCaster.GetSlope((double)num2, (double)num3, (double)cellPos.x, (double)cellPos.y, true) <= endSlope)
			{
				if (DiscreteShadowCaster.GetVisDistance(num2, num3, cellPos.x, cellPos.y) <= num)
				{
					if (DiscreteShadowCaster.DoesOcclude(num2, num3))
					{
						if (num3 + 1 < Grid.HeightInCells && !DiscreteShadowCaster.DoesOcclude(num2, num3 + 1) && !DiscreteShadowCaster.DoesOcclude(num2 + 1, num3 + 1))
						{
							DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, DiscreteShadowCaster.GetSlope((double)num2 + 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true), visiblePoints);
						}
					}
					else
					{
						if (num3 + 1 < Grid.HeightInCells && DiscreteShadowCaster.DoesOcclude(num2, num3 + 1))
						{
							startSlope = -DiscreteShadowCaster.GetSlope((double)num2 - 0.5, (double)num3 + 0.5, (double)cellPos.x, (double)cellPos.y, true);
						}
						if (!DiscreteShadowCaster.DoesOcclude(num2 + 1, num3))
						{
							visiblePoints.Add(Grid.XYToCell(num2, num3));
						}
					}
				}
				num3--;
			}
			num3++;
			break;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		else if (num2 >= Grid.WidthInCells)
		{
			num2 = Grid.WidthInCells - 1;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		else if (num3 >= Grid.HeightInCells)
		{
			num3 = Grid.HeightInCells - 1;
		}
		if ((depth < range) & !DiscreteShadowCaster.DoesOcclude(num2, num3))
		{
			DiscreteShadowCaster.ScanOctant(cellPos, range, depth + 1, octant, startSlope, endSlope, visiblePoints);
		}
	}

	private static double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
	{
		double num;
		if (pInvert)
		{
			num = (pY1 - pY2) / (pX1 - pX2);
		}
		else
		{
			num = (pX1 - pX2) / (pY1 - pY2);
		}
		return num;
	}

	private static int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
	{
		return (pX1 - pX2) * (pX1 - pX2) + (pY1 - pY2) * (pY1 - pY2);
	}

	public enum Octant
	{
		N_NW,
		N_NE,
		E_NE,
		E_SE,
		S_SE,
		S_SW,
		W_SW,
		W_NW
	}
}
