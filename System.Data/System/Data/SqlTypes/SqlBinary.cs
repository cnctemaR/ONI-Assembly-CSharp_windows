using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlBinary : IXmlSerializable, IComparable, INullable
	{
		public SqlBinary(byte[] value)
		{
			this.value = value;
			this.notNull = true;
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

		public byte this[int index]
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				if (index >= this.Length)
				{
					throw new IndexOutOfRangeException("The index parameter indicates a position beyond the length of the byte array.");
				}
				return this.value[index];
			}
		}

		public int Length
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException("The property contains Null.");
				}
				return this.value.Length;
			}
		}

		public byte[] Value
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

		public static SqlBinary Add(SqlBinary x, SqlBinary y)
		{
			return x + y;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is SqlBinary))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Data.SqlTypes.SqlBinary"));
			}
			return this.CompareTo((SqlBinary)value);
		}

		public int CompareTo(SqlBinary value)
		{
			if (value.IsNull)
			{
				return 1;
			}
			return SqlBinary.Compare(this, value);
		}

		public static SqlBinary Concat(SqlBinary x, SqlBinary y)
		{
			return x + y;
		}

		public override bool Equals(object value)
		{
			if (!(value is SqlBinary))
			{
				return false;
			}
			if (this.IsNull)
			{
				return ((SqlBinary)value).IsNull;
			}
			return !((SqlBinary)value).IsNull && (bool)(this == (SqlBinary)value);
		}

		public static SqlBoolean Equals(SqlBinary x, SqlBinary y)
		{
			return x == y;
		}

		public override int GetHashCode()
		{
			int num = 10;
			for (int i = 0; i < this.value.Length; i++)
			{
				num = 91 * num + (int)this.value[i];
			}
			return num;
		}

		public static SqlBoolean GreaterThan(SqlBinary x, SqlBinary y)
		{
			return x > y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlBinary x, SqlBinary y)
		{
			return x >= y;
		}

		public static SqlBoolean LessThan(SqlBinary x, SqlBinary y)
		{
			return x < y;
		}

		public static SqlBoolean LessThanOrEqual(SqlBinary x, SqlBinary y)
		{
			return x <= y;
		}

		public static SqlBoolean NotEquals(SqlBinary x, SqlBinary y)
		{
			return x != y;
		}

		public SqlGuid ToSqlGuid()
		{
			return (SqlGuid)this;
		}

		public override string ToString()
		{
			if (!this.notNull)
			{
				return "Null";
			}
			return "SqlBinary(" + this.value.Length + ")";
		}

		private static int Compare(SqlBinary x, SqlBinary y)
		{
			int num = 0;
			if (x.Value.Length != y.Value.Length)
			{
				num = x.Value.Length - y.Value.Length;
				if (num > 0)
				{
					for (int i = x.Value.Length - 1; i > x.Value.Length - num; i--)
					{
						if (x.Value[i] != 0)
						{
							return 1;
						}
					}
				}
				else
				{
					for (int j = y.Value.Length - 1; j > y.Value.Length - num; j--)
					{
						if (y.Value[j] != 0)
						{
							return -1;
						}
					}
				}
			}
			int num2 = ((num <= 0) ? x.Value.Length : y.Value.Length);
			for (int k = num2 - 1; k > 0; k--)
			{
				byte b = x.Value[k];
				byte b2 = y.Value[k];
				if (b > b2)
				{
					return 1;
				}
				if (b < b2)
				{
					return -1;
				}
			}
			return 0;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema");
		}

		public static SqlBinary operator +(SqlBinary x, SqlBinary y)
		{
			byte[] array = new byte[x.Value.Length + y.Value.Length];
			int num = 0;
			int i;
			for (i = 0; i < x.Value.Length; i++)
			{
				array[i] = x.Value[i];
			}
			while (i < x.Value.Length + y.Value.Length)
			{
				array[i] = y.Value[num];
				num++;
				i++;
			}
			return new SqlBinary(array);
		}

		public static SqlBoolean operator ==(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.Compare(x, y) == 0);
		}

		public static SqlBoolean operator >(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.Compare(x, y) > 0);
		}

		public static SqlBoolean operator >=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.Compare(x, y) >= 0);
		}

		public static SqlBoolean operator !=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.Compare(x, y) != 0);
		}

		public static SqlBoolean operator <(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.Compare(x, y) < 0);
		}

		public static SqlBoolean operator <=(SqlBinary x, SqlBinary y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			return new SqlBoolean(SqlBinary.Compare(x, y) <= 0);
		}

		public static explicit operator byte[](SqlBinary x)
		{
			return x.Value;
		}

		public static explicit operator SqlBinary(SqlGuid x)
		{
			return new SqlBinary(x.ToByteArray());
		}

		public static implicit operator SqlBinary(byte[] x)
		{
			return new SqlBinary(x);
		}

		private byte[] value;

		private bool notNull;

		public static readonly SqlBinary Null;
	}
}
