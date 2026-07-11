using System;
using System.Data.Common;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlInt32 : INullable, IComparable, IXmlSerializable
	{
		private SqlInt32(bool fNull)
		{
			this.m_fNotNull = false;
			this.m_value = 0;
		}

		public SqlInt32(int value)
		{
			this.m_value = value;
			this.m_fNotNull = true;
		}

		public bool IsNull
		{
			get
			{
				return !this.m_fNotNull;
			}
		}

		public int Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return this.m_value;
			}
		}

		public static implicit operator SqlInt32(int x)
		{
			return new SqlInt32(x);
		}

		public static explicit operator int(SqlInt32 x)
		{
			return x.Value;
		}

		public override string ToString()
		{
			if (!this.IsNull)
			{
				return this.m_value.ToString(null);
			}
			return SQLResource.NullString;
		}

		public static SqlInt32 Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32(int.Parse(s, null));
		}

		public static SqlInt32 operator -(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32(-x.m_value);
			}
			return SqlInt32.Null;
		}

		public static SqlInt32 operator ~(SqlInt32 x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32(~x.m_value);
			}
			return SqlInt32.Null;
		}

		public static SqlInt32 operator +(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlInt32.Null;
			}
			int num = x.m_value + y.m_value;
			if (SqlInt32.SameSignInt(x.m_value, y.m_value) && !SqlInt32.SameSignInt(x.m_value, num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32(num);
		}

		public static SqlInt32 operator -(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlInt32.Null;
			}
			int num = x.m_value - y.m_value;
			if (!SqlInt32.SameSignInt(x.m_value, y.m_value) && SqlInt32.SameSignInt(y.m_value, num))
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32(num);
		}

		public static SqlInt32 operator *(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlInt32.Null;
			}
			long num = (long)x.m_value * (long)y.m_value;
			long num2 = num & SqlInt32.s_lBitNotIntMax;
			if (num2 != 0L && num2 != SqlInt32.s_lBitNotIntMax)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32((int)num);
		}

		public static SqlInt32 operator /(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlInt32.Null;
			}
			if (y.m_value == 0)
			{
				throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
			}
			if ((long)x.m_value == SqlInt32.s_iIntMin && y.m_value == -1)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32(x.m_value / y.m_value);
		}

		public static SqlInt32 operator %(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlInt32.Null;
			}
			if (y.m_value == 0)
			{
				throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
			}
			if ((long)x.m_value == SqlInt32.s_iIntMin && y.m_value == -1)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32(x.m_value % y.m_value);
		}

		public static SqlInt32 operator &(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlInt32(x.m_value & y.m_value);
			}
			return SqlInt32.Null;
		}

		public static SqlInt32 operator |(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlInt32(x.m_value | y.m_value);
			}
			return SqlInt32.Null;
		}

		public static SqlInt32 operator ^(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlInt32(x.m_value ^ y.m_value);
			}
			return SqlInt32.Null;
		}

		public static explicit operator SqlInt32(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32((int)x.ByteValue);
			}
			return SqlInt32.Null;
		}

		public static implicit operator SqlInt32(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32((int)x.Value);
			}
			return SqlInt32.Null;
		}

		public static implicit operator SqlInt32(SqlInt16 x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32((int)x.Value);
			}
			return SqlInt32.Null;
		}

		public static explicit operator SqlInt32(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			long value = x.Value;
			if (value > 2147483647L || value < -2147483648L)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32((int)value);
		}

		public static explicit operator SqlInt32(SqlSingle x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			float value = x.Value;
			if (value > 2.1474836E+09f || value < -2.1474836E+09f)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32((int)value);
		}

		public static explicit operator SqlInt32(SqlDouble x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			double value = x.Value;
			if (value > 2147483647.0 || value < -2147483648.0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlInt32((int)value);
		}

		public static explicit operator SqlInt32(SqlMoney x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32(x.ToInt32());
			}
			return SqlInt32.Null;
		}

		public static explicit operator SqlInt32(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			x.AdjustScale((int)(-(int)x.Scale), true);
			long num = (long)((ulong)x._data1);
			if (!x.IsPositive)
			{
				num = -num;
			}
			if (x._bLen > 1 || num > 2147483647L || num < -2147483648L)
			{
				throw new OverflowException(SQLResource.ConversionOverflowMessage);
			}
			return new SqlInt32((int)num);
		}

		public static explicit operator SqlInt32(SqlString x)
		{
			if (!x.IsNull)
			{
				return new SqlInt32(int.Parse(x.Value, null));
			}
			return SqlInt32.Null;
		}

		private static bool SameSignInt(int x, int y)
		{
			return ((long)(x ^ y) & (long)((ulong)int.MinValue)) == 0L;
		}

		public static SqlBoolean operator ==(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value == y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator !=(SqlInt32 x, SqlInt32 y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value < y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value > y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator <=(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value <= y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >=(SqlInt32 x, SqlInt32 y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value >= y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlInt32 OnesComplement(SqlInt32 x)
		{
			return ~x;
		}

		public static SqlInt32 Add(SqlInt32 x, SqlInt32 y)
		{
			return x + y;
		}

		public static SqlInt32 Subtract(SqlInt32 x, SqlInt32 y)
		{
			return x - y;
		}

		public static SqlInt32 Multiply(SqlInt32 x, SqlInt32 y)
		{
			return x * y;
		}

		public static SqlInt32 Divide(SqlInt32 x, SqlInt32 y)
		{
			return x / y;
		}

		public static SqlInt32 Mod(SqlInt32 x, SqlInt32 y)
		{
			return x % y;
		}

		public static SqlInt32 Modulus(SqlInt32 x, SqlInt32 y)
		{
			return x % y;
		}

		public static SqlInt32 BitwiseAnd(SqlInt32 x, SqlInt32 y)
		{
			return x & y;
		}

		public static SqlInt32 BitwiseOr(SqlInt32 x, SqlInt32 y)
		{
			return x | y;
		}

		public static SqlInt32 Xor(SqlInt32 x, SqlInt32 y)
		{
			return x ^ y;
		}

		public static SqlBoolean Equals(SqlInt32 x, SqlInt32 y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlInt32 x, SqlInt32 y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlInt32 x, SqlInt32 y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlInt32 x, SqlInt32 y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlInt32 x, SqlInt32 y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlInt32 x, SqlInt32 y)
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

		public SqlInt64 ToSqlInt64()
		{
			return this;
		}

		public SqlMoney ToSqlMoney()
		{
			return this;
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
			if (value is SqlInt32)
			{
				SqlInt32 sqlInt = (SqlInt32)value;
				return this.CompareTo(sqlInt);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlInt32));
		}

		public int CompareTo(SqlInt32 value)
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
			if (!(value is SqlInt32))
			{
				return false;
			}
			SqlInt32 sqlInt = (SqlInt32)value;
			if (sqlInt.IsNull || this.IsNull)
			{
				return sqlInt.IsNull && this.IsNull;
			}
			return (this == sqlInt).Value;
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
			this.m_value = XmlConvert.ToInt32(reader.ReadElementString());
			this.m_fNotNull = true;
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(XmlConvert.ToString(this.m_value));
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema");
		}

		private bool m_fNotNull;

		private int m_value;

		private static readonly long s_iIntMin = -2147483648L;

		private static readonly long s_lBitNotIntMax = -2147483648L;

		public static readonly SqlInt32 Null = new SqlInt32(true);

		public static readonly SqlInt32 Zero = new SqlInt32(0);

		public static readonly SqlInt32 MinValue = new SqlInt32(int.MinValue);

		public static readonly SqlInt32 MaxValue = new SqlInt32(int.MaxValue);
	}
}
