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
	public struct SqlMoney : INullable, IComparable, IXmlSerializable
	{
		private SqlMoney(bool fNull)
		{
			this._fNotNull = false;
			this._value = 0L;
		}

		internal SqlMoney(long value, int ignored)
		{
			this._value = value;
			this._fNotNull = true;
		}

		public SqlMoney(int value)
		{
			this._value = (long)value * SqlMoney.s_lTickBase;
			this._fNotNull = true;
		}

		public SqlMoney(long value)
		{
			if (value < SqlMoney.s_minLong || value > SqlMoney.s_maxLong)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			this._value = value * SqlMoney.s_lTickBase;
			this._fNotNull = true;
		}

		public SqlMoney(decimal value)
		{
			SqlDecimal sqlDecimal = new SqlDecimal(value);
			sqlDecimal.AdjustScale(SqlMoney.s_iMoneyScale - (int)sqlDecimal.Scale, true);
			if (sqlDecimal._data3 != 0U || sqlDecimal._data4 != 0U)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			bool isPositive = sqlDecimal.IsPositive;
			ulong num = (ulong)sqlDecimal._data1 + ((ulong)sqlDecimal._data2 << 32);
			if ((isPositive && num > 9223372036854775807UL) || (!isPositive && num > 9223372036854775808UL))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			this._value = (long)(isPositive ? num : (-(long)num));
			this._fNotNull = true;
		}

		public SqlMoney(double value)
		{
			this = new SqlMoney(new decimal(value));
		}

		public bool IsNull
		{
			get
			{
				return !this._fNotNull;
			}
		}

		public decimal Value
		{
			get
			{
				if (this._fNotNull)
				{
					return this.ToDecimal();
				}
				throw new SqlNullValueException();
			}
		}

		public decimal ToDecimal()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			bool flag = false;
			long num = this._value;
			if (this._value < 0L)
			{
				flag = true;
				num = -this._value;
			}
			return new decimal((int)num, (int)(num >> 32), 0, flag, (byte)SqlMoney.s_iMoneyScale);
		}

		public long ToInt64()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			long num = this._value / (SqlMoney.s_lTickBase / 10L);
			bool flag = num >= 0L;
			long num2 = num % 10L;
			num /= 10L;
			if (num2 >= 5L)
			{
				if (flag)
				{
					num += 1L;
				}
				else
				{
					num -= 1L;
				}
			}
			return num;
		}

		internal long ToSqlInternalRepresentation()
		{
			if (this.IsNull)
			{
				throw new SqlNullValueException();
			}
			return this._value;
		}

		public int ToInt32()
		{
			return checked((int)this.ToInt64());
		}

		public double ToDouble()
		{
			return decimal.ToDouble(this.ToDecimal());
		}

		public static implicit operator SqlMoney(decimal x)
		{
			return new SqlMoney(x);
		}

		public static explicit operator SqlMoney(double x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(long x)
		{
			return new SqlMoney(new decimal(x));
		}

		public static explicit operator decimal(SqlMoney x)
		{
			return x.Value;
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return SQLResource.NullString;
			}
			return this.ToDecimal().ToString("#0.00##", null);
		}

		public static SqlMoney Parse(string s)
		{
			SqlMoney @null;
			decimal num;
			if (s == SQLResource.NullString)
			{
				@null = SqlMoney.Null;
			}
			else if (decimal.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowParentheses | NumberStyles.AllowDecimalPoint | NumberStyles.AllowCurrencySymbol, NumberFormatInfo.InvariantInfo, out num))
			{
				@null = new SqlMoney(num);
			}
			else
			{
				@null = new SqlMoney(decimal.Parse(s, NumberStyles.Currency, NumberFormatInfo.CurrentInfo));
			}
			return @null;
		}

		public static SqlMoney operator -(SqlMoney x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			if (x._value == SqlMoney.s_minLong)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlMoney(-x._value, 0);
		}

		public static SqlMoney operator +(SqlMoney x, SqlMoney y)
		{
			SqlMoney sqlMoney;
			try
			{
				sqlMoney = ((x.IsNull || y.IsNull) ? SqlMoney.Null : new SqlMoney(checked(x._value + y._value), 0));
			}
			catch (OverflowException)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return sqlMoney;
		}

		public static SqlMoney operator -(SqlMoney x, SqlMoney y)
		{
			SqlMoney sqlMoney;
			try
			{
				sqlMoney = ((x.IsNull || y.IsNull) ? SqlMoney.Null : new SqlMoney(checked(x._value - y._value), 0));
			}
			catch (OverflowException)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return sqlMoney;
		}

		public static SqlMoney operator *(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlMoney(decimal.Multiply(x.ToDecimal(), y.ToDecimal()));
			}
			return SqlMoney.Null;
		}

		public static SqlMoney operator /(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlMoney(decimal.Divide(x.ToDecimal(), y.ToDecimal()));
			}
			return SqlMoney.Null;
		}

		public static explicit operator SqlMoney(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney((int)x.ByteValue);
			}
			return SqlMoney.Null;
		}

		public static implicit operator SqlMoney(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney((int)x.Value);
			}
			return SqlMoney.Null;
		}

		public static implicit operator SqlMoney(SqlInt16 x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney((int)x.Value);
			}
			return SqlMoney.Null;
		}

		public static implicit operator SqlMoney(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney(x.Value);
			}
			return SqlMoney.Null;
		}

		public static implicit operator SqlMoney(SqlInt64 x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney(x.Value);
			}
			return SqlMoney.Null;
		}

		public static explicit operator SqlMoney(SqlSingle x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney((double)x.Value);
			}
			return SqlMoney.Null;
		}

		public static explicit operator SqlMoney(SqlDouble x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney(x.Value);
			}
			return SqlMoney.Null;
		}

		public static explicit operator SqlMoney(SqlDecimal x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney(x.Value);
			}
			return SqlMoney.Null;
		}

		public static explicit operator SqlMoney(SqlString x)
		{
			if (!x.IsNull)
			{
				return new SqlMoney(decimal.Parse(x.Value, NumberStyles.Currency, null));
			}
			return SqlMoney.Null;
		}

		public static SqlBoolean operator ==(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value == y._value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator !=(SqlMoney x, SqlMoney y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value < y._value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value > y._value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator <=(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value <= y._value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >=(SqlMoney x, SqlMoney y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x._value >= y._value);
			}
			return SqlBoolean.Null;
		}

		public static SqlMoney Add(SqlMoney x, SqlMoney y)
		{
			return x + y;
		}

		public static SqlMoney Subtract(SqlMoney x, SqlMoney y)
		{
			return x - y;
		}

		public static SqlMoney Multiply(SqlMoney x, SqlMoney y)
		{
			return x * y;
		}

		public static SqlMoney Divide(SqlMoney x, SqlMoney y)
		{
			return x / y;
		}

		public static SqlBoolean Equals(SqlMoney x, SqlMoney y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlMoney x, SqlMoney y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlMoney x, SqlMoney y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlMoney x, SqlMoney y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlMoney x, SqlMoney y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlMoney x, SqlMoney y)
		{
			return x >= y;
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		public SqlDouble ToSqlDouble()
		{
			return this;
		}

		public SqlInt16 ToSqlInt16()
		{
			return (SqlInt16)this;
		}

		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
		}

		public SqlDecimal ToSqlDecimal()
		{
			return this;
		}

		public SqlSingle ToSqlSingle()
		{
			return this;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public int CompareTo(object value)
		{
			if (value is SqlMoney)
			{
				SqlMoney sqlMoney = (SqlMoney)value;
				return this.CompareTo(sqlMoney);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlMoney));
		}

		public int CompareTo(SqlMoney value)
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
			if (!(value is SqlMoney))
			{
				return false;
			}
			SqlMoney sqlMoney = (SqlMoney)value;
			if (sqlMoney.IsNull || this.IsNull)
			{
				return sqlMoney.IsNull && this.IsNull;
			}
			return (this == sqlMoney).Value;
		}

		public override int GetHashCode()
		{
			if (!this.IsNull)
			{
				return this._value.GetHashCode();
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
				this._fNotNull = false;
				return;
			}
			SqlMoney sqlMoney = new SqlMoney(XmlConvert.ToDecimal(reader.ReadElementString()));
			this._fNotNull = sqlMoney._fNotNull;
			this._value = sqlMoney._value;
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(XmlConvert.ToString(this.ToDecimal()));
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
		}

		private bool _fNotNull;

		private long _value;

		internal static readonly int s_iMoneyScale = 4;

		private static readonly long s_lTickBase = 10000L;

		private static readonly double s_dTickBase = (double)SqlMoney.s_lTickBase;

		private static readonly long s_minLong = long.MinValue / SqlMoney.s_lTickBase;

		private static readonly long s_maxLong = long.MaxValue / SqlMoney.s_lTickBase;

		public static readonly SqlMoney Null = new SqlMoney(true);

		public static readonly SqlMoney Zero = new SqlMoney(0);

		public static readonly SqlMoney MinValue = new SqlMoney(long.MinValue, 0);

		public static readonly SqlMoney MaxValue = new SqlMoney(long.MaxValue, 0);
	}
}
