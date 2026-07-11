using System;
using System.Data.Common;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlByte : INullable, IComparable, IXmlSerializable
	{
		private SqlByte(bool fNull)
		{
			this.m_fNotNull = false;
			this.m_value = 0;
		}

		public SqlByte(byte value)
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

		public byte Value
		{
			get
			{
				if (this.m_fNotNull)
				{
					return this.m_value;
				}
				throw new SqlNullValueException();
			}
		}

		public static implicit operator SqlByte(byte x)
		{
			return new SqlByte(x);
		}

		public static explicit operator byte(SqlByte x)
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

		public static SqlByte Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlByte.Null;
			}
			return new SqlByte(byte.Parse(s, null));
		}

		public static SqlByte operator ~(SqlByte x)
		{
			if (!x.IsNull)
			{
				return new SqlByte(~x.m_value);
			}
			return SqlByte.Null;
		}

		public static SqlByte operator +(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlByte.Null;
			}
			int num = (int)(x.m_value + y.m_value);
			if ((num & SqlByte.s_iBitNotByteMax) != 0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlByte((byte)num);
		}

		public static SqlByte operator -(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlByte.Null;
			}
			int num = (int)(x.m_value - y.m_value);
			if ((num & SqlByte.s_iBitNotByteMax) != 0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlByte((byte)num);
		}

		public static SqlByte operator *(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlByte.Null;
			}
			int num = (int)(x.m_value * y.m_value);
			if ((num & SqlByte.s_iBitNotByteMax) != 0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			return new SqlByte((byte)num);
		}

		public static SqlByte operator /(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlByte.Null;
			}
			if (y.m_value != 0)
			{
				return new SqlByte(x.m_value / y.m_value);
			}
			throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
		}

		public static SqlByte operator %(SqlByte x, SqlByte y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlByte.Null;
			}
			if (y.m_value != 0)
			{
				return new SqlByte(x.m_value % y.m_value);
			}
			throw new DivideByZeroException(SQLResource.DivideByZeroMessage);
		}

		public static SqlByte operator &(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlByte(x.m_value & y.m_value);
			}
			return SqlByte.Null;
		}

		public static SqlByte operator |(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlByte(x.m_value | y.m_value);
			}
			return SqlByte.Null;
		}

		public static SqlByte operator ^(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlByte(x.m_value ^ y.m_value);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlBoolean x)
		{
			if (!x.IsNull)
			{
				return new SqlByte(x.ByteValue);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlMoney x)
		{
			if (!x.IsNull)
			{
				return new SqlByte(checked((byte)x.ToInt32()));
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return SqlByte.Null;
			}
			if (x.Value > 255 || x.Value < 0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			if (!x.IsNull)
			{
				return new SqlByte((byte)x.Value);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return SqlByte.Null;
			}
			if (x.Value > 255 || x.Value < 0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			if (!x.IsNull)
			{
				return new SqlByte((byte)x.Value);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return SqlByte.Null;
			}
			if (x.Value > 255L || x.Value < 0L)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			if (!x.IsNull)
			{
				return new SqlByte((byte)x.Value);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlSingle x)
		{
			if (x.IsNull)
			{
				return SqlByte.Null;
			}
			if (x.Value > 255f || x.Value < 0f)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			if (!x.IsNull)
			{
				return new SqlByte((byte)x.Value);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlDouble x)
		{
			if (x.IsNull)
			{
				return SqlByte.Null;
			}
			if (x.Value > 255.0 || x.Value < 0.0)
			{
				throw new OverflowException(SQLResource.ArithOverflowMessage);
			}
			if (!x.IsNull)
			{
				return new SqlByte((byte)x.Value);
			}
			return SqlByte.Null;
		}

		public static explicit operator SqlByte(SqlDecimal x)
		{
			return (SqlByte)((SqlInt32)x);
		}

		public static explicit operator SqlByte(SqlString x)
		{
			if (!x.IsNull)
			{
				return new SqlByte(byte.Parse(x.Value, null));
			}
			return SqlByte.Null;
		}

		public static SqlBoolean operator ==(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value == y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator !=(SqlByte x, SqlByte y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value < y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value > y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator <=(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value <= y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >=(SqlByte x, SqlByte y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(x.m_value >= y.m_value);
			}
			return SqlBoolean.Null;
		}

		public static SqlByte OnesComplement(SqlByte x)
		{
			return ~x;
		}

		public static SqlByte Add(SqlByte x, SqlByte y)
		{
			return x + y;
		}

		public static SqlByte Subtract(SqlByte x, SqlByte y)
		{
			return x - y;
		}

		public static SqlByte Multiply(SqlByte x, SqlByte y)
		{
			return x * y;
		}

		public static SqlByte Divide(SqlByte x, SqlByte y)
		{
			return x / y;
		}

		public static SqlByte Mod(SqlByte x, SqlByte y)
		{
			return x % y;
		}

		public static SqlByte Modulus(SqlByte x, SqlByte y)
		{
			return x % y;
		}

		public static SqlByte BitwiseAnd(SqlByte x, SqlByte y)
		{
			return x & y;
		}

		public static SqlByte BitwiseOr(SqlByte x, SqlByte y)
		{
			return x | y;
		}

		public static SqlByte Xor(SqlByte x, SqlByte y)
		{
			return x ^ y;
		}

		public static SqlBoolean Equals(SqlByte x, SqlByte y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlByte x, SqlByte y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlByte x, SqlByte y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlByte x, SqlByte y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlByte x, SqlByte y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlByte x, SqlByte y)
		{
			return x >= y;
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlDouble ToSqlDouble()
		{
			return this;
		}

		public SqlInt16 ToSqlInt16()
		{
			return this;
		}

		public SqlInt32 ToSqlInt32()
		{
			return this;
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
			if (value is SqlByte)
			{
				SqlByte sqlByte = (SqlByte)value;
				return this.CompareTo(sqlByte);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlByte));
		}

		public int CompareTo(SqlByte value)
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
			if (!(value is SqlByte))
			{
				return false;
			}
			SqlByte sqlByte = (SqlByte)value;
			if (sqlByte.IsNull || this.IsNull)
			{
				return sqlByte.IsNull && this.IsNull;
			}
			return (this == sqlByte).Value;
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
			this.m_value = XmlConvert.ToByte(reader.ReadElementString());
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
			return new XmlQualifiedName("unsignedByte", "http://www.w3.org/2001/XMLSchema");
		}

		private bool m_fNotNull;

		private byte m_value;

		private static readonly int s_iBitNotByteMax = -256;

		public static readonly SqlByte Null = new SqlByte(true);

		public static readonly SqlByte Zero = new SqlByte(0);

		public static readonly SqlByte MinValue = new SqlByte(0);

		public static readonly SqlByte MaxValue = new SqlByte(byte.MaxValue);
	}
}
