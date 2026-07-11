using System;

namespace System.Globalization
{
	internal class CCGregorianCalendar
	{
		public static bool is_leap_year(int year)
		{
			if (CCMath.mod(year, 4) != 0)
			{
				return false;
			}
			int num = CCMath.mod(year, 400);
			return num != 100 && num != 200 && num != 300;
		}

		public static int fixed_from_dmy(int day, int month, int year)
		{
			int num = 0;
			num += 365 * (year - 1);
			num += CCMath.div(year - 1, 4);
			num -= CCMath.div(year - 1, 100);
			num += CCMath.div(year - 1, 400);
			num += CCMath.div(367 * month - 362, 12);
			if (month > 2)
			{
				num += ((!CCGregorianCalendar.is_leap_year(year)) ? (-2) : (-1));
			}
			return num + day;
		}

		public static int year_from_fixed(int date)
		{
			int num = date - 1;
			int num2 = CCMath.div_mod(out num, num, 146097);
			int num3 = CCMath.div_mod(out num, num, 36524);
			int num4 = CCMath.div_mod(out num, num, 1461);
			int num5 = CCMath.div(num, 365);
			int num6 = 400 * num2 + 100 * num3 + 4 * num4 + num5;
			return (num3 != 4 && num5 != 4) ? (num6 + 1) : num6;
		}

		public static void my_from_fixed(out int month, out int year, int date)
		{
			year = CCGregorianCalendar.year_from_fixed(date);
			int num = date - CCGregorianCalendar.fixed_from_dmy(1, 1, year);
			int num2;
			if (date < CCGregorianCalendar.fixed_from_dmy(1, 3, year))
			{
				num2 = 0;
			}
			else if (CCGregorianCalendar.is_leap_year(year))
			{
				num2 = 1;
			}
			else
			{
				num2 = 2;
			}
			month = CCMath.div(12 * (num + num2) + 373, 367);
		}

		public static void dmy_from_fixed(out int day, out int month, out int year, int date)
		{
			CCGregorianCalendar.my_from_fixed(out month, out year, date);
			day = date - CCGregorianCalendar.fixed_from_dmy(1, month, year) + 1;
		}

		public static int month_from_fixed(int date)
		{
			int num;
			int num2;
			CCGregorianCalendar.my_from_fixed(out num, out num2, date);
			return num;
		}

		public static int day_from_fixed(int date)
		{
			int num;
			int num2;
			int num3;
			CCGregorianCalendar.dmy_from_fixed(out num, out num2, out num3, date);
			return num;
		}

		public static int date_difference(int dayA, int monthA, int yearA, int dayB, int monthB, int yearB)
		{
			return CCGregorianCalendar.fixed_from_dmy(dayB, monthB, yearB) - CCGregorianCalendar.fixed_from_dmy(dayA, monthA, yearA);
		}

		public static int day_number(int day, int month, int year)
		{
			return CCGregorianCalendar.date_difference(31, 12, year - 1, day, month, year);
		}

		public static int days_remaining(int day, int month, int year)
		{
			return CCGregorianCalendar.date_difference(day, month, year, 31, 12, year);
		}

		public static DateTime AddMonths(DateTime time, int months)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCGregorianCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num3 += months;
			num4 += CCMath.div_mod(out num3, num3, 12);
			int daysInMonth = CCGregorianCalendar.GetDaysInMonth(num4, num3);
			if (num2 > daysInMonth)
			{
				num2 = daysInMonth;
			}
			num = CCGregorianCalendar.fixed_from_dmy(num2, num3, num4);
			return CCFixed.ToDateTime(num).Add(time.TimeOfDay);
		}

		public static DateTime AddYears(DateTime time, int years)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCGregorianCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num4 += years;
			int daysInMonth = CCGregorianCalendar.GetDaysInMonth(num4, num3);
			if (num2 > daysInMonth)
			{
				num2 = daysInMonth;
			}
			num = CCGregorianCalendar.fixed_from_dmy(num2, num3, num4);
			return CCFixed.ToDateTime(num).Add(time.TimeOfDay);
		}

		public static int GetDayOfMonth(DateTime time)
		{
			return CCGregorianCalendar.day_from_fixed(CCFixed.FromDateTime(time));
		}

		public static int GetDayOfYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2 = CCGregorianCalendar.year_from_fixed(num);
			int num3 = CCGregorianCalendar.fixed_from_dmy(1, 1, num2);
			return num - num3 + 1;
		}

		public static int GetDaysInMonth(int year, int month)
		{
			int num = CCGregorianCalendar.fixed_from_dmy(1, month, year);
			int num2 = CCGregorianCalendar.fixed_from_dmy(1, month + 1, year);
			return num2 - num;
		}

		public static int GetDaysInYear(int year)
		{
			int num = CCGregorianCalendar.fixed_from_dmy(1, 1, year);
			int num2 = CCGregorianCalendar.fixed_from_dmy(1, 1, year + 1);
			return num2 - num;
		}

		public static int GetMonth(DateTime time)
		{
			return CCGregorianCalendar.month_from_fixed(CCFixed.FromDateTime(time));
		}

		public static int GetYear(DateTime time)
		{
			return CCGregorianCalendar.year_from_fixed(CCFixed.FromDateTime(time));
		}

		public static bool IsLeapDay(int year, int month, int day)
		{
			return CCGregorianCalendar.is_leap_year(year) && month == 2 && day == 29;
		}

		public static DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int milliseconds)
		{
			return CCFixed.ToDateTime(CCGregorianCalendar.fixed_from_dmy(day, month, year), hour, minute, second, (double)milliseconds);
		}

		private const int epoch = 1;

		public enum Month
		{
			january = 1,
			february,
			march,
			april,
			may,
			june,
			july,
			august,
			september,
			october,
			november,
			december
		}
	}
}
