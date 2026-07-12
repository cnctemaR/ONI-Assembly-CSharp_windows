using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class GuidDataContract : PrimitiveDataContract
	{
		internal GuidDataContract()
			: this(DictionaryGlobals.GuidLocalName, DictionaryGlobals.SerializationNamespace)
		{
		}

		internal GuidDataContract(XmlDictionaryString name, XmlDictionaryString ns)
			: base(typeof(Guid), name, ns)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteGuid";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsGuid";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteGuid((Guid)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsGuid(), context);
			}
			return reader.ReadElementContentAsGuid();
		}
	}
}
