using System;

namespace UnityEngine.Yoga
{
	internal static class YogaConstants
	{
		public static bool IsUndefined(float value)
		{
			return float.IsNaN(value);
		}

		public static bool IsUndefined(YogaValue value)
		{
			return value.Unit == YogaUnit.Undefined;
		}

		public const float Undefined = float.NaN;
	}
}
