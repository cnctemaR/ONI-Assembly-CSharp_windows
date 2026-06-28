using System;
using UnityEngine;

public class TravelTubeNavMask : NavMask
{
	public TravelTubeNavMask(GameObject agent)
	{
		this.agent = agent.GetComponent<Navigator>();
	}

	public override bool IsTraversable(PathFinder.PotentialPath path, int from_cell, int cost, int transition_id, PathFinderAbilities abilities)
	{
		NavGrid.Transition transition = this.agent.NavGrid.transitions[transition_id];
		if (transition.start != NavType.Floor || transition.end != NavType.Tube)
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
		return component && component.IsTraversable(this.agent);
	}

	private Navigator agent;
}
