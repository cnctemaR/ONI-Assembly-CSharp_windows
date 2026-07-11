using System;
using System.IO;

namespace System.Globalization
{
	[MonoTODO("Serialization format not compatible with .NET")]
	[Serializable]
	public class UmAlQuraCalendar : Calendar
	{
		public UmAlQuraCalendar()
		{
			this.M_AbbrEraNames = new string[] { "A.H." };
			this.M_EraNames = new string[] { "Anno Hegirae" };
			if (this.twoDigitYearMax == 99)
			{
				this.twoDigitYearMax = 1451;
			}
		}

		public override int[] Eras
		{
			get
			{
				return new int[] { 1 };
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

		internal virtual int AddHijriDate
		{
			get
			{
				return this.M_AddHijriDate;
			}
			set
			{
				base.CheckReadOnly();
				if (value < -3 && value > 3)
				{
					throw new ArgumentOutOfRangeException("AddHijriDate", "Value should be between -3 and 3.");
				}
				this.M_AddHijriDate = value;
			}
		}

		internal void M_CheckFixedHijri(string param, int rdHijri)
		{
			if (rdHijri < UmAlQuraCalendar.M_MinFixed || rdHijri > UmAlQuraCalendar.M_MaxFixed - this.AddHijriDate)
			{
				StringWriter stringWriter = new StringWriter();
				int num;
				int num2;
				int num3;
				CCHijriCalendar.dmy_from_fixed(out num, out num2, out num3, UmAlQuraCalendar.M_MaxFixed - this.AddHijriDate);
				if (this.AddHijriDate != 0)
				{
					stringWriter.Write("This HijriCalendar (AddHijriDate {0}) allows dates from 1. 1. 1 to {1}. {2}. {3}.", new object[] { this.AddHijriDate, num, num2, num3 });
				}
				else
				{
					stringWriter.Write("HijriCalendar allows dates from 1.1.1 to {0}.{1}.{2}.", num, num2, num3);
				}
				throw new ArgumentOutOfRangeException(param, stringWriter.ToString());
			}
		}

		internal void M_CheckDateTime(DateTime time)
		{
			int num = CCFixed.FromDateTime(time) - this.AddHijriDate;
			this.M_CheckFixedHijri("time", num);
		}

		internal int M_FromDateTime(DateTime time)
		{
			return CCFixed.FromDateTime(time) - this.AddHijriDate;
		}

		internal DateTime M_ToDateTime(int rd)
		{
			return CCFixed.ToDateTime(rd + this.AddHijriDate);
		}

		internal DateTime M_ToDateTime(int date, int hour, int minute, int second, int milliseconds)
		{
			return CCFixed.ToDateTime(date + this.AddHijriDate, hour, minute, second, (double)milliseconds);
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = 1;
			}
			if (era != 1)
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			this.M_CheckEra(ref era);
			base.M_ArgumentInRange("year", year, 1, 9666);
		}

		internal void M_CheckYME(int year, int month, ref int era)
		{
			this.M_CheckYE(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
			if (year == 9666)
			{
				int num = CCHijriCalendar.fixed_from_dmy(1, month, year);
				this.M_CheckFixedHijri("month", num);
			}
		}

		internal void M_CheckYMDE(int year, int month, int day, ref int era)
		{
			this.M_CheckYME(year, month, ref era);
			base.M_ArgumentInRange("day", day, 1, this.GetDaysInMonth(year, month, 1));
			if (year == 9666)
			{
				int num = CCHijriCalendar.fixed_from_dmy(day, month, year);
				this.M_CheckFixedHijri("day", num);
			}
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			int num = this.M_FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCHijriCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num3 += months;
			num4 += CCMath.div_mod(out num3, num3, 12);
			num = CCHijriCalendar.fixed_from_dmy(num2, num3, num4);
			this.M_CheckFixedHijri("time", num);
			return this.M_ToDateTime(num).Add(time.TimeOfDay);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			int num = this.M_FromDateTime(time);
			int num2;
			int num3;
			int num4;
			CCHijriCalendar.dmy_from_fixed(out num2, out num3, out num4, num);
			num4 += years;
			num = CCHijriCalendar.fixed_from_dmy(num2, num3, num4);
			this.M_CheckFixedHijri("time", num);
			return this.M_ToDateTime(num).Add(time.TimeOfDay);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			int num = this.M_FromDateTime(time);
			this.M_CheckFixedHijri("time", num);
			return CCHijriCalendar.day_from_fixed(num);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			int num = this.M_FromDateTime(time);
			this.M_CheckFixedHijri("time", num);
			return CCFixed.day_of_week(num);
		}

		public override int GetDayOfYear(DateTime time)
		{
			int num = this.M_FromDateTime(time);
			this.M_CheckFixedHijri("time", num);
			int num2 = CCHijriCalendar.year_from_fixed(num);
			int num3 = CCHijriCalendar.fixed_from_dmy(1, 1, num2);
			return num - num3 + 1;
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			int num = CCHijriCalendar.fixed_from_dmy(1, month, year);
			int num2 = CCHijriCalendar.fixed_from_dmy(1, month + 1, year);
			return num2 - num;
		}

		public override int GetDaysInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			int num = CCHijriCalendar.fixed_from_dmy(1, 1, year);
			int num2 = CCHijriCalendar.fixed_from_dmy(1, 1, year + 1);
			return num2 - num;
		}

		public override int GetEra(DateTime time)
		{
			this.M_CheckDateTime(time);
			return 1;
		}

		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			int num = this.M_FromDateTime(time);
			this.M_CheckFixedHijri("time", num);
			return CCHijriCalendar.month_from_fixed(num);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return 12;
		}

		public override int GetYear(DateTime time)
		{
			int num = this.M_FromDateTime(time);
			this.M_CheckFixedHijri("time", num);
			return CCHijriCalendar.year_from_fixed(num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			return this.IsLeapYear(year) && month == 12 && day == 30;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			this.M_CheckYME(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return CCHijriCalendar.is_leap_year(year);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			this.M_CheckYMDE(year, month, day, ref era);
			base.M_CheckHMSM(hour, minute, second, millisecond);
			int num = CCHijriCalendar.fixed_from_dmy(day, month, year);
			return this.M_ToDateTime(num, hour, minute, second, millisecond);
		}

		public override int ToFourDigitYear(int year)
		{
			return base.ToFourDigitYear(year);
		}

		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.LunarCalendar;
			}
		}

		public override DateTime MinSupportedDateTime
		{
			get
			{
				return UmAlQuraCalendar.Min;
			}
		}

		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return UmAlQuraCalendar.Max;
			}
		}

		public const int UmAlQuraEra = 1;

		internal static readonly int M_MinFixed = CCHijriCalendar.fixed_from_dmy(1, 1, 1);

		internal static readonly int M_MaxFixed = CCGregorianCalendar.fixed_from_dmy(31, 12, 9999);

		internal int M_AddHijriDate;

		private static DateTime Min = new DateTime(622, 7, 18, 0, 0, 0);

		private static DateTime Max = new DateTime(9999, 12, 31, 11, 59, 59);
	}
}
