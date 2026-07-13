using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public enum FilterFunctionType
	{
		None,
		Custom,
		Tint,
		Opacity,
		Invert,
		Grayscale,
		Sepia,
		Blur,
		Contrast,
		HueRotate,
		[Obsolete("Use Enum.GetValues(typeof(FilterFunctionType)).Length instead", false)]
		Count
	}
}
