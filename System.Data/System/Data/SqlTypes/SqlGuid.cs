using System;
using System.Data.Common;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlGuid : INullable, IComparable, IXmlSerializable
	{
		private SqlGuid(bool fNull)
		{
			this.m_value = null;
		}

		public SqlGuid(byte[] value)
		{
			if (value == null || value.Length != SqlGuid.s_sizeOfGuid)
			{
				throw new ArgumentException(SQLResource.InvalidArraySizeMessage);
			}
			this.m_value = new byte[SqlGuid.s_sizeOfGuid];
			value.CopyTo(this.m_value, 0);
		}

		internal SqlGuid(byte[] value, bool ignored)
		{
			if (value == null || value.Length != SqlGuid.s_sizeOfGuid)
			{
				throw new ArgumentException(SQLResource.InvalidArraySizeMessage);
			}
			this.m_value = value;
		}

		public SqlGuid(string s)
		{
			this.m_value = new Guid(s).ToByteArray();
		}

		public SqlGuid(Guid g)
		{
			this.m_value = g.ToByteArray();
		}

		public SqlGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			this = new SqlGuid(new Guid(a, b, c, d, e, f, g, h, i, j, k));
		}

		public bool IsNull
		{
			get
			{
				return this.m_value == null;
			}
		}

		public Guid Value
		{
			get
			{
				if (this.IsNull)
				{
					throw new SqlNullValueException();
				}
				return new Guid(this.m_value);
			}
		}

		public static implicit operator SqlGuid(Guid x)
		{
			return new SqlGuid(x);
		}

		public static explicit operator Guid(SqlGuid x)
		{
			return x.Value;
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[SqlGuid.s_sizeOfGuid];
			this.m_value.CopyTo(array, 0);
			return array;
		}

		public override string ToString()
		{
			if (this.IsNull)
			{
				return SQLResource.NullString;
			}
			Guid guid = new Guid(this.m_value);
			return guid.ToString();
		}

		public static SqlGuid Parse(string s)
		{
			if (s == SQLResource.NullString)
			{
				return SqlGuid.Null;
			}
			return new SqlGuid(s);
		}

		private static EComparison Compare(SqlGuid x, SqlGuid y)
		{
			int i = 0;
			while (i < SqlGuid.s_sizeOfGuid)
			{
				byte b = x.m_value[SqlGuid.s_rgiGuidOrder[i]];
				byte b2 = y.m_value[SqlGuid.s_rgiGuidOrder[i]];
				if (b != b2)
				{
					if (b >= b2)
					{
						return EComparison.GT;
					}
					return EComparison.LT;
				}
				else
				{
					i++;
				}
			}
			return EComparison.EQ;
		}

		public static explicit operator SqlGuid(SqlString x)
		{
			if (!x.IsNull)
			{
				return new SqlGuid(x.Value);
			}
			return SqlGuid.Null;
		}

		public static explicit operator SqlGuid(SqlBinary x)
		{
			if (!x.IsNull)
			{
				return new SqlGuid(x.Value);
			}
			return SqlGuid.Null;
		}

		public static SqlBoolean operator ==(SqlGuid x, SqlGuid y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(SqlGuid.Compare(x, y) == EComparison.EQ);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator !=(SqlGuid x, SqlGuid y)
		{
			return !(x == y);
		}

		public static SqlBoolean operator <(SqlGuid x, SqlGuid y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(SqlGuid.Compare(x, y) == EComparison.LT);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator >(SqlGuid x, SqlGuid y)
		{
			if (!x.IsNull && !y.IsNull)
			{
				return new SqlBoolean(SqlGuid.Compare(x, y) == EComparison.GT);
			}
			return SqlBoolean.Null;
		}

		public static SqlBoolean operator <=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = SqlGuid.Compare(x, y);
			return new SqlBoolean(ecomparison == EComparison.LT || ecomparison == EComparison.EQ);
		}

		public static SqlBoolean operator >=(SqlGuid x, SqlGuid y)
		{
			if (x.IsNull || y.IsNull)
			{
				return SqlBoolean.Null;
			}
			EComparison ecomparison = SqlGuid.Compare(x, y);
			return new SqlBoolean(ecomparison == EComparison.GT || ecomparison == EComparison.EQ);
		}

		public static SqlBoolean Equals(SqlGuid x, SqlGuid y)
		{
			return x == y;
		}

		public static SqlBoolean NotEquals(SqlGuid x, SqlGuid y)
		{
			return x != y;
		}

		public static SqlBoolean LessThan(SqlGuid x, SqlGuid y)
		{
			return x < y;
		}

		public static SqlBoolean GreaterThan(SqlGuid x, SqlGuid y)
		{
			return x > y;
		}

		public static SqlBoolean LessThanOrEqual(SqlGuid x, SqlGuid y)
		{
			return x <= y;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlGuid x, SqlGuid y)
		{
			return x >= y;
		}

		public SqlString ToSqlString()
		{
			return (SqlString)this;
		}

		public SqlBinary ToSqlBinary()
		{
			return (SqlBinary)this;
		}

		public int CompareTo(object value)
		{
			if (value is SqlGuid)
			{
				SqlGuid sqlGuid = (SqlGuid)value;
				return this.CompareTo(sqlGuid);
			}
			throw ADP.WrongType(value.GetType(), typeof(SqlGuid));
		}

		public int CompareTo(SqlGuid value)
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
			if (!(value is SqlGuid))
			{
				return false;
			}
			SqlGuid sqlGuid = (SqlGuid)value;
			if (sqlGuid.IsNull || this.IsNull)
			{
				return sqlGuid.IsNull && this.IsNull;
			}
			return (this == sqlGuid).Value;
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
				this.m_value = null;
				return;
			}
			this.m_value = new Guid(reader.ReadElementString()).ToByteArray();
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			if (this.IsNull)
			{
				writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			writer.WriteString(XmlConvert.ToString(new Guid(this.m_value)));
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
		}

		private static readonly int s_sizeOfGuid = 16;

		private static readonly int[] s_rgiGuidOrder = new int[]
		{
			10, 11, 12, 13, 14, 15, 8, 9, 6, 7,
			4, 5, 0, 1, 2, 3
		};

		private byte[] m_value;

		public static readonly SqlGuid Null = new SqlGuid(true);
	}
}
