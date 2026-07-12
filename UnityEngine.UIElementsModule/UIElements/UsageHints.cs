using System;

namespace UnityEngine.UIElements
{
	[Flags]
	public enum UsageHints
	{
		None = 0,
		DynamicTransform = 1,
		GroupTransform = 2,
		MaskContainer = 4,
		DynamicColor = 8
	}
}
