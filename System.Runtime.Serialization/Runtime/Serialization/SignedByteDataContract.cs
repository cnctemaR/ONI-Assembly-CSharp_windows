using System;

namespace System.Runtime.Serialization
{
	internal class SignedByteDataContract : PrimitiveDataContract
	{
		internal SignedByteDataContract()
			: base(typeof(sbyte), DictionaryGlobals.SignedByteLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteSignedByte";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsSignedByte";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteSignedByte((sbyte)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsSignedByte(), context);
			}
			return reader.ReadElementContentAsSignedByte();
		}
	}
}
