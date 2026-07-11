using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[XmlSchemaProvider("GetXsdType")]
	[Serializable]
	public sealed class SqlXml : INullable, IXmlSerializable
	{
		public SqlXml()
		{
		}

		public SqlXml(Stream value)
		{
		}

		public SqlXml(XmlReader value)
		{
		}

		public bool IsNull
		{
			get
			{
				throw null;
			}
		}

		public static SqlXml Null
		{
			get
			{
				throw null;
			}
		}

		public string Value
		{
			get
			{
				throw null;
			}
		}

		public XmlReader CreateReader()
		{
			throw null;
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			throw null;
		}

		[MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw null;
		}

		[MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader r)
		{
		}

		[MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
		}
	}
}
