using System;
using System.Collections;
using System.Security.Policy;

namespace System.Xml.Serialization
{
	public class XmlSerializerFactory
	{
		public XmlSerializer CreateSerializer(Type type)
		{
			return this.CreateSerializer(type, null, null, null, null);
		}

		public XmlSerializer CreateSerializer(XmlTypeMapping xmlTypeMapping)
		{
			Hashtable hashtable = XmlSerializerFactory.serializersBySource;
			XmlSerializer xmlSerializer2;
			lock (hashtable)
			{
				XmlSerializer xmlSerializer = (XmlSerializer)XmlSerializerFactory.serializersBySource[xmlTypeMapping.Source];
				if (xmlSerializer == null)
				{
					xmlSerializer = new XmlSerializer(xmlTypeMapping);
					XmlSerializerFactory.serializersBySource[xmlTypeMapping.Source] = xmlSerializer;
				}
				xmlSerializer2 = xmlSerializer;
			}
			return xmlSerializer2;
		}

		public XmlSerializer CreateSerializer(Type type, string defaultNamespace)
		{
			return this.CreateSerializer(type, null, null, null, defaultNamespace);
		}

		public XmlSerializer CreateSerializer(Type type, Type[] extraTypes)
		{
			return this.CreateSerializer(type, null, extraTypes, null, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides)
		{
			return this.CreateSerializer(type, overrides, null, null, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlRootAttribute root)
		{
			return this.CreateSerializer(type, null, null, root, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
		{
			XmlTypeSerializationSource xmlTypeSerializationSource = new XmlTypeSerializationSource(type, root, overrides, defaultNamespace, extraTypes);
			Hashtable hashtable = XmlSerializerFactory.serializersBySource;
			XmlSerializer xmlSerializer2;
			lock (hashtable)
			{
				XmlSerializer xmlSerializer = (XmlSerializer)XmlSerializerFactory.serializersBySource[xmlTypeSerializationSource];
				if (xmlSerializer == null)
				{
					xmlSerializer = new XmlSerializer(type, overrides, extraTypes, root, defaultNamespace);
					XmlSerializerFactory.serializersBySource[xmlSerializer.Mapping.Source] = xmlSerializer;
				}
				xmlSerializer2 = xmlSerializer;
			}
			return xmlSerializer2;
		}

		[MonoTODO]
		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence)
		{
			throw new NotImplementedException();
		}

		private static Hashtable serializersBySource = new Hashtable();
	}
}
