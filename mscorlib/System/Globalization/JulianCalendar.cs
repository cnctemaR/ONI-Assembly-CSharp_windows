using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	[Serializable]
	public class JulianCalendar : Calendar
	{
		public JulianCalendar()
		{
			this.M_AbbrEraNames = new string[] { "C.E." };
			this.M_EraNames = new string[] { "Common Era" };
			if (this.twoDigitYearMax == 99)
			{
				this.twoDigitYearMax = 2029;
			}
		}

		public override int[] Eras
		{
			get
			{
				return new int[] { JulianCalendar.JulianEra };
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

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = JulianCalendar.JulianEra;
			}
			if (era != JulianCalendar.JulianEra)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			this.M_CheckEra(ref era);
			base.M_ArgumentInRange("year", year, 1, 9999);
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			this.M_CheckYE(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			this.M_CheckYME(year, month, ref era);
			base.M_ArgumentInRange("day", day, 1, this.GetDaysInMonth(year, month, era));
			if (year == 9999 && ((month == 10 && day > 19) || month > 10))
			{
				throw new ArgumentOutOfRangeException("The maximum Julian date is 19. 10. 9999.");
			}
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCJulianCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num3 += months;
			num4 += CCMath.div_mod(out num3, num3, 12);
			num = CCJulianCalendar.fixed_from_dmy(num2, num3, num4);
			return CCFixed.ToDateTime(num).Add(time.TimeOfDay);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCJulianCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num4 += years;
			num = CCJulianCalendar.fixed_from_dmy(num2, num3, num4);
			return CCFixed.ToDateTime(num).Add(time.TimeOfDay);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			return CCJulianCalendar.day_from_fixed(num);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(num);
		}

		public override int GetDayOfYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2 = CCJulianCalendar.year_from_fixed(num);
			int num3 = CCJulianCalendar.fixed_from_dmy(1, 1, num2);
			return num - num3 + 1;
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			int num = CCJulianCalendar.fixed_from_dmy(1, month, year);
			int num2 = CCJulianCalendar.fixed_from_dmy(1, month + 1, year);
			return num2 - num;
		}

		public override int GetDaysInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			int num = CCJulianCalendar.fixed_from_dmy(1, 1, year);
			int num2 = CCJulianCalendar.fixed_from_dmy(1, 1, year + 1);
			return num2 - num;
		}

		public override int GetEra(DateTime time)
		{
			return JulianCalendar.JulianEra;
		}

		[ComVisible(false)]
		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			return CCJulianCalendar.month_from_fixed(num);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			return CCJulianCalendar.year_from_fixed(num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			return this.IsLeapYear(year) && month == 2 && day == 29;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return CCJulianCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			base.M_CheckHMSM(hour, minute, second, millisecond);
			int num = CCJulianCalendar.fixed_from_dmy(day, month, year);
			return CCFixed.ToDateTime(num, hour, minute, second, (double)millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			return base.ToFourDigitYear(year);
		}

		[ComVisible(false)]
		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.SolarCalendar;
			}
		}

		[ComVisible(false)]
		public override DateTime MinSupportedDateTime
		{
			get
			{
				return JulianCalendar.JulianMin;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return JulianCalendar.JulianMax;
			}
		}

		public static readonly int JulianEra = 1;

		private static DateTime JulianMin = new DateTime(1, 1, 1, 0, 0, 0);

		private static DateTime JulianMax = new DateTime(9999, 12, 31, 11, 59, 59);
	}
}
