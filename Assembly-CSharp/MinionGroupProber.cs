using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MinionGroupProber")]
public class MinionGroupProber : KMonoBehaviour
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
		this.cells = new int[Grid.CellCount];
	}

	public bool IsReachable(int cell)
	{
		return Grid.IsValidCell(cell) && this.cells[cell] > 0;
	}

	public bool IsReachable(int cell, CellOffset[] offsets)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		foreach (CellOffset cellOffset in offsets)
		{
			if (this.IsReachable(Grid.OffsetCell(cell, cellOffset)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAllReachable(int cell, CellOffset[] offsets)
	{
		if (this.IsReachable(cell))
		{
			return true;
		}
		foreach (CellOffset cellOffset in offsets)
		{
			if (this.IsReachable(Grid.OffsetCell(cell, cellOffset)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReachable(Workable workable)
	{
		return this.IsReachable(Grid.PosToCell(workable), workable.GetOffsets());
	}

	public void Occupy(List<int> cells)
	{
		foreach (int num in cells)
		{
			Interlocked.Increment(ref this.cells[num]);
		}
	}

	public void OccupyST(List<int> cells)
	{
		foreach (int num in cells)
		{
			this.cells[num]++;
		}
	}

	public void Occupy(int cell)
	{
		Interlocked.Increment(ref this.cells[cell]);
	}

	public void Vacate(List<int> cells)
	{
		foreach (int num in cells)
		{
			Interlocked.Decrement(ref this.cells[num]);
		}
	}

	public void VacateST(List<int> cells)
	{
		foreach (int num in cells)
		{
			this.cells[num]--;
		}
	}

	public void Vacate(int cell)
	{
		Interlocked.Decrement(ref this.cells[cell]);
	}

	private static MinionGroupProber Instance;

	private int[] cells;
}
