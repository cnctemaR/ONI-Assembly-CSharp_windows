using System;
using System.Globalization;

namespace System.Collections.Specialized
{
	internal static class FixedStringLookup
	{
		internal static bool Contains(string[][] lookupTable, string value, bool ignoreCase)
		{
			int length = value.Length;
			if (length <= 0 || length - 1 >= lookupTable.Length)
			{
				return false;
			}
			string[] array = lookupTable[length - 1];
			return array != null && FixedStringLookup.Contains(array, value, ignoreCase);
		}

		private static bool Contains(string[] array, string value, bool ignoreCase)
		{
			int num = 0;
			int num2 = array.Length;
			int i = 0;
			while (i < value.Length)
			{
				char c;
				if (ignoreCase)
				{
					c = char.ToLower(value[i], CultureInfo.InvariantCulture);
				}
				else
				{
					c = value[i];
				}
				if (num2 - num <= 1)
				{
					if (c != array[num][i])
					{
						return false;
					}
					i++;
				}
				else
				{
					if (!FixedStringLookup.FindCharacter(array, c, i, ref num, ref num2))
					{
						return false;
					}
					i++;
				}
			}
			return true;
		}

		private static bool FindCharacter(string[] array, char value, int pos, ref int min, ref int max)
		{
			int num = min;
			while (min < max)
			{
				num = (min + max) / 2;
				char c = array[num][pos];
				if (value == c)
				{
					int num2 = num;
					while (num2 > min && array[num2 - 1][pos] == value)
					{
						num2--;
					}
					min = num2;
					int num3 = num + 1;
					while (num3 < max && array[num3][pos] == value)
					{
						num3++;
					}
					max = num3;
					return true;
				}
				if (value < c)
				{
					max = num;
				}
				else
				{
					min = num + 1;
				}
			}
			return false;
		}
	}
}
