using System;
using System.Data.Common;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlDateTime : INullable, IComparable, IXmlSerializable
	{
		private SqlDateTime(bool fNull)
		{
			this.m_fNotNull = false;
			this.m_day = 0;
			this.m_time = 0;
		}

		public SqlDateTime(DateTime value)
		{
			this = SqlDateTime.FromDateTime(value);
		}

		public SqlDateTime(int year, int month, int day)
		{
			this = new SqlDateTime(year, month, day, 0, 0, 0, 0.0);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second)
		{
			this = new SqlDateTime(year, month, day, hour, minute, second, 0.0);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second, double millisecond)
		{
			if (year >= SqlDateTime.s_minYear && year <= SqlDateTime.s_maxYear && month >= 1 && month <= 12)
			{
				int[] array = (SqlDateTime.IsLeapYear(year) ? SqlDateTime.s_daysToMonth366 : SqlDateTime.s_daysToMonth365);
				if (day >= 1 && day <= array[month] - array[month - 1])
				{
					int num = year - 1;
					int num2 = num * 365 + num / 4 - num / 100 + num / 400 + array[month - 1] + day - 1;
					num2 -= SqlDateTime.s_dayBase;
					if (num2 >= SqlDateTime.s_minDay && num2 <= SqlDateTime.s_maxDay && hour >= 0 && hour < 24 && minute >= 0 && minute < 60 && second >= 0 && second < 60 && millisecond >= 0.0 && millisecond < 1000.0)
					{
						double num3 = millisecond * SqlDateTime.s_SQLTicksPerMillisecond + 0.5;
						int num4 = hour * SqlDateTime.SQLTicksPerHour + minute * SqlDateTime.SQLTicksPerMinute + second * SqlDateTime.SQLTicksPerSecond + (int)num3;
						if (num4 > SqlDateTime.s_maxTime)
						{
							num4 = 0;
							num2++;
						}
						this = new SqlDateTime(num2, num4);
						return;
					}
				}
			}
			throw new SqlTypeException(SQLResource.InvalidDateTimeMessage);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second, int bilisecond)
		{
			this = new SqlDateTime(year, month, day, hour, minute, second, (double)bilisecond / 1000.0);
		}

		public SqlDateTime(int dayTicks, int timeTicks)
		{
			if (dayTicks < SqlDateTime.s_minDay || dayTicks > SqlDateTime.s_maxDay || timeTicks < SqlDateTime.s_minTime || timeTicks > SqlDateTime.s_maxTime)
			{
				this.m_fNotNull = false;
				throw new OverflowException(SQLResource.DateTimeOverflowMessage);
			}
			this.m_day = dayTicks;
			this.m_time = timeTicks;
			this.m_fNotNull = true;
		}

		internal SqlDateTime(double dblVal)
		{
			if (dblVal < (double)SqlDateTime.s_minDay || dblVal >= (double)(SqlDateTime.s_maxDay + 1))
			{
				throw new OverflowException(SQLResource.DateTimeOverflowMessage);
			}
			int num = (int)dblVal;
			int num2 = (int)((dblVal - (double)num) * (double)SqlDateTime.s_SQLTicksPerDay);
			if (num2 < 0)
			{
				num--;
				num2 += SqlDateTime.s_SQLTicksPerDay;
			}
			else if (num2 >= SqlDateTime.s_SQLTicksPerDay)
			{
				num++;
				num2 -= SqlDateTime.s_SQLTicksPerDay;
			}
			this = new SqlDateTime(num, num2);
		}

		public bool IsNull
		{
			get
			{
				return !this.m_fNotNull;
			}
		}

		private static TimeSpan ToTimeSpan(SqlDateTime value)
		{
			long num = (long)((double)value.m_time / SqlDateTime.s_SQLTicksPerMillisecond + 0.5);
			return new TimeSpan((long)value.m_day * 864000000000L + num * 10000L);
		}

		private static DateTime ToDateTime(SqlDateTime value)
		{
			return SqlDateTime.s_SQLBaseDate.Add(SqlDateTime.ToTimeSpan(value));
		}

		internal static DateTime ToDateTime(int daypart, int timepart)
		{
			if (daypart < SqlDateTime.s_minDay || daypart > SqlDateTime.s_maxDay || timepart < SqlDateTime.s_minTime || timepart > SqlDateTime.s_maxTime)
			{
				throw new OverflowException(SQLResource.DateTimeOverflowMessage);
			}
			long num = (long)daypart * 864000000000L;
			long num2 = (long)((double)timepart / SqlDateTime.s_SQLTicksPerMillisecond + 0.5) * 10000L;
			return new DateTime(SqlDateTime.s_SQLBaseDateTicks + num + num2);
		}

		private static SqlDateTime FromTimeSpan(TimeSpan value)
		{
			if (value < SqlDateTime.s_minTimeSpan || value > SqlDateTime.s_maxTimeSpan)
			{
				throw new SqlTypeException(SQLResource.DateTimeOverflowMessage);
			}
			int num = value.Days;
			long num2 = value.Ticks - (long)num * 864000000000L;
			if (num2 < 0L)
			{
				num--;
				num2 += 864000000000L;
			}
			int num3 = (int)((double)num2 / 10000.0 * SqlDateTime.s_SQLTicksPerMillisecond + 0.5);
			if (num3 > SqlDateTime.s_maxTime)
			{
				num3 = 0;
				num++;
			}
			return new SqlDateTime(num, num3);
		}

		private static SqlDateTime FromDateTime(DateTime value)
		{
			if (value == DateTime.MaxValue)
			{
				return SqlDateTime.MaxValue;
			}
			return SqlDateTime.FromTimeSpan(value.Subtract(SqlDateTime.s_SQLBaseDate));
		}

		public DateTime Value
		{
			get
			{
				if (this.m_fNotNull)
				{
					return SqlDateTime.ToDateTime(this);
				}
				throw new SqlNullValueException();
			}
		}

		public int DayTicks
		{
			get
			{
				if (this.m_fNotNull)
				{
					return this.m_day;
				}
				throw new SqlNullValueException();
			}
		}

		public int TimeTicks
		{
			get
			{
				if (this.m_fNotNull)
				{
					return this.m_time;
				}
				throw new SqlNullValueException();
			}
		}

		public static implicit operator SqlDateTime(DateTime value)
		{
			return new SqlDateTime(value);
		}

		public static explicit operator DateTime(SqlDateTime x)
		{
			return SqlDateTime.ToDateTime(x);
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return SQLResource.NullString;
			}
			return SqlDateTime.ToDateTime(this).ToString(null);
		}

		public static SqlDateTime Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlDateTime.Null;
			}
			DateTime dateTime;
			try
			{
				dateTime = DateTime.Parse(s, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)CultureInfo.CurrentCulture.GetFormat(typeof(DateTimeFormatInfo));
				dateTime = DateTime.ParseExact(s, SqlDateTime.s_dateTimeFormats, dateTimeFormatInfo, DateTimeStyles.AllowWhiteSpaces);
			}
			return new SqlDateTime(dateTime);
		}

		public static SqlDateTime operator +(SqlDateTime x, TimeSpan t)
		{
			if (!x.IsNull)
			{
				return SqlDateTime.FromDateTime(SqlDateTime.ToDateTime(x) + t);
			}
			return SqlDateTime.Null;
		}

		public static SqlDateTime operator -(SqlDateTime x, TimeSpan t)
		{
			if (!x.IsNull)
			{
				return SqlDateTime.FromDateTime(SqlDateTime.ToDateTime(x) - t);
			}
			return SqlDateTime.Null;
		}

		public static SqlDateTime Add(SqlDateTime x, TimeSpan t)
		{
			return x + t;
		}

		public static SqlDateTime Subtract(SqlDateTime x, TimeSpan t)
		{
			return x - t;
		}

		public static explicit operator SqlDateTime(SqlString x)
		{
			if (!x.IsNull)
			{
				return SqlDateTime.Parse(x.Value);
			}
			return SqlDateTime.Null;
		}

		private static bool IsLeapYear(int year)
		{
			return year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
		}

		public static SqlBoolean operator ==(SqlDateTime x, SqlDateTime y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_day == y.m_day && x.m_time == y.m_time);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator !=(SqlDateTime x, SqlDateTime y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlDateTime x, SqlDateTime y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_day < y.m_day || (x.m_day == y.m_day && x.m_time < y.m_time));
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >(SqlDateTime x, SqlDateTime y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_day > y.m_day || (x.m_day == y.m_day && x.m_time > y.m_time));
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator <=(SqlDateTime x, SqlDateTime y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_day < y.m_day || (x.m_day == y.m_day && x.m_time <= y.m_time));
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >=(SqlDateTime x, SqlDateTime y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_day > y.m_day || (x.m_day == y.m_day && x.m_time >= y.m_time));
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean Equals(SqlDateTime x, SqlDateTime y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlDateTime x, SqlDateTime y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlDateTime x, SqlDateTime y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlDateTime x, SqlDateTime y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDateTime x, SqlDateTime y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDateTime x, SqlDateTime y)
		{
			return x >= y;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public int CompareTo(object value)
		{
			if (value is SqlDateTime)
			{
				SqlDateTime sqlDateTime = (SqlDateTime)value;
				return this.CompareTo(sqlDateTime);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlDateTime));
		}

		public int CompareTo(SqlDateTime value)
		{
			if (this.IsNull)
			{
				if (!value.IsNull)
				{
					return -1;
				}
				return 0;
			}
			else
			{
				if (value.IsNull)
				{
					return 1;
				}
				if (this < value)
				{
					return -1;
				}
				if (this > value)
				{
					return 1;
				}
				return 0;
			}
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDateTime))
			{
				return false;
			}
			SqlDateTime sqlDateTime = (SqlDateTime)value;
			if (sqlDateTime.IsNull || this.IsNull)
			{
				return sqlDateTime.IsNull && this.IsNull;
			}
			return (this == sqlDateTime).Value;
		}

		public override int GetHashCode()
		{
			if (!this.IsNull)
			{
				return this.Value.GetHashCode();
			}
			return 0;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			string attribute = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null && XmlConvert.ToBoolean(attribute))
			{
				reader.ReadElementString();
				this.m_fNotNull = false;
				return;
			}
			DateTime dateTime = XmlConvert.ToDateTime(reader.ReadElementString(), XmlDateTimeSerializationMode.RoundtripKind);
			if (dateTime.Kind != DateTimeKind.Unspecified)
			{
				throw new SqlTypeException(SQLResource.TimeZoneSpecifiedMessage);
			}
			SqlDateTime sqlDateTime = SqlDateTime.FromDateTime(dateTime);
			this.m_day = sqlDateTime.DayTicks;
			this.m_time = sqlDateTime.TimeTicks;
			this.m_fNotNull = true;
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(XmlConvert.ToString(this.Value, SqlDateTime.s_ISO8601_DateTimeFormat));
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");
		}

		private bool m_fNotNull;

		private int m_day;

		private int m_time;

		private static readonly double s_SQLTicksPerMillisecond = 0.3;

		public static readonly int SQLTicksPerSecond = 300;

		public static readonly int SQLTicksPerMinute = SqlDateTime.SQLTicksPerSecond * 60;

		public static readonly int SQLTicksPerHour = SqlDateTime.SQLTicksPerMinute * 60;

		private static readonly int s_SQLTicksPerDay = SqlDateTime.SQLTicksPerHour * 24;

		private static readonly long s_ticksPerSecond = 10000000L;

		private static readonly DateTime s_SQLBaseDate = new DateTime(1900, 1, 1);

		private static readonly long s_SQLBaseDateTicks = SqlDateTime.s_SQLBaseDate.Ticks;

		private static readonly int s_minYear = 1753;

		private static readonly int s_maxYear = 9999;

		private static readonly int s_minDay = -53690;

		private static readonly int s_maxDay = 2958463;

		private static readonly int s_minTime = 0;

		private static readonly int s_maxTime = SqlDateTime.s_SQLTicksPerDay - 1;

		private static readonly int s_dayBase = 693595;

		private static readonly int[] s_daysToMonth365 = new int[]
		{
			0, 31, 59, 90, 120, 151, 181, 212, 243, 273,
			304, 334, 365
		};

		private static readonly int[] s_daysToMonth366 = new int[]
		{
			0, 31, 60, 91, 121, 152, 182, 213, 244, 274,
			305, 335, 366
		};

		private static readonly DateTime s_minDateTime = new DateTime(1753, 1, 1);

		private static readonly DateTime s_maxDateTime = DateTime.MaxValue;

		private static readonly TimeSpan s_minTimeSpan = SqlDateTime.s_minDateTime.Subtract(SqlDateTime.s_SQLBaseDate);

		private static readonly TimeSpan s_maxTimeSpan = SqlDateTime.s_maxDateTime.Subtract(SqlDateTime.s_SQLBaseDate);

		private static readonly string s_ISO8601_DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fff";

		private static readonly string[] s_dateTimeFormats = new string[] { "MMM d yyyy hh:mm:ss:ffftt", "MMM d yyyy hh:mm:ss:fff", "d MMM yyyy hh:mm:ss:ffftt", "d MMM yyyy hh:mm:ss:fff", "hh:mm:ss:ffftt", "hh:mm:ss:fff", "yyMMdd", "yyyyMMdd" };

		private const DateTimeStyles x_DateTimeStyle = DateTimeStyles.AllowWhiteSpaces;

		public static readonly SqlDateTime MinValue = new SqlDateTime(SqlDateTime.s_minDay, 0);

		public static readonly SqlDateTime MaxValue = new SqlDateTime(SqlDateTime.s_maxDay, SqlDateTime.s_maxTime);

		public static readonly SqlDateTime Null = new SqlDateTime(true);
	}
}
