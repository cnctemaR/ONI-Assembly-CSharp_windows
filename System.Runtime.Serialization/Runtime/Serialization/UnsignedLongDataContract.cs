using System;

namespace System.Runtime.Serialization
{
	internal class UnsignedLongDataContract : PrimitiveDataContract
	{
		internal UnsignedLongDataContract()
			: base(typeof(ulong), DictionaryGlobals.UnsignedLongLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteUnsignedLong";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsUnsignedLong";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteUnsignedLong((ulong)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsUnsignedLong(), context);
			}
			return reader.ReadElementContentAsUnsignedLong();
		}
	}
}
