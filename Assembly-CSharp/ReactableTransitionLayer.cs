using System;

public class ReactableTransitionLayer : TransitionDriver.InterruptOverrideLayer
{
	public ReactableTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	protected override bool IsOverrideComplete()
	{
		return !this.reactionMonitor.IsReacting() && base.IsOverrideComplete();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		if (this.reactionMonitor == null)
		{
			this.reactionMonitor = navigator.GetSMI<ReactionMonitor.Instance>();
		}
		this.reactionMonitor.PollForReactables(transition);
		if (this.reactionMonitor.IsReacting())
		{
			base.BeginTransition(navigator, transition);
			transition.start = this.originalTransition.start;
			transition.end = this.originalTransition.end;
		}
	}

	private ReactionMonitor.Instance reactionMonitor;
}
