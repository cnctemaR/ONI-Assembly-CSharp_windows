using System;

namespace System.Runtime.Serialization
{
	internal class UnsignedByteDataContract : PrimitiveDataContract
	{
		internal UnsignedByteDataContract()
			: base(typeof(byte), DictionaryGlobals.UnsignedByteLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteUnsignedByte";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsUnsignedByte";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteUnsignedByte((byte)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsUnsignedByte(), context);
			}
			return reader.ReadElementContentAsUnsignedByte();
		}
	}
}
