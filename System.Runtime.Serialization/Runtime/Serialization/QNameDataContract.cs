using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class QNameDataContract : PrimitiveDataContract
	{
		internal QNameDataContract()
			: base(typeof(XmlQualifiedName), DictionaryGlobals.QNameLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteQName";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsQName";
			}
		}

		internal override bool IsPrimitive
		{
			get
			{
				return false;
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteQName((XmlQualifiedName)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsQName(), context);
			}
			if (!base.TryReadNullAtTopLevel(reader))
			{
				return reader.ReadElementContentAsQName();
			}
			return null;
		}

		internal override void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (ns == DictionaryGlobals.SerializationNamespace)
			{
				writer.WriteStartElement("z", name, ns);
				return;
			}
			if (ns != null && ns.Value != null && ns.Value.Length > 0)
			{
				writer.WriteStartElement("q", name, ns);
				return;
			}
			writer.WriteStartElement(name, ns);
		}
	}
}
