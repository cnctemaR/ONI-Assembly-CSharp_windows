using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class GregorianCalendar : Calendar
	{
		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			if (this.m_type == (GregorianCalendarTypes)0)
			{
				this.m_type = GregorianCalendarTypes.Localized;
			}
			if (this.m_type < GregorianCalendarTypes.Localized || this.m_type > GregorianCalendarTypes.TransliteratedFrench)
			{
				throw new SerializationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("The deserialized value of the member \"{0}\" in the class \"{1}\" is out of range."), "type", "GregorianCalendar"));
			}
		}

		[ComVisible(false)]
		public override DateTime MinSupportedDateTime
		{
			get
			{
				return DateTime.MinValue;
			}
		}

		[ComVisible(false)]
		public override DateTime MaxSupportedDateTime
		{
			get
			{
				return DateTime.MaxValue;
			}
		}

		[ComVisible(false)]
		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.SolarCalendar;
			}
		}

		internal static Calendar GetDefaultInstance()
		{
			if (GregorianCalendar.s_defaultInstance == null)
			{
				GregorianCalendar.s_defaultInstance = new GregorianCalendar();
			}
			return GregorianCalendar.s_defaultInstance;
		}

		public GregorianCalendar()
			: this(GregorianCalendarTypes.Localized)
		{
		}

		public GregorianCalendar(GregorianCalendarTypes type)
		{
			if (type < GregorianCalendarTypes.Localized || type > GregorianCalendarTypes.TransliteratedFrench)
			{
				throw new ArgumentOutOfRangeException("type", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					GregorianCalendarTypes.Localized,
					GregorianCalendarTypes.TransliteratedFrench
				}));
			}
			this.m_type = type;
		}

		public virtual GregorianCalendarTypes CalendarType
		{
			get
			{
				return this.m_type;
			}
			set
			{
				base.VerifyWritable();
				if (value - GregorianCalendarTypes.Localized <= 1 || value - GregorianCalendarTypes.MiddleEastFrench <= 3)
				{
					this.m_type = value;
					return;
				}
				throw new ArgumentOutOfRangeException("m_type", Environment.GetResourceString("Enum value was out of legal range."));
			}
		}

		internal override int ID
		{
			get
			{
				return (int)this.m_type;
			}
		}

		internal virtual int GetDatePart(long ticks, int part)
		{
			int i = (int)(ticks / 864000000000L);
			int num = i / 146097;
			i -= num * 146097;
			int num2 = i / 36524;
			if (num2 == 4)
			{
				num2 = 3;
			}
			i -= num2 * 36524;
			int num3 = i / 1461;
			i -= num3 * 1461;
			int num4 = i / 365;
			if (num4 == 4)
			{
				num4 = 3;
			}
			if (part == 0)
			{
				return num * 400 + num2 * 100 + num3 * 4 + num4 + 1;
			}
			i -= num4 * 365;
			if (part == 1)
			{
				return i + 1;
			}
			int[] array = ((num4 == 3 && (num3 != 24 || num2 == 3)) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
			int num5 = i >> 6;
			while (i >= array[num5])
			{
				num5++;
			}
			if (part == 2)
			{
				return num5;
			}
			return i - array[num5 - 1] + 1;
		}

		internal static long GetAbsoluteDate(int year, int month, int day)
		{
			if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
			{
				int[] array = ((year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
				if (day >= 1 && day <= array[month] - array[month - 1])
				{
					int num = year - 1;
					return (long)(num * 365 + num / 4 - num / 100 + num / 400 + array[month - 1] + day - 1);
				}
			}
			throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("Year, Month, and Day parameters describe an un-representable DateTime."));
		}

		internal virtual long DateToTicks(int year, int month, int day)
		{
			return GregorianCalendar.GetAbsoluteDate(year, month, day) * 864000000000L;
		}

		public override DateTime AddMonths(DateTime time, int months)
		{
			if (months < -120000 || months > 120000)
			{
				throw new ArgumentOutOfRangeException("months", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), -120000, 120000));
			}
			int num = this.GetDatePart(time.Ticks, 0);
			int num2 = this.GetDatePart(time.Ticks, 2);
			int num3 = this.GetDatePart(time.Ticks, 3);
			int num4 = num2 - 1 + months;
			if (num4 >= 0)
			{
				num2 = num4 % 12 + 1;
				num += num4 / 12;
			}
			else
			{
				num2 = 12 + (num4 + 1) % 12;
				num += (num4 - 11) / 12;
			}
			int[] array = ((num % 4 == 0 && (num % 100 != 0 || num % 400 == 0)) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
			int num5 = array[num2] - array[num2 - 1];
			if (num3 > num5)
			{
				num3 = num5;
			}
			long num6 = this.DateToTicks(num, num2, num3) + time.Ticks % 864000000000L;
			Calendar.CheckAddResult(num6, this.MinSupportedDateTime, this.MaxSupportedDateTime);
			return new DateTime(num6);
		}

		public override DateTime AddYears(DateTime time, int years)
		{
			return this.AddMonths(time, years * 12);
		}

		public override int GetDayOfMonth(DateTime time)
		{
			return this.GetDatePart(time.Ticks, 3);
		}

		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			return (DayOfWeek)(time.Ticks / 864000000000L + 1L) % (DayOfWeek)7;
		}

		public override int GetDayOfYear(DateTime time)
		{
			return this.GetDatePart(time.Ticks, 1);
		}

		public override int GetDaysInMonth(int year, int month, int era)
		{
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[] { 1, 9999 }));
			}
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("Month must be between one and twelve."));
			}
			int[] array = ((year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) ? GregorianCalendar.DaysToMonth366 : GregorianCalendar.DaysToMonth365);
			return array[month] - array[month - 1];
		}

		public override int GetDaysInYear(int year, int era)
		{
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, 9999));
			}
			if (year % 4 != 0 || (year % 100 == 0 && year % 400 != 0))
			{
				return 365;
			}
			return 366;
		}

		public override int GetEra(DateTime time)
		{
			return 1;
		}

		public override int[] Eras
		{
			get
			{
				return new int[] { 1 };
			}
		}

		public override int GetMonth(DateTime time)
		{
			return this.GetDatePart(time.Ticks, 2);
		}

		public override int GetMonthsInYear(int year, int era)
		{
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year >= 1 && year <= 9999)
			{
				return 12;
			}
			throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, 9999));
		}

		public override int GetYear(DateTime time)
		{
			return this.GetDatePart(time.Ticks, 0);
		}

		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[] { 1, 12 }));
			}
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[] { 1, 9999 }));
			}
			if (day < 1 || day > this.GetDaysInMonth(year, month))
			{
				throw new ArgumentOutOfRangeException("day", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					1,
					this.GetDaysInMonth(year, month)
				}));
			}
			return this.IsLeapYear(year) && (month == 2 && day == 29);
		}

		[ComVisible(false)]
		public override int GetLeapMonth(int year, int era)
		{
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, 9999));
			}
			return 0;
		}

		public override bool IsLeapMonth(int year, int month, int era)
		{
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, 9999));
			}
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[] { 1, 12 }));
			}
			return false;
		}

		public override bool IsLeapYear(int year, int era)
		{
			if (era != 0 && era != 1)
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
			if (year >= 1 && year <= 9999)
			{
				return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
			}
			throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, 9999));
		}

		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			if (era == 0 || era == 1)
			{
				return new DateTime(year, month, day, hour, minute, second, millisecond);
			}
			throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
		}

		internal override bool TryToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era, out DateTime result)
		{
			if (era == 0 || era == 1)
			{
				return DateTime.TryCreate(year, month, day, hour, minute, second, millisecond, out result);
			}
			result = DateTime.MinValue;
			return false;
		}

		public override int TwoDigitYearMax
		{
			get
			{
				if (this.twoDigitYearMax == -1)
				{
					this.twoDigitYearMax = Calendar.GetSystemTwoDigitYearSetting(this.ID, 2029);
				}
				return this.twoDigitYearMax;
			}
			set
			{
				base.VerifyWritable();
				if (value < 99 || value > 9999)
				{
					throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 99, 9999));
				}
				this.twoDigitYearMax = value;
			}
		}

		public override int ToFourDigitYear(int year)
		{
			if (year < 0)
			{
				throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("Non-negative number required."));
			}
			if (year > 9999)
			{
				throw new ArgumentOutOfRangeException("year", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), 1, 9999));
			}
			return base.ToFourDigitYear(year);
		}

		public const int ADEra = 1;

		internal const int DatePartYear = 0;

		internal const int DatePartDayOfYear = 1;

		internal const int DatePartMonth = 2;

		internal const int DatePartDay = 3;

		internal const int MaxYear = 9999;

		internal GregorianCalendarTypes m_type;

		internal static readonly int[] DaysToMonth365 = new int[]
		{
			0, 31, 59, 90, 120, 151, 181, 212, 243, 273,
			304, 334, 365
		};

		internal static readonly int[] DaysToMonth366 = new int[]
		{
			0, 31, 60, 91, 121, 152, 182, 213, 244, 274,
			305, 335, 366
		};

		private static volatile Calendar s_defaultInstance;

		private const int DEFAULT_TWO_DIGIT_YEAR_MAX = 2029;
	}
}
