using System;

public class SadSnailTransitionLayer : TransitionDriver.OverrideLayer
{
	public SadSnailTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		this.desiccationMonitor = navigator.GetSMI<DesiccationMonitor.Instance>();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		if (this.desiccationMonitor == null || !this.desiccationMonitor.IsDesiccating())
		{
			return;
		}
		string text = HashCache.Get().Get(transition.anim.HashValue) + "_sad";
		if (navigator.animController.HasAnimation(text))
		{
			transition.anim = text;
		}
	}

	private DesiccationMonitor.Instance desiccationMonitor;
}
