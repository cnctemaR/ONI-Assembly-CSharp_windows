using System;

namespace UnityEngine.UIElements
{
	public enum PropagationPhase
	{
		None,
		TrickleDown,
		BubbleUp = 3,
		[Obsolete("PropagationPhase.AtTarget has been removed as part of an event propagation simplification. Events now propagate through the TrickleDown phase followed immediately by the BubbleUp phase. Please use TrickleDown or BubbleUp. You can check if the event target is the current element by testing event.target == this in your local callback.", false)]
		AtTarget = 2,
		[Obsolete("PropagationPhase.DefaultAction has been removed as part of an event propagation simplification. ExecuteDefaultAction now occurs as part of the BubbleUp phase. Please use BubbleUp.", false)]
		DefaultAction = 4,
		[Obsolete("PropagationPhase.DefaultActionAtTarget has been removed as part of an event propagation simplification. ExecuteDefaultActionAtTarget now occurs as part of the BubbleUp phase. Please use BubbleUp", false)]
		DefaultActionAtTarget
	}
}
