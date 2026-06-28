using System;
using UnityEngine;

public class LiquidTileEdging
{
	private void Update()
	{
		int num;
		int num2;
		int num3;
		int num4;
		Grid.GetVisibleExtents(out num, out num2, out num3, out num4);
		num = Math.Max(0, num);
		num2 = Math.Max(0, num2);
		num = Mathf.Min(num, Grid.WidthInCells - 1);
		num2 = Mathf.Min(num2, Grid.HeightInCells - 1);
		num3 = Mathf.CeilToInt((float)num3);
		num4 = Mathf.CeilToInt((float)num4);
		num3 = Mathf.Max(num3, 0);
		num4 = Mathf.Max(num4, 0);
		num3 = Mathf.Min(num3, Grid.WidthInCells - 1);
		num4 = Mathf.Min(num4, Grid.HeightInCells - 1);
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		for (int i = num2; i < num4; i++)
		{
			for (int j = num; j < num3; j++)
			{
				int num8 = i * Grid.WidthInCells + j;
				Element element = Grid.Element[num8];
				if (element.IsSolid)
				{
					num5++;
				}
				else if (element.IsLiquid)
				{
					num6++;
				}
				else
				{
					num7++;
				}
			}
		}
	}
}
