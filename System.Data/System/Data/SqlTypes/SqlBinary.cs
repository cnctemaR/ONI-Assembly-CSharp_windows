using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public struct SqlBinary : INullable, IComparable, IXmlSerializable
	{
		public SqlBinary(byte[] value)
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

		public byte this[int index]
		{
			get
			{
				throw null;
			}
		}

		public int Length
		{
			get
			{
				throw null;
			}
		}

		public byte[] Value
		{
			get
			{
				throw null;
			}
		}

		public static SqlBinary Add(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public int CompareTo(SqlBinary value)
		{
			throw null;
		}

		public int CompareTo(object value)
		{
			throw null;
		}

		public static SqlBinary Concat(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean Equals(SqlBinary x, SqlBinary y)
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

		public static SqlBoolean GreaterThan(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean GreaterThanOrEqual(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean LessThan(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean LessThanOrEqual(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean NotEquals(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBinary operator +(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean operator ==(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static explicit operator byte[](SqlBinary x)
		{
			throw null;
		}

		public static explicit operator SqlBinary(SqlGuid x)
		{
			throw null;
		}

		public static SqlBoolean operator >(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean operator >=(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static implicit operator SqlBinary(byte[] x)
		{
			throw null;
		}

		public static SqlBoolean operator !=(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean operator <(SqlBinary x, SqlBinary y)
		{
			throw null;
		}

		public static SqlBoolean operator <=(SqlBinary x, SqlBinary y)
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

		public SqlGuid ToSqlGuid()
		{
			throw null;
		}

		public override string ToString()
		{
			throw null;
		}

		public static readonly SqlBinary Null;
	}
}
