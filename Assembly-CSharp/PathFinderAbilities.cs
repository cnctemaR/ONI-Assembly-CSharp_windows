using System;
using System.Collections.Generic;

public struct PathFinderAbilities
{
	public void AddMask(NavMask mask)
	{
		if (this.masks == null)
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

	public bool CanTraverse(PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, int underwater_cost)
	{
		if (this.masks != null && !this.ignoreNavigationMasks)
		{
			for (int i = 0; i < this.masks.Count; i++)
			{
				if (!this.masks[i].IsTraversable(path, from_cell, cost, transition_id, this))
				{
					return false;
				}
			}
		}
		return path.HasFlag(PathFinder.PotentialPath.Flags.HasSuit) || path.navType == NavType.Tube || underwater_cost <= this.maxUnderwaterCost;
	}

	public void ApplyTraversalToPath(ref PathFinder.PotentialPath path, int from_cell)
	{
		if (this.masks != null && !this.ignoreNavigationMasks)
		{
			for (int i = 0; i < this.masks.Count; i++)
			{
				this.masks[i].ApplyTraversalToPath(ref path, from_cell);
			}
		}
	}

	public int maxUnderwaterCost;

	public bool ignoreNavigationMasks;

	private List<NavMask> masks;
}
