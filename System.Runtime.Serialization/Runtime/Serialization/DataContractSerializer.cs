using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace System.Runtime.Serialization
{
	public sealed class DataContractSerializer : XmlObjectSerializer
	{
		public DataContractSerializer(Type type)
			: this(type, null)
		{
		}

		public DataContractSerializer(Type type, IEnumerable<Type> knownTypes)
			: this(type, knownTypes, int.MaxValue, false, false, null)
		{
		}

		public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: this(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, null)
		{
		}

		public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate, DataContractResolver dataContractResolver)
		{
			this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, dataContractResolver, false);
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace)
			: this(type, rootName, rootNamespace, null)
		{
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes)
			: this(type, rootName, rootNamespace, knownTypes, int.MaxValue, false, false, null)
		{
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: this(type, rootName, rootNamespace, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, null)
		{
		}

		public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate, DataContractResolver dataContractResolver)
		{
			XmlDictionary xmlDictionary = new XmlDictionary(2);
			this.Initialize(type, xmlDictionary.Add(rootName), xmlDictionary.Add(DataContract.GetNamespace(rootNamespace)), knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, dataContractResolver, false);
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
			: this(type, rootName, rootNamespace, null)
		{
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes)
			: this(type, rootName, rootNamespace, knownTypes, int.MaxValue, false, false, null, null)
		{
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
			: this(type, rootName, rootNamespace, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, null)
		{
		}

		public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate, DataContractResolver dataContractResolver)
		{
			this.Initialize(type, rootName, rootNamespace, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, dataContractResolver, false);
		}

		public DataContractSerializer(Type type, DataContractSerializerSettings settings)
		{
			if (settings == null)
			{
				settings = new DataContractSerializerSettings();
			}
			this.Initialize(type, settings.RootName, settings.RootNamespace, settings.KnownTypes, settings.MaxItemsInObjectGraph, settings.IgnoreExtensionDataObject, settings.PreserveObjectReferences, settings.DataContractSurrogate, settings.DataContractResolver, settings.SerializeReadOnlyTypes);
		}

		private void Initialize(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate, DataContractResolver dataContractResolver, bool serializeReadOnlyTypes)
		{
			XmlObjectSerializer.CheckNull(type, "type");
			this.rootType = type;
			if (knownTypes != null)
			{
				this.knownTypeList = new List<Type>();
				foreach (Type type2 in knownTypes)
				{
					this.knownTypeList.Add(type2);
				}
			}
			if (maxItemsInObjectGraph < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxItemsInObjectGraph", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			this.maxItemsInObjectGraph = maxItemsInObjectGraph;
			this.ignoreExtensionDataObject = ignoreExtensionDataObject;
			this.preserveObjectReferences = preserveObjectReferences;
			this.dataContractSurrogate = dataContractSurrogate;
			this.dataContractResolver = dataContractResolver;
			this.serializeReadOnlyTypes = serializeReadOnlyTypes;
		}

		private void Initialize(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate, DataContractResolver dataContractResolver, bool serializeReadOnlyTypes)
		{
			this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate, dataContractResolver, serializeReadOnlyTypes);
			this.rootName = rootName;
			this.rootNamespace = rootNamespace;
		}

		public ReadOnlyCollection<Type> KnownTypes
		{
			get
			{
				if (this.knownTypeCollection == null)
				{
					if (this.knownTypeList != null)
					{
						this.knownTypeCollection = new ReadOnlyCollection<Type>(this.knownTypeList);
					}
					else
					{
						this.knownTypeCollection = new ReadOnlyCollection<Type>(Globals.EmptyTypeArray);
					}
				}
				return this.knownTypeCollection;
			}
		}

		internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			get
			{
				if (this.knownDataContracts == null && this.knownTypeList != null)
				{
					this.knownDataContracts = XmlObjectSerializerContext.GetDataContractsForKnownTypes(this.knownTypeList);
				}
				return this.knownDataContracts;
			}
		}

		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.maxItemsInObjectGraph;
			}
		}

		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
		}

		public bool PreserveObjectReferences
		{
			get
			{
				return this.preserveObjectReferences;
			}
		}

		public bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignoreExtensionDataObject;
			}
		}

		public DataContractResolver DataContractResolver
		{
			get
			{
				return this.dataContractResolver;
			}
		}

		public bool SerializeReadOnlyTypes
		{
			get
			{
				return this.serializeReadOnlyTypes;
			}
		}

		private DataContract RootContract
		{
			get
			{
				if (this.rootContract == null)
				{
					this.rootContract = DataContract.GetDataContract((this.dataContractSurrogate == null) ? this.rootType : DataContractSerializer.GetSurrogatedType(this.dataContractSurrogate, this.rootType));
					this.needsContractNsAtRoot = base.CheckIfNeedsContractNsAtRoot(this.rootName, this.rootNamespace, this.rootContract);
				}
				return this.rootContract;
			}
		}

		internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
		{
			this.InternalWriteObject(writer, graph, null);
		}

		internal override void InternalWriteObject(XmlWriterDelegator writer, object graph, DataContractResolver dataContractResolver)
		{
			this.InternalWriteStartObject(writer, graph);
			this.InternalWriteObjectContent(writer, graph, dataContractResolver);
			this.InternalWriteEndObject(writer);
		}

		public override void WriteObject(XmlWriter writer, object graph)
		{
			base.WriteObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		public override void WriteStartObject(XmlWriter writer, object graph)
		{
			base.WriteStartObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		public override void WriteObjectContent(XmlWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		public override void WriteEndObject(XmlWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new XmlWriterDelegator(writer));
		}

		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			base.WriteStartObjectHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new XmlWriterDelegator(writer));
		}

		public void WriteObject(XmlDictionaryWriter writer, object graph, DataContractResolver dataContractResolver)
		{
			base.WriteObjectHandleExceptions(new XmlWriterDelegator(writer), graph, dataContractResolver);
		}

		public override object ReadObject(XmlReader reader)
		{
			return base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), true);
		}

		public override object ReadObject(XmlReader reader, bool verifyObjectName)
		{
			return base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), verifyObjectName);
		}

		public override bool IsStartObject(XmlReader reader)
		{
			return base.IsStartObjectHandleExceptions(new XmlReaderDelegator(reader));
		}

		public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
		{
			return base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), verifyObjectName);
		}

		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			return base.IsStartObjectHandleExceptions(new XmlReaderDelegator(reader));
		}

		public object ReadObject(XmlDictionaryReader reader, bool verifyObjectName, DataContractResolver dataContractResolver)
		{
			return base.ReadObjectHandleExceptions(new XmlReaderDelegator(reader), verifyObjectName, dataContractResolver);
		}

		internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
		{
			base.WriteRootElement(writer, this.RootContract, this.rootName, this.rootNamespace, this.needsContractNsAtRoot);
		}

		internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
		{
			this.InternalWriteObjectContent(writer, graph, null);
		}

		internal void InternalWriteObjectContent(XmlWriterDelegator writer, object graph, DataContractResolver dataContractResolver)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[] { this.MaxItemsInObjectGraph })));
			}
			DataContract dataContract = this.RootContract;
			Type underlyingType = dataContract.UnderlyingType;
			Type type = ((graph == null) ? underlyingType : graph.GetType());
			if (this.dataContractSurrogate != null)
			{
				graph = DataContractSerializer.SurrogateToDataContractType(this.dataContractSurrogate, graph, underlyingType, ref type);
			}
			if (dataContractResolver == null)
			{
				dataContractResolver = this.DataContractResolver;
			}
			if (graph == null)
			{
				if (base.IsRootXmlAny(this.rootName, dataContract))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("A null value cannot be serialized at the top level for IXmlSerializable root type '{0}' since its IsAny setting is 'true'. This type must write all its contents including the root element. Verify that the IXmlSerializable implementation is correct.", new object[] { underlyingType })));
				}
				XmlObjectSerializer.WriteNull(writer);
				return;
			}
			else if (underlyingType == type)
			{
				if (dataContract.CanContainReferences)
				{
					XmlObjectSerializerWriteContext xmlObjectSerializerWriteContext = XmlObjectSerializerWriteContext.CreateContext(this, dataContract, dataContractResolver);
					xmlObjectSerializerWriteContext.HandleGraphAtTopLevel(writer, graph, dataContract);
					xmlObjectSerializerWriteContext.SerializeWithoutXsiType(dataContract, writer, graph, underlyingType.TypeHandle);
					return;
				}
				dataContract.WriteXmlValue(writer, graph, null);
				return;
			}
			else
			{
				if (base.IsRootXmlAny(this.rootName, dataContract))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An object of type '{0}' cannot be serialized at the top level for IXmlSerializable root type '{1}' since its IsAny setting is 'true'. This type must write all its contents including the root element. Verify that the IXmlSerializable implementation is correct.", new object[] { type, dataContract.UnderlyingType })));
				}
				dataContract = DataContractSerializer.GetDataContract(dataContract, underlyingType, type);
				XmlObjectSerializerWriteContext xmlObjectSerializerWriteContext2 = XmlObjectSerializerWriteContext.CreateContext(this, this.RootContract, dataContractResolver);
				if (dataContract.CanContainReferences)
				{
					xmlObjectSerializerWriteContext2.HandleGraphAtTopLevel(writer, graph, dataContract);
				}
				xmlObjectSerializerWriteContext2.OnHandleIsReference(writer, dataContract, graph);
				xmlObjectSerializerWriteContext2.SerializeWithXsiTypeAtTopLevel(dataContract, writer, graph, underlyingType.TypeHandle, type);
				return;
			}
		}

		internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
		{
			if (declaredType.IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
			{
				return declaredTypeContract;
			}
			if (declaredType.IsArray)
			{
				return declaredTypeContract;
			}
			return DataContract.GetDataContract(objectType.TypeHandle, objectType, SerializationMode.SharedContract);
		}

		internal void SetDataContractSurrogate(IDataContractSurrogate adapter)
		{
			this.dataContractSurrogate = adapter;
		}

		internal override void InternalWriteEndObject(XmlWriterDelegator writer)
		{
			if (!base.IsRootXmlAny(this.rootName, this.RootContract))
			{
				writer.WriteEndElement();
			}
		}

		internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
		{
			return this.InternalReadObject(xmlReader, verifyObjectName, null);
		}

		internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName, DataContractResolver dataContractResolver)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[] { this.MaxItemsInObjectGraph })));
			}
			if (dataContractResolver == null)
			{
				dataContractResolver = this.DataContractResolver;
			}
			if (verifyObjectName)
			{
				if (!this.InternalIsStartObject(xmlReader))
				{
					XmlDictionaryString topLevelElementName;
					XmlDictionaryString topLevelElementNamespace;
					if (this.rootName == null)
					{
						topLevelElementName = this.RootContract.TopLevelElementName;
						topLevelElementNamespace = this.RootContract.TopLevelElementNamespace;
					}
					else
					{
						topLevelElementName = this.rootName;
						topLevelElementNamespace = this.rootNamespace;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(global::System.Runtime.Serialization.SR.GetString("Expecting element '{1}' from namespace '{0}'.", new object[] { topLevelElementNamespace, topLevelElementName }), xmlReader));
				}
			}
			else if (!base.IsStartElement(xmlReader))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(global::System.Runtime.Serialization.SR.GetString("Expecting state '{0}' when ReadObject is called.", new object[] { XmlNodeType.Element }), xmlReader));
			}
			DataContract dataContract = this.RootContract;
			if (dataContract.IsPrimitive && dataContract.UnderlyingType == this.rootType)
			{
				return dataContract.ReadXmlValue(xmlReader, null);
			}
			if (base.IsRootXmlAny(this.rootName, dataContract))
			{
				return XmlObjectSerializerReadContext.ReadRootIXmlSerializable(xmlReader, dataContract as XmlDataContract, false);
			}
			return XmlObjectSerializerReadContext.CreateContext(this, dataContract, dataContractResolver).InternalDeserialize(xmlReader, this.rootType, dataContract, null, null);
		}

		internal override bool InternalIsStartObject(XmlReaderDelegator reader)
		{
			return base.IsRootElement(reader, this.RootContract, this.rootName, this.rootNamespace);
		}

		internal override Type GetSerializeType(object graph)
		{
			if (graph != null)
			{
				return graph.GetType();
			}
			return this.rootType;
		}

		internal override Type GetDeserializeType()
		{
			return this.rootType;
		}

		internal static object SurrogateToDataContractType(IDataContractSurrogate dataContractSurrogate, object oldObj, Type surrogatedDeclaredType, ref Type objType)
		{
			object objectToSerialize = DataContractSurrogateCaller.GetObjectToSerialize(dataContractSurrogate, oldObj, objType, surrogatedDeclaredType);
			if (objectToSerialize != oldObj)
			{
				if (objectToSerialize == null)
				{
					objType = Globals.TypeOfObject;
				}
				else
				{
					objType = objectToSerialize.GetType();
				}
			}
			return objectToSerialize;
		}

		internal static Type GetSurrogatedType(IDataContractSurrogate dataContractSurrogate, Type type)
		{
			return DataContractSurrogateCaller.GetDataContractType(dataContractSurrogate, DataContract.UnwrapNullableType(type));
		}

		private Type rootType;

		private DataContract rootContract;

		private bool needsContractNsAtRoot;

		private XmlDictionaryString rootName;

		private XmlDictionaryString rootNamespace;

		private int maxItemsInObjectGraph;

		private bool ignoreExtensionDataObject;

		private bool preserveObjectReferences;

		private IDataContractSurrogate dataContractSurrogate;

		private ReadOnlyCollection<Type> knownTypeCollection;

		internal IList<Type> knownTypeList;

		internal Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

		private DataContractResolver dataContractResolver;

		private bool serializeReadOnlyTypes;
	}
}
