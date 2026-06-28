using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlDateTime : IXmlSerializable, IComparable, INullable
	{
		public SqlDateTime(DateTime value)
		{
			this.value = value;
			this.notNull = true;
			SqlDateTime.CheckRange(this);
		}

		public SqlDateTime(int dayTicks, int timeTicks)
		{
			try
			{
				long num = SqlDateTime.SQLTicksToMilliseconds(timeTicks);
				this.value = SqlDateTime.zero_day.AddDays((double)dayTicks).AddMilliseconds((double)num);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			this.notNull = true;
			SqlDateTime.CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day)
		{
			try
			{
				this.value = new DateTime(year, month, day);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			this.notNull = true;
			SqlDateTime.CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second)
		{
			try
			{
				this.value = new DateTime(year, month, day, hour, minute, second);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			this.notNull = true;
			SqlDateTime.CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second, double millisecond)
		{
			try
			{
				long num = (long)(millisecond * 10000.0);
				long num2 = SqlDateTime.SQLTicksToMilliseconds(SqlDateTime.TimeSpanTicksToSQLTicks(num));
				DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
				this.value = dateTime.AddMilliseconds((double)num2);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			this.notNull = true;
			SqlDateTime.CheckRange(this);
		}

		public SqlDateTime(int year, int month, int day, int hour, int minute, int second, int bilisecond)
		{
			try
			{
				long num = (long)(bilisecond * 10);
				long num2 = SqlDateTime.SQLTicksToMilliseconds(SqlDateTime.TimeSpanTicksToSQLTicks(num));
				DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
				this.value = dateTime.AddMilliseconds((double)num2);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new SqlTypeException(ex.Message);
			}
			this.notNull = true;
			SqlDateTime.CheckRange(this);
		}

		static SqlDateTime()
		{
			DateTime dateTime = new DateTime(9999, 12, 31, 23, 59, 59);
			long num = dateTime.Ticks + 9970000L;
			SqlDateTime.MaxValue.value = new DateTime(num);
			SqlDateTime.MaxValue.notNull = true;
			SqlDateTime.MinValue.value = new DateTime(1753, 1, 1);
			SqlDateTime.MinValue.notNull = true;
		}

		[MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		private static int TimeSpanTicksToSQLTicks(long ticks)
		{
			return (int)(ticks * (long)SqlDateTime.SQLTicksPerSecond / 10000000L);
		}

		private static long SQLTicksToMilliseconds(int timeTicks)
		{
			return (long)((double)timeTicks * 1000.0 / (double)SqlDateTime.SQLTicksPerSecond + 0.5);
		}

		public int DayTicks
		{
			get
			{
				return (this.Value - SqlDateTime.zero_day).Days;
			}
		}

		public bool IsNull
		{
			get
			{
				return !this.notNull;
			}
		}

		public int TimeTicks
		{
			get
			{
				return SqlDateTime.TimeSpanTicksToSQLTicks(this.Value.TimeOfDay.Ticks);
			}
		}

		public DateTime Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				return this.value;
			}
		}

		private static void CheckRange(SqlDateTime target)
		{
			if (target.IsNull)
			{
				return;
			}
			if (target.value > SqlDateTime.MaxValue.value || target.value < SqlDateTime.MinValue.value)
			{
				throw new SqlTypeException(string.Format("SqlDateTime overflow. Must be between {0} and {1}. Value was {2}", SqlDateTime.MinValue.Value, SqlDateTime.MaxValue.Value, target.value));
			}
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlDateTime))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Data.SqlTypes.SqlDateTime"));
			}
			return this.CompareTo((SqlDateTime)value);
		}

		public int CompareTo(SqlDateTime value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlDateTime))
			{
				return false;
			}
			if (this.IsNull)
			{
				return ((SqlDateTime)value).IsNull;
			}
			return !((SqlDateTime)value).IsNull && (bool)(this == (SqlDateTime)value);
		}

		public static SqlBoolean Equals(SqlDateTime x, SqlDateTime y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public static SqlDateTime Add(SqlDateTime x, TimeSpan t)
		{
			return x + t;
		}

		public static SqlDateTime Subtract(SqlDateTime x, TimeSpan t)
		{
			return x - t;
		}

		public static SqlBoolean GreaterThan(SqlDateTime x, SqlDateTime y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlDateTime x, SqlDateTime y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlDateTime x, SqlDateTime y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlDateTime x, SqlDateTime y)
		{
			return x <= y;
		}

		public static SqlBoolean NotEquals(SqlDateTime x, SqlDateTime y)
		{
			return x != y;
		}

		public static SqlDateTime Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("Argument cannot be null");
			}
			DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
			try
			{
				return new SqlDateTime(DateTime.Parse(s, currentInfo));
			}
			catch (Exception)
			{
			}
			try
			{
				return new SqlDateTime(DateTime.Parse(s, CultureInfo.InvariantCulture));
			}
			catch (Exception)
			{
			}
			throw new FormatException(string.Format("String {0} is not recognized as valid DateTime.", s));
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return "Null";
			}
			return this.value.ToString(CultureInfo.InvariantCulture);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlDateTime operator +(SqlDateTime x, TimeSpan t)
		{
			if (x.IsNull)
			{
				return SqlDateTime.Null;
			}
			return new SqlDateTime(x.Value + t);
		}

		public static SqlBoolean operator ==(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(!(x.Value == y.Value));
		}

		public static SqlBoolean operator <(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlDateTime x, SqlDateTime y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlDateTime operator -(SqlDateTime x, TimeSpan t)
		{
			if (x.IsNull)
			{
				return x;
			}
			return new SqlDateTime(x.Value - t);
		}

		public static explicit operator DateTime(SqlDateTime x)
		{
			return x.Value;
		}

		public static explicit operator SqlDateTime(SqlString x)
		{
			return SqlDateTime.Parse(x.Value);
		}

		public static implicit operator SqlDateTime(DateTime value)
		{
			return new SqlDateTime(value);
		}

		private DateTime value;

		private bool notNull;

		public static readonly SqlDateTime MaxValue;

		public static readonly SqlDateTime MinValue;

		public static readonly SqlDateTime Null;

		public static readonly int SQLTicksPerHour = 1080000;

		public static readonly int SQLTicksPerMinute = 18000;

		public static readonly int SQLTicksPerSecond = 300;

		private static readonly DateTime zero_day = new DateTime(1900, 1, 1);
	}
}
