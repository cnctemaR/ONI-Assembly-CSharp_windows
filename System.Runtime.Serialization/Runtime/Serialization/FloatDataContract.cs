using System;

namespace System.Runtime.Serialization
{
	internal class FloatDataContract : PrimitiveDataContract
	{
		internal FloatDataContract()
			: base(typeof(float), DictionaryGlobals.FloatLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteFloat";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsFloat";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteFloat((float)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsFloat(), context);
			}
			return reader.ReadElementContentAsFloat();
		}
	}
}
