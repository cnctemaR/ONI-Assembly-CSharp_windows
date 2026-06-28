using System;
using System.Collections;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlFormatterSerializer
	{
		public XmlFormatterSerializer(XmlDictionaryWriter writer, KnownTypeCollection types, bool ignoreUnknown, int maxItems, string root_ns)
		{
			this.writer = writer;
			this.types = types;
			this.ignore_unknown = ignoreUnknown;
			this.max_items = maxItems;
		}

		public static void Serialize(XmlDictionaryWriter writer, object graph, KnownTypeCollection types, bool ignoreUnknown, int maxItems, string root_ns)
		{
			new XmlFormatterSerializer(writer, types, ignoreUnknown, maxItems, root_ns).Serialize((graph == null) ? null : graph.GetType(), graph);
		}

		public ArrayList SerializingObjects
		{
			get
			{
				return this.objects;
			}
		}

		public IDictionary References
		{
			get
			{
				return this.references;
			}
		}

		public XmlDictionaryWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		public void Serialize(Type type, object graph)
		{
			if (graph == null)
			{
				this.writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			}
			else
			{
				Type type2 = graph.GetType();
				SerializationMap serializationMap = this.types.FindUserMap(type2);
				if (serializationMap == null)
				{
					type2 = this.types.GetSerializedType(type2);
					serializationMap = this.types.FindUserMap(type2);
				}
				if (serializationMap == null)
				{
					this.types.Add(type2);
					serializationMap = this.types.FindUserMap(type2);
				}
				if (type2 != type && (serializationMap == null || serializationMap.OutputXsiType))
				{
					XmlQualifiedName xmlName = this.types.GetXmlName(type2);
					string text = xmlName.Name;
					string text2 = xmlName.Namespace;
					if (xmlName == XmlQualifiedName.Empty)
					{
						text = XmlConvert.EncodeLocalName(type2.Name);
						text2 = "http://schemas.datacontract.org/2004/07/" + type2.Namespace;
					}
					else if (xmlName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
					{
						text2 = "http://www.w3.org/2001/XMLSchema";
					}
					if (this.writer.LookupPrefix(text2) == null)
					{
						this.writer.WriteXmlnsAttribute(null, text2);
					}
					this.writer.WriteStartAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
					this.writer.WriteQualifiedName(text, text2);
					this.writer.WriteEndAttribute();
				}
				XmlQualifiedName predefinedTypeName = KnownTypeCollection.GetPredefinedTypeName(type2);
				if (predefinedTypeName != XmlQualifiedName.Empty)
				{
					this.SerializePrimitive(type, graph, predefinedTypeName);
				}
				else
				{
					serializationMap.Serialize(graph, this);
				}
			}
		}

		public void SerializePrimitive(Type type, object graph, XmlQualifiedName qname)
		{
			this.writer.WriteString(KnownTypeCollection.PredefinedTypeObjectToString(graph));
		}

		public void WriteStartElement(string rootName, string rootNamespace, string currentNamespace)
		{
			this.writer.WriteStartElement(rootName, rootNamespace);
			if (!string.IsNullOrEmpty(currentNamespace) && currentNamespace != rootNamespace)
			{
				this.writer.WriteXmlnsAttribute(null, currentNamespace);
			}
		}

		public void WriteEndElement()
		{
			this.writer.WriteEndElement();
		}

		private XmlDictionaryWriter writer;

		private object graph;

		private KnownTypeCollection types;

		private bool save_id;

		private bool ignore_unknown;

		private IDataContractSurrogate surrogate;

		private int max_items;

		private ArrayList objects = new ArrayList();

		private Hashtable references = new Hashtable();
	}
}
