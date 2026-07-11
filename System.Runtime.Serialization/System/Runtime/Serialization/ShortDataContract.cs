using System;

namespace System.Runtime.Serialization
{
	internal class ShortDataContract : PrimitiveDataContract
	{
		internal ShortDataContract()
			: base(typeof(short), DictionaryGlobals.ShortLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteShort";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsShort";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteShort((short)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsShort(), context);
			}
			return reader.ReadElementContentAsShort();
		}
	}
}
