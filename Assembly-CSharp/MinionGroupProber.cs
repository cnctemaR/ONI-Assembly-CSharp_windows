using System;
using UnityEngine;

public class MinionGroupProber : KMonoBehaviour, IGroupProber
{
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
			this.proberCells[i] = -10000;
		}
	}

	public bool IsReachable(Workable workable)
	{
		int num = Grid.PosToCell(workable);
		return this.IsReachable(num, workable.GetOffsets());
	}

	public bool IsReachable(int cell, int current_frame)
	{
		if (Grid.IsValidCell(cell))
		{
			int count = Components.LiveMinionIdentities.Count;
			int num = current_frame - this.proberCells[cell];
			if (num <= count)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsReachable(int cell)
	{
		return this.IsReachable(cell, Time.frameCount);
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

	public void SetProberCell(int cell)
	{
		this.proberCells[cell] = Time.frameCount;
	}

	private static MinionGroupProber Instance;

	private int[] proberCells;
}
