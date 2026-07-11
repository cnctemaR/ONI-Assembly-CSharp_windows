using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class ObjectDataContract : PrimitiveDataContract
	{
		internal ObjectDataContract()
			: base(typeof(object), DictionaryGlobals.ObjectLocalName, DictionaryGlobals.SchemaNamespace)
		{
		}

		internal override string WriteMethodName
		{
			get
			{
				return "WriteAnyType";
			}
		}

		internal override string ReadMethodName
		{
			get
			{
				return "ReadElementContentAsAnyType";
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator writer, object obj, XmlObjectSerializerWriteContext context)
		{
		}

		public override object ReadXmlValue(XmlReaderDelegator reader, XmlObjectSerializerReadContext context)
		{
			object obj;
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				obj = new object();
			}
			else
			{
				string localName = reader.LocalName;
				string namespaceURI = reader.NamespaceURI;
				reader.Read();
				try
				{
					reader.ReadEndElement();
					obj = new object();
				}
				catch (XmlException ex)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Element {0} from namespace {1} cannot have child contents to be deserialized as an object. Please use XElement to deserialize this pattern of XML.", new object[] { localName, namespaceURI }), ex));
				}
			}
			if (context != null)
			{
				return base.HandleReadValue(obj, context);
			}
			return obj;
		}

		internal override bool CanContainReferences
		{
			get
			{
				return true;
			}
		}

		internal override bool IsPrimitive
		{
			get
			{
				return false;
			}
		}
	}
}
