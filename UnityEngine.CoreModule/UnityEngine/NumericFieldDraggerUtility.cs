using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine
{
	[MovedFrom("UnityEditor")]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
	internal class NumericFieldDraggerUtility
	{
		public static float Acceleration(bool shiftPressed, bool altPressed)
		{
			return (float)(shiftPressed ? 4 : 1) * (altPressed ? 0.25f : 1f);
		}

		public static float NiceDelta(Vector2 deviceDelta, float acceleration)
		{
			deviceDelta.y = -deviceDelta.y;
			bool flag = Mathf.Abs(Mathf.Abs(deviceDelta.x) - Mathf.Abs(deviceDelta.y)) / Mathf.Max(Mathf.Abs(deviceDelta.x), Mathf.Abs(deviceDelta.y)) > 0.1f;
			if (flag)
			{
				bool flag2 = Mathf.Abs(deviceDelta.x) > Mathf.Abs(deviceDelta.y);
				if (flag2)
				{
					NumericFieldDraggerUtility.s_UseYSign = false;
				}
				else
				{
					NumericFieldDraggerUtility.s_UseYSign = true;
				}
			}
			bool flag3 = NumericFieldDraggerUtility.s_UseYSign;
			float num;
			if (flag3)
			{
				num = Mathf.Sign(deviceDelta.y) * deviceDelta.magnitude * acceleration;
			}
			else
			{
				num = Mathf.Sign(deviceDelta.x) * deviceDelta.magnitude * acceleration;
			}
			return num;
		}

		public static double CalculateFloatDragSensitivity(double value)
		{
			bool flag = double.IsInfinity(value) || double.IsNaN(value);
			double num;
			if (flag)
			{
				num = 0.0;
			}
			else
			{
				num = Math.Max(1.0, Math.Pow(Math.Abs(value), 0.5)) * 0.029999999329447746;
			}
			return num;
		}

		public static double CalculateFloatDragSensitivity(double value, double minValue, double maxValue)
		{
			bool flag = double.IsInfinity(value) || double.IsNaN(value);
			double num;
			if (flag)
			{
				num = 0.0;
			}
			else
			{
				double num2 = Math.Abs(maxValue - minValue);
				num = num2 / 100.0 * 0.029999999329447746;
			}
			return num;
		}

		public static long CalculateIntDragSensitivity(long value)
		{
			return (long)NumericFieldDraggerUtility.CalculateIntDragSensitivity((double)value);
		}

		public static ulong CalculateIntDragSensitivity(ulong value)
		{
			return (ulong)NumericFieldDraggerUtility.CalculateIntDragSensitivity(value);
		}

		public static double CalculateIntDragSensitivity(double value)
		{
			return Math.Max(1.0, Math.Pow(Math.Abs(value), 0.5) * 0.029999999329447746);
		}

		public static long CalculateIntDragSensitivity(long value, long minValue, long maxValue)
		{
			long num = Math.Abs(maxValue - minValue);
			return Math.Max(1L, (long)(0.03f * (float)num / 100f));
		}

		private static bool s_UseYSign;

		private const float kDragSensitivity = 0.03f;
	}
}
