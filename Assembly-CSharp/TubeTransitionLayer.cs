using System;
using UnityEngine;

public class TubeTransitionLayer : TransitionDriver.OverrideLayer
{
	public TubeTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		this.tube_traveller = navigator.GetSMI<TubeTraveller.Instance>();
		if (navigator.CurrentNavType == NavType.Tube && !this.tube_traveller.inTube)
		{
			this.tube_traveller.OnTubeTransition(true);
		}
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		this.tube_traveller.OnPathAdvanced(null);
		if (transition.start != NavType.Tube && transition.end == NavType.Tube)
		{
			int num = Grid.PosToCell(navigator);
			this.entrance = this.GetEntrance(num);
			return;
		}
		this.entrance = null;
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		if (transition.start != NavType.Tube && transition.end == NavType.Tube && this.entrance)
		{
			this.entrance.ConsumeCharge(navigator.gameObject);
			this.entrance = null;
		}
		this.tube_traveller.OnTubeTransition(transition.end == NavType.Tube);
	}

	private TravelTubeEntrance GetEntrance(int cell)
	{
		if (!Grid.HasUsableTubeEntrance(cell, this.tube_traveller.prefabInstanceID))
		{
			return null;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			TravelTubeEntrance component = gameObject.GetComponent<TravelTubeEntrance>();
			if (component != null && component.isSpawned)
			{
				return component;
			}
		}
		return null;
	}

	private TubeTraveller.Instance tube_traveller;

	private TravelTubeEntrance entrance;
}
