using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public abstract class EastAsianLunisolarCalendar : Calendar
	{
		internal EastAsianLunisolarCalendar(CCEastAsianLunisolarEraHandler eraHandler)
		{
			this.M_EraHandler = eraHandler;
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
			this.M_EraHandler.CheckDateTime(time);
		}

		internal virtual int ActualCurrentEra
		{
			get
			{
				return 1;
			}
		}

		internal void M_CheckEra(ref int era)
		{
			if (era == 0)
			{
				era = this.ActualCurrentEra;
			}
			if (!this.M_EraHandler.ValidEra(era))
			{
				throw new ArgumentException("Era value was not valid.");
			}
		}

		internal int M_CheckYEG(int year, ref int era)
		{
			this.M_CheckEra(ref era);
			return this.M_EraHandler.GregorianYear(year, era);
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

		[MonoTODO]
		public override DateTime AddMonths(DateTime time, int months)
		{
			DateTime dateTime = CCEastAsianLunisolarCalendar.AddMonths(time, months);
			this.M_CheckDateTime(dateTime);
			return dateTime;
		}

		[MonoTODO]
		public override DateTime AddYears(DateTime time, int years)
		{
			DateTime dateTime = CCEastAsianLunisolarCalendar.AddYears(time, years);
			this.M_CheckDateTime(dateTime);
			return dateTime;
		}

		[MonoTODO]
		public override int GetDayOfMonth(DateTime time)
		{
			this.M_CheckDateTime(time);
			return CCEastAsianLunisolarCalendar.GetDayOfMonth(time);
		}

		[MonoTODO]
		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			this.M_CheckDateTime(time);
			int num = CCFixed.FromDateTime(time);
			return CCFixed.day_of_week(num);
		}

		[MonoTODO]
		public override int GetDayOfYear(DateTime time)
		{
			this.M_CheckDateTime(time);
			return CCEastAsianLunisolarCalendar.GetDayOfYear(time);
		}

		[MonoTODO]
		public override int GetDaysInMonth(int year, int month, int era)
		{
			int num = this.M_CheckYMEG(year, month, ref era);
			return CCEastAsianLunisolarCalendar.GetDaysInMonth(num, month);
		}

		[MonoTODO]
		public override int GetDaysInYear(int year, int era)
		{
			int num = this.M_CheckYEG(year, ref era);
			return CCEastAsianLunisolarCalendar.GetDaysInYear(num);
		}

		[MonoTODO]
		public override int GetLeapMonth(int year, int era)
		{
			return base.GetLeapMonth(year, era);
		}

		[MonoTODO]
		public override int GetMonth(DateTime time)
		{
			this.M_CheckDateTime(time);
			return CCEastAsianLunisolarCalendar.GetMonth(time);
		}

		[MonoTODO]
		public override int GetMonthsInYear(int year, int era)
		{
			this.M_CheckYE(year, ref era);
			return (!this.IsLeapYear(year, era)) ? 12 : 13;
		}

		public override int GetYear(DateTime time)
		{
			int num = CCFixed.FromDateTime(time);
			int num2;
			return this.M_EraHandler.EraYear(out num2, num);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			int num = this.M_CheckYMDEG(year, month, day, ref era);
			return CCEastAsianLunisolarCalendar.IsLeapMonth(num, month);
		}

		[MonoTODO]
		public override bool IsLeapMonth(int year, int month, int era)
		{
			int num = this.M_CheckYMEG(year, month, ref era);
			return CCEastAsianLunisolarCalendar.IsLeapMonth(num, month);
		}

		public override bool IsLeapYear(int year, int era)
		{
			int num = this.M_CheckYEG(year, ref era);
			return CCEastAsianLunisolarCalendar.IsLeapYear(num);
		}

		[MonoTODO]
		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			int num = this.M_CheckYMDEG(year, month, day, ref era);
			base.M_CheckHMSM(hour, minute, second, millisecond);
			return CCGregorianCalendar.ToDateTime(num, month, day, hour, minute, second, millisecond);
		}

		[MonoTODO]
		public override int ToFourDigitYear(int year)
		{
			if (year < 0)
			{
				throw new ArgumentOutOfRangeException("year", "Non-negative number required.");
			}
			int num = 0;
			this.M_CheckYE(year, ref num);
			return year;
		}

		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.LunisolarCalendar;
			}
		}

		public int GetCelestialStem(int sexagenaryYear)
		{
			if (sexagenaryYear < 1 || 60 < sexagenaryYear)
			{
				throw new ArgumentOutOfRangeException("sexagendaryYear is less than 0 or greater than 60");
			}
			return (sexagenaryYear - 1) % 10 + 1;
		}

		public virtual int GetSexagenaryYear(DateTime time)
		{
			return (this.GetYear(time) - 1900) % 60;
		}

		public int GetTerrestrialBranch(int sexagenaryYear)
		{
			if (sexagenaryYear < 1 || 60 < sexagenaryYear)
			{
				throw new ArgumentOutOfRangeException("sexagendaryYear is less than 0 or greater than 60");
			}
			return (sexagenaryYear - 1) % 12 + 1;
		}

		internal readonly CCEastAsianLunisolarEraHandler M_EraHandler;
	}
}
