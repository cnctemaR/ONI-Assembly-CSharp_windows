using System;
using UnityEngine;

namespace TMPro
{
	internal static class SetPropertyUtility
	{
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			bool flag;
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				flag = false;
			}
			else
			{
				currentValue = newValue;
				flag = true;
			}
			return flag;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			bool flag;
			if (currentValue.Equals(newValue))
			{
				flag = false;
			}
			else
			{
				currentValue = newValue;
				flag = true;
			}
			return flag;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			bool flag;
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				flag = false;
			}
			else
			{
				currentValue = newValue;
				flag = true;
			}
			return flag;
		}
	}
}
