using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal class XmlSerializableMap : SerializationMap
	{
		public XmlSerializableMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
			: base(type, qname, knownTypes)
		{
		}

		public override void Serialize(object graph, XmlFormatterSerializer serializer)
		{
			IXmlSerializable xmlSerializable = graph as IXmlSerializable;
			if (xmlSerializable == null)
			{
				throw new SerializationException();
			}
			xmlSerializable.WriteXml(serializer.Writer);
		}

		public override object DeserializeObject(XmlReader reader, XmlFormatterDeserializer deserializer)
		{
			IXmlSerializable xmlSerializable = (IXmlSerializable)FormatterServices.GetUninitializedObject(this.RuntimeType);
			xmlSerializable.ReadXml(reader);
			return xmlSerializable;
		}

		public override XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
		{
			return null;
		}
	}
}
