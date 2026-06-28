using System;

namespace FileHelpers
{
	internal static class ExHelper
	{
		public static void CheckNullOrEmpty(string val, string paramName)
		{
			if (string.IsNullOrEmpty(val))
			{
				throw new ArgumentNullException(paramName, "Value can't be null or empty");
			}
		}

		public static void CheckNullParam(string param, string paramName)
		{
			if (string.IsNullOrEmpty(param))
			{
				throw new ArgumentNullException(paramName, paramName + " can't be neither null nor empty");
			}
		}

		public static void CheckNullParam(object param, string paramName)
		{
			if (param == null)
			{
				throw new ArgumentNullException(paramName, paramName + " can't be null");
			}
		}

		public static void CheckDifferentsParams(object param1, string param1Name, object param2, string param2Name)
		{
			if (param1 == param2)
			{
				throw new ArgumentException(param1Name + " can't be the same as " + param2Name, param1Name + " and " + param2Name);
			}
		}

		public static void PositiveValue(int val)
		{
			if (val < 0)
			{
				throw new ArgumentException("The value must be greater than or equal to 0.");
			}
		}
	}
}
