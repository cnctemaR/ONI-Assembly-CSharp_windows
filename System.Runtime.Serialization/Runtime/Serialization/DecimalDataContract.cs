using System;

namespace System.Runtime.Serialization
{
	internal class DecimalDataContract : PrimitiveDataContract
	{
		internal DecimalDataContract()
			: base(typeof(decimal), DictionaryGlobals.DecimalLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteDecimal";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsDecimal";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteDecimal((decimal)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsDecimal(), context);
			}
			return reader.ReadElementContentAsDecimal();
		}
	}
}
