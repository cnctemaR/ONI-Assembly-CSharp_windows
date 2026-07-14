using System;

public class FullPuftTransitionLayer : TransitionDriver.OverrideLayer
{
	public FullPuftTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		this.calorie_monitor = navigator.GetSMI<CreatureCalorieMonitor.Instance>();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		if (this.calorie_monitor != null && this.calorie_monitor.stomach.IsReadyToPoop())
		{
			string text = HashCache.Get().Get(transition.anim.HashValue) + "_full";
			if (navigator.animController.HasAnimation(text))
			{
				transition.anim = text;
			}
		}
	}

	private CreatureCalorieMonitor.Instance calorie_monitor;
}
