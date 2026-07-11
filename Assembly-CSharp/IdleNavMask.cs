using System;

internal struct IdleNavMask
{
	public IdleNavMask(Navigator navigator)
	{
		this.enabled = false;
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, int transition_id)
	{
		return !this.enabled || (!Grid.PreventIdlingOnCell[path.cell] && !Grid.PreventIdlingOnCell[from_cell]);
	}

	public bool enabled;
}
