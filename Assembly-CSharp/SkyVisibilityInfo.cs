using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public readonly struct SkyVisibilityInfo
{
	public SkyVisibilityInfo(CellOffset scanLeftOffset, int scanLeftCount, CellOffset scanRightOffset, int scanRightCount, int verticalStep)
	{
		this.scanLeftOffset = scanLeftOffset;
		this.scanLeftCount = scanLeftCount;
		this.scanRightOffset = scanRightOffset;
		this.scanRightCount = scanRightCount;
		this.verticalStep = verticalStep;
		this.totalColumnsCount = scanLeftCount + scanRightCount + (scanRightOffset.x - scanLeftOffset.x + 1);
	}

	[return: TupleElementNames(new string[] { "isAnyVisible", "percentVisible01" })]
	public ValueTuple<bool, float> GetVisibilityOf(GameObject gameObject)
	{
		return this.GetVisibilityOf(Grid.PosToCell(gameObject));
	}

	[return: TupleElementNames(new string[] { "isAnyVisible", "percentVisible01" })]
	public ValueTuple<bool, float> GetVisibilityOf(int buildingCenterCellId)
	{
		int num = 0;
		WorldContainer world = ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[buildingCenterCellId]);
		num += SkyVisibilityInfo.ScanAndGetVisibleCellCount(Grid.OffsetCell(buildingCenterCellId, this.scanLeftOffset), -1, this.verticalStep, this.scanLeftCount, world);
		num += SkyVisibilityInfo.ScanAndGetVisibleCellCount(Grid.OffsetCell(buildingCenterCellId, this.scanRightOffset), 1, this.verticalStep, this.scanRightCount, world);
		if (this.scanLeftOffset.x == this.scanRightOffset.x)
		{
			num = Mathf.Max(0, num - 1);
		}
		return new ValueTuple<bool, float>(num > 0, (float)num / (float)this.totalColumnsCount);
	}

	public void CollectVisibleCellsTo(HashSet<int> visibleCells, int buildingBottomLeftCellId, WorldContainer originWorld)
	{
		SkyVisibilityInfo.ScanAndCollectVisibleCellsTo(visibleCells, Grid.OffsetCell(buildingBottomLeftCellId, this.scanLeftOffset), -1, this.verticalStep, this.scanLeftCount, originWorld);
		SkyVisibilityInfo.ScanAndCollectVisibleCellsTo(visibleCells, Grid.OffsetCell(buildingBottomLeftCellId, this.scanRightOffset), 1, this.verticalStep, this.scanRightCount, originWorld);
	}

	private static void ScanAndCollectVisibleCellsTo(HashSet<int> visibleCells, int originCellId, int stepX, int stepY, int stepCountInclusive, WorldContainer originWorld)
	{
		for (int i = 0; i <= stepCountInclusive; i++)
		{
			int num = Grid.OffsetCell(originCellId, i * stepX, i * stepY);
			if (!SkyVisibilityInfo.IsVisible(num, originWorld))
			{
				break;
			}
			visibleCells.Add(num);
		}
	}

	private static int ScanAndGetVisibleCellCount(int originCellId, int stepX, int stepY, int stepCountInclusive, WorldContainer originWorld)
	{
		for (int i = 0; i <= stepCountInclusive; i++)
		{
			if (!SkyVisibilityInfo.IsVisible(Grid.OffsetCell(originCellId, i * stepX, i * stepY), originWorld))
			{
				return i;
			}
		}
		return stepCountInclusive + 1;
	}

	public static bool IsVisible(int cellId, WorldContainer originWorld)
	{
		if (!Grid.IsValidCell(cellId))
		{
			return false;
		}
		if (Grid.ExposedToSunlight[cellId] > 0)
		{
			return true;
		}
		WorldContainer world = ClusterManager.Instance.GetWorld((int)Grid.WorldIdx[cellId]);
		if (world != null && world.IsModuleInterior)
		{
			return true;
		}
		originWorld != world;
		return false;
	}

	public readonly CellOffset scanLeftOffset;

	public readonly int scanLeftCount;

	public readonly CellOffset scanRightOffset;

	public readonly int scanRightCount;

	public readonly int verticalStep;

	public readonly int totalColumnsCount;
}
