using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlGuid : INullable, IComparable, IXmlSerializable
	{
		public SqlGuid(byte[] value)
		{
			throw null;
		}

		public SqlGuid(Guid g)
		{
			throw null;
		}

		public SqlGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
		{
			throw null;
		}

		public SqlGuid(string s)
		{
			throw null;
		}

		public bool IsNull
		{
			get
			{
				throw null;
			}
		}

		public Guid Value
		{
			get
			{
				throw null;
			}
		}

		public int CompareTo(SqlGuid value)
		{
			throw null;
		}

		public int CompareTo(object value)
		{
			throw null;
		}

		public static SqlBoolean Equals(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public override bool Equals(object value)
		{
			throw null;
		}

		public override int GetHashCode()
		{
			throw null;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			throw null;
		}

		public static SqlBoolean GreaterThan(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean LessThan(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean LessThanOrEqual(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean NotEquals(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean operator ==(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static explicit operator SqlGuid(SqlBinary x)
		{
			throw null;
		}

		public static explicit operator Guid(SqlGuid x)
		{
			throw null;
		}

		public static explicit operator SqlGuid(SqlString x)
		{
			throw null;
		}

		public static SqlBoolean operator >(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean operator >=(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static implicit operator SqlGuid(Guid x)
		{
			throw null;
		}

		public static SqlBoolean operator !=(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean operator <(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlBoolean operator <=(SqlGuid x, SqlGuid y)
		{
			throw null;
		}

		public static SqlGuid Parse(string s)
		{
			throw null;
		}

		[MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw null;
		}

		[MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
		}

		[MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
		}

		public byte[] ToByteArray()
		{
			throw null;
		}

		public SqlBinary ToSqlBinary()
		{
			throw null;
		}

		public SqlString ToSqlString()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public static readonly SqlGuid Null;
	}
}
