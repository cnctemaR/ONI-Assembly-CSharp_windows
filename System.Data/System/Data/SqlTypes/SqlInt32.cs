using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlInt32 : IXmlSerializable, IComparable, INullable
	{
		public SqlInt32(int value)
		{
			this.value = value;
			this.notNull = true;
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader == null)
			{
				return;
			}
			switch (reader.ReadState)
			{
			case ReadState.Error:
			case ReadState.EndOfFile:
			case ReadState.Closed:
				return;
			default:
				reader.MoveToContent();
				if (reader.EOF)
				{
					return;
				}
				reader.Read();
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					return;
				}
				if (reader.Value.Length > 0)
				{
					if (string.Compare("Null", reader.Value) == 0)
					{
						this.notNull = false;
						return;
					}
					this.value = int.Parse(reader.Value);
					this.notNull = true;
				}
				return;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(this.ToString());
		}

		public bool IsNull
		{
			get
			{
				return !this.notNull;
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
				return this.value;
			}
		}

		public static SqlInt32 Add(SqlInt32 x, SqlInt32 y)
		{
			return x + y;
		}

		public static SqlInt32 BitwiseAnd(SqlInt32 x, SqlInt32 y)
		{
			return x & y;
		}

		public static SqlInt32 BitwiseOr(SqlInt32 x, SqlInt32 y)
		{
			return x | y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlInt32))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Data.SqlTypes.SqlInt32"));
			}
			return this.CompareSqlInt32((SqlInt32)value);
		}

		public int CompareTo(SqlInt32 value)
		{
			return this.CompareSqlInt32(value);
		}

		private int CompareSqlInt32(SqlInt32 value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public static SqlInt32 Divide(SqlInt32 x, SqlInt32 y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlInt32))
			{
				return false;
			}
			if (this.IsNull)
			{
				return ((SqlInt32)value).IsNull;
			}
			return !((SqlInt32)value).IsNull && (bool)(this == (SqlInt32)value);
		}

		public static SqlBoolean Equals(SqlInt32 x, SqlInt32 y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return this.value;
		}

		public static SqlBoolean GreaterThan(SqlInt32 x, SqlInt32 y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlInt32 x, SqlInt32 y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlInt32 x, SqlInt32 y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlInt32 x, SqlInt32 y)
		{
			return x <= y;
		}

		public static SqlInt32 Mod(SqlInt32 x, SqlInt32 y)
		{
			return x % y;
		}

		public static SqlInt32 Modulus(SqlInt32 x, SqlInt32 y)
		{
			return x % y;
		}

		public static SqlInt32 Multiply(SqlInt32 x, SqlInt32 y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlInt32 x, SqlInt32 y)
		{
			return x != y;
		}

		public static SqlInt32 OnesComplement(SqlInt32 x)
		{
			return ~x;
		}

		public static SqlInt32 Parse(string s)
		{
			return new SqlInt32(int.Parse(s));
		}

		public static SqlInt32 Subtract(SqlInt32 x, SqlInt32 y)
		{
			return x - y;
		}

		public SqlBoolean ToSqlBoolean()
		{
			return (SqlBoolean)this;
		}

		public SqlByte ToSqlByte()
		{
			return (SqlByte)this;
		}

		public SqlDecimal ToSqlDecimal()
		{
			return this;
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

		public SqlSingle ToSqlSingle()
		{
			return this;
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
			return this.value.ToString();
		}

		public static SqlInt32 Xor(SqlInt32 x, SqlInt32 y)
		{
			return x ^ y;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			if (schemaSet != null && schemaSet.Count == 0)
			{
				XmlSchema xmlSchema = new XmlSchema();
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = "int";
				xmlSchema.Items.Add(xmlSchemaComplexType);
				schemaSet.Add(xmlSchema);
			}
			return new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlInt32 operator +(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(checked(x.Value + y.Value));
		}

		public static SqlInt32 operator &(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(x.Value & y.Value);
		}

		public static SqlInt32 operator |(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(x.Value | y.Value);
		}

		public static SqlInt32 operator /(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(x.Value / y.Value);
		}

		public static SqlBoolean operator ==(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlInt32 operator ^(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(x.Value ^ y.Value);
		}

		public static SqlBoolean operator >(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value != y.Value);
		}

		public static SqlBoolean operator <(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlInt32 x, SqlInt32 y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlInt32 operator %(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(x.Value % y.Value);
		}

		public static SqlInt32 operator *(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(checked(x.Value * y.Value));
		}

		public static SqlInt32 operator ~(SqlInt32 x)
		{
			return new SqlInt32(~x.Value);
		}

		public static SqlInt32 operator -(SqlInt32 x, SqlInt32 y)
		{
			return new SqlInt32(checked(x.Value - y.Value));
		}

		public static SqlInt32 operator -(SqlInt32 x)
		{
			return new SqlInt32(-x.Value);
		}

		public static explicit operator SqlInt32(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32((int)x.ByteValue);
		}

		public static explicit operator SqlInt32(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32((int)x.Value);
		}

		public static explicit operator SqlInt32(SqlDouble x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32(checked((int)x.Value));
		}

		public static explicit operator int(SqlInt32 x)
		{
			return x.Value;
		}

		public static explicit operator SqlInt32(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32(checked((int)x.Value));
		}

		public static explicit operator SqlInt32(SqlMoney x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32((int)Math.Round(x.Value));
		}

		public static explicit operator SqlInt32(SqlSingle x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32(checked((int)x.Value));
		}

		public static explicit operator SqlInt32(SqlString x)
		{
			return SqlInt32.Parse(x.Value);
		}

		public static implicit operator SqlInt32(int x)
		{
			return new SqlInt32(x);
		}

		public static implicit operator SqlInt32(SqlByte x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32((int)x.Value);
		}

		public static implicit operator SqlInt32(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return SqlInt32.Null;
			}
			return new SqlInt32((int)x.Value);
		}

		private int value;

		private bool notNull;

		public static readonly SqlInt32 MaxValue = new SqlInt32(int.MaxValue);

		public static readonly SqlInt32 MinValue = new SqlInt32(int.MinValue);

		public static readonly SqlInt32 Null;

		public static readonly SqlInt32 Zero = new SqlInt32(0);
	}
}
