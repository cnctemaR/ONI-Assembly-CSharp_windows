using System;
using UnityEngine;

public class NavigationFeatureMask : NavMask
{
	public NavigationFeatureMask(GameObject agent)
	{
		this.agent = agent.GetComponent<Navigator>();
	}

	public override bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities)
	{
		Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(from_cell);
		return navigationFeature == null || navigationFeature.IsTraversable(this.agent, path, from_cell, cost, abilities);
	}

	public override void ApplyTraversalToPath(ref PathFinder.PotentialPath path, int from_cell)
	{
		Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(from_cell);
		if (navigationFeature != null)
		{
			navigationFeature.ApplyTraversalToPath(this.agent, ref path, from_cell);
		}
	}

	private Navigator agent;
}
