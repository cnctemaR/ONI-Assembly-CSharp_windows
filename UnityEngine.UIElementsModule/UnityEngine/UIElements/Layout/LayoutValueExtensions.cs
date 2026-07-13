using System;

namespace UnityEngine.UIElements.Layout
{
	internal static class LayoutValueExtensions
	{
		public static LayoutValue Percent(this float value)
		{
			return LayoutValue.Percent(value);
		}

		public static LayoutValue Pt(this float value)
		{
			return LayoutValue.Point(value);
		}

		public static LayoutValue Percent(this int value)
		{
			return LayoutValue.Percent((float)value);
		}

		public static LayoutValue Pt(this int value)
		{
			return LayoutValue.Point((float)value);
		}
	}
}
