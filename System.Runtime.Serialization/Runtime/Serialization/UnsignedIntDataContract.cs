using System;

namespace System.Runtime.Serialization
{
	internal class UnsignedIntDataContract : PrimitiveDataContract
	{
		internal UnsignedIntDataContract()
			: base(typeof(uint), DictionaryGlobals.UnsignedIntLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteUnsignedInt";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsUnsignedInt";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteUnsignedInt((uint)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsUnsignedInt(), context);
			}
			return reader.ReadElementContentAsUnsignedInt();
		}
	}
}
