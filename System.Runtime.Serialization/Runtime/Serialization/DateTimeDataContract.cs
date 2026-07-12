using System;

namespace System.Runtime.Serialization
{
	internal class DateTimeDataContract : PrimitiveDataContract
	{
		internal DateTimeDataContract()
			: base(typeof(DateTime), DictionaryGlobals.DateTimeLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteDateTime";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsDateTime";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteDateTime((DateTime)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsDateTime(), context);
			}
			return reader.ReadElementContentAsDateTime();
		}
	}
}
