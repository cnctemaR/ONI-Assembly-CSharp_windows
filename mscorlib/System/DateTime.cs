using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct DateTime : IFormattable, IConvertible, IComparable, IComparable<DateTime>, IEquatable<DateTime>
	{
		public DateTime(long ticks)
		{
			this.ticks = new TimeSpan(ticks);
			if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks)
			{
				string text = Locale.GetText("Value {0} is outside the valid range [{1},{2}].", new object[]
				{
					ticks,
					DateTime.MinValue.Ticks,
					DateTime.MaxValue.Ticks
				});
				throw new ArgumentOutOfRangeException("ticks", text);
			}
			this.kind = DateTimeKind.Unspecified;
		}

		public DateTime(int year, int month, int day)
		{
			this = new DateTime(year, month, day, 0, 0, 0, 0);
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second)
		{
			this = new DateTime(year, month, day, hour, minute, second, 0);
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			if (year < 1 || year > 9999 || month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month) || hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59 || millisecond < 0 || millisecond > 999)
			{
				throw new ArgumentOutOfRangeException("Parameters describe an unrepresentable DateTime.");
			}
			this.ticks = new TimeSpan(DateTime.AbsoluteDays(year, month, day), hour, minute, second, millisecond);
			this.kind = DateTimeKind.Unspecified;
		}

		public DateTime(int year, int month, int day, Calendar calendar)
		{
			this = new DateTime(year, month, day, 0, 0, 0, 0, calendar);
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar)
		{
			this = new DateTime(year, month, day, hour, minute, second, 0, calendar);
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar)
		{
			if (calendar == null)
			{
				throw new ArgumentNullException("calendar");
			}
			this.ticks = calendar.ToDateTime(year, month, day, hour, minute, second, millisecond).ticks;
			this.kind = DateTimeKind.Unspecified;
		}

		internal DateTime(bool check, TimeSpan value)
		{
			if (check && (value.Ticks < DateTime.MinValue.Ticks || value.Ticks > DateTime.MaxValue.Ticks))
			{
				throw new ArgumentOutOfRangeException();
			}
			this.ticks = value;
			this.kind = DateTimeKind.Unspecified;
		}

		public DateTime(long ticks, DateTimeKind kind)
		{
			this = new DateTime(ticks);
			this.CheckDateTimeKind(kind);
			this.kind = kind;
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, DateTimeKind kind)
		{
			this = new DateTime(year, month, day, hour, minute, second);
			this.CheckDateTimeKind(kind);
			this.kind = kind;
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
		{
			this = new DateTime(year, month, day, hour, minute, second, millisecond);
			this.CheckDateTimeKind(kind);
			this.kind = kind;
		}

		public DateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, DateTimeKind kind)
		{
			this = new DateTime(year, month, day, hour, minute, second, millisecond, calendar);
			this.CheckDateTimeKind(kind);
			this.kind = kind;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return this;
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			if (targetType == typeof(DateTime))
			{
				return this;
			}
			if (targetType == typeof(string))
			{
				return this.ToString(provider);
			}
			if (targetType == typeof(object))
			{
				return this;
			}
			throw new InvalidCastException();
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			throw new InvalidCastException();
		}

		private static int AbsoluteDays(int year, int month, int day)
		{
			int num = 0;
			int i = 1;
			int[] array = ((!DateTime.IsLeapYear(year)) ? DateTime.daysmonth : DateTime.daysmonthleap);
			while (i < month)
			{
				num += array[i++];
			}
			return day - 1 + num + 365 * (year - 1) + (year - 1) / 4 - (year - 1) / 100 + (year - 1) / 400;
		}

		private int FromTicks(DateTime.Which what)
		{
			int num = 1;
			int[] array = DateTime.daysmonth;
			int i = this.ticks.Days;
			int num2 = i / 146097;
			i -= num2 * 146097;
			int num3 = i / 36524;
			if (num3 == 4)
			{
				num3 = 3;
			}
			i -= num3 * 36524;
			int num4 = i / 1461;
			i -= num4 * 1461;
			int num5 = i / 365;
			if (num5 == 4)
			{
				num5 = 3;
			}
			if (what == DateTime.Which.Year)
			{
				return num2 * 400 + num3 * 100 + num4 * 4 + num5 + 1;
			}
			i -= num5 * 365;
			if (what == DateTime.Which.DayYear)
			{
				return i + 1;
			}
			if (num5 == 3 && (num3 == 3 || num4 != 24))
			{
				array = DateTime.daysmonthleap;
			}
			while (i >= array[num])
			{
				i -= array[num++];
			}
			if (what == DateTime.Which.Month)
			{
				return num;
			}
			return i + 1;
		}

		public DateTime Date
		{
			get
			{
				return new DateTime(this.Year, this.Month, this.Day)
				{
					kind = this.kind
				};
			}
		}

		public int Month
		{
			get
			{
				return this.FromTicks(DateTime.Which.Month);
			}
		}

		public int Day
		{
			get
			{
				return this.FromTicks(DateTime.Which.Day);
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return (this.ticks.Days + DayOfWeek.Monday) % (DayOfWeek)7;
			}
		}

		public int DayOfYear
		{
			get
			{
				return this.FromTicks(DateTime.Which.DayYear);
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return new TimeSpan(this.ticks.Ticks % 864000000000L);
			}
		}

		public int Hour
		{
			get
			{
				return this.ticks.Hours;
			}
		}

		public int Minute
		{
			get
			{
				return this.ticks.Minutes;
			}
		}

		public int Second
		{
			get
			{
				return this.ticks.Seconds;
			}
		}

		public int Millisecond
		{
			get
			{
				return this.ticks.Milliseconds;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetTimeMonotonic();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern long GetNow();

		public static DateTime Now
		{
			get
			{
				long now = DateTime.GetNow();
				DateTime dateTime = new DateTime(now);
				if (now - DateTime.last_now > 600000000L)
				{
					DateTime.to_local_time_span_object = TimeZone.CurrentTimeZone.GetLocalTimeDiff(dateTime);
					DateTime.last_now = now;
				}
				DateTime dateTime2 = dateTime + (TimeSpan)DateTime.to_local_time_span_object;
				dateTime2.kind = DateTimeKind.Local;
				return dateTime2;
			}
		}

		public long Ticks
		{
			get
			{
				return this.ticks.Ticks;
			}
		}

		public static DateTime Today
		{
			get
			{
				DateTime now = DateTime.Now;
				return new DateTime(now.Year, now.Month, now.Day)
				{
					kind = now.kind
				};
			}
		}

		public static DateTime UtcNow
		{
			get
			{
				return new DateTime(DateTime.GetNow(), DateTimeKind.Utc);
			}
		}

		public int Year
		{
			get
			{
				return this.FromTicks(DateTime.Which.Year);
			}
		}

		public DateTimeKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public DateTime Add(TimeSpan value)
		{
			DateTime dateTime = this.AddTicks(value.Ticks);
			dateTime.kind = this.kind;
			return dateTime;
		}

		public DateTime AddDays(double value)
		{
			return this.AddMilliseconds(Math.Round(value * 86400000.0));
		}

		public DateTime AddTicks(long value)
		{
			if (value + this.ticks.Ticks > 3155378975999999999L || value + this.ticks.Ticks < 0L)
			{
				throw new ArgumentOutOfRangeException();
			}
			return new DateTime(value + this.ticks.Ticks)
			{
				kind = this.kind
			};
		}

		public DateTime AddHours(double value)
		{
			return this.AddMilliseconds(value * 3600000.0);
		}

		public DateTime AddMilliseconds(double value)
		{
			if (value * 10000.0 > 9.223372036854776E+18 || value * 10000.0 < -9.223372036854776E+18)
			{
				throw new ArgumentOutOfRangeException();
			}
			long num = (long)Math.Round(value * 10000.0);
			return this.AddTicks(num);
		}

		private DateTime AddRoundedMilliseconds(double ms)
		{
			if (ms * 10000.0 > 9.223372036854776E+18 || ms * 10000.0 < -9.223372036854776E+18)
			{
				throw new ArgumentOutOfRangeException();
			}
			long num = (long)(ms += ((ms <= 0.0) ? (-0.5) : 0.5)) * 10000L;
			return this.AddTicks(num);
		}

		public DateTime AddMinutes(double value)
		{
			return this.AddMilliseconds(value * 60000.0);
		}

		public DateTime AddMonths(int months)
		{
			int num = this.Day;
			int num2 = this.Month + months % 12;
			int num3 = this.Year + months / 12;
			if (num2 < 1)
			{
				num2 = 12 + num2;
				num3--;
			}
			else if (num2 > 12)
			{
				num2 -= 12;
				num3++;
			}
			int num4 = DateTime.DaysInMonth(num3, num2);
			if (num > num4)
			{
				num = num4;
			}
			DateTime dateTime = new DateTime(num3, num2, num);
			dateTime.kind = this.kind;
			return dateTime.Add(this.TimeOfDay);
		}

		public DateTime AddSeconds(double value)
		{
			return this.AddMilliseconds(value * 1000.0);
		}

		public DateTime AddYears(int value)
		{
			return this.AddMonths(value * 12);
		}

		public static int Compare(DateTime t1, DateTime t2)
		{
			if (t1.ticks < t2.ticks)
			{
				return -1;
			}
			if (t1.ticks > t2.ticks)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is DateTime))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.DateTime"));
			}
			return DateTime.Compare(this, (DateTime)value);
		}

		public bool IsDaylightSavingTime()
		{
			return this.kind != DateTimeKind.Utc && TimeZone.CurrentTimeZone.IsDaylightSavingTime(this);
		}

		public int CompareTo(DateTime value)
		{
			return DateTime.Compare(this, value);
		}

		public bool Equals(DateTime value)
		{
			return value.ticks == this.ticks;
		}

		public long ToBinary()
		{
			DateTimeKind dateTimeKind = this.kind;
			if (dateTimeKind == DateTimeKind.Utc)
			{
				return this.Ticks | 4611686018427387904L;
			}
			if (dateTimeKind != DateTimeKind.Local)
			{
				return this.Ticks;
			}
			return this.ToUniversalTime().Ticks | long.MinValue;
		}

		public static DateTime FromBinary(long dateData)
		{
			ulong num = (ulong)dateData >> 62;
			if (num == (ulong)0)
			{
				return new DateTime(dateData, DateTimeKind.Unspecified);
			}
			if (num != (ulong)1)
			{
				DateTime dateTime = new DateTime(dateData & 4611686018427387903L, DateTimeKind.Utc);
				return dateTime.ToLocalTime();
			}
			return new DateTime(dateData ^ 4611686018427387904L, DateTimeKind.Utc);
		}

		public static DateTime SpecifyKind(DateTime value, DateTimeKind kind)
		{
			return new DateTime(value.Ticks, kind);
		}

		public static int DaysInMonth(int year, int month)
		{
			if (month < 1 || month > 12)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException();
			}
			int[] array = ((!DateTime.IsLeapYear(year)) ? DateTime.daysmonth : DateTime.daysmonthleap);
			return array[month];
		}

		public override bool Equals(object value)
		{
			return value is DateTime && ((DateTime)value).ticks == this.ticks;
		}

		public static bool Equals(DateTime t1, DateTime t2)
		{
			return t1.ticks == t2.ticks;
		}

		public static DateTime FromFileTime(long fileTime)
		{
			if (fileTime < 0L)
			{
				throw new ArgumentOutOfRangeException("fileTime", "< 0");
			}
			DateTime dateTime = new DateTime(504911232000000000L + fileTime);
			return dateTime.ToLocalTime();
		}

		public static DateTime FromFileTimeUtc(long fileTime)
		{
			if (fileTime < 0L)
			{
				throw new ArgumentOutOfRangeException("fileTime", "< 0");
			}
			return new DateTime(504911232000000000L + fileTime);
		}

		public static DateTime FromOADate(double d)
		{
			if (d <= -657435.0 || d >= 2958466.0)
			{
				throw new ArgumentException("d", "[-657435,2958466]");
			}
			DateTime dateTime = new DateTime(599264352000000000L);
			if (d < 0.0)
			{
				double num = Math.Ceiling(d);
				dateTime = dateTime.AddRoundedMilliseconds(num * 86400000.0);
				double num2 = num - d;
				dateTime = dateTime.AddRoundedMilliseconds(num2 * 86400000.0);
			}
			else
			{
				dateTime = dateTime.AddRoundedMilliseconds(d * 86400000.0);
			}
			return dateTime;
		}

		public string[] GetDateTimeFormats()
		{
			return this.GetDateTimeFormats(CultureInfo.CurrentCulture);
		}

		public string[] GetDateTimeFormats(char format)
		{
			if ("dDgGfFmMrRstTuUyY".IndexOf(format) < 0)
			{
				throw new FormatException("Invalid format character.");
			}
			return new string[] { this.ToString(format.ToString()) };
		}

		public string[] GetDateTimeFormats(IFormatProvider provider)
		{
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)provider.GetFormat(typeof(DateTimeFormatInfo));
			ArrayList arrayList = new ArrayList();
			foreach (char c in "dDgGfFmMrRstTuUyY")
			{
				arrayList.AddRange(this.GetDateTimeFormats(c, dateTimeFormatInfo));
			}
			return arrayList.ToArray(typeof(string)) as string[];
		}

		public string[] GetDateTimeFormats(char format, IFormatProvider provider)
		{
			if ("dDgGfFmMrRstTuUyY".IndexOf(format) < 0)
			{
				throw new FormatException("Invalid format character.");
			}
			bool flag = false;
			if (format == 'U')
			{
				flag = true;
			}
			DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)provider.GetFormat(typeof(DateTimeFormatInfo));
			return this.GetDateTimeFormats(flag, dateTimeFormatInfo.GetAllRawDateTimePatterns(format), dateTimeFormatInfo);
		}

		private string[] GetDateTimeFormats(bool adjustutc, string[] patterns, DateTimeFormatInfo dfi)
		{
			string[] array = new string[patterns.Length];
			DateTime dateTime = ((!adjustutc) ? this : this.ToUniversalTime());
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = DateTimeUtils.ToString(dateTime, patterns[i], dfi);
			}
			return array;
		}

		private void CheckDateTimeKind(DateTimeKind kind)
		{
			if (kind != DateTimeKind.Unspecified && kind != DateTimeKind.Utc && kind != DateTimeKind.Local)
			{
				throw new ArgumentException("Invalid DateTimeKind value.", "kind");
			}
		}

		public override int GetHashCode()
		{
			return (int)this.ticks.Ticks;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.DateTime;
		}

		public static bool IsLeapYear(int year)
		{
			if (year < 1 || year > 9999)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
		}

		public static DateTime Parse(string s)
		{
			return DateTime.Parse(s, null);
		}

		public static DateTime Parse(string s, IFormatProvider provider)
		{
			return DateTime.Parse(s, provider, DateTimeStyles.AllowWhiteSpaces);
		}

		public static DateTime Parse(string s, IFormatProvider provider, DateTimeStyles styles)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			Exception ex = null;
			DateTime dateTime;
			DateTimeOffset dateTimeOffset;
			if (!DateTime.CoreParse(s, provider, styles, out dateTime, out dateTimeOffset, true, ref ex))
			{
				throw ex;
			}
			return dateTime;
		}

		internal static bool CoreParse(string s, IFormatProvider provider, DateTimeStyles styles, out DateTime result, out DateTimeOffset dto, bool setExceptionOnError, ref Exception exception)
		{
			dto = new DateTimeOffset(0L, TimeSpan.Zero);
			if (s == null || s.Length == 0)
			{
				if (setExceptionOnError)
				{
					exception = new FormatException("String was not recognized as a valid DateTime.");
				}
				result = DateTime.MinValue;
				return false;
			}
			if (provider == null)
			{
				provider = CultureInfo.CurrentCulture;
			}
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
			string[] array = DateTime.YearMonthDayFormats(instance, setExceptionOnError, ref exception);
			if (array == null)
			{
				result = DateTime.MinValue;
				return false;
			}
			bool flag = false;
			foreach (string text in array)
			{
				bool flag2 = false;
				if (DateTime._DoParse(s, text, string.Empty, false, out result, out dto, instance, styles, true, ref flag2, ref flag))
				{
					return true;
				}
				if (flag2)
				{
					for (int j = 0; j < DateTime.ParseTimeFormats.Length; j++)
					{
						if (DateTime._DoParse(s, text, DateTime.ParseTimeFormats[j], false, out result, out dto, instance, styles, true, ref flag2, ref flag))
						{
							return true;
						}
					}
				}
			}
			int num = instance.MonthDayPattern.IndexOf('d');
			int num2 = instance.MonthDayPattern.IndexOf('M');
			if (num == -1 || num2 == -1)
			{
				result = DateTime.MinValue;
				if (setExceptionOnError)
				{
					exception = new FormatException(Locale.GetText("Order of month and date is not defined by {0}", new object[] { instance.MonthDayPattern }));
				}
				return false;
			}
			bool flag3 = num < num2;
			string[] array2 = ((!flag3) ? DateTime.MonthDayShortFormats : DateTime.DayMonthShortFormats);
			for (int k = 0; k < array2.Length; k++)
			{
				bool flag4 = false;
				if (DateTime._DoParse(s, array2[k], string.Empty, false, out result, out dto, instance, styles, true, ref flag4, ref flag))
				{
					return true;
				}
			}
			for (int l = 0; l < DateTime.ParseTimeFormats.Length; l++)
			{
				string text2 = DateTime.ParseTimeFormats[l];
				bool flag5 = false;
				if (DateTime._DoParse(s, text2, string.Empty, false, out result, out dto, instance, styles, false, ref flag5, ref flag))
				{
					return true;
				}
				if (flag5)
				{
					for (int m = 0; m < array2.Length; m++)
					{
						if (DateTime._DoParse(s, text2, array2[m], false, out result, out dto, instance, styles, false, ref flag5, ref flag))
						{
							return true;
						}
					}
					foreach (string text3 in array)
					{
						if (text3[text3.Length - 1] != 'T')
						{
							if (DateTime._DoParse(s, text2, text3, false, out result, out dto, instance, styles, false, ref flag5, ref flag))
							{
								return true;
							}
						}
					}
				}
			}
			if (DateTime.ParseExact(s, instance.GetAllDateTimePatternsInternal(), instance, styles, out result, false, ref flag, setExceptionOnError, ref exception))
			{
				return true;
			}
			if (!setExceptionOnError)
			{
				return false;
			}
			exception = new FormatException("String was not recognized as a valid DateTime.");
			return false;
		}

		public static DateTime ParseExact(string s, string format, IFormatProvider provider)
		{
			return DateTime.ParseExact(s, format, provider, DateTimeStyles.None);
		}

		private static string[] YearMonthDayFormats(DateTimeFormatInfo dfi, bool setExceptionOnError, ref Exception exc)
		{
			int num = dfi.ShortDatePattern.IndexOf('d');
			int num2 = dfi.ShortDatePattern.IndexOf('M');
			int num3 = dfi.ShortDatePattern.IndexOf('y');
			if (num == -1 || num2 == -1 || num3 == -1)
			{
				if (setExceptionOnError)
				{
					exc = new FormatException(Locale.GetText("Order of year, month and date is not defined by {0}", new object[] { dfi.ShortDatePattern }));
				}
				return null;
			}
			if (num3 < num2)
			{
				if (num2 < num)
				{
					return DateTime.ParseYearMonthDayFormats;
				}
				if (num3 < num)
				{
					return DateTime.ParseYearDayMonthFormats;
				}
				if (setExceptionOnError)
				{
					exc = new FormatException(Locale.GetText("Order of date, year and month defined by {0} is not supported", new object[] { dfi.ShortDatePattern }));
				}
				return null;
			}
			else
			{
				if (num < num2)
				{
					return DateTime.ParseDayMonthYearFormats;
				}
				if (num < num3)
				{
					return DateTime.ParseMonthDayYearFormats;
				}
				if (setExceptionOnError)
				{
					exc = new FormatException(Locale.GetText("Order of month, year and date defined by {0} is not supported", new object[] { dfi.ShortDatePattern }));
				}
				return null;
			}
		}

		private static int _ParseNumber(string s, int valuePos, int min_digits, int digits, bool leadingzero, bool sloppy_parsing, out int num_parsed)
		{
			int num = 0;
			if (sloppy_parsing)
			{
				leadingzero = false;
			}
			if (!leadingzero)
			{
				int num2 = 0;
				int i = valuePos;
				while (i < s.Length && i < digits + valuePos)
				{
					if (!char.IsDigit(s[i]))
					{
						break;
					}
					num2++;
					i++;
				}
				digits = num2;
			}
			if (digits < min_digits)
			{
				num_parsed = -1;
				return 0;
			}
			if (s.Length - valuePos < digits)
			{
				num_parsed = -1;
				return 0;
			}
			for (int i = valuePos; i < digits + valuePos; i++)
			{
				char c = s[i];
				if (!char.IsDigit(c))
				{
					num_parsed = -1;
					return 0;
				}
				num = num * 10 + (int)((byte)(c - '0'));
			}
			num_parsed = digits;
			return num;
		}

		private static int _ParseEnum(string s, int sPos, string[] values, string[] invValues, bool exact, out int num_parsed)
		{
			for (int i = values.Length - 1; i >= 0; i--)
			{
				if (!exact && invValues[i].Length > values[i].Length)
				{
					if (invValues[i].Length > 0 && DateTime._ParseString(s, sPos, 0, invValues[i], out num_parsed))
					{
						return i;
					}
					if (values[i].Length > 0 && DateTime._ParseString(s, sPos, 0, values[i], out num_parsed))
					{
						return i;
					}
				}
				else
				{
					if (values[i].Length > 0 && DateTime._ParseString(s, sPos, 0, values[i], out num_parsed))
					{
						return i;
					}
					if (!exact && invValues[i].Length > 0 && DateTime._ParseString(s, sPos, 0, invValues[i], out num_parsed))
					{
						return i;
					}
				}
			}
			num_parsed = -1;
			return -1;
		}

		private static bool _ParseString(string s, int sPos, int maxlength, string value, out int num_parsed)
		{
			if (maxlength <= 0)
			{
				maxlength = value.Length;
			}
			if (sPos + maxlength <= s.Length && string.Compare(s, sPos, value, 0, maxlength, true, CultureInfo.InvariantCulture) == 0)
			{
				num_parsed = maxlength;
				return true;
			}
			num_parsed = -1;
			return false;
		}

		private static bool _ParseAmPm(string s, int valuePos, int num, DateTimeFormatInfo dfi, bool exact, out int num_parsed, ref int ampm)
		{
			num_parsed = -1;
			if (ampm != -1)
			{
				return false;
			}
			if (DateTime.IsLetter(s, valuePos))
			{
				DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
				if ((!exact && DateTime._ParseString(s, valuePos, num, invariantInfo.PMDesignator, out num_parsed)) || (dfi.PMDesignator != string.Empty && DateTime._ParseString(s, valuePos, num, dfi.PMDesignator, out num_parsed)))
				{
					ampm = 1;
				}
				else
				{
					if ((exact || !DateTime._ParseString(s, valuePos, num, invariantInfo.AMDesignator, out num_parsed)) && !DateTime._ParseString(s, valuePos, num, dfi.AMDesignator, out num_parsed))
					{
						return false;
					}
					if (exact || num_parsed != 0)
					{
						ampm = 0;
					}
				}
				return true;
			}
			if (dfi.AMDesignator != string.Empty)
			{
				return false;
			}
			if (exact)
			{
				ampm = 0;
			}
			num_parsed = 0;
			return true;
		}

		private static bool _ParseTimeSeparator(string s, int sPos, DateTimeFormatInfo dfi, bool exact, out int num_parsed)
		{
			return DateTime._ParseString(s, sPos, 0, dfi.TimeSeparator, out num_parsed) || (!exact && DateTime._ParseString(s, sPos, 0, ":", out num_parsed));
		}

		private static bool _ParseDateSeparator(string s, int sPos, DateTimeFormatInfo dfi, bool exact, out int num_parsed)
		{
			num_parsed = -1;
			if (exact && s[sPos] != '/')
			{
				return false;
			}
			if (DateTime._ParseTimeSeparator(s, sPos, dfi, exact, out num_parsed) || char.IsDigit(s[sPos]) || char.IsLetter(s[sPos]))
			{
				return false;
			}
			num_parsed = 1;
			return true;
		}

		private static bool IsLetter(string s, int pos)
		{
			return pos < s.Length && char.IsLetter(s[pos]);
		}

		private static bool _DoParse(string s, string firstPart, string secondPart, bool exact, out DateTime result, out DateTimeOffset dto, DateTimeFormatInfo dfi, DateTimeStyles style, bool firstPartIsDate, ref bool incompleteFormat, ref bool longYear)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			dto = new DateTimeOffset(0L, TimeSpan.Zero);
			bool flag4 = !exact && secondPart != null;
			incompleteFormat = false;
			int num = 0;
			string text = firstPart;
			bool flag5 = false;
			DateTimeFormatInfo invariantInfo = DateTimeFormatInfo.InvariantInfo;
			if (text.Length == 1)
			{
				text = DateTimeUtils.GetStandardPattern(text[0], dfi, out flag, out flag2);
			}
			result = new DateTime(0L);
			if (text == null)
			{
				return false;
			}
			if (s == null)
			{
				return false;
			}
			if ((style & DateTimeStyles.AllowLeadingWhite) != DateTimeStyles.None)
			{
				text = text.TrimStart(null);
				s = s.TrimStart(null);
			}
			if ((style & DateTimeStyles.AllowTrailingWhite) != DateTimeStyles.None)
			{
				text = text.TrimEnd(null);
				s = s.TrimEnd(null);
			}
			if (flag2)
			{
				dfi = invariantInfo;
			}
			if ((style & DateTimeStyles.AllowInnerWhite) != DateTimeStyles.None)
			{
				flag3 = true;
			}
			string text2 = text;
			int num2 = text.Length;
			int num3 = 0;
			int num4 = 0;
			if (num2 == 0)
			{
				return false;
			}
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			int num9 = -1;
			int num10 = -1;
			int num11 = -1;
			double num12 = -1.0;
			int num13 = -1;
			int num14 = -1;
			int num15 = -1;
			int num16 = -1;
			bool flag6 = true;
			while (num != s.Length)
			{
				int num17 = 0;
				if (flag4 && num3 + num4 == 0)
				{
					bool flag7 = DateTime.IsLetter(s, num);
					if (flag7)
					{
						if (s[num] == 'Z')
						{
							num17 = 1;
						}
						else
						{
							DateTime._ParseString(s, num, 0, "GMT", out num17);
						}
						if (num17 > 0 && !DateTime.IsLetter(s, num + num17))
						{
							num += num17;
							flag = true;
							continue;
						}
					}
					if (!flag5 && DateTime._ParseAmPm(s, num, 0, dfi, exact, out num17, ref num13))
					{
						if (DateTime.IsLetter(s, num + num17))
						{
							num13 = -1;
						}
						else if (num17 > 0)
						{
							num += num17;
							continue;
						}
					}
					if (!flag5 && num6 == -1 && flag7)
					{
						num6 = DateTime._ParseEnum(s, num, dfi.RawDayNames, invariantInfo.RawDayNames, exact, out num17);
						if (num6 == -1)
						{
							num6 = DateTime._ParseEnum(s, num, dfi.RawAbbreviatedDayNames, invariantInfo.RawAbbreviatedDayNames, exact, out num17);
						}
						if (num6 != -1 && !DateTime.IsLetter(s, num + num17))
						{
							num += num17;
							continue;
						}
						num6 = -1;
					}
					if (char.IsWhiteSpace(s[num]) || s[num] == ',')
					{
						num++;
						continue;
					}
					num17 = 0;
				}
				if (num3 + num4 >= num2)
				{
					if (flag4 && num4 == 0)
					{
						flag5 = flag6 && firstPart[firstPart.Length - 1] == 'T';
						if (flag6 || !(text == string.Empty))
						{
							num3 = 0;
							if (flag6)
							{
								text = secondPart;
							}
							else
							{
								text = string.Empty;
							}
							text2 = text;
							num2 = text2.Length;
							flag6 = false;
							continue;
						}
					}
					IL_0EA9:
					if (num3 + 1 < num2 && text2[num3] == '.' && text2[num3 + 1] == 'F')
					{
						num3++;
						while (num3 < num2 && text2[num3] == 'F')
						{
							num3++;
						}
					}
					while (num3 < num2 && text2[num3] == 'K')
					{
						num3++;
					}
					if (num3 < num2)
					{
						return false;
					}
					if (s.Length > num)
					{
						if (num == 0)
						{
							return false;
						}
						if (char.IsDigit(s[num]) && char.IsDigit(s[num - 1]))
						{
							return false;
						}
						if (char.IsLetter(s[num]) && char.IsLetter(s[num - 1]))
						{
							return false;
						}
						incompleteFormat = true;
						return false;
					}
					else
					{
						if (num9 == -1)
						{
							num9 = 0;
						}
						if (num10 == -1)
						{
							num10 = 0;
						}
						if (num11 == -1)
						{
							num11 = 0;
						}
						if (num12 == -1.0)
						{
							num12 = 0.0;
						}
						if (num5 == -1 && num7 == -1 && num8 == -1)
						{
							if ((style & DateTimeStyles.NoCurrentDateDefault) != DateTimeStyles.None)
							{
								num5 = 1;
								num7 = 1;
								num8 = 1;
							}
							else
							{
								num5 = DateTime.Today.Day;
								num7 = DateTime.Today.Month;
								num8 = DateTime.Today.Year;
							}
						}
						if (num5 == -1)
						{
							num5 = 1;
						}
						if (num7 == -1)
						{
							num7 = 1;
						}
						if (num8 == -1)
						{
							if ((style & DateTimeStyles.NoCurrentDateDefault) != DateTimeStyles.None)
							{
								num8 = 1;
							}
							else
							{
								num8 = DateTime.Today.Year;
							}
						}
						if (num13 == 0 && num9 == 12)
						{
							num9 = 0;
						}
						if (num13 == 1 && (!flag4 || num9 < 12))
						{
							num9 += 12;
						}
						if (num8 < 1 || num8 > 9999 || num7 < 1 || num7 > 12 || num5 < 1 || num5 > DateTime.DaysInMonth(num8, num7) || num9 < 0 || num9 > 23 || num10 < 0 || num10 > 59 || num11 < 0 || num11 > 59)
						{
							return false;
						}
						result = new DateTime(num8, num7, num5, num9, num10, num11, 0);
						result = result.AddSeconds(num12);
						if (num6 != -1 && num6 != (int)result.DayOfWeek)
						{
							return false;
						}
						if (num14 == -1)
						{
							if (result != DateTime.MinValue)
							{
								try
								{
									dto = new DateTimeOffset(result);
								}
								catch
								{
								}
							}
						}
						else
						{
							if (num16 == -1)
							{
								num16 = 0;
							}
							if (num15 == -1)
							{
								num15 = 0;
							}
							if (num14 == 1)
							{
								num15 = -num15;
								num16 = -num16;
							}
							try
							{
								dto = new DateTimeOffset(result, new TimeSpan(num15, num16, 0));
							}
							catch
							{
							}
						}
						bool flag8 = (style & DateTimeStyles.AdjustToUniversal) != DateTimeStyles.None;
						if (num14 != -1)
						{
							long num18 = (result.ticks - dto.Offset).Ticks;
							if (num18 < 0L)
							{
								num18 += 864000000000L;
							}
							result = new DateTime(false, new TimeSpan(num18));
							result.kind = DateTimeKind.Utc;
							if ((style & DateTimeStyles.RoundtripKind) != DateTimeStyles.None)
							{
								result = result.ToLocalTime();
							}
						}
						else if (flag || (style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
						{
							result.kind = DateTimeKind.Utc;
						}
						else if ((style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None)
						{
							result.kind = DateTimeKind.Local;
						}
						bool flag9 = !flag8 && (style & DateTimeStyles.RoundtripKind) == DateTimeStyles.None;
						if (result.kind != DateTimeKind.Unspecified)
						{
							if (flag8)
							{
								result = result.ToUniversalTime();
							}
							else if (flag9)
							{
								result = result.ToLocalTime();
							}
						}
						return true;
					}
				}
				else
				{
					bool flag10 = true;
					if (text2[num3] == '\'')
					{
						num4 = 1;
						while (num3 + num4 < num2)
						{
							if (text2[num3 + num4] == '\'')
							{
								break;
							}
							if (num == s.Length || s[num] != text2[num3 + num4])
							{
								return false;
							}
							num++;
							num4++;
						}
						num3 += num4 + 1;
						num4 = 0;
					}
					else if (text2[num3] == '"')
					{
						num4 = 1;
						while (num3 + num4 < num2)
						{
							if (text2[num3 + num4] == '"')
							{
								break;
							}
							if (num == s.Length || s[num] != text2[num3 + num4])
							{
								return false;
							}
							num++;
							num4++;
						}
						num3 += num4 + 1;
						num4 = 0;
					}
					else if (text2[num3] == '\\')
					{
						num3 += num4 + 1;
						num4 = 0;
						if (num3 >= num2)
						{
							return false;
						}
						if (s[num] != text2[num3])
						{
							return false;
						}
						num++;
						num3++;
					}
					else if (text2[num3] == '%')
					{
						num3++;
					}
					else if (char.IsWhiteSpace(s[num]) || (s[num] == ',' && ((!exact && text2[num3] == '/') || char.IsWhiteSpace(text2[num3]))))
					{
						num++;
						num4 = 0;
						if (exact && (style & DateTimeStyles.AllowInnerWhite) == DateTimeStyles.None)
						{
							if (!char.IsWhiteSpace(text2[num3]))
							{
								return false;
							}
							num3++;
						}
						else
						{
							int i;
							for (i = num; i < s.Length; i++)
							{
								if (!char.IsWhiteSpace(s[i]) && s[i] != ',')
								{
									break;
								}
							}
							num = i;
							for (i = num3; i < text2.Length; i++)
							{
								if (!char.IsWhiteSpace(text2[i]) && text2[i] != ',')
								{
									break;
								}
							}
							num3 = i;
							if (!exact && num3 < text2.Length && text2[num3] == '/' && !DateTime._ParseDateSeparator(s, num, dfi, exact, out num17))
							{
								num3++;
							}
						}
					}
					else if (num3 + num4 + 1 < num2 && text2[num3 + num4 + 1] == text2[num3 + num4])
					{
						num4++;
					}
					else
					{
						char c = text2[num3];
						switch (c)
						{
						case 'F':
							flag10 = false;
							goto IL_0A82;
						case 'G':
							if (s[num] != 'G')
							{
								return false;
							}
							if (num3 + 2 < num2 && num + 2 < s.Length && text2[num3 + 1] == 'M' && s[num + 1] == 'M' && text2[num3 + 2] == 'T' && s[num + 2] == 'T')
							{
								flag = true;
								num4 = 2;
								num17 = 3;
							}
							else
							{
								num4 = 0;
								num17 = 1;
							}
							break;
						case 'H':
							if (num9 != -1 || (!flag4 && num13 >= 0))
							{
								return false;
							}
							if (num4 == 0)
							{
								num9 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
							}
							else
							{
								num9 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
							}
							if (num9 >= 24)
							{
								return false;
							}
							break;
						default:
							switch (c)
							{
							case 's':
								if (num11 != -1)
								{
									return false;
								}
								if (num4 == 0)
								{
									num11 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
								}
								else
								{
									num11 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
								}
								if (num11 >= 60)
								{
									return false;
								}
								break;
							case 't':
								if (!DateTime._ParseAmPm(s, num, (num4 <= 0) ? 1 : 0, dfi, exact, out num17, ref num13))
								{
									return false;
								}
								break;
							default:
								switch (c)
								{
								case 'd':
									if ((num4 < 2 && num5 != -1) || (num4 >= 2 && num6 != -1))
									{
										return false;
									}
									if (num4 == 0)
									{
										num5 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
									}
									else if (num4 == 1)
									{
										num5 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
									}
									else if (num4 == 2)
									{
										num6 = DateTime._ParseEnum(s, num, dfi.RawAbbreviatedDayNames, invariantInfo.RawAbbreviatedDayNames, exact, out num17);
									}
									else
									{
										num6 = DateTime._ParseEnum(s, num, dfi.RawDayNames, invariantInfo.RawDayNames, exact, out num17);
									}
									break;
								default:
									if (c != '/')
									{
										if (c != ':')
										{
											if (c != 'Z')
											{
												if (c != 'm')
												{
													if (s[num] != text2[num3])
													{
														return false;
													}
													num4 = 0;
													num17 = 1;
												}
												else
												{
													if (num10 != -1)
													{
														return false;
													}
													if (num4 == 0)
													{
														num10 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
													}
													else
													{
														num10 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
													}
													if (num10 >= 60)
													{
														return false;
													}
												}
											}
											else
											{
												if (s[num] != 'Z')
												{
													return false;
												}
												num4 = 0;
												num17 = 1;
												flag = true;
											}
										}
										else if (!DateTime._ParseTimeSeparator(s, num, dfi, exact, out num17))
										{
											return false;
										}
									}
									else
									{
										if (!DateTime._ParseDateSeparator(s, num, dfi, exact, out num17))
										{
											return false;
										}
										num4 = 0;
									}
									break;
								case 'f':
									goto IL_0A82;
								case 'h':
									if (num9 != -1)
									{
										return false;
									}
									if (num4 == 0)
									{
										num9 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
									}
									else
									{
										num9 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
									}
									if (num9 > 12)
									{
										return false;
									}
									if (num9 == 12)
									{
										num9 = 0;
									}
									break;
								}
								break;
							case 'y':
								if (num8 != -1)
								{
									return false;
								}
								if (num4 == 0)
								{
									num8 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
								}
								else if (num4 < 3)
								{
									num8 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
								}
								else
								{
									num8 = DateTime._ParseNumber(s, num, (!exact) ? 3 : 4, 4, false, flag3, out num17);
									if (num8 >= 1000 && num17 == 4 && !longYear && s.Length > 4 + num)
									{
										int num19 = 0;
										int num20 = DateTime._ParseNumber(s, num, 5, 5, false, flag3, out num19);
										longYear = num20 > 9999;
									}
									num4 = 3;
								}
								if (num17 <= 2)
								{
									num8 += ((num8 >= 30) ? 1900 : 2000);
								}
								break;
							case 'z':
								if (num14 != -1)
								{
									return false;
								}
								if (s[num] == '+')
								{
									num14 = 0;
								}
								else
								{
									if (s[num] != '-')
									{
										return false;
									}
									num14 = 1;
								}
								num++;
								if (num4 == 0)
								{
									num15 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
								}
								else if (num4 == 1)
								{
									num15 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
								}
								else
								{
									num15 = DateTime._ParseNumber(s, num, 1, 2, true, true, out num17);
									num += num17;
									if (num17 < 0)
									{
										return false;
									}
									num17 = 0;
									if ((num < s.Length && char.IsDigit(s[num])) || DateTime._ParseTimeSeparator(s, num, dfi, exact, out num17))
									{
										num += num17;
										num16 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
										if (num17 < 0)
										{
											return false;
										}
									}
									else
									{
										if (!flag4)
										{
											return false;
										}
										num17 = 0;
									}
								}
								break;
							}
							break;
						case 'K':
							if (s[num] == 'Z')
							{
								num++;
								flag = true;
							}
							else if (s[num] == '+' || s[num] == '-')
							{
								if (num14 != -1)
								{
									return false;
								}
								if (s[num] == '+')
								{
									num14 = 0;
								}
								else if (s[num] == '-')
								{
									num14 = 1;
								}
								num++;
								num15 = DateTime._ParseNumber(s, num, 0, 2, true, flag3, out num17);
								num += num17;
								if (num17 < 0)
								{
									return false;
								}
								if (char.IsDigit(s[num]))
								{
									num17 = 0;
								}
								else if (!DateTime._ParseString(s, num, 0, dfi.TimeSeparator, out num17))
								{
									return false;
								}
								num += num17;
								num16 = DateTime._ParseNumber(s, num, 0, 2, true, flag3, out num17);
								num4 = 2;
								if (num17 < 0)
								{
									return false;
								}
							}
							break;
						case 'M':
							if (num7 != -1)
							{
								return false;
							}
							if (flag4)
							{
								num17 = -1;
								if (num4 == 0 || num4 == 3)
								{
									num7 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
								}
								if (num4 > 1 && num17 == -1)
								{
									num7 = DateTime._ParseEnum(s, num, dfi.RawMonthNames, invariantInfo.RawMonthNames, exact, out num17) + 1;
								}
								if (num4 > 1 && num17 == -1)
								{
									num7 = DateTime._ParseEnum(s, num, dfi.RawAbbreviatedMonthNames, invariantInfo.RawAbbreviatedMonthNames, exact, out num17) + 1;
								}
							}
							else if (num4 == 0)
							{
								num7 = DateTime._ParseNumber(s, num, 1, 2, false, flag3, out num17);
							}
							else if (num4 == 1)
							{
								num7 = DateTime._ParseNumber(s, num, 1, 2, true, flag3, out num17);
							}
							else if (num4 == 2)
							{
								num7 = DateTime._ParseEnum(s, num, dfi.RawAbbreviatedMonthNames, invariantInfo.RawAbbreviatedMonthNames, exact, out num17) + 1;
							}
							else
							{
								num7 = DateTime._ParseEnum(s, num, dfi.RawMonthNames, invariantInfo.RawMonthNames, exact, out num17) + 1;
							}
							break;
						}
						IL_0DF3:
						if (num17 < 0)
						{
							return false;
						}
						num += num17;
						if (!exact && !flag4)
						{
							c = text2[num3];
							if (c == 'F' || c == 'f' || c == 'm' || c == 's' || c == 'z')
							{
								if (s.Length > num && s[num] == 'Z' && (num3 + 1 == text2.Length || text2[num3 + 1] != 'Z'))
								{
									flag = true;
									num++;
								}
							}
						}
						num3 = num3 + num4 + 1;
						num4 = 0;
						continue;
						IL_0A82:
						if (num4 > 6 || num12 != -1.0)
						{
							return false;
						}
						double num21 = (double)DateTime._ParseNumber(s, num, 0, num4 + 1, flag10, flag3, out num17);
						if (num17 == -1)
						{
							return false;
						}
						num12 = num21 / Math.Pow(10.0, (double)num17);
						goto IL_0DF3;
					}
				}
			}
			goto IL_0EA9;
		}

		public static DateTime ParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			return DateTime.ParseExact(s, new string[] { format }, provider, style);
		}

		public static DateTime ParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style)
		{
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
			DateTime.CheckStyle(style);
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (formats == null)
			{
				throw new ArgumentNullException("formats");
			}
			if (formats.Length == 0)
			{
				throw new FormatException("Format specifier was invalid.");
			}
			bool flag = false;
			Exception ex = null;
			DateTime dateTime;
			if (!DateTime.ParseExact(s, formats, instance, style, out dateTime, true, ref flag, true, ref ex))
			{
				throw ex;
			}
			return dateTime;
		}

		private static void CheckStyle(DateTimeStyles style)
		{
			if ((style & DateTimeStyles.RoundtripKind) != DateTimeStyles.None && ((style & DateTimeStyles.AdjustToUniversal) != DateTimeStyles.None || (style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None || (style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None))
			{
				throw new ArgumentException("The DateTimeStyles value RoundtripKind cannot be used with the values AssumeLocal, Asersal or AdjustToUniversal.", "style");
			}
			if ((style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None && (style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None)
			{
				throw new ArgumentException("The DateTimeStyles values AssumeLocal and AssumeUniversal cannot be used together.", "style");
			}
		}

		public static bool TryParse(string s, out DateTime result)
		{
			if (s != null)
			{
				try
				{
					Exception ex = null;
					DateTimeOffset dateTimeOffset;
					return DateTime.CoreParse(s, null, DateTimeStyles.AllowWhiteSpaces, out result, out dateTimeOffset, false, ref ex);
				}
				catch
				{
				}
			}
			result = DateTime.MinValue;
			return false;
		}

		public static bool TryParse(string s, IFormatProvider provider, DateTimeStyles styles, out DateTime result)
		{
			if (s != null)
			{
				try
				{
					Exception ex = null;
					DateTimeOffset dateTimeOffset;
					return DateTime.CoreParse(s, provider, styles, out result, out dateTimeOffset, false, ref ex);
				}
				catch
				{
				}
			}
			result = DateTime.MinValue;
			return false;
		}

		public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
		{
			return DateTime.TryParseExact(s, new string[] { format }, provider, style, out result);
		}

		public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out DateTime result)
		{
			bool flag2;
			try
			{
				DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
				bool flag = false;
				Exception ex = null;
				flag2 = DateTime.ParseExact(s, formats, instance, style, out result, true, ref flag, false, ref ex);
			}
			catch
			{
				result = DateTime.MinValue;
				flag2 = false;
			}
			return flag2;
		}

		private static bool ParseExact(string s, string[] formats, DateTimeFormatInfo dfi, DateTimeStyles style, out DateTime ret, bool exact, ref bool longYear, bool setExceptionOnError, ref Exception exception)
		{
			bool flag = false;
			for (int i = 0; i < formats.Length; i++)
			{
				string text = formats[i];
				if (text == null || text == string.Empty)
				{
					break;
				}
				DateTime dateTime;
				DateTimeOffset dateTimeOffset;
				if (DateTime._DoParse(s, formats[i], null, exact, out dateTime, out dateTimeOffset, dfi, style, false, ref flag, ref longYear))
				{
					ret = dateTime;
					return true;
				}
			}
			if (setExceptionOnError)
			{
				exception = new FormatException("Invalid format string");
			}
			ret = DateTime.MinValue;
			return false;
		}

		public TimeSpan Subtract(DateTime value)
		{
			return new TimeSpan(this.ticks.Ticks) - value.ticks;
		}

		public DateTime Subtract(TimeSpan value)
		{
			TimeSpan timeSpan = new TimeSpan(this.ticks.Ticks) - value;
			return new DateTime(true, timeSpan)
			{
				kind = this.kind
			};
		}

		public long ToFileTime()
		{
			DateTime dateTime = this.ToUniversalTime();
			if (dateTime.Ticks < 504911232000000000L)
			{
				throw new ArgumentOutOfRangeException("file time is not valid");
			}
			return dateTime.Ticks - 504911232000000000L;
		}

		public long ToFileTimeUtc()
		{
			if (this.Ticks < 504911232000000000L)
			{
				throw new ArgumentOutOfRangeException("file time is not valid");
			}
			return this.Ticks - 504911232000000000L;
		}

		public string ToLongDateString()
		{
			return this.ToString("D");
		}

		public string ToLongTimeString()
		{
			return this.ToString("T");
		}

		public double ToOADate()
		{
			long num = this.Ticks;
			if (num == 0L)
			{
				return 0.0;
			}
			if (num < 31242239136000000L)
			{
				return -657434.999;
			}
			TimeSpan timeSpan = new TimeSpan(this.Ticks - 599264352000000000L);
			double num2 = timeSpan.TotalDays;
			if (num < 599264352000000000L)
			{
				double num3 = Math.Ceiling(num2);
				num2 = num3 - 2.0 - (num2 - num3);
			}
			else if (num2 >= 2958466.0)
			{
				num2 = 2958465.99999999;
			}
			return num2;
		}

		public string ToShortDateString()
		{
			return this.ToString("d");
		}

		public string ToShortTimeString()
		{
			return this.ToString("t");
		}

		public override string ToString()
		{
			return this.ToString("G", null);
		}

		public string ToString(IFormatProvider provider)
		{
			return this.ToString(null, provider);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			DateTimeFormatInfo instance = DateTimeFormatInfo.GetInstance(provider);
			if (format == null || format == string.Empty)
			{
				format = "G";
			}
			bool flag = false;
			bool flag2 = false;
			if (format.Length == 1)
			{
				char c = format[0];
				format = DateTimeUtils.GetStandardPattern(c, instance, out flag, out flag2);
				if (c == 'U')
				{
					return DateTimeUtils.ToString(this.ToUniversalTime(), format, instance);
				}
				if (format == null)
				{
					throw new FormatException("format is not one of the format specifier characters defined for DateTimeFormatInfo");
				}
			}
			return DateTimeUtils.ToString(this, format, instance);
		}

		public DateTime ToLocalTime()
		{
			return TimeZone.CurrentTimeZone.ToLocalTime(this);
		}

		public DateTime ToUniversalTime()
		{
			return TimeZone.CurrentTimeZone.ToUniversalTime(this);
		}

		public static DateTime operator +(DateTime d, TimeSpan t)
		{
			return new DateTime(true, d.ticks + t)
			{
				kind = d.kind
			};
		}

		public static bool operator ==(DateTime d1, DateTime d2)
		{
			return d1.ticks == d2.ticks;
		}

		public static bool operator >(DateTime t1, DateTime t2)
		{
			return t1.ticks > t2.ticks;
		}

		public static bool operator >=(DateTime t1, DateTime t2)
		{
			return t1.ticks >= t2.ticks;
		}

		public static bool operator !=(DateTime d1, DateTime d2)
		{
			return d1.ticks != d2.ticks;
		}

		public static bool operator <(DateTime t1, DateTime t2)
		{
			return t1.ticks < t2.ticks;
		}

		public static bool operator <=(DateTime t1, DateTime t2)
		{
			return t1.ticks <= t2.ticks;
		}

		public static TimeSpan operator -(DateTime d1, DateTime d2)
		{
			return new TimeSpan((d1.ticks - d2.ticks).Ticks);
		}

		public static DateTime operator -(DateTime d, TimeSpan t)
		{
			return new DateTime(true, d.ticks - t)
			{
				kind = d.kind
			};
		}

		private const int dp400 = 146097;

		private const int dp100 = 36524;

		private const int dp4 = 1461;

		private const long w32file_epoch = 504911232000000000L;

		private const long MAX_VALUE_TICKS = 3155378975999999999L;

		internal const long UnixEpoch = 621355968000000000L;

		private const long ticks18991230 = 599264352000000000L;

		private const double OAMinValue = -657435.0;

		private const double OAMaxValue = 2958466.0;

		private const string formatExceptionMessage = "String was not recognized as a valid DateTime.";

		private TimeSpan ticks;

		private DateTimeKind kind;

		public static readonly DateTime MaxValue = new DateTime(false, new TimeSpan(3155378975999999999L));

		public static readonly DateTime MinValue = new DateTime(false, new TimeSpan(0L));

		private static readonly string[] ParseTimeFormats = new string[] { "H:m:s.fffffffzzz", "H:m:s.fffffff", "H:m:s tt zzz", "H:m:szzz", "H:m:s", "H:mzzz", "H:m", "H tt", "H'時'm'分's'秒'" };

		private static readonly string[] ParseYearDayMonthFormats = new string[] { "yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/d/MMMM", "yyyy/MMM/d", "d/MMMM/yyyy", "MMM/d/yyyy", "d/yyyy/MMMM", "MMM/yyyy/d", "yy/d/M" };

		private static readonly string[] ParseYearMonthDayFormats = new string[]
		{
			"yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/MMMM/d", "yyyy/d/MMM", "MMMM/d/yyyy", "d/MMM/yyyy", "MMMM/yyyy/d", "d/yyyy/MMM", "yy/MMMM/d",
			"yy/d/MMM", "MMM/yy/d"
		};

		private static readonly string[] ParseDayMonthYearFormats = new string[]
		{
			"yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/MMMM/d", "yyyy/d/MMM", "d/MMMM/yyyy", "MMM/d/yyyy", "MMMM/yyyy/d", "d/yyyy/MMM", "d/MMMM/yy",
			"yy/MMM/d", "d/yy/MMM", "yy/d/MMM", "MMM/d/yy", "MMM/yy/d"
		};

		private static readonly string[] ParseMonthDayYearFormats = new string[]
		{
			"yyyy/M/dT", "M/yyyy/dT", "yyyy'年'M'月'd'日", "yyyy/MMMM/d", "yyyy/d/MMM", "MMMM/d/yyyy", "d/MMM/yyyy", "MMMM/yyyy/d", "d/yyyy/MMM", "MMMM/d/yy",
			"MMM/yy/d", "d/MMM/yy", "yy/MMM/d", "d/yy/MMM", "yy/d/MMM"
		};

		private static readonly string[] MonthDayShortFormats = new string[] { "MMMM/d", "d/MMM", "yyyy/MMMM" };

		private static readonly string[] DayMonthShortFormats = new string[] { "d/MMMM", "MMM/yy", "yyyy/MMMM" };

		private static readonly int[] daysmonth = new int[]
		{
			0, 31, 28, 31, 30, 31, 30, 31, 31, 30,
			31, 30, 31
		};

		private static readonly int[] daysmonthleap = new int[]
		{
			0, 31, 29, 31, 30, 31, 30, 31, 31, 30,
			31, 30, 31
		};

		private static object to_local_time_span_object;

		private static long last_now;

		private enum Which
		{
			Day,
			DayYear,
			Month,
			Year
		}
	}
}
