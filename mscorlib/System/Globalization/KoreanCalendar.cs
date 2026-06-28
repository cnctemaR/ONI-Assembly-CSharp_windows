using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[MonoTODO("Serialization format not compatible with .NET")]
	[Serializable]
	public class KoreanCalendar : Calendar
	{
		public KoreanCalendar()
		{
			this.M_AbbrEraNames = new string[] { "K.C.E." };
			this.M_EraNames = new string[] { "Korean Current Era" };
			if (this.twoDigitYearMax == 99)
			{
				this.twoDigitYearMax = 4362;
			}
		}

		static KoreanCalendar()
		{
			KoreanCalendar.M_EraHandler.appendEra(1, CCGregorianCalendar.fixed_from_dmy(1, 1, -2332));
		}

		public override int[] Eras
		{
			get
			{
				return (int[])KoreanCalendar.M_EraHandler.Eras.Clone();
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
				era = 1;
			}
			if (!KoreanCalendar.M_EraHandler.ValidEra(era))
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal int M_CheckYEG(int year, ref int era)
		{
			this.M_CheckEra(ref era);
			return KoreanCalendar.M_EraHandler.GregorianYear(year, era);
		}

		internal override void M_CheckYE(int year, ref int era)
		{
			this.M_CheckYEG(year, ref era);
		}

		internal int M_CheckYMEG(int year, int month, ref int era)
		{
			int num = this.M_CheckYEG(year, ref era);
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", "Month must be between one and twelve.");
			}
			return num;
		}

		internal int M_CheckYMDEG(int year, int month, int day, ref int era)
		{
			int num = this.M_CheckYMEG(year, month, ref era);
			base.M_ArgumentInRange("day", day, 1, this.GetDaysInMonth(year, month, era));
			return num;
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			return CCGregorianCalendar.AddMonths(time, months);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			return CCGregorianCalendar.AddYears(time, years);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			return CCGregorianCalendar.GetDayOfMonth(time);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(num);
		}

		public override int GetDayOfYear(DateTime time)
		{
			return CCGregorianCalendar.GetDayOfYear(time);
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			int num = this.M_CheckYMEG(year, month, ref era);
			return CCGregorianCalendar.GetDaysInMonth(num, month);
		}

		public override int GetDaysInYear(int year, int era)
		{
			int num = this.M_CheckYEG(year, ref era);
			return CCGregorianCalendar.GetDaysInYear(num);
		}

		public override int GetEra(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			KoreanCalendar.M_EraHandler.EraYear(out num2, num);
			return num2;
		}

		[ComVisible(false)]
		public override int GetLeapMonth(int year, int era)
		{
			return 0;
		}

		public override int GetMonth(DateTime time)
		{
			return CCGregorianCalendar.GetMonth(time);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			this.M_CheckYEG(year, ref era);
			return 12;
		}

		[ComVisible(false)]
		public override int GetWeekOfYear(DateTime time, CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
		{
			return base.GetWeekOfYear(time, rule, firstDayOfWeek);
		}

		public override int GetYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			return KoreanCalendar.M_EraHandler.EraYear(out num2, num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			int num = this.M_CheckYMDEG(year, month, day, ref era);
			return CCGregorianCalendar.IsLeapDay(num, month, day);
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			this.M_CheckYMEG(year, month, ref era);
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			int num = this.M_CheckYEG(year, ref era);
			return CCGregorianCalendar.is_leap_year(num);
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			int num = this.M_CheckYMDEG(year, month, day, ref era);
			base.M_CheckHMSM(hour, minute, second, millisecond);
			return CCGregorianCalendar.ToDateTime(num, month, day, hour, minute, second, millisecond);
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
				return KoreanCalendar.KoreanMin;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return KoreanCalendar.KoreanMax;
			}
		}

		public const int KoreanEra = 1;

		internal static readonly CCGregorianEraHandler M_EraHandler = new CCGregorianEraHandler();

		private static DateTime KoreanMin = new DateTime(1, 1, 1, 0, 0, 0);

		private static DateTime KoreanMax = new DateTime(9999, 12, 31, 11, 59, 59);
	}
}
