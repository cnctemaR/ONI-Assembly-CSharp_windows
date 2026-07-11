using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class CharDataContract : PrimitiveDataContract
	{
		internal CharDataContract()
			: this(DictionaryGlobals.CharLocalName, DictionaryGlobals.SerializationNamespace)
		{
		}

		internal CharDataContract(XmlDictionaryString name, XmlDictionaryString ns)
			: base(typeof(char), name, ns)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteChar";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsChar";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteChar((char)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsChar(), context);
			}
			return reader.ReadElementContentAsChar();
		}
	}
}
