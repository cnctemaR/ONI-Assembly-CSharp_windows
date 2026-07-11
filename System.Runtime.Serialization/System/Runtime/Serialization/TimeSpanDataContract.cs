using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class TimeSpanDataContract : PrimitiveDataContract
	{
		internal TimeSpanDataContract()
			: this(DictionaryGlobals.TimeSpanLocalName, DictionaryGlobals.SerializationNamespace)
		{
		}

		internal TimeSpanDataContract(XmlDictionaryString name, XmlDictionaryString ns)
			: base(typeof(TimeSpan), name, ns)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteTimeSpan";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsTimeSpan";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteTimeSpan((TimeSpan)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsTimeSpan(), context);
			}
			return reader.ReadElementContentAsTimeSpan();
		}
	}
}
