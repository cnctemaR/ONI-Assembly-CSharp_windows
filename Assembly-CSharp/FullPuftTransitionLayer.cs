using System;

public class FullPuftTransitionLayer : TransitionDriver.OverrideLayer
{
	public FullPuftTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		CreatureCalorieMonitor.Instance smi = navigator.GetSMI<CreatureCalorieMonitor.Instance>();
		if (smi != null && smi.stomach.IsReadyToPoop())
		{
			KBatchedAnimController component = navigator.GetComponent<KBatchedAnimController>();
			string text = HashCache.Get().Get(transition.anim.HashValue) + "_full";
			if (component.HasAnimation(text))
			{
				transition.anim = text;
			}
		}
	}
}
