using System;

public class NavMask
{
	public virtual bool IsTraversable(int cell, int from_cell, int cost, PathFinderAbilities abilities)
	{
		return true;
	}
}
