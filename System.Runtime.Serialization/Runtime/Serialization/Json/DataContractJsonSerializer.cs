using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	[TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
	public sealed class DataContractJsonSerializer : XmlObjectSerializer
	{
		public DataContractJsonSerializer(Type type)
			: this(type, null)
		{
		}

		public DataContractJsonSerializer(Type type, string rootName)
			: this(type, rootName, null)
		{
		}

		public DataContractJsonSerializer(Type type, XmlDictionaryString rootName)
			: this(type, rootName, null)
		{
		}

		public DataContractJsonSerializer(Type type, IEnumerable<Type> knownTypes)
			: this(type, knownTypes, int.MaxValue, false, null, false)
		{
		}

		public DataContractJsonSerializer(Type type, string rootName, IEnumerable<Type> knownTypes)
			: this(type, rootName, knownTypes, int.MaxValue, false, null, false)
		{
		}

		public DataContractJsonSerializer(Type type, XmlDictionaryString rootName, IEnumerable<Type> knownTypes)
			: this(type, rootName, knownTypes, int.MaxValue, false, null, false)
		{
		}

		public DataContractJsonSerializer(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation)
		{
			EmitTypeInformation emitTypeInformation = (alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded);
			this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, false, null, false);
		}

		public DataContractJsonSerializer(Type type, string rootName, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation)
		{
			EmitTypeInformation emitTypeInformation = (alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded);
			XmlDictionary xmlDictionary = new XmlDictionary(2);
			this.Initialize(type, xmlDictionary.Add(rootName), knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, false, null, false);
		}

		public DataContractJsonSerializer(Type type, XmlDictionaryString rootName, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation)
		{
			EmitTypeInformation emitTypeInformation = (alwaysEmitTypeInformation ? EmitTypeInformation.Always : EmitTypeInformation.AsNeeded);
			this.Initialize(type, rootName, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, false, null, false);
		}

		public DataContractJsonSerializer(Type type, DataContractJsonSerializerSettings settings)
		{
			if (settings == null)
			{
				settings = new DataContractJsonSerializerSettings();
			}
			XmlDictionaryString xmlDictionaryString = ((settings.RootName == null) ? null : new XmlDictionary(1).Add(settings.RootName));
			this.Initialize(type, xmlDictionaryString, settings.KnownTypes, settings.MaxItemsInObjectGraph, settings.IgnoreExtensionDataObject, settings.DataContractSurrogate, settings.EmitTypeInformation, settings.SerializeReadOnlyTypes, settings.DateTimeFormat, settings.UseSimpleDictionaryFormat);
		}

		public IDataContractSurrogate DataContractSurrogate
		{
			get
			{
				return this.dataContractSurrogate;
			}
		}

		public bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignoreExtensionDataObject;
			}
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

		internal bool AlwaysEmitTypeInformation
		{
			get
			{
				return this.emitTypeInformation == EmitTypeInformation.Always;
			}
		}

		public EmitTypeInformation EmitTypeInformation
		{
			get
			{
				return this.emitTypeInformation;
			}
		}

		public bool SerializeReadOnlyTypes
		{
			get
			{
				return this.serializeReadOnlyTypes;
			}
		}

		public DateTimeFormat DateTimeFormat
		{
			get
			{
				return this.dateTimeFormat;
			}
		}

		public bool UseSimpleDictionaryFormat
		{
			get
			{
				return this.useSimpleDictionaryFormat;
			}
		}

		private DataContract RootContract
		{
			get
			{
				if (this.rootContract == null)
				{
					this.rootContract = DataContract.GetDataContract((this.dataContractSurrogate == null) ? this.rootType : DataContractSerializer.GetSurrogatedType(this.dataContractSurrogate, this.rootType));
					DataContractJsonSerializer.CheckIfTypeIsReference(this.rootContract);
				}
				return this.rootContract;
			}
		}

		private XmlDictionaryString RootName
		{
			get
			{
				return this.rootName ?? JsonGlobals.rootDictionaryString;
			}
		}

		public override bool IsStartObject(XmlReader reader)
		{
			return base.IsStartObjectHandleExceptions(new JsonReaderDelegator(reader));
		}

		public override bool IsStartObject(XmlDictionaryReader reader)
		{
			return base.IsStartObjectHandleExceptions(new JsonReaderDelegator(reader));
		}

		public override object ReadObject(Stream stream)
		{
			XmlObjectSerializer.CheckNull(stream, "stream");
			return this.ReadObject(JsonReaderWriterFactory.CreateJsonReader(stream, XmlDictionaryReaderQuotas.Max));
		}

		public override object ReadObject(XmlReader reader)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), true);
		}

		public override object ReadObject(XmlReader reader, bool verifyObjectName)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), verifyObjectName);
		}

		public override object ReadObject(XmlDictionaryReader reader)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), true);
		}

		public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
		{
			return base.ReadObjectHandleExceptions(new JsonReaderDelegator(reader, this.DateTimeFormat), verifyObjectName);
		}

		public override void WriteEndObject(XmlWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new JsonWriterDelegator(writer));
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new JsonWriterDelegator(writer));
		}

		public override void WriteObject(Stream stream, object graph)
		{
			XmlObjectSerializer.CheckNull(stream, "stream");
			XmlDictionaryWriter xmlDictionaryWriter = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false);
			this.WriteObject(xmlDictionaryWriter, graph);
			xmlDictionaryWriter.Flush();
		}

		public override void WriteObject(XmlWriter writer, object graph)
		{
			base.WriteObjectHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		public override void WriteObject(XmlDictionaryWriter writer, object graph)
		{
			base.WriteObjectHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		public override void WriteObjectContent(XmlWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new JsonWriterDelegator(writer, this.DateTimeFormat), graph);
		}

		public override void WriteStartObject(XmlWriter writer, object graph)
		{
			base.WriteStartObjectHandleExceptions(new JsonWriterDelegator(writer), graph);
		}

		public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
		{
			base.WriteStartObjectHandleExceptions(new JsonWriterDelegator(writer), graph);
		}

		internal static bool CheckIfJsonNameRequiresMapping(string jsonName)
		{
			if (jsonName != null)
			{
				if (!DataContract.IsValidNCName(jsonName))
				{
					return true;
				}
				for (int i = 0; i < jsonName.Length; i++)
				{
					if (XmlJsonWriter.CharacterNeedsEscaping(jsonName[i]))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool CheckIfJsonNameRequiresMapping(XmlDictionaryString jsonName)
		{
			return jsonName != null && DataContractJsonSerializer.CheckIfJsonNameRequiresMapping(jsonName.Value);
		}

		internal static bool CheckIfXmlNameRequiresMapping(string xmlName)
		{
			return xmlName != null && DataContractJsonSerializer.CheckIfJsonNameRequiresMapping(DataContractJsonSerializer.ConvertXmlNameToJsonName(xmlName));
		}

		internal static bool CheckIfXmlNameRequiresMapping(XmlDictionaryString xmlName)
		{
			return xmlName != null && DataContractJsonSerializer.CheckIfXmlNameRequiresMapping(xmlName.Value);
		}

		internal static string ConvertXmlNameToJsonName(string xmlName)
		{
			return XmlConvert.DecodeName(xmlName);
		}

		internal static XmlDictionaryString ConvertXmlNameToJsonName(XmlDictionaryString xmlName)
		{
			if (xmlName != null)
			{
				return new XmlDictionary().Add(DataContractJsonSerializer.ConvertXmlNameToJsonName(xmlName.Value));
			}
			return null;
		}

		internal static bool IsJsonLocalName(XmlReaderDelegator reader, string elementName)
		{
			string text;
			return XmlObjectSerializerReadContextComplexJson.TryGetJsonLocalName(reader, out text) && elementName == text;
		}

		internal static object ReadJsonValue(DataContract contract, XmlReaderDelegator reader, XmlObjectSerializerReadContextComplexJson context)
		{
			return JsonDataContract.GetJsonDataContract(contract).ReadJsonValue(reader, context);
		}

		internal static void WriteJsonNull(XmlWriterDelegator writer)
		{
			writer.WriteAttributeString(null, "type", null, "null");
		}

		internal static void WriteJsonValue(JsonDataContract contract, XmlWriterDelegator writer, object graph, XmlObjectSerializerWriteContextComplexJson context, RuntimeTypeHandle declaredTypeHandle)
		{
			contract.WriteJsonValue(writer, graph, context, declaredTypeHandle);
		}

		internal override Type GetDeserializeType()
		{
			return this.rootType;
		}

		internal override Type GetSerializeType(object graph)
		{
			if (graph != null)
			{
				return graph.GetType();
			}
			return this.rootType;
		}

		internal override bool InternalIsStartObject(XmlReaderDelegator reader)
		{
			return base.IsRootElement(reader, this.RootContract, this.RootName, XmlDictionaryString.Empty) || DataContractJsonSerializer.IsJsonLocalName(reader, this.RootName.Value);
		}

		internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[] { this.MaxItemsInObjectGraph })));
			}
			if (verifyObjectName)
			{
				if (!this.InternalIsStartObject(xmlReader))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(global::System.Runtime.Serialization.SR.GetString("Expecting element '{1}' from namespace '{0}'.", new object[]
					{
						XmlDictionaryString.Empty,
						this.RootName
					}), xmlReader));
				}
			}
			else if (!base.IsStartElement(xmlReader))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(global::System.Runtime.Serialization.SR.GetString("Expecting state '{0}' when ReadObject is called.", new object[] { XmlNodeType.Element }), xmlReader));
			}
			DataContract dataContract = this.RootContract;
			if (dataContract.IsPrimitive && dataContract.UnderlyingType == this.rootType)
			{
				return DataContractJsonSerializer.ReadJsonValue(dataContract, xmlReader, null);
			}
			return XmlObjectSerializerReadContextComplexJson.CreateContext(this, dataContract).InternalDeserialize(xmlReader, this.rootType, dataContract, null, null);
		}

		internal override void InternalWriteEndObject(XmlWriterDelegator writer)
		{
			writer.WriteEndElement();
		}

		internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
		{
			this.InternalWriteStartObject(writer, graph);
			this.InternalWriteObjectContent(writer, graph);
			this.InternalWriteEndObject(writer);
		}

		internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
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
			if (graph == null)
			{
				DataContractJsonSerializer.WriteJsonNull(writer);
				return;
			}
			if (underlyingType == type)
			{
				if (dataContract.CanContainReferences)
				{
					XmlObjectSerializerWriteContextComplexJson xmlObjectSerializerWriteContextComplexJson = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, dataContract);
					xmlObjectSerializerWriteContextComplexJson.OnHandleReference(writer, graph, true);
					xmlObjectSerializerWriteContextComplexJson.SerializeWithoutXsiType(dataContract, writer, graph, underlyingType.TypeHandle);
					return;
				}
				DataContractJsonSerializer.WriteJsonValue(JsonDataContract.GetJsonDataContract(dataContract), writer, graph, null, underlyingType.TypeHandle);
				return;
			}
			else
			{
				XmlObjectSerializerWriteContextComplexJson xmlObjectSerializerWriteContextComplexJson2 = XmlObjectSerializerWriteContextComplexJson.CreateContext(this, this.RootContract);
				dataContract = DataContractJsonSerializer.GetDataContract(dataContract, underlyingType, type);
				if (dataContract.CanContainReferences)
				{
					xmlObjectSerializerWriteContextComplexJson2.OnHandleReference(writer, graph, true);
					xmlObjectSerializerWriteContextComplexJson2.SerializeWithXsiTypeAtTopLevel(dataContract, writer, graph, underlyingType.TypeHandle, type);
					return;
				}
				xmlObjectSerializerWriteContextComplexJson2.SerializeWithoutXsiType(dataContract, writer, graph, underlyingType.TypeHandle);
				return;
			}
		}

		internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
		{
			if (this.rootNameRequiresMapping)
			{
				writer.WriteStartElement("a", "item", "item");
				writer.WriteAttributeString(null, "item", null, this.RootName.Value);
				return;
			}
			writer.WriteStartElement(this.RootName, XmlDictionaryString.Empty);
		}

		private void AddCollectionItemTypeToKnownTypes(Type knownType)
		{
			Type type = knownType;
			Type type2;
			while (CollectionDataContract.IsCollection(type, out type2))
			{
				if (type2.IsGenericType && type2.GetGenericTypeDefinition() == Globals.TypeOfKeyValue)
				{
					type2 = Globals.TypeOfKeyValuePair.MakeGenericType(type2.GetGenericArguments());
				}
				this.knownTypeList.Add(type2);
				type = type2;
			}
		}

		private void Initialize(Type type, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, EmitTypeInformation emitTypeInformation, bool serializeReadOnlyTypes, DateTimeFormat dateTimeFormat, bool useSimpleDictionaryFormat)
		{
			XmlObjectSerializer.CheckNull(type, "type");
			this.rootType = type;
			if (knownTypes != null)
			{
				this.knownTypeList = new List<Type>();
				foreach (Type type2 in knownTypes)
				{
					this.knownTypeList.Add(type2);
					if (type2 != null)
					{
						this.AddCollectionItemTypeToKnownTypes(type2);
					}
				}
			}
			if (maxItemsInObjectGraph < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxItemsInObjectGraph", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			this.maxItemsInObjectGraph = maxItemsInObjectGraph;
			this.ignoreExtensionDataObject = ignoreExtensionDataObject;
			this.dataContractSurrogate = dataContractSurrogate;
			this.emitTypeInformation = emitTypeInformation;
			this.serializeReadOnlyTypes = serializeReadOnlyTypes;
			this.dateTimeFormat = dateTimeFormat;
			this.useSimpleDictionaryFormat = useSimpleDictionaryFormat;
		}

		private void Initialize(Type type, XmlDictionaryString rootName, IEnumerable<Type> knownTypes, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, EmitTypeInformation emitTypeInformation, bool serializeReadOnlyTypes, DateTimeFormat dateTimeFormat, bool useSimpleDictionaryFormat)
		{
			this.Initialize(type, knownTypes, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, emitTypeInformation, serializeReadOnlyTypes, dateTimeFormat, useSimpleDictionaryFormat);
			this.rootName = DataContractJsonSerializer.ConvertXmlNameToJsonName(rootName);
			this.rootNameRequiresMapping = DataContractJsonSerializer.CheckIfJsonNameRequiresMapping(this.rootName);
		}

		internal static void CheckIfTypeIsReference(DataContract dataContract)
		{
			if (dataContract.IsReference)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unsupported value for IsReference for type '{0}', IsReference value is {1}.", new object[]
				{
					DataContract.GetClrTypeFullName(dataContract.UnderlyingType),
					dataContract.IsReference
				})));
			}
		}

		internal static DataContract GetDataContract(DataContract declaredTypeContract, Type declaredType, Type objectType)
		{
			DataContract dataContract = DataContractSerializer.GetDataContract(declaredTypeContract, declaredType, objectType);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
			return dataContract;
		}

		internal IList<Type> knownTypeList;

		internal Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

		private EmitTypeInformation emitTypeInformation;

		private IDataContractSurrogate dataContractSurrogate;

		private bool ignoreExtensionDataObject;

		private ReadOnlyCollection<Type> knownTypeCollection;

		private int maxItemsInObjectGraph;

		private DataContract rootContract;

		private XmlDictionaryString rootName;

		private bool rootNameRequiresMapping;

		private Type rootType;

		private bool serializeReadOnlyTypes;

		private DateTimeFormat dateTimeFormat;

		private bool useSimpleDictionaryFormat;
	}
}
