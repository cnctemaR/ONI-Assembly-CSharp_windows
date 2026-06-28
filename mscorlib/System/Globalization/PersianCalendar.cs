using System;

namespace System.Globalization
{
	[Serializable]
	public class PersianCalendar : Calendar
	{
		public PersianCalendar()
		{
			this.M_AbbrEraNames = new string[] { "A.P." };
			this.M_EraNames = new string[] { "Anno Persico" };
			if (this.twoDigitYearMax == 99)
			{
				this.twoDigitYearMax = 1410;
			}
		}

		public override int[] Eras
		{
			get
			{
				return new int[] { PersianCalendar.PersianEra };
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
				base.M_ArgumentInRange("value", value, 100, this.M_MaxYear);
				this.twoDigitYearMax = value;
			}
		}

		internal void M_CheckDateTime(DateTime time)
		{
			if (time.Ticks < 196036416000000000L)
			{
				throw new ArgumentOutOfRangeException("time", "Only positive Persian years are supported.");
			}
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = PersianCalendar.PersianEra;
			}
			if (era != PersianCalendar.PersianEra)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			this.M_CheckEra(ref era);
			if (year < 1 || year > this.M_MaxYear)
			{
				throw new ArgumentOutOfRangeException("year", "Only Persian years between 1 and 9378, inclusive, are supported.");
			}
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			this.M_CheckYE(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
			if (year == this.M_MaxYear && month > 10)
			{
				throw new ArgumentOutOfRangeException("month", "Months in year 9378 must be between one and ten.");
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			this.M_CheckYME(year, month, ref era);
			base.M_ArgumentInRange("day", day, 1, this.GetDaysInMonth(year, month, era));
			if (year == this.M_MaxYear && month == 10 && day > 10)
			{
				throw new ArgumentOutOfRangeException("day", "Days in month 10 of year 9378 must be between one and ten.");
			}
		}

		internal int fixed_from_dmy(int day, int month, int year)
		{
			int num = 226894;
			num += 365 * (year - 1);
			num += (8 * year + 21) / 33;
			if (month <= 7)
			{
				num += 31 * (month - 1);
			}
			else
			{
				num += 30 * (month - 1) + 6;
			}
			return num + day;
		}

		internal int year_from_fixed(int date)
		{
			return (33 * (date - 226895) + 3) / 12053 + 1;
		}

		internal void my_from_fixed(out int month, out int year, int date)
		{
			year = this.year_from_fixed(date);
			int num = date - this.fixed_from_dmy(1, 1, year);
			if (num < 216)
			{
				month = num / 31 + 1;
			}
			else
			{
				month = (num - 6) / 30 + 1;
			}
		}

		internal void dmy_from_fixed(out int day, out int month, out int year, int date)
		{
			year = this.year_from_fixed(date);
			day = date - this.fixed_from_dmy(1, 1, year);
			if (day < 216)
			{
				month = day / 31 + 1;
				day = day % 31 + 1;
			}
			else
			{
				month = (day - 6) / 30 + 1;
				day = (day - 6) % 30 + 1;
			}
		}

		internal bool is_leap_year(int year)
		{
			return (25 * year + 11) % 33 < 8;
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			this.dmy_from_fixed(out num2, out num3, out num4, num);
			num3 += months;
			num4 += CCMath.div_mod(out num3, num3, 12);
			num = this.fixed_from_dmy(num2, num3, num4);
			DateTime dateTime = CCFixed.ToDateTime(num).Add(time.TimeOfDay);
			this.M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			this.dmy_from_fixed(out num2, out num3, out num4, num);
			num4 += years;
			num = this.fixed_from_dmy(num2, num3, num4);
			DateTime dateTime = CCFixed.ToDateTime(num).Add(time.TimeOfDay);
			this.M_CheckDateTime(dateTime);
			return dateTime;
		}

		public override int GetDayOfMonth(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			this.dmy_from_fixed(out num2, out num3, out num4, num);
			return num2;
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
			int num2 = this.year_from_fixed(num);
			int num3 = this.fixed_from_dmy(1, 1, num2);
			return num - num3 + 1;
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			if (month <= 6)
			{
				return 31;
			}
			if (month == 12 && !this.is_leap_year(year))
			{
				return 29;
			}
			return 30;
		}

		public override int GetDaysInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return (!this.is_leap_year(year)) ? 365 : 366;
		}

		public override int GetEra(DateTime time)
		{
			this.M_CheckDateTime(time);
			return PersianCalendar.PersianEra;
		}

		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			this.my_from_fixed(out num2, out num3, num);
			return num2;
		}

		public override int GetMonthsInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			return this.year_from_fixed(num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			return this.is_leap_year(year) && month == 12 && day == 30;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return this.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			base.M_CheckHMSM(hour, minute, second, millisecond);
			int num = this.fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(num, hour, minute, second, (double)millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			base.M_ArgumentInRange("year", year, 0, 99);
			int num = this.twoDigitYearMax % 100;
			int num2 = this.twoDigitYearMax - num;
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
				return CalendarAlgorithmType.SolarCalendar;
			}
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return PersianCalendar.PersianMin;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return PersianCalendar.PersianMax;
			}
		}

		internal const long M_MinTicks = 196036416000000000L;

		internal const int M_MinYear = 1;

		internal const int epoch = 226895;

		public static readonly int PersianEra = 1;

		private static DateTime PersianMin = new DateTime(622, 3, 21, 0, 0, 0);

		private static DateTime PersianMax = new DateTime(9999, 12, 31, 11, 59, 59);
	}
}
