using System;

namespace System.Runtime.Serialization
{
	internal class UriDataContract : PrimitiveDataContract
	{
		internal UriDataContract()
			: base(typeof(Uri), DictionaryGlobals.UriLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteUri";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsUri";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
			writer.WriteUri((Uri)obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			if (context != null)
			{
				return base.HandleReadValue(reader.ReadElementContentAsUri(), context);
			}
			if (!base.TryReadNullAtTopLevel(reader))
			{
				return reader.ReadElementContentAsUri();
			}
			return null;
		}
	}
}
