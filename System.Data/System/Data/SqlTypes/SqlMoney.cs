using System;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlMoney : IXmlSerializable, IComparable, INullable
	{
		public SqlMoney(decimal value)
		{
			if (value > 922337203685477.5807m || value < -922337203685477.5808m)
			{
				throw new OverflowException();
			}
			this.value = decimal.Round(value, 4);
			this.notNull = true;
		}

		public SqlMoney(double value)
		{
			this = new SqlMoney((decimal)value);
		}

		public SqlMoney(int value)
		{
			this = new SqlMoney(value);
		}

		public SqlMoney(long value)
		{
			this = new SqlMoney(value);
		}

		static SqlMoney()
		{
			SqlMoney.MoneyFormat.NumberDecimalDigits = 4;
			SqlMoney.MoneyFormat.NumberGroupSeparator = string.Empty;
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

		public bool IsNull
		{
			get
			{
				return !this.notNull;
			}
		}

		public decimal Value
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

		public static SqlMoney Add(SqlMoney x, SqlMoney y)
		{
			return x + y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlMoney))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Data.SqlTypes.SqlMoney"));
			}
			return this.CompareSqlMoney((SqlMoney)value);
		}

		private int CompareSqlMoney(SqlMoney value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return this.value.CompareTo(value.Value);
		}

		public int CompareTo(SqlMoney value)
		{
			return this.CompareSqlMoney(value);
		}

		public static SqlMoney Divide(SqlMoney x, SqlMoney y)
		{
			return x / y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlMoney))
			{
				return false;
			}
			if (this.IsNull)
			{
				return ((SqlMoney)value).IsNull;
			}
			return !((SqlMoney)value).IsNull && (bool)(this == (SqlMoney)value);
		}

		public static SqlBoolean Equals(SqlMoney x, SqlMoney y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			return (int)this.value;
		}

		public static SqlBoolean GreaterThan(SqlMoney x, SqlMoney y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlMoney x, SqlMoney y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlMoney x, SqlMoney y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlMoney x, SqlMoney y)
		{
			return x <= y;
		}

		public static SqlMoney Multiply(SqlMoney x, SqlMoney y)
		{
			return x * y;
		}

		public static SqlBoolean NotEquals(SqlMoney x, SqlMoney y)
		{
			return x != y;
		}

		public static SqlMoney Parse(string s)
		{
			decimal num = decimal.Parse(s);
			if (num > SqlMoney.MaxValue.Value || num < SqlMoney.MinValue.Value)
			{
				throw new OverflowException();
			}
			return new SqlMoney(num);
		}

		public static SqlMoney Subtract(SqlMoney x, SqlMoney y)
		{
			return x - y;
		}

		public decimal ToDecimal()
		{
			return this.value;
		}

		public double ToDouble()
		{
			return (double)this.value;
		}

		public int ToInt32()
		{
			return (int)Math.Round(this.value);
		}

		public long ToInt64()
		{
			return (long)Math.Round(this.value);
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

		public SqlInt32 ToSqlInt32()
		{
			return (SqlInt32)this;
		}

		public SqlInt64 ToSqlInt64()
		{
			return (SqlInt64)this;
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
			if (!this.notNull)
			{
				return "Null";
			}
			return this.value.ToString("N", SqlMoney.MoneyFormat);
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlMoney operator +(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value + y.Value);
		}

		public static SqlMoney operator /(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value / y.Value);
		}

		public static SqlBoolean operator ==(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value == y.Value);
		}

		public static SqlBoolean operator >(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value > y.Value);
		}

		public static SqlBoolean operator >=(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value >= y.Value);
		}

		public static SqlBoolean operator !=(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(!(x.Value == y.Value));
		}

		public static SqlBoolean operator <(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value < y.Value);
		}

		public static SqlBoolean operator <=(SqlMoney x, SqlMoney y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(x.Value <= y.Value);
		}

		public static SqlMoney operator *(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value * y.Value);
		}

		public static SqlMoney operator -(SqlMoney x, SqlMoney y)
		{
			return new SqlMoney(x.Value - y.Value);
		}

		public static SqlMoney operator -(SqlMoney x)
		{
			return new SqlMoney(-x.Value);
		}

		public static explicit operator SqlMoney(SqlBoolean x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney(x.ByteValue);
		}

		public static explicit operator SqlMoney(SqlDecimal x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney(x.Value);
		}

		public static explicit operator SqlMoney(SqlDouble x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney((decimal)x.Value);
		}

		public static explicit operator decimal(SqlMoney x)
		{
			return x.Value;
		}

		public static explicit operator SqlMoney(SqlSingle x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney((decimal)x.Value);
		}

		public static explicit operator SqlMoney(SqlString x)
		{
			return SqlMoney.Parse(x.Value);
		}

		public static explicit operator SqlMoney(double x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(long x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(decimal x)
		{
			return new SqlMoney(x);
		}

		public static implicit operator SqlMoney(SqlByte x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney(x.Value);
		}

		public static implicit operator SqlMoney(SqlInt16 x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney(x.Value);
		}

		public static implicit operator SqlMoney(SqlInt32 x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney(x.Value);
		}

		public static implicit operator SqlMoney(SqlInt64 x)
		{
			if (x.IsNull)
			{
				return SqlMoney.Null;
			}
			return new SqlMoney(x.Value);
		}

		private decimal value;

		private bool notNull;

		public static readonly SqlMoney MaxValue = new SqlMoney(922337203685477.5807m);

		public static readonly SqlMoney MinValue = new SqlMoney(-922337203685477.5808m);

		public static readonly SqlMoney Null;

		public static readonly SqlMoney Zero = new SqlMoney(0);

		private static readonly NumberFormatInfo MoneyFormat = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
	}
}
