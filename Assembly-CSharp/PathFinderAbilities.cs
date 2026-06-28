using System;
using System.Collections.Generic;

public struct PathFinderAbilities
{
	public void AddMask(NavMask mask)
	{
		if (this.masks != null)
		{
			this.masks = new List<NavMask>();
		}
		this.masks.Add(mask);
	}

	public void RemoveMask(NavMask mask)
	{
		if (this.masks != null)
		{
			this.masks.Remove(mask);
		}
	}

	public bool CanTraverse(int cell, int underwater_cost)
	{
		if (this.masks != null)
		{
			for (int i = 0; i < this.masks.Count; i++)
			{
				if (!this.masks[i].IsTraversable(cell))
				{
					return false;
				}
			}
		}
		return underwater_cost <= this.maxUnderwaterCost && (!Grid.SuitRequired[cell] || (byte)(this.flags & PathFinderFlags.SuitRequired) != 0);
	}

	public PathFinderFlags flags;

	public int maxUnderwaterCost;

	private List<NavMask> masks;
}
