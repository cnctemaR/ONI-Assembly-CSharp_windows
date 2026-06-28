using System;

public class NavMask
{
	public virtual bool IsTraversable(int cell, int from_cell, PathFinderAbilities abilities)
	{
		return true;
	}
}
