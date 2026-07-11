using System;
using System.Collections.Generic;
using System.Diagnostics;

public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
	public static void DestroyInstance()
	{
		MinionGroupProber.Instance = null;
	}

	public static MinionGroupProber Get()
	{
		return MinionGroupProber.Instance;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MinionGroupProber.Instance = this;
		this.proberCells = new int[Grid.CellCount];
		for (int i = 0; i < this.proberCells.Length; i++)
		{
			this.proberCells[i] = -1;
		}
		this.pathGrids = new List<PathGrid>();
		this.pendingPathGridRemovals = new List<int>();
	}

	public bool IsReachable(Workable workable)
	{
		int num = Grid.PosToCell(workable);
		return this.IsReachable(num, workable.GetOffsets());
	}

	public bool IsReachable(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		this.LaunderCells();
		int num = this.proberCells[cell];
		if (num == -1)
		{
			return false;
		}
		if (num == -2)
		{
			return true;
		}
		DebugUtil.Assert(num < this.pathGrids.Count);
		return this.pathGrids[num].GetCost(cell) != -1;
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (Grid.IsValidCell(cell))
		{
			int num = offsets.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = Grid.OffsetCell(cell, offsets[i]);
				if (this.IsReachable(num2))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetProberCell(int cell, PathGrid pathGrid)
	{
		int num = this.pathGrids.IndexOf(pathGrid);
		if (num == -1)
		{
			object obj = this.pathGrids;
			lock (obj)
			{
				this.pathGrids.Add(pathGrid);
				num = this.pathGrids.Count - 1;
			}
		}
		this.proberCells[cell] = num;
	}

	public void ProxyProberCell(int cell, bool set)
	{
		this.proberCells[cell] = ((!set) ? (-1) : (-2));
	}

	public bool ReleasePathGrid(PathGrid pathGrid)
	{
		int num = this.pathGrids.IndexOf(pathGrid);
		if (num != -1)
		{
			this.pendingPathGridRemovals.Add(num);
			return true;
		}
		return false;
	}

	private void LaunderCells()
	{
		if (this.pendingPathGridRemovals.Count == 0)
		{
			return;
		}
		this.pendingPathGridRemovals.Sort();
		for (int num = 0; num != this.proberCells.Length; num++)
		{
			for (int num2 = this.pendingPathGridRemovals.Count - 1; num2 != -1; num2--)
			{
				if (this.pendingPathGridRemovals[num2] <= this.proberCells[num])
				{
					this.proberCells[num] -= num2 + 1;
					break;
				}
			}
		}
		for (int num3 = this.pendingPathGridRemovals.Count - 1; num3 != -1; num3--)
		{
			this.pathGrids.RemoveAt(this.pendingPathGridRemovals[num3]);
		}
		this.pendingPathGridRemovals.Clear();
	}

	[Conditional("MINION_GROUP_PROBER_UNIT_TESTS")]
	private void TestLaunderCells()
	{
		List<PathGrid> list = new List<PathGrid>();
		for (int num = 0; num != 10; num++)
		{
			PathGrid pathGrid = new PathGrid(10, 10, false, new NavType[1]);
			list.Add(pathGrid);
			pathGrid.SetGroupProber(this);
			pathGrid.BeginUpdate(0, false);
			pathGrid.EndUpdate(true);
		}
		PathFinder.Cell cell = new PathFinder.Cell
		{
			cost = 10,
			queryId = 1
		};
		for (int num2 = 0; num2 != 100; num2++)
		{
			list[num2 % list.Count].SetCell(new PathFinder.PotentialPath
			{
				cell = num2
			}, ref cell);
		}
		int num3 = 0;
		for (int num4 = 0; num4 != 100; num4++)
		{
			if (this.IsReachable(num4))
			{
				num3++;
			}
		}
		DebugUtil.Assert(num3 == 100);
		this.ReleasePathGrid(list[5]);
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 1);
		list[5] = null;
		num3 = 0;
		for (int num5 = 0; num5 != 100; num5++)
		{
			if (this.IsReachable(num5))
			{
				num3++;
			}
		}
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num3 == 90);
		this.ReleasePathGrid(list[9]);
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 1);
		list[9] = null;
		num3 = 0;
		for (int num6 = 0; num6 != 100; num6++)
		{
			if (this.IsReachable(num6))
			{
				num3++;
			}
		}
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num3 == 80);
		this.ReleasePathGrid(list[0]);
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 1);
		list[0] = null;
		num3 = 0;
		for (int num7 = 0; num7 != 100; num7++)
		{
			if (this.IsReachable(num7))
			{
				num3++;
			}
		}
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num3 == 70);
		this.ReleasePathGrid(list[1]);
		this.ReleasePathGrid(list[3]);
		this.ReleasePathGrid(list[7]);
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 3);
		list[1] = null;
		list[3] = null;
		list[7] = null;
		num3 = 0;
		for (int num8 = 0; num8 != 100; num8++)
		{
			if (this.IsReachable(num8))
			{
				num3++;
			}
		}
		DebugUtil.Assert(this.pendingPathGridRemovals.Count == 0);
		DebugUtil.Assert(num3 == 40);
		foreach (PathGrid pathGrid2 in list)
		{
			this.ReleasePathGrid(pathGrid2);
		}
		this.LaunderCells();
	}

	private static MinionGroupProber Instance;

	private List<PathGrid> pathGrids;

	private List<int> pendingPathGridRemovals;

	private int[] proberCells;

	private const int InvalidIndex = -1;

	private const int ProxyIndex = -2;
}
