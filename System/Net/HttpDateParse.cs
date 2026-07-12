using System;
using System.Globalization;

namespace System.Net
{
	internal static class HttpDateParse
	{
		private static char MAKE_UPPER(char c)
		{
			return char.ToUpper(c, CultureInfo.InvariantCulture);
		}

		private static int MapDayMonthToDword(char[] lpszDay, int index)
		{
			switch (HttpDateParse.MAKE_UPPER(lpszDay[index]))
			{
			case 'A':
			{
				char c = HttpDateParse.MAKE_UPPER(lpszDay[index + 1]);
				if (c == 'P')
				{
					return 4;
				}
				if (c != 'U')
				{
					return -999;
				}
				return 8;
			}
			case 'D':
				return 12;
			case 'F':
			{
				char c = HttpDateParse.MAKE_UPPER(lpszDay[index + 1]);
				if (c == 'E')
				{
					return 2;
				}
				if (c == 'R')
				{
					return 5;
				}
				return -999;
			}
			case 'G':
				return -1000;
			case 'J':
			{
				char c = HttpDateParse.MAKE_UPPER(lpszDay[index + 1]);
				if (c != 'A')
				{
					if (c == 'U')
					{
						char c2 = HttpDateParse.MAKE_UPPER(lpszDay[index + 2]);
						if (c2 == 'L')
						{
							return 7;
						}
						if (c2 == 'N')
						{
							return 6;
						}
					}
					return -999;
				}
				return 1;
			}
			case 'M':
			{
				char c = HttpDateParse.MAKE_UPPER(lpszDay[index + 1]);
				if (c != 'A')
				{
					if (c == 'O')
					{
						return 1;
					}
				}
				else
				{
					char c2 = HttpDateParse.MAKE_UPPER(lpszDay[index + 2]);
					if (c2 == 'R')
					{
						return 3;
					}
					if (c2 == 'Y')
					{
						return 5;
					}
				}
				return -999;
			}
			case 'N':
				return 11;
			case 'O':
				return 10;
			case 'S':
			{
				char c = HttpDateParse.MAKE_UPPER(lpszDay[index + 1]);
				if (c == 'A')
				{
					return 6;
				}
				if (c == 'E')
				{
					return 9;
				}
				if (c != 'U')
				{
					return -999;
				}
				return 0;
			}
			case 'T':
			{
				char c = HttpDateParse.MAKE_UPPER(lpszDay[index + 1]);
				if (c == 'H')
				{
					return 4;
				}
				if (c == 'U')
				{
					return 2;
				}
				return -999;
			}
			case 'U':
				return -1000;
			case 'W':
				return 3;
			}
			return -999;
		}

		public static bool ParseHttpDate(string DateString, out DateTime dtOut)
		{
			int num = 0;
			int num2 = 0;
			int num3 = -1;
			bool flag = false;
			int[] array = new int[8];
			bool flag2 = true;
			char[] array2 = DateString.ToCharArray();
			dtOut = default(DateTime);
			while (num < DateString.Length && num2 < 8)
			{
				if (array2[num] >= '0' && array2[num] <= '9')
				{
					array[num2] = 0;
					do
					{
						array[num2] *= 10;
						array[num2] += (int)(array2[num] - '0');
						num++;
					}
					while (num < DateString.Length && array2[num] >= '0' && array2[num] <= '9');
					num2++;
				}
				else if ((array2[num] >= 'A' && array2[num] <= 'Z') || (array2[num] >= 'a' && array2[num] <= 'z'))
				{
					array[num2] = HttpDateParse.MapDayMonthToDword(array2, num);
					num3 = num2;
					if (array[num2] == -999 && (!flag || num2 != 6))
					{
						flag2 = false;
						return flag2;
					}
					if (num2 == 1)
					{
						flag = true;
					}
					do
					{
						num++;
					}
					while (num < DateString.Length && ((array2[num] >= 'A' && array2[num] <= 'Z') || (array2[num] >= 'a' && array2[num] <= 'z')));
					num2++;
				}
				else
				{
					num++;
				}
			}
			int num4 = 0;
			int num5;
			int num6;
			int num7;
			int num8;
			int num9;
			int num10;
			if (flag)
			{
				num5 = array[2];
				num6 = array[1];
				num7 = array[3];
				num8 = array[4];
				num9 = array[5];
				if (num3 != 6)
				{
					num10 = array[6];
				}
				else
				{
					num10 = array[7];
				}
			}
			else
			{
				num5 = array[1];
				num6 = array[2];
				num10 = array[3];
				num7 = array[4];
				num8 = array[5];
				num9 = array[6];
			}
			if (num10 < 100)
			{
				num10 += ((num10 < 80) ? 2000 : 1900);
			}
			if (num2 < 4 || num5 > 31 || num7 > 23 || num8 > 59 || num9 > 59)
			{
				return false;
			}
			dtOut = new DateTime(num10, num6, num5, num7, num8, num9, num4);
			if (num3 == 6)
			{
				dtOut = dtOut.ToUniversalTime();
			}
			if (num2 > 7 && array[7] != -1000)
			{
				double num11 = (double)array[7];
				dtOut.AddHours(num11);
			}
			dtOut = dtOut.ToLocalTime();
			return flag2;
		}

		private const int BASE_DEC = 10;

		private const int DATE_INDEX_DAY_OF_WEEK = 0;

		private const int DATE_1123_INDEX_DAY = 1;

		private const int DATE_1123_INDEX_MONTH = 2;

		private const int DATE_1123_INDEX_YEAR = 3;

		private const int DATE_1123_INDEX_HRS = 4;

		private const int DATE_1123_INDEX_MINS = 5;

		private const int DATE_1123_INDEX_SECS = 6;

		private const int DATE_ANSI_INDEX_MONTH = 1;

		private const int DATE_ANSI_INDEX_DAY = 2;

		private const int DATE_ANSI_INDEX_HRS = 3;

		private const int DATE_ANSI_INDEX_MINS = 4;

		private const int DATE_ANSI_INDEX_SECS = 5;

		private const int DATE_ANSI_INDEX_YEAR = 6;

		private const int DATE_INDEX_TZ = 7;

		private const int DATE_INDEX_LAST = 7;

		private const int MAX_FIELD_DATE_ENTRIES = 8;

		private const int DATE_TOKEN_JANUARY = 1;

		private const int DATE_TOKEN_FEBRUARY = 2;

		private const int DATE_TOKEN_Microsoft = 3;

		private const int DATE_TOKEN_APRIL = 4;

		private const int DATE_TOKEN_MAY = 5;

		private const int DATE_TOKEN_JUNE = 6;

		private const int DATE_TOKEN_JULY = 7;

		private const int DATE_TOKEN_AUGUST = 8;

		private const int DATE_TOKEN_SEPTEMBER = 9;

		private const int DATE_TOKEN_OCTOBER = 10;

		private const int DATE_TOKEN_NOVEMBER = 11;

		private const int DATE_TOKEN_DECEMBER = 12;

		private const int DATE_TOKEN_LAST_MONTH = 13;

		private const int DATE_TOKEN_SUNDAY = 0;

		private const int DATE_TOKEN_MONDAY = 1;

		private const int DATE_TOKEN_TUESDAY = 2;

		private const int DATE_TOKEN_WEDNESDAY = 3;

		private const int DATE_TOKEN_THURSDAY = 4;

		private const int DATE_TOKEN_FRIDAY = 5;

		private const int DATE_TOKEN_SATURDAY = 6;

		private const int DATE_TOKEN_LAST_DAY = 7;

		private const int DATE_TOKEN_GMT = -1000;

		private const int DATE_TOKEN_LAST = -1000;

		private const int DATE_TOKEN_ERROR = -999;
	}
}
