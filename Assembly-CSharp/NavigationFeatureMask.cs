using System;

internal struct NavigationFeatureMask
{
	public NavigationFeatureMask(Navigator navigator)
	{
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, PathFinderAbilities abilities)
	{
		Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(from_cell);
		return navigationFeature == null || navigationFeature.IsTraversable(agent, path, from_cell, cost, abilities);
	}

	public void ApplyTraversalToPath(Navigator agent, ref PathFinder.PotentialPath path, int from_cell)
	{
		Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(from_cell);
		if (navigationFeature != null)
		{
			navigationFeature.ApplyTraversalToPath(agent, ref path, from_cell);
		}
	}
}
