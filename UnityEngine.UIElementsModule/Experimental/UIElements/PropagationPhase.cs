using System;

namespace UnityEngine.Experimental.UIElements
{
	public enum PropagationPhase
	{
		None,
		TrickleDown,
		[Obsolete("Use TrickleDown instead of Capture.")]
		Capture = 1,
		AtTarget,
		BubbleUp,
		DefaultAction
	}
}
