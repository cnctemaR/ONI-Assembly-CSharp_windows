using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	public struct DateTimeOffset : IComparable, IFormattable, ISerializable, IDeserializationCallback, IComparable<DateTimeOffset>, IEquatable<DateTimeOffset>
	{
		public DateTimeOffset(long ticks, TimeSpan offset)
		{
			this.m_offsetMinutes = DateTimeOffset.ValidateOffset(offset);
			DateTime dateTime = new DateTime(ticks);
			this.m_dateTime = DateTimeOffset.ValidateDate(dateTime, offset);
		}

		public DateTimeOffset(DateTime dateTime)
		{
			TimeSpan localUtcOffset;
			if (dateTime.Kind != DateTimeKind.Utc)
			{
				localUtcOffset = TimeZoneInfo.GetLocalUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
			}
			else
			{
				localUtcOffset = new TimeSpan(0L);
			}
			this.m_offsetMinutes = DateTimeOffset.ValidateOffset(localUtcOffset);
			this.m_dateTime = DateTimeOffset.ValidateDate(dateTime, localUtcOffset);
		}

		public DateTimeOffset(DateTime dateTime, TimeSpan offset)
		{
			if (dateTime.Kind == DateTimeKind.Local)
			{
				if (offset != TimeZoneInfo.GetLocalUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime))
				{
					throw new ArgumentException(Environment.GetResourceString("The UTC Offset of the local dateTime parameter does not match the offset argument."), "offset");
				}
			}
			else if (dateTime.Kind == DateTimeKind.Utc && offset != TimeSpan.Zero)
			{
				throw new ArgumentException(Environment.GetResourceString("The UTC Offset for Utc DateTime instances must be 0."), "offset");
			}
			this.m_offsetMinutes = DateTimeOffset.ValidateOffset(offset);
			this.m_dateTime = DateTimeOffset.ValidateDate(dateTime, offset);
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, TimeSpan offset)
		{
			this.m_offsetMinutes = DateTimeOffset.ValidateOffset(offset);
			this.m_dateTime = DateTimeOffset.ValidateDate(new DateTime(year, month, day, hour, minute, second), offset);
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeSpan offset)
		{
			this.m_offsetMinutes = DateTimeOffset.ValidateOffset(offset);
			this.m_dateTime = DateTimeOffset.ValidateDate(new DateTime(year, month, day, hour, minute, second, millisecond), offset);
		}

		public DateTimeOffset(int year, int month, int day, int hour, int minute, int second, int millisecond, Calendar calendar, TimeSpan offset)
		{
			this.m_offsetMinutes = DateTimeOffset.ValidateOffset(offset);
			this.m_dateTime = DateTimeOffset.ValidateDate(new DateTime(year, month, day, hour, minute, second, millisecond, calendar), offset);
		}

		public static DateTimeOffset Now
		{
			get
			{
				return new DateTimeOffset(DateTime.Now);
			}
		}

		public static DateTimeOffset UtcNow
		{
			get
			{
				return new DateTimeOffset(DateTime.UtcNow);
			}
		}

		public DateTime DateTime
		{
			get
			{
				return this.ClockDateTime;
			}
		}

		public DateTime UtcDateTime
		{
			get
			{
				return DateTime.SpecifyKind(this.m_dateTime, DateTimeKind.Utc);
			}
		}

		public DateTime LocalDateTime
		{
			get
			{
				return this.UtcDateTime.ToLocalTime();
			}
		}

		public DateTimeOffset ToOffset(TimeSpan offset)
		{
			return new DateTimeOffset((this.m_dateTime + offset).Ticks, offset);
		}

		private DateTime ClockDateTime
		{
			get
			{
				return new DateTime((this.m_dateTime + this.Offset).Ticks, DateTimeKind.Unspecified);
			}
		}

		public DateTime Date
		{
			get
			{
				return this.ClockDateTime.Date;
			}
		}

		public int Day
		{
			get
			{
				return this.ClockDateTime.Day;
			}
		}

		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.ClockDateTime.DayOfWeek;
			}
		}

		public int DayOfYear
		{
			get
			{
				return this.ClockDateTime.DayOfYear;
			}
		}

		public int Hour
		{
			get
			{
				return this.ClockDateTime.Hour;
			}
		}

		public int Millisecond
		{
			get
			{
				return this.ClockDateTime.Millisecond;
			}
		}

		public int Minute
		{
			get
			{
				return this.ClockDateTime.Minute;
			}
		}

		public int Month
		{
			get
			{
				return this.ClockDateTime.Month;
			}
		}

		public TimeSpan Offset
		{
			get
			{
				return new TimeSpan(0, (int)this.m_offsetMinutes, 0);
			}
		}

		public int Second
		{
			get
			{
				return this.ClockDateTime.Second;
			}
		}

		public long Ticks
		{
			get
			{
				return this.ClockDateTime.Ticks;
			}
		}

		public long UtcTicks
		{
			get
			{
				return this.UtcDateTime.Ticks;
			}
		}

		public TimeSpan TimeOfDay
		{
			get
			{
				return this.ClockDateTime.TimeOfDay;
			}
		}

		public int Year
		{
			get
			{
				return this.ClockDateTime.Year;
			}
		}

		public DateTimeOffset Add(TimeSpan timeSpan)
		{
			return new DateTimeOffset(this.ClockDateTime.Add(timeSpan), this.Offset);
		}

		public DateTimeOffset AddDays(double days)
		{
			return new DateTimeOffset(this.ClockDateTime.AddDays(days), this.Offset);
		}

		public DateTimeOffset AddHours(double hours)
		{
			return new DateTimeOffset(this.ClockDateTime.AddHours(hours), this.Offset);
		}

		public DateTimeOffset AddMilliseconds(double milliseconds)
		{
			return new DateTimeOffset(this.ClockDateTime.AddMilliseconds(milliseconds), this.Offset);
		}

		public DateTimeOffset AddMinutes(double minutes)
		{
			return new DateTimeOffset(this.ClockDateTime.AddMinutes(minutes), this.Offset);
		}

		public DateTimeOffset AddMonths(int months)
		{
			return new DateTimeOffset(this.ClockDateTime.AddMonths(months), this.Offset);
		}

		public DateTimeOffset AddSeconds(double seconds)
		{
			return new DateTimeOffset(this.ClockDateTime.AddSeconds(seconds), this.Offset);
		}

		public DateTimeOffset AddTicks(long ticks)
		{
			return new DateTimeOffset(this.ClockDateTime.AddTicks(ticks), this.Offset);
		}

		public DateTimeOffset AddYears(int years)
		{
			return new DateTimeOffset(this.ClockDateTime.AddYears(years), this.Offset);
		}

		public static int Compare(DateTimeOffset first, DateTimeOffset second)
		{
			return DateTime.Compare(first.UtcDateTime, second.UtcDateTime);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is DateTimeOffset))
			{
				throw new ArgumentException(Environment.GetResourceString("Object must be of type DateTimeOffset."));
			}
			DateTime utcDateTime = ((DateTimeOffset)obj).UtcDateTime;
			DateTime utcDateTime2 = this.UtcDateTime;
			if (utcDateTime2 > utcDateTime)
			{
				return 1;
			}
			if (utcDateTime2 < utcDateTime)
			{
				return -1;
			}
			return 0;
		}

		public int CompareTo(DateTimeOffset other)
		{
			DateTime utcDateTime = other.UtcDateTime;
			DateTime utcDateTime2 = this.UtcDateTime;
			if (utcDateTime2 > utcDateTime)
			{
				return 1;
			}
			if (utcDateTime2 < utcDateTime)
			{
				return -1;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			return obj is DateTimeOffset && this.UtcDateTime.Equals(((DateTimeOffset)obj).UtcDateTime);
		}

		public bool Equals(DateTimeOffset other)
		{
			return this.UtcDateTime.Equals(other.UtcDateTime);
		}

		public bool EqualsExact(DateTimeOffset other)
		{
			return this.ClockDateTime == other.ClockDateTime && this.Offset == other.Offset && this.ClockDateTime.Kind == other.ClockDateTime.Kind;
		}

		public static bool Equals(DateTimeOffset first, DateTimeOffset second)
		{
			return DateTime.Equals(first.UtcDateTime, second.UtcDateTime);
		}

		public static DateTimeOffset FromFileTime(long fileTime)
		{
			return new DateTimeOffset(DateTime.FromFileTime(fileTime));
		}

		public static DateTimeOffset FromUnixTimeSeconds(long seconds)
		{
			if (seconds < -62135596800L || seconds > 253402300799L)
			{
				throw new ArgumentOutOfRangeException("seconds", string.Format(Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), -62135596800L, 253402300799L));
			}
			return new DateTimeOffset(seconds * 10000000L + 621355968000000000L, TimeSpan.Zero);
		}

		public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
		{
			if (milliseconds < -62135596800000L || milliseconds > 253402300799999L)
			{
				throw new ArgumentOutOfRangeException("milliseconds", string.Format(Environment.GetResourceString("Valid values are between {0} and {1}, inclusive."), -62135596800000L, 253402300799999L));
			}
			return new DateTimeOffset(milliseconds * 10000L + 621355968000000000L, TimeSpan.Zero);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			try
			{
				this.m_offsetMinutes = DateTimeOffset.ValidateOffset(this.Offset);
				this.m_dateTime = DateTimeOffset.ValidateDate(this.ClockDateTime, this.Offset);
			}
			catch (ArgumentException ex)
			{
				throw new SerializationException(Environment.GetResourceString("An error occurred while deserializing the object.  The serialized data is corrupt."), ex);
			}
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("DateTime", this.m_dateTime);
			info.AddValue("OffsetMinutes", this.m_offsetMinutes);
		}

		private DateTimeOffset(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.m_dateTime = (DateTime)info.GetValue("DateTime", typeof(DateTime));
			this.m_offsetMinutes = (short)info.GetValue("OffsetMinutes", typeof(short));
		}

		public override int GetHashCode()
		{
			return this.UtcDateTime.GetHashCode();
		}

		public static DateTimeOffset Parse(string input)
		{
			TimeSpan timeSpan;
			return new DateTimeOffset(DateTimeParse.Parse(input, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out timeSpan).Ticks, timeSpan);
		}

		public static DateTimeOffset Parse(string input, IFormatProvider formatProvider)
		{
			return DateTimeOffset.Parse(input, formatProvider, DateTimeStyles.None);
		}

		public static DateTimeOffset Parse(string input, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			styles = DateTimeOffset.ValidateStyles(styles, "styles");
			TimeSpan timeSpan;
			return new DateTimeOffset(DateTimeParse.Parse(input, DateTimeFormatInfo.GetInstance(formatProvider), styles, out timeSpan).Ticks, timeSpan);
		}

		public static DateTimeOffset ParseExact(string input, string format, IFormatProvider formatProvider)
		{
			return DateTimeOffset.ParseExact(input, format, formatProvider, DateTimeStyles.None);
		}

		public static DateTimeOffset ParseExact(string input, string format, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			styles = DateTimeOffset.ValidateStyles(styles, "styles");
			TimeSpan timeSpan;
			return new DateTimeOffset(DateTimeParse.ParseExact(input, format, DateTimeFormatInfo.GetInstance(formatProvider), styles, out timeSpan).Ticks, timeSpan);
		}

		public static DateTimeOffset ParseExact(string input, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles)
		{
			styles = DateTimeOffset.ValidateStyles(styles, "styles");
			TimeSpan timeSpan;
			return new DateTimeOffset(DateTimeParse.ParseExactMultiple(input, formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out timeSpan).Ticks, timeSpan);
		}

		public TimeSpan Subtract(DateTimeOffset value)
		{
			return this.UtcDateTime.Subtract(value.UtcDateTime);
		}

		public DateTimeOffset Subtract(TimeSpan value)
		{
			return new DateTimeOffset(this.ClockDateTime.Subtract(value), this.Offset);
		}

		public long ToFileTime()
		{
			return this.UtcDateTime.ToFileTime();
		}

		public long ToUnixTimeSeconds()
		{
			return this.UtcDateTime.Ticks / 10000000L - 62135596800L;
		}

		public long ToUnixTimeMilliseconds()
		{
			return this.UtcDateTime.Ticks / 10000L - 62135596800000L;
		}

		public DateTimeOffset ToLocalTime()
		{
			return this.ToLocalTime(false);
		}

		internal DateTimeOffset ToLocalTime(bool throwOnOverflow)
		{
			return new DateTimeOffset(this.UtcDateTime.ToLocalTime(throwOnOverflow));
		}

		public override string ToString()
		{
			return DateTimeFormat.Format(this.ClockDateTime, null, DateTimeFormatInfo.CurrentInfo, this.Offset);
		}

		public string ToString(string format)
		{
			return DateTimeFormat.Format(this.ClockDateTime, format, DateTimeFormatInfo.CurrentInfo, this.Offset);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return DateTimeFormat.Format(this.ClockDateTime, null, DateTimeFormatInfo.GetInstance(formatProvider), this.Offset);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			return DateTimeFormat.Format(this.ClockDateTime, format, DateTimeFormatInfo.GetInstance(formatProvider), this.Offset);
		}

		public DateTimeOffset ToUniversalTime()
		{
			return new DateTimeOffset(this.UtcDateTime);
		}

		public static bool TryParse(string input, out DateTimeOffset result)
		{
			DateTime dateTime;
			TimeSpan timeSpan;
			bool flag = DateTimeParse.TryParse(input, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out dateTime, out timeSpan);
			result = new DateTimeOffset(dateTime.Ticks, timeSpan);
			return flag;
		}

		public static bool TryParse(string input, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			styles = DateTimeOffset.ValidateStyles(styles, "styles");
			DateTime dateTime;
			TimeSpan timeSpan;
			bool flag = DateTimeParse.TryParse(input, DateTimeFormatInfo.GetInstance(formatProvider), styles, out dateTime, out timeSpan);
			result = new DateTimeOffset(dateTime.Ticks, timeSpan);
			return flag;
		}

		public static bool TryParseExact(string input, string format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			styles = DateTimeOffset.ValidateStyles(styles, "styles");
			DateTime dateTime;
			TimeSpan timeSpan;
			bool flag = DateTimeParse.TryParseExact(input, format, DateTimeFormatInfo.GetInstance(formatProvider), styles, out dateTime, out timeSpan);
			result = new DateTimeOffset(dateTime.Ticks, timeSpan);
			return flag;
		}

		public static bool TryParseExact(string input, string[] formats, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
		{
			styles = DateTimeOffset.ValidateStyles(styles, "styles");
			DateTime dateTime;
			TimeSpan timeSpan;
			bool flag = DateTimeParse.TryParseExactMultiple(input, formats, DateTimeFormatInfo.GetInstance(formatProvider), styles, out dateTime, out timeSpan);
			result = new DateTimeOffset(dateTime.Ticks, timeSpan);
			return flag;
		}

		private static short ValidateOffset(TimeSpan offset)
		{
			long ticks = offset.Ticks;
			if (ticks % 600000000L != 0L)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset must be specified in whole minutes."), "offset");
			}
			if (ticks < -504000000000L || ticks > 504000000000L)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("Offset must be within plus or minus 14 hours."));
			}
			return (short)(offset.Ticks / 600000000L);
		}

		private static DateTime ValidateDate(DateTime dateTime, TimeSpan offset)
		{
			long num = dateTime.Ticks - offset.Ticks;
			if (num < 0L || num > 3155378975999999999L)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("The UTC time represented when the offset is applied must be between year 0 and 10,000."));
			}
			return new DateTime(num, DateTimeKind.Unspecified);
		}

		private static DateTimeStyles ValidateStyles(DateTimeStyles style, string parameterName)
		{
			if ((style & ~(DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal | DateTimeStyles.AssumeUniversal | DateTimeStyles.RoundtripKind)) != DateTimeStyles.None)
			{
				throw new ArgumentException(Environment.GetResourceString("An undefined DateTimeStyles value is being used."), parameterName);
			}
			if ((style & DateTimeStyles.AssumeLocal) != DateTimeStyles.None && (style & DateTimeStyles.AssumeUniversal) != DateTimeStyles.None)
			{
				throw new ArgumentException(Environment.GetResourceString("The DateTimeStyles values AssumeLocal and AssumeUniversal cannot be used together."), parameterName);
			}
			if ((style & DateTimeStyles.NoCurrentDateDefault) != DateTimeStyles.None)
			{
				throw new ArgumentException(Environment.GetResourceString("The DateTimeStyles value 'NoCurrentDateDefault' is not allowed when parsing DateTimeOffset."), parameterName);
			}
			style &= ~DateTimeStyles.RoundtripKind;
			style &= ~DateTimeStyles.AssumeLocal;
			return style;
		}

		public static implicit operator DateTimeOffset(DateTime dateTime)
		{
			return new DateTimeOffset(dateTime);
		}

		public static DateTimeOffset operator +(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
		{
			return new DateTimeOffset(dateTimeOffset.ClockDateTime + timeSpan, dateTimeOffset.Offset);
		}

		public static DateTimeOffset operator -(DateTimeOffset dateTimeOffset, TimeSpan timeSpan)
		{
			return new DateTimeOffset(dateTimeOffset.ClockDateTime - timeSpan, dateTimeOffset.Offset);
		}

		public static TimeSpan operator -(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime - right.UtcDateTime;
		}

		public static bool operator ==(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime == right.UtcDateTime;
		}

		public static bool operator !=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime != right.UtcDateTime;
		}

		public static bool operator <(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime < right.UtcDateTime;
		}

		public static bool operator <=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime <= right.UtcDateTime;
		}

		public static bool operator >(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime > right.UtcDateTime;
		}

		public static bool operator >=(DateTimeOffset left, DateTimeOffset right)
		{
			return left.UtcDateTime >= right.UtcDateTime;
		}

		internal const long MaxOffset = 504000000000L;

		internal const long MinOffset = -504000000000L;

		private const long UnixEpochTicks = 621355968000000000L;

		private const long UnixEpochSeconds = 62135596800L;

		private const long UnixEpochMilliseconds = 62135596800000L;

		public static readonly DateTimeOffset MinValue = new DateTimeOffset(0L, TimeSpan.Zero);

		public static readonly DateTimeOffset MaxValue = new DateTimeOffset(3155378975999999999L, TimeSpan.Zero);

		private DateTime m_dateTime;

		private short m_offsetMinutes;
	}
}
