using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Configuration;
using System.Runtime.Serialization.Formatters;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace System.Runtime.Serialization
{
	public sealed class NetDataContractSerializer : XmlObjectSerializer, IFormatter
	{
		public NetDataContractSerializer()
			: this(new StreamingContext(StreamingContextStates.All))
		{
		}

		public NetDataContractSerializer(StreamingContext context)
			: this(context, int.MaxValue, false, FormatterAssemblyStyle.Full, null)
		{
		}

		public NetDataContractSerializer(StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		{
			this.Initialize(context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
		}

		public NetDataContractSerializer(string rootName, string rootNamespace)
			: this(rootName, rootNamespace, new StreamingContext(StreamingContextStates.All), int.MaxValue, false, FormatterAssemblyStyle.Full, null)
		{
		}

		public NetDataContractSerializer(string rootName, string rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		{
			XmlDictionary xmlDictionary = new XmlDictionary(2);
			this.Initialize(xmlDictionary.Add(rootName), xmlDictionary.Add(DataContract.GetNamespace(rootNamespace)), context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
		}

		public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
			: this(rootName, rootNamespace, new StreamingContext(StreamingContextStates.All), int.MaxValue, false, FormatterAssemblyStyle.Full, null)
		{
		}

		public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		{
			this.Initialize(rootName, rootNamespace, context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
		}

		private void Initialize(StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		{
			this.context = context;
			if (maxItemsInObjectGraph < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxItemsInObjectGraph", global::System.Runtime.Serialization.SR.GetString("The value of this argument must be non-negative.")));
			}
			this.maxItemsInObjectGraph = maxItemsInObjectGraph;
			this.ignoreExtensionDataObject = ignoreExtensionDataObject;
			this.surrogateSelector = surrogateSelector;
			this.AssemblyFormat = assemblyFormat;
		}

		private void Initialize(XmlDictionaryString rootName, XmlDictionaryString rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		{
			this.Initialize(context, maxItemsInObjectGraph, ignoreExtensionDataObject, assemblyFormat, surrogateSelector);
			this.rootName = rootName;
			this.rootNamespace = rootNamespace;
		}

		internal static bool UnsafeTypeForwardingEnabled
		{
			[SecuritySafeCritical]
			get
			{
				if (NetDataContractSerializer.unsafeTypeForwardingEnabled == null)
				{
					NetDataContractSerializerSection netDataContractSerializerSection;
					if (NetDataContractSerializerSection.TryUnsafeGetSection(out netDataContractSerializerSection))
					{
						NetDataContractSerializer.unsafeTypeForwardingEnabled = new bool?(netDataContractSerializerSection.EnableUnsafeTypeForwarding);
					}
					else
					{
						NetDataContractSerializer.unsafeTypeForwardingEnabled = new bool?(false);
					}
				}
				return NetDataContractSerializer.unsafeTypeForwardingEnabled.Value;
			}
		}

		public StreamingContext Context
		{
			get
			{
				return this.context;
			}
			set
			{
				this.context = value;
			}
		}

		public SerializationBinder Binder
		{
			get
			{
				return this.binder;
			}
			set
			{
				this.binder = value;
			}
		}

		public ISurrogateSelector SurrogateSelector
		{
			get
			{
				return this.surrogateSelector;
			}
			set
			{
				this.surrogateSelector = value;
			}
		}

		public FormatterAssemblyStyle AssemblyFormat
		{
			get
			{
				return this.assemblyFormat;
			}
			set
			{
				if (value != FormatterAssemblyStyle.Full && value != FormatterAssemblyStyle.Simple)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("'{0}': invalid assembly format.", new object[] { value })));
				}
				this.assemblyFormat = value;
			}
		}

		public int MaxItemsInObjectGraph
		{
			get
			{
				return this.maxItemsInObjectGraph;
			}
		}

		public bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignoreExtensionDataObject;
			}
		}

		public void Serialize(Stream stream, object graph)
		{
			base.WriteObject(stream, graph);
		}

		public object Deserialize(Stream stream)
		{
			return base.ReadObject(stream);
		}

		internal override void InternalWriteObject(XmlWriterDelegator writer, object graph)
		{
			Hashtable hashtable = null;
			DataContract dataContract = this.GetDataContract(graph, ref hashtable);
			this.InternalWriteStartObject(writer, graph, dataContract);
			this.InternalWriteObjectContent(writer, graph, dataContract, hashtable);
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

		internal override void InternalWriteStartObject(XmlWriterDelegator writer, object graph)
		{
			Hashtable hashtable = null;
			DataContract dataContract = this.GetDataContract(graph, ref hashtable);
			this.InternalWriteStartObject(writer, graph, dataContract);
		}

		private void InternalWriteStartObject(XmlWriterDelegator writer, object graph, DataContract contract)
		{
			base.WriteRootElement(writer, contract, this.rootName, this.rootNamespace, base.CheckIfNeedsContractNsAtRoot(this.rootName, this.rootNamespace, contract));
		}

		public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
		{
			base.WriteObjectContentHandleExceptions(new XmlWriterDelegator(writer), graph);
		}

		internal override void InternalWriteObjectContent(XmlWriterDelegator writer, object graph)
		{
			Hashtable hashtable = null;
			DataContract dataContract = this.GetDataContract(graph, ref hashtable);
			this.InternalWriteObjectContent(writer, graph, dataContract, hashtable);
		}

		private void InternalWriteObjectContent(XmlWriterDelegator writer, object graph, DataContract contract, Hashtable surrogateDataContracts)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[] { this.MaxItemsInObjectGraph })));
			}
			if (base.IsRootXmlAny(this.rootName, contract))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("For type '{0}', IsAny is not supported by NetDataContractSerializer.", new object[] { contract.UnderlyingType })));
			}
			if (graph == null)
			{
				XmlObjectSerializer.WriteNull(writer);
				return;
			}
			Type type = graph.GetType();
			if (contract.UnderlyingType != type)
			{
				contract = this.GetDataContract(graph, ref surrogateDataContracts);
			}
			XmlObjectSerializerWriteContext xmlObjectSerializerWriteContext = null;
			if (contract.CanContainReferences)
			{
				xmlObjectSerializerWriteContext = XmlObjectSerializerWriteContext.CreateContext(this, surrogateDataContracts);
				xmlObjectSerializerWriteContext.HandleGraphAtTopLevel(writer, graph, contract);
			}
			NetDataContractSerializer.WriteClrTypeInfo(writer, contract, this.binder);
			contract.WriteXmlValue(writer, graph, xmlObjectSerializerWriteContext);
		}

		internal static void WriteClrTypeInfo(XmlWriterDelegator writer, DataContract dataContract, SerializationBinder binder)
		{
			if (!dataContract.IsISerializable && !(dataContract is SurrogateDataContract))
			{
				TypeInformation typeInformation = null;
				Type originalUnderlyingType = dataContract.OriginalUnderlyingType;
				string text = null;
				string text2 = null;
				if (binder != null)
				{
					binder.BindToName(originalUnderlyingType, out text2, out text);
				}
				if (text == null)
				{
					typeInformation = NetDataContractSerializer.GetTypeInformation(originalUnderlyingType);
					text = typeInformation.FullTypeName;
				}
				if (text2 == null)
				{
					text2 = ((typeInformation == null) ? NetDataContractSerializer.GetTypeInformation(originalUnderlyingType).AssemblyString : typeInformation.AssemblyString);
					if (!NetDataContractSerializer.UnsafeTypeForwardingEnabled && !originalUnderlyingType.Assembly.IsFullyTrusted && !NetDataContractSerializer.IsAssemblyNameForwardingSafe(originalUnderlyingType.Assembly.FullName, text2))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' in assembly '{1}' cannot be forwarded from assembly '{2}'.", new object[]
						{
							DataContract.GetClrTypeFullName(originalUnderlyingType),
							originalUnderlyingType.Assembly.FullName,
							text2
						})));
					}
				}
				NetDataContractSerializer.WriteClrTypeInfo(writer, text, text2);
			}
		}

		internal static void WriteClrTypeInfo(XmlWriterDelegator writer, Type dataContractType, SerializationBinder binder, string defaultClrTypeName, string defaultClrAssemblyName)
		{
			string text = null;
			string text2 = null;
			if (binder != null)
			{
				binder.BindToName(dataContractType, out text2, out text);
			}
			if (text == null)
			{
				text = defaultClrTypeName;
			}
			if (text2 == null)
			{
				text2 = defaultClrAssemblyName;
			}
			NetDataContractSerializer.WriteClrTypeInfo(writer, text, text2);
		}

		internal static void WriteClrTypeInfo(XmlWriterDelegator writer, Type dataContractType, SerializationBinder binder, SerializationInfo serInfo)
		{
			TypeInformation typeInformation = null;
			string text = null;
			string text2 = null;
			if (binder != null)
			{
				binder.BindToName(dataContractType, out text2, out text);
			}
			if (text == null)
			{
				if (serInfo.IsFullTypeNameSetExplicit)
				{
					text = serInfo.FullTypeName;
				}
				else
				{
					typeInformation = NetDataContractSerializer.GetTypeInformation(serInfo.ObjectType);
					text = typeInformation.FullTypeName;
				}
			}
			if (text2 == null)
			{
				if (serInfo.IsAssemblyNameSetExplicit)
				{
					text2 = serInfo.AssemblyName;
				}
				else
				{
					text2 = ((typeInformation == null) ? NetDataContractSerializer.GetTypeInformation(serInfo.ObjectType).AssemblyString : typeInformation.AssemblyString);
				}
			}
			NetDataContractSerializer.WriteClrTypeInfo(writer, text, text2);
		}

		private static void WriteClrTypeInfo(XmlWriterDelegator writer, string clrTypeName, string clrAssemblyName)
		{
			if (clrTypeName != null)
			{
				writer.WriteAttributeString("z", DictionaryGlobals.ClrTypeLocalName, DictionaryGlobals.SerializationNamespace, DataContract.GetClrTypeString(clrTypeName));
			}
			if (clrAssemblyName != null)
			{
				writer.WriteAttributeString("z", DictionaryGlobals.ClrAssemblyLocalName, DictionaryGlobals.SerializationNamespace, DataContract.GetClrTypeString(clrAssemblyName));
			}
		}

		public override void WriteEndObject(XmlDictionaryWriter writer)
		{
			base.WriteEndObjectHandleExceptions(new XmlWriterDelegator(writer));
		}

		internal override void InternalWriteEndObject(XmlWriterDelegator writer)
		{
			writer.WriteEndElement();
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

		internal override object InternalReadObject(XmlReaderDelegator xmlReader, bool verifyObjectName)
		{
			if (this.MaxItemsInObjectGraph == 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[] { this.MaxItemsInObjectGraph })));
			}
			if (!base.IsStartElement(xmlReader))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationExceptionWithReaderDetails(global::System.Runtime.Serialization.SR.GetString("Expecting state '{0}' when ReadObject is called.", new object[] { XmlNodeType.Element }), xmlReader));
			}
			return XmlObjectSerializerReadContext.CreateContext(this).InternalDeserialize(xmlReader, null, null, null);
		}

		internal override bool InternalIsStartObject(XmlReaderDelegator reader)
		{
			return base.IsStartElement(reader);
		}

		internal DataContract GetDataContract(object obj, ref Hashtable surrogateDataContracts)
		{
			return this.GetDataContract((obj == null) ? Globals.TypeOfObject : obj.GetType(), ref surrogateDataContracts);
		}

		internal DataContract GetDataContract(Type type, ref Hashtable surrogateDataContracts)
		{
			return this.GetDataContract(type.TypeHandle, type, ref surrogateDataContracts);
		}

		internal DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type, ref Hashtable surrogateDataContracts)
		{
			DataContract dataContract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, this.Context, typeHandle, type, ref surrogateDataContracts);
			if (dataContract != null)
			{
				return dataContract;
			}
			if (this.cachedDataContract == null)
			{
				dataContract = DataContract.GetDataContract(typeHandle, type, SerializationMode.SharedType);
				this.cachedDataContract = dataContract;
				return dataContract;
			}
			DataContract dataContract2 = this.cachedDataContract;
			if (dataContract2.UnderlyingType.TypeHandle.Equals(typeHandle))
			{
				return dataContract2;
			}
			return DataContract.GetDataContract(typeHandle, type, SerializationMode.SharedType);
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static ISerializationSurrogate GetSurrogate(Type type, ISurrogateSelector surrogateSelector, StreamingContext context)
		{
			ISurrogateSelector surrogateSelector2;
			return surrogateSelector.GetSurrogate(type, context, out surrogateSelector2);
		}

		internal static DataContract GetDataContractFromSurrogateSelector(ISurrogateSelector surrogateSelector, StreamingContext context, RuntimeTypeHandle typeHandle, Type type, ref Hashtable surrogateDataContracts)
		{
			if (surrogateSelector == null)
			{
				return null;
			}
			if (type == null)
			{
				type = Type.GetTypeFromHandle(typeHandle);
			}
			DataContract builtInDataContract = DataContract.GetBuiltInDataContract(type);
			if (builtInDataContract != null)
			{
				return builtInDataContract;
			}
			if (surrogateDataContracts != null)
			{
				DataContract dataContract = (DataContract)surrogateDataContracts[type];
				if (dataContract != null)
				{
					return dataContract;
				}
			}
			DataContract dataContract2 = null;
			ISerializationSurrogate surrogate = NetDataContractSerializer.GetSurrogate(type, surrogateSelector, context);
			if (surrogate != null)
			{
				dataContract2 = new SurrogateDataContract(type, surrogate);
			}
			else if (type.IsArray)
			{
				Type elementType = type.GetElementType();
				DataContract dataContract3 = NetDataContractSerializer.GetDataContractFromSurrogateSelector(surrogateSelector, context, elementType.TypeHandle, elementType, ref surrogateDataContracts);
				if (dataContract3 == null)
				{
					dataContract3 = DataContract.GetDataContract(elementType.TypeHandle, elementType, SerializationMode.SharedType);
				}
				dataContract2 = new CollectionDataContract(type, dataContract3);
			}
			if (dataContract2 != null)
			{
				if (surrogateDataContracts == null)
				{
					surrogateDataContracts = new Hashtable();
				}
				surrogateDataContracts.Add(type, dataContract2);
				return dataContract2;
			}
			return null;
		}

		internal static TypeInformation GetTypeInformation(Type type)
		{
			TypeInformation typeInformation = null;
			object obj = NetDataContractSerializer.typeNameCache[type];
			if (obj == null)
			{
				bool flag;
				string clrAssemblyName = DataContract.GetClrAssemblyName(type, out flag);
				typeInformation = new TypeInformation(DataContract.GetClrTypeFullNameUsingTypeForwardedFromAttribute(type), clrAssemblyName, flag);
				Hashtable hashtable = NetDataContractSerializer.typeNameCache;
				lock (hashtable)
				{
					NetDataContractSerializer.typeNameCache[type] = typeInformation;
					return typeInformation;
				}
			}
			typeInformation = (TypeInformation)obj;
			return typeInformation;
		}

		private static bool IsAssemblyNameForwardingSafe(string originalAssemblyName, string newAssemblyName)
		{
			if (originalAssemblyName == newAssemblyName)
			{
				return true;
			}
			AssemblyName assemblyName = new AssemblyName(originalAssemblyName);
			AssemblyName assemblyName2 = new AssemblyName(newAssemblyName);
			return !string.Equals(assemblyName2.Name, "mscorlib", StringComparison.OrdinalIgnoreCase) && !string.Equals(assemblyName2.Name, "mscorlib.dll", StringComparison.OrdinalIgnoreCase) && NetDataContractSerializer.IsPublicKeyTokenForwardingSafe(assemblyName.GetPublicKeyToken(), assemblyName2.GetPublicKeyToken());
		}

		private static bool IsPublicKeyTokenForwardingSafe(byte[] sourceToken, byte[] destinationToken)
		{
			if (sourceToken == null || destinationToken == null || sourceToken.Length == 0 || destinationToken.Length == 0 || sourceToken.Length != destinationToken.Length)
			{
				return false;
			}
			for (int i = 0; i < sourceToken.Length; i++)
			{
				if (sourceToken[i] != destinationToken[i])
				{
					return false;
				}
			}
			return true;
		}

		private XmlDictionaryString rootName;

		private XmlDictionaryString rootNamespace;

		private StreamingContext context;

		private SerializationBinder binder;

		private ISurrogateSelector surrogateSelector;

		private int maxItemsInObjectGraph;

		private bool ignoreExtensionDataObject;

		private FormatterAssemblyStyle assemblyFormat;

		private DataContract cachedDataContract;

		private static Hashtable typeNameCache = new Hashtable();

		private static bool? unsafeTypeForwardingEnabled;
	}
}
