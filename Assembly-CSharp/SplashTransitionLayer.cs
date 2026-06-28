using System;
using UnityEngine;

public class SplashTransitionLayer : TransitionDriver.OverrideLayer
{
	public SplashTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		this.lastSplashTime = Time.time;
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void RefreshSplashes(Navigator navigator)
	{
		if (this.lastSplashTime + 1f < Time.time && Grid.Element[Grid.PosToCell(navigator.transform.position)].IsLiquid)
		{
			this.lastSplashTime = Time.time;
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("splash_step_kanim", navigator.transform.position + new Vector3(0f, 0.75f, -0.1f), null, false, Grid.SceneLayer.Front, false);
			kbatchedAnimController.Play("fx1", KAnim.PlayMode.Once, 1f, 0f);
			kbatchedAnimController.destroyOnAnimComplete = true;
		}
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		this.RefreshSplashes(navigator);
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
		this.RefreshSplashes(navigator);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		this.RefreshSplashes(navigator);
	}

	private float lastSplashTime;

	private const float SPLASH_INTERVAL = 1f;
}
