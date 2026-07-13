using System;

namespace UnityEngine.Timeline
{
	internal static class CurveEditUtility
	{
		public static AnimationCurve CreateMatchingCurve(AnimationCurve curve)
		{
			Keyframe[] keys = curve.keys;
			for (int num = 0; num != keys.Length; num++)
			{
				if (!float.IsPositiveInfinity(keys[num].inTangent))
				{
					keys[num].inTangent = -keys[num].inTangent;
				}
				if (!float.IsPositiveInfinity(keys[num].outTangent))
				{
					keys[num].outTangent = -keys[num].outTangent;
				}
				keys[num].value = 1f - keys[num].value;
			}
			return new AnimationCurve(keys);
		}
	}
}
