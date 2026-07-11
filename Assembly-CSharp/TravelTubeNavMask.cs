using System;
using UnityEngine;

internal struct TravelTubeNavMask
{
	public TravelTubeNavMask(Navigator navigator)
	{
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id)
	{
		if (path.navType != NavType.Tube || from_nav_type != NavType.Floor)
		{
			return true;
		}
		if (!Grid.HasTubeEntrance[from_cell])
		{
			return false;
		}
		GameObject gameObject = Grid.Objects[from_cell, 1];
		if (gameObject == null)
		{
			return false;
		}
		TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
		return component && component.IsTraversable(agent);
	}
}
