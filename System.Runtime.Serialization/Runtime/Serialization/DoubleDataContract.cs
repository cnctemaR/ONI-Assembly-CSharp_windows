using System;

namespace System.Runtime.Serialization
{
	internal class DoubleDataContract : PrimitiveDataContract
	{
		internal DoubleDataContract()
			: base(typeof(double), DictionaryGlobals.DoubleLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteDouble";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsDouble";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteDouble((double)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsDouble(), context);
			}
			return reader.ReadElementContentAsDouble();
		}
	}
}
