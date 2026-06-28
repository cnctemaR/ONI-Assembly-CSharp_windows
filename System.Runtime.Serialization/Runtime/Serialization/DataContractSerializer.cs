using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace System.Runtime.Serialization
{
	public sealed class DataContractSerializer : XmlObjectSerializer
	{
		public DataContractSerializer(Type type)
			: this(type, Type.EmptyTypes)
		{
		}

		public DataContractSerializer(Type type, IEnumerable<Type> knownTypes)
		{
			this.max_items = 65536;
			base..ctor();
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.type = type;
			this.known_types = new KnownTypeCollection();
			this.PopulateTypes(knownTypes);
			this.known_types.TryRegister(type);
			XmlQualifiedName qname = this.known_types.GetQName(type);
			this.FillDictionaryString(qname.Name, qname.Namespace);
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace)
			: this(type, rootName, rootNamespace, Type.EmptyTypes)
		{
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
			: this(type, rootName, rootNamespace, Type.EmptyTypes)
		{
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes)
		{
			this.max_items = 65536;
			base..ctor();
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (rootName == null)
			{
				throw new ArgumentNullException("rootName");
			}
			if (rootNamespace == null)
			{
				throw new ArgumentNullException("rootNamespace");
			}
			this.type = type;
			this.PopulateTypes(knownTypes);
			this.FillDictionaryString(rootName, rootNamespace);
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes)
		{
			this.max_items = 65536;
			base..ctor();
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (rootName == null)
			{
				throw new ArgumentNullException("rootName");
			}
			if (rootNamespace == null)
			{
				throw new ArgumentNullException("rootNamespace");
			}
			this.type = type;
			this.PopulateTypes(knownTypes);
			this.root_name = rootName;
			this.root_ns = rootNamespace;
		}

		public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: this(type, knownTypes)
		{
			this.Initialize(maxObjectsInGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: this(type, rootName, rootNamespace, knownTypes)
		{
			this.Initialize(maxObjectsInGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: this(type, rootName, rootNamespace, knownTypes)
		{
			this.Initialize(maxObjectsInGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
		}

		private void PopulateTypes(IEnumerable<Type> knownTypes)
		{
			if (this.known_types == null)
			{
				this.known_types = new KnownTypeCollection();
			}
			if (knownTypes != null)
			{
				foreach (Type type in knownTypes)
				{
					this.known_types.TryRegister(type);
				}
			}
			Type elementType = this.type;
			if (this.type.HasElementType)
			{
				elementType = this.type.GetElementType();
			}
			foreach (KnownTypeAttribute knownTypeAttribute in elementType.GetCustomAttributes(typeof(KnownTypeAttribute), true))
			{
				this.known_types.TryRegister(knownTypeAttribute.Type);
			}
		}

		private void FillDictionaryString(string name, string ns)
		{
			XmlDictionary xmlDictionary = new XmlDictionary();
			this.root_name = xmlDictionary.Add(name);
			this.root_ns = xmlDictionary.Add(ns);
			this.names_filled = true;
		}

		private void Initialize(int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		{
			if (maxObjectsInGraph < 0)
			{
				throw new ArgumentOutOfRangeException("maxObjectsInGraph must not be negative.");
			}
			this.max_items = maxObjectsInGraph;
			this.ignore_ext = ignoreExtensionDataObject;
			this.preserve_refs = preserveObjectReferences;
			this.surrogate = dataContractSurrogate;
			this.PopulateTypes(Type.EmptyTypes);
		}

		public bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignore_ext;
			}
		}

		public ReadOnlyCollection<Type> KnownTypes
		{
			get
			{
				return this.known_runtime_types;
			}
		}

		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.surrogate;
			}
		}

		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.max_items;
			}
		}

		public bool PreserveObjectReferences
		{
			get
			{
				return this.preserve_refs;
			}
		}

		[MonoTODO]
		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			throw new NotImplementedException();
		}

		public override bool IsStartObject(XmlReader reader)
		{
			return this.IsStartObject(XmlDictionaryReader.CreateDictionaryReader(reader));
		}

		public override object ReadObject(XmlReader reader)
		{
			return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader));
		}

		public override object ReadObject(XmlReader reader, bool verifyObjectName)
		{
			return this.ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader), verifyObjectName);
		}

		[MonoTODO]
		public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
		{
			int count = this.known_types.Count;
			this.known_types.Add(this.type);
			bool isEmptyElement = reader.IsEmptyElement;
			object obj = XmlFormatterDeserializer.Deserialize(reader, this.type, this.known_types, this.surrogate, this.root_name.Value, this.root_ns.Value, verifyObjectName);
			while (this.known_types.Count > count)
			{
				this.known_types.RemoveAt(count);
			}
			return obj;
		}

		private void ReadRootStartElement(XmlReader reader, Type type)
		{
			SerializationMap serializationMap = this.known_types.FindUserMap(type);
			XmlQualifiedName xmlQualifiedName = ((serializationMap == null) ? KnownTypeCollection.GetPredefinedTypeName(type) : serializationMap.XmlName);
			reader.MoveToContent();
			reader.ReadStartElement(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
			reader.Read();
		}

		public override void WriteObject(XmlWriter writer, object graph)
		{
			XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
			this.WriteObject(xmlDictionaryWriter, graph);
		}

		[MonoTODO("support arrays; support Serializable; support SharedType; use DataContractSurrogate")]
		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			if (graph == null)
			{
				return;
			}
			int count = this.known_types.Count;
			XmlFormatterSerializer.Serialize(writer, graph, this.known_types, this.ignore_ext, this.max_items, this.root_ns.Value);
			while (this.known_types.Count > count)
			{
				this.known_types.RemoveAt(count);
			}
		}

		public override void WriteObjectContent(XmlWriter writer, object graph)
		{
			XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
			this.WriteObjectContent(xmlDictionaryWriter, graph);
		}

		public override void WriteStartObject(XmlWriter writer, object graph)
		{
			this.WriteStartObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
		}

		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			Type type = this.type;
			if (this.root_name.Value == string.Empty)
			{
				throw new InvalidDataContractException("Type '" + this.type.ToString() + "' cannot have a DataContract attribute Name set to null or empty string.");
			}
			if (graph == null)
			{
				if (this.names_filled)
				{
					writer.WriteStartElement(this.root_name.Value, this.root_ns.Value);
				}
				else
				{
					writer.WriteStartElement(this.root_name, this.root_ns);
				}
				writer.WriteAttributeString("i", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				return;
			}
			XmlQualifiedName qname = this.known_types.GetQName(type);
			XmlQualifiedName qname2 = this.known_types.GetQName(graph.GetType());
			this.known_types.Add(graph.GetType());
			if (this.names_filled)
			{
				writer.WriteStartElement(this.root_name.Value, this.root_ns.Value);
			}
			else
			{
				writer.WriteStartElement(this.root_name, this.root_ns);
			}
			if (this.root_ns.Value != qname.Namespace && qname.Namespace != "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				writer.WriteXmlnsAttribute(null, qname.Namespace);
			}
			if (qname == qname2)
			{
				if (qname.Namespace != "http://schemas.microsoft.com/2003/10/Serialization/" && !type.IsEnum)
				{
					writer.WriteXmlnsAttribute("i", "http://www.w3.org/2001/XMLSchema-instance");
				}
				return;
			}
			this.known_types.Add(type);
			XmlQualifiedName xmlQualifiedName = KnownTypeCollection.GetPredefinedTypeName(graph.GetType());
			if (xmlQualifiedName == XmlQualifiedName.Empty)
			{
				xmlQualifiedName = qname2;
			}
			else
			{
				xmlQualifiedName = new XmlQualifiedName(xmlQualifiedName.Name, "http://www.w3.org/2001/XMLSchema");
			}
			writer.WriteStartAttribute("i", "type", "http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteQualifiedName(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
			writer.WriteEndAttribute();
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			writer.WriteEndElement();
		}

		public override void WriteEndObject(XmlWriter writer)
		{
			this.WriteEndObject(XmlDictionaryWriter.CreateDictionaryWriter(writer));
		}

		private const string xmlns = "http://www.w3.org/2000/xmlns/";

		private Type type;

		private bool ignore_ext;

		private bool preserve_refs;

		private StreamingContext context;

		private ReadOnlyCollection<Type> known_runtime_types;

		private KnownTypeCollection known_types;

		private IDataContractSurrogate surrogate;

		private int max_items;

		private bool names_filled;

		private XmlDictionaryString root_name;

		private XmlDictionaryString root_ns;
	}
}
