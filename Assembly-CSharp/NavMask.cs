using System;

public class NavMask
{
	public virtual bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities)
	{
		return true;
	}

	public virtual void ApplyTraversalToPath(ref PathFinder.PotentialPath path, int from_cell)
	{
	}
}
