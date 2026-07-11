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
	public struct SqlBinary : INullable, IComparable, IXmlSerializable
	{
		private SqlBinary(bool fNull)
		{
			this._value = null;
		}

		public SqlBinary(byte[] value)
		{
			if (value == null)
			{
				this._value = null;
				return;
			}
			this._value = new byte[value.Length];
			value.CopyTo(this._value, 0);
		}

		internal SqlBinary(byte[] value, bool ignored)
		{
			this._value = value;
		}

		public bool IsNull
		{
			get
			{
				return this._value == null;
			}
		}

		public byte[] Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				byte[] array = new byte[this._value.Length];
				this._value.CopyTo(array, 0);
				return array;
			}
		}

		public byte this[int index]
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return this._value[index];
			}
		}

		public int Length
		{
			get
			{
				if (!this.IsNull)
				{
					return this._value.Length;
				}
				throw new SqlNullValueException();
			}
		}

		public static implicit operator SqlBinary(byte[] x)
		{
			return new SqlBinary(x);
		}

		public static explicit operator byte[](SqlBinary x)
		{
			return x.Value;
		}

		public override string ToString()
		{
			if (!this.IsNull)
			{
				return "SqlBinary(" + this._value.Length.ToString(CultureInfo.InvariantCulture) + ")";
			}
			return SQLResource.NullString;
		}

		public static SqlBinary operator +(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBinary.Null;
			}
			byte[] array = new byte[x.Value.Length + y.Value.Length];
			x.Value.CopyTo(array, 0);
			y.Value.CopyTo(array, x.Value.Length);
			return new SqlBinary(array);
		}

		private static EComparison PerformCompareByte(byte[] x, byte[] y)
		{
			int num = ((x.Length < y.Length) ? x.Length : y.Length);
			int i = 0;
			while (i < num)
			{
				if (x[i] != y[i])
				{
					if (x[i] < y[i])
					{
						return EComparison.LT;
					}
					return EComparison.GT;
				}
				else
				{
					i++;
				}
			}
			if (x.Length == y.Length)
			{
				return EComparison.EQ;
			}
			byte b = 0;
			if (x.Length < y.Length)
			{
				for (i = num; i < y.Length; i++)
				{
					if (y[i] != b)
					{
						return EComparison.LT;
					}
				}
			}
			else
			{
				for (i = num; i < x.Length; i++)
				{
					if (x[i] != b)
					{
						return EComparison.GT;
					}
				}
			}
			return EComparison.EQ;
		}

		public static explicit operator SqlBinary(SqlGuid x)
		{
			if (!x.IsNull)
			{
				return new SqlBinary(x.ToByteArray());
			}
			return SqlBinary.Null;
		}

		public static SqlBoolean operator ==(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.PerformCompareByte(x.Value, y.Value) == EComparison.EQ);
		}

		public static SqlBoolean operator !=(SqlBinary x, SqlBinary y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.PerformCompareByte(x.Value, y.Value) == EComparison.LT);
		}

		public static SqlBoolean operator >(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.PerformCompareByte(x.Value, y.Value) == EComparison.GT);
		}

		public static SqlBoolean operator <=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = SqlBinary.PerformCompareByte(x.Value, y.Value);
			return new SqlBoolean(ecomparison == EComparison.LT || ecomparison == EComparison.EQ);
		}

		public static SqlBoolean operator >=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = SqlBinary.PerformCompareByte(x.Value, y.Value);
			return new SqlBoolean(ecomparison == EComparison.GT || ecomparison == EComparison.EQ);
		}

		public static SqlBinary Add(SqlBinary x, SqlBinary y)
		{
			return x + y;
		}

		public static SqlBinary Concat(SqlBinary x, SqlBinary y)
		{
			return x + y;
		}

		public static SqlBoolean Equals(SqlBinary x, SqlBinary y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlBinary x, SqlBinary y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlBinary x, SqlBinary y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlBinary x, SqlBinary y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlBinary x, SqlBinary y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlBinary x, SqlBinary y)
		{
			return x >= y;
		}

		public SqlGuid ToSqlGuid()
		{
			return (SqlGuid)this;
		}

		public int CompareTo(object value)
		{
			if (value is SqlBinary)
			{
				SqlBinary sqlBinary = (SqlBinary)value;
				return this.CompareTo(sqlBinary);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlBinary));
		}

		public int CompareTo(SqlBinary value)
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
			if (!(value is SqlBinary))
			{
				return false;
			}
			SqlBinary sqlBinary = (SqlBinary)value;
			if (sqlBinary.IsNull || this.IsNull)
			{
				return sqlBinary.IsNull && this.IsNull;
			}
			return (this == sqlBinary).Value;
		}

		internal static int HashByteArray(byte[] rgbValue, int length)
		{
			if (length <= 0)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				int num2 = (num >> 28) & 255;
				num <<= 4;
				num = num ^ (int)rgbValue[i] ^ num2;
			}
			return num;
		}

		public override int GetHashCode()
		{
			if (this.IsNull)
			{
				return 0;
			}
			int num = this._value.Length;
			while (num > 0 && this._value[num - 1] == 0)
			{
				num--;
			}
			return SqlBinary.HashByteArray(this._value, num);
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
				this._value = null;
				return;
			}
			string text = reader.ReadElementString();
			if (text == null)
			{
				this._value = Array.Empty<byte>();
				return;
			}
			text = text.Trim();
			if (text.Length == 0)
			{
				this._value = Array.Empty<byte>();
				return;
			}
			this._value = Convert.FromBase64String(text);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(Convert.ToBase64String(this._value));
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
		}

		private byte[] _value;

		public static readonly SqlBinary Null = new SqlBinary(true);
	}
}
