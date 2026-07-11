using System;

namespace UnityEngine.Experimental.UIElements
{
	internal enum CallbackPhase
	{
		TargetAndBubbleUp = 1,
		TrickleDownAndTarget,
		[Obsolete("Use TrickleDownAndTarget instead of CaptureAndTarget.")]
		CaptureAndTarget = 2
	}
}
