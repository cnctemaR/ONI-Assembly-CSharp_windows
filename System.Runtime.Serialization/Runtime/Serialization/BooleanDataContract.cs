using System;

namespace System.Runtime.Serialization
{
	internal class BooleanDataContract : PrimitiveDataContract
	{
		internal BooleanDataContract()
			: base(typeof(bool), DictionaryGlobals.BooleanLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteBoolean";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsBoolean";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteBoolean((bool)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsBoolean(), context);
			}
			return reader.ReadElementContentAsBoolean();
		}
	}
}
