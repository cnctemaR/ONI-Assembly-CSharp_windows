using System;

namespace System.Runtime.Serialization
{
	internal class UnsignedShortDataContract : PrimitiveDataContract
	{
		internal UnsignedShortDataContract()
			: base(typeof(ushort), DictionaryGlobals.UnsignedShortLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteUnsignedShort";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsUnsignedShort";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteUnsignedShort((ushort)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsUnsignedShort(), context);
			}
			return reader.ReadElementContentAsUnsignedShort();
		}
	}
}
