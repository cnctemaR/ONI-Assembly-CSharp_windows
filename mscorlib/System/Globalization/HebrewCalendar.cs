using System;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with.NET")]
	[Serializable]
	public class HebrewCalendar : Calendar
	{
		public HebrewCalendar()
		{
			this.M_AbbrEraNames = new string[] { "A.M." };
			this.M_EraNames = new string[] { "Anno Mundi" };
			if (this.twoDigitYearMax == 99)
			{
				this.twoDigitYearMax = 5790;
			}
		}

		internal override int M_MaxYear
		{
			get
			{
				return 6000;
			}
		}

		public override int[] Eras
		{
			get
			{
				return new int[] { HebrewCalendar.HebrewEra };
			}
		}

		public override int TwoDigitYearMax
		{
			get
			{
				return this.twoDigitYearMax;
			}
			set
			{
				base.CheckReadOnly();
				base.M_ArgumentInRange("value", value, 5343, this.M_MaxYear);
				this.twoDigitYearMax = value;
			}
		}

		internal void M_CheckDateTime(DateTime time)
		{
			if (time.Ticks < 499147488000000000L || time.Ticks > 706783967999999999L)
			{
				throw new ArgumentOutOfRangeException("time", "Only hebrew years between 5343 and 6000, inclusive, are supported.");
			}
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = HebrewCalendar.HebrewEra;
			}
			if (era != HebrewCalendar.HebrewEra)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			this.M_CheckEra(ref era);
			if (year < 5343 || year > this.M_MaxYear)
			{
				throw new ArgumentOutOfRangeException("year", "Only hebrew years between 5343 and 6000, inclusive, are supported.");
			}
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			this.M_CheckYE(year, ref era);
			int num = CCHebrewCalendar.last_month_of_year(year);
			if (month < 1 || month > num)
			{
				StringWriter stringWriter = new StringWriter();
				stringWriter.Write("Month must be between 1 and {0}.", num);
				throw new ArgumentOutOfRangeException("month", stringWriter.ToString());
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			this.M_CheckYME(year, month, ref era);
			base.M_ArgumentInRange("day", day, 1, this.GetDaysInMonth(year, month, era));
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			DateTime dateTime;
			if (months == 0)
			{
				dateTime = time;
			}
			else
			{
				int num = CCFixed.FromDateTime(time);
				int num2;
				int num3;
				int num4;
				CCHebrewCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
				num3 = this.M_Month(num3, num4);
				if (months < 0)
				{
					while (months < 0)
					{
						if (num3 + months > 0)
						{
							num3 += months;
							months = 0;
						}
						else
						{
							months += num3;
							num4--;
							num3 = this.GetMonthsInYear(num4);
						}
					}
				}
				else
				{
					while (months > 0)
					{
						int monthsInYear = this.GetMonthsInYear(num4);
						if (num3 + months <= monthsInYear)
						{
							num3 += months;
							months = 0;
						}
						else
						{
							months -= monthsInYear - num3 + 1;
							num3 = 1;
							num4++;
						}
					}
				}
				dateTime = this.ToDateTime(num4, num3, num2, 0, 0, 0, 0).Add(time.TimeOfDay);
			}
			this.M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCHebrewCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num4 += years;
			num = CCHebrewCalendar.fixed_from_dmy(num2, num3, num4);
			DateTime dateTime = CCFixed.ToDateTime(num).Add(time.TimeOfDay);
			this.M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override int GetDayOfMonth(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			return CCHebrewCalendar.day_from_fixed(num);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(num);
		}

		public override int GetDayOfYear(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			int num2 = CCHebrewCalendar.year_from_fixed(num);
			int num3 = CCHebrewCalendar.fixed_from_dmy(1, 7, num2);
			return num - num3 + 1;
		}

		internal int M_CCMonth(int month, int year)
		{
			if (month <= 6)
			{
				return 6 + month;
			}
			int num = CCHebrewCalendar.last_month_of_year(year);
			if (num == 12)
			{
				return month - 6;
			}
			return (month > 7) ? (month - 7) : (6 + month);
		}

		internal int M_Month(int ccmonth, int year)
		{
			if (ccmonth >= 7)
			{
				return ccmonth - 6;
			}
			int num = CCHebrewCalendar.last_month_of_year(year);
			return ccmonth + ((num != 12) ? 7 : 6);
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			int num = this.M_CCMonth(month, year);
			return CCHebrewCalendar.last_day_of_month(num, year);
		}

		public override int GetDaysInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			int num = CCHebrewCalendar.fixed_from_dmy(1, 7, year);
			int num2 = CCHebrewCalendar.fixed_from_dmy(1, 7, year + 1);
			return num2 - num;
		}

		public override int GetEra(DateTime time)
		{
			this.M_CheckDateTime(time);
			return HebrewCalendar.HebrewEra;
		}

		public override int GetLeapMonth(int year, int era)
		{
			return (!this.IsLeapMonth(year, 7, era)) ? 0 : 7;
		}

		public override int GetMonth(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			CCHebrewCalendar.my_from_fixed(out num2, out num3, num);
			return this.M_Month(num2, num3);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return CCHebrewCalendar.last_month_of_year(year);
		}

		public override int GetYear(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			return CCHebrewCalendar.year_from_fixed(num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			return this.IsLeapYear(year) && (month == 7 || (month == 6 && day == 30));
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			return this.IsLeapYear(year) && month == 7;
		}

		public override bool IsLeapYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return CCHebrewCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			base.M_CheckHMSM(hour, minute, second, millisecond);
			int num = this.M_CCMonth(month, year);
			int num2 = CCHebrewCalendar.fixed_from_dmy(day, num, year);
			return CCFixed.ToDateTime(num2, hour, minute, second, (double)millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			base.M_ArgumentInRange("year", year, 0, this.M_MaxYear - 1);
			int num = this.twoDigitYearMax % 100;
			int num2 = this.twoDigitYearMax - num;
			if (year >= 100)
			{
				return year;
			}
			if (year <= num)
			{
				return num2 + year;
			}
			return num2 + year - 100;
		}

		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.LunisolarCalendar;
			}
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return HebrewCalendar.Min;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return HebrewCalendar.Max;
			}
		}

		internal const long M_MinTicks = 499147488000000000L;

		internal const long M_MaxTicks = 706783967999999999L;

		internal const int M_MinYear = 5343;

		public static readonly int HebrewEra = 1;

		private static DateTime Min = new DateTime(1583, 1, 1, 0, 0, 0);

		private static DateTime Max = new DateTime(2239, 9, 29, 11, 59, 59);
	}
}
