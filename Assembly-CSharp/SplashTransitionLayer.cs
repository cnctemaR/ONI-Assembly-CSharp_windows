using System;
using UnityEngine;

public class SplashTransitionLayer : TransitionDriver.OverrideLayer
{
	public SplashTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		this.lastSplashTime = Time.time;
	}

	private void RefreshSplashes(Navigator navigator, Navigator.ActiveTransition transition)
	{
		if (navigator == null || navigator.IsSwimming())
		{
			return;
		}
		if (transition.end == NavType.Tube)
		{
			return;
		}
		Vector3 position = navigator.transform.GetPosition();
		if (this.lastSplashTime + 1f < Time.time && Grid.Element[Grid.PosToCell(position)].IsLiquid)
		{
			this.lastSplashTime = Time.time;
			Game.Instance.SpawnFX(SpawnFXHashes.SplashStep, position + new Vector3(0f, 0.75f, -0.1f), 0f);
		}
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		this.RefreshSplashes(navigator, transition);
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
		this.RefreshSplashes(navigator, transition);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		this.RefreshSplashes(navigator, transition);
	}

	private float lastSplashTime;

	private const float SPLASH_INTERVAL = 1f;
}
