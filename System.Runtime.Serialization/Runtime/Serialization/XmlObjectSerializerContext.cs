using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlObjectSerializerContext
	{
		internal XmlObjectSerializerContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject, DataContractResolver dataContractResolver)
		{
			this.serializer = serializer;
			this.itemCount = 1;
			this.maxItemsInObjectGraph = maxItemsInObjectGraph;
			this.streamingContext = streamingContext;
			this.ignoreExtensionDataObject = ignoreExtensionDataObject;
			this.dataContractResolver = dataContractResolver;
		}

		internal XmlObjectSerializerContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
			: this(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject, null)
		{
		}

		internal XmlObjectSerializerContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
			: this(serializer, serializer.MaxItemsInObjectGraph, new StreamingContext(StreamingContextStates.All), serializer.IgnoreExtensionDataObject, dataContractResolver)
		{
			this.rootTypeDataContract = rootTypeDataContract;
			this.serializerKnownTypeList = serializer.knownTypeList;
		}

		internal XmlObjectSerializerContext(NetDataContractSerializer serializer)
			: this(serializer, serializer.MaxItemsInObjectGraph, serializer.Context, serializer.IgnoreExtensionDataObject)
		{
		}

		internal virtual SerializationMode Mode
		{
			get
			{
				return SerializationMode.SharedContract;
			}
		}

		internal virtual bool IsGetOnlyCollection
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[SecuritySafeCritical]
		public void DemandSerializationFormatterPermission()
		{
		}

		[SecuritySafeCritical]
		public void DemandMemberAccessPermission()
		{
		}

		public StreamingContext GetStreamingContext()
		{
			return this.streamingContext;
		}

		internal static MethodInfo IncrementItemCountMethod
		{
			get
			{
				if (XmlObjectSerializerContext.incrementItemCountMethod == null)
				{
					XmlObjectSerializerContext.incrementItemCountMethod = typeof(XmlObjectSerializerContext).GetMethod("IncrementItemCount", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return XmlObjectSerializerContext.incrementItemCountMethod;
			}
		}

		public void IncrementItemCount(int count)
		{
			if (count > this.maxItemsInObjectGraph - this.itemCount)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Maximum number of items that can be serialized or deserialized in an object graph is '{0}'.", new object[] { this.maxItemsInObjectGraph })));
			}
			this.itemCount += count;
		}

		internal int RemainingItemCount
		{
			get
			{
				return this.maxItemsInObjectGraph - this.itemCount;
			}
		}

		internal bool IgnoreExtensionDataObject
		{
			get
			{
				return this.ignoreExtensionDataObject;
			}
		}

		protected DataContractResolver DataContractResolver
		{
			get
			{
				return this.dataContractResolver;
			}
		}

		protected KnownTypeDataContractResolver KnownTypeResolver
		{
			get
			{
				if (this.knownTypeResolver == null)
				{
					this.knownTypeResolver = new KnownTypeDataContractResolver(this);
				}
				return this.knownTypeResolver;
			}
		}

		internal DataContract GetDataContract(Type type)
		{
			return this.GetDataContract(type.TypeHandle, type);
		}

		internal virtual DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
		{
			if (this.IsGetOnlyCollection)
			{
				return DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(typeHandle), typeHandle, type, this.Mode);
			}
			return DataContract.GetDataContract(typeHandle, type, this.Mode);
		}

		internal virtual DataContract GetDataContractSkipValidation(int typeId, RuntimeTypeHandle typeHandle, Type type)
		{
			if (this.IsGetOnlyCollection)
			{
				return DataContract.GetGetOnlyCollectionDataContractSkipValidation(typeId, typeHandle, type);
			}
			return DataContract.GetDataContractSkipValidation(typeId, typeHandle, type);
		}

		internal virtual DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
		{
			if (this.IsGetOnlyCollection)
			{
				return DataContract.GetGetOnlyCollectionDataContract(id, typeHandle, null, this.Mode);
			}
			return DataContract.GetDataContract(id, typeHandle, this.Mode);
		}

		internal virtual void CheckIfTypeSerializable(Type memberType, bool isMemberTypeSerializable)
		{
			if (!isMemberTypeSerializable)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be serialized. Consider marking it with the DataContractAttribute attribute, and marking all of its members you want serialized with the DataMemberAttribute attribute. Alternatively, you can ensure that the type is public and has a parameterless constructor - all public members of the type will then be serialized, and no attributes will be required.", new object[] { memberType })));
			}
		}

		internal virtual Type GetSurrogatedType(Type type)
		{
			return type;
		}

		private Dictionary<XmlQualifiedName, DataContract> SerializerKnownDataContracts
		{
			get
			{
				if (!this.isSerializerKnownDataContractsSetExplicit)
				{
					this.serializerKnownDataContracts = this.serializer.KnownDataContracts;
					this.isSerializerKnownDataContractsSetExplicit = true;
				}
				return this.serializerKnownDataContracts;
			}
		}

		private DataContract GetDataContractFromSerializerKnownTypes(XmlQualifiedName qname)
		{
			Dictionary<XmlQualifiedName, DataContract> dictionary = this.SerializerKnownDataContracts;
			if (dictionary == null)
			{
				return null;
			}
			DataContract dataContract;
			if (!dictionary.TryGetValue(qname, out dataContract))
			{
				return null;
			}
			return dataContract;
		}

		internal static Dictionary<XmlQualifiedName, DataContract> GetDataContractsForKnownTypes(IList<Type> knownTypeList)
		{
			if (knownTypeList == null)
			{
				return null;
			}
			Dictionary<XmlQualifiedName, DataContract> dictionary = new Dictionary<XmlQualifiedName, DataContract>();
			Dictionary<Type, Type> dictionary2 = new Dictionary<Type, Type>();
			for (int i = 0; i < knownTypeList.Count; i++)
			{
				Type type = knownTypeList[i];
				if (type == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("One of the known types provided to the serializer via '{0}' argument was invalid because it was null. All known types specified must be non-null values.", new object[] { "knownTypes" })));
				}
				DataContract.CheckAndAdd(type, dictionary2, ref dictionary);
			}
			return dictionary;
		}

		internal bool IsKnownType(DataContract dataContract, Dictionary<XmlQualifiedName, DataContract> knownDataContracts, Type declaredType)
		{
			bool flag = false;
			if (knownDataContracts != null)
			{
				this.scopedKnownTypes.Push(knownDataContracts);
				flag = true;
			}
			bool flag2 = this.IsKnownType(dataContract, declaredType);
			if (flag)
			{
				this.scopedKnownTypes.Pop();
			}
			return flag2;
		}

		internal bool IsKnownType(DataContract dataContract, Type declaredType)
		{
			DataContract dataContract2 = this.ResolveDataContractFromKnownTypes(dataContract.StableName.Name, dataContract.StableName.Namespace, null, declaredType);
			return dataContract2 != null && dataContract2.UnderlyingType == dataContract.UnderlyingType;
		}

		private DataContract ResolveDataContractFromKnownTypes(XmlQualifiedName typeName)
		{
			DataContract dataContract = PrimitiveDataContract.GetPrimitiveDataContract(typeName.Name, typeName.Namespace);
			if (dataContract == null)
			{
				dataContract = this.scopedKnownTypes.GetDataContract(typeName);
				if (dataContract == null)
				{
					dataContract = this.GetDataContractFromSerializerKnownTypes(typeName);
				}
			}
			return dataContract;
		}

		private DataContract ResolveDataContractFromDataContractResolver(XmlQualifiedName typeName, Type declaredType)
		{
			if (TD.DCResolverResolveIsEnabled())
			{
				TD.DCResolverResolve(typeName.Name + ":" + typeName.Namespace);
			}
			Type type = this.DataContractResolver.ResolveName(typeName.Name, typeName.Namespace, declaredType, this.KnownTypeResolver);
			if (type == null)
			{
				return null;
			}
			return this.GetDataContract(type);
		}

		internal Type ResolveNameFromKnownTypes(XmlQualifiedName typeName)
		{
			DataContract dataContract = this.ResolveDataContractFromKnownTypes(typeName);
			if (dataContract == null)
			{
				return null;
			}
			return dataContract.OriginalUnderlyingType;
		}

		protected DataContract ResolveDataContractFromKnownTypes(string typeName, string typeNs, DataContract memberTypeContract, Type declaredType)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(typeName, typeNs);
			DataContract dataContract;
			if (this.DataContractResolver == null)
			{
				dataContract = this.ResolveDataContractFromKnownTypes(xmlQualifiedName);
			}
			else
			{
				dataContract = this.ResolveDataContractFromDataContractResolver(xmlQualifiedName, declaredType);
			}
			if (dataContract == null)
			{
				if (memberTypeContract != null && !memberTypeContract.UnderlyingType.IsInterface && memberTypeContract.StableName == xmlQualifiedName)
				{
					dataContract = memberTypeContract;
				}
				if (dataContract == null && this.rootTypeDataContract != null)
				{
					dataContract = this.ResolveDataContractFromRootDataContract(xmlQualifiedName);
				}
			}
			return dataContract;
		}

		protected virtual DataContract ResolveDataContractFromRootDataContract(XmlQualifiedName typeQName)
		{
			if (this.rootTypeDataContract.StableName == typeQName)
			{
				return this.rootTypeDataContract;
			}
			DataContract dataContract;
			for (CollectionDataContract collectionDataContract = this.rootTypeDataContract as CollectionDataContract; collectionDataContract != null; collectionDataContract = dataContract as CollectionDataContract)
			{
				dataContract = this.GetDataContract(this.GetSurrogatedType(collectionDataContract.ItemType));
				if (dataContract.StableName == typeQName)
				{
					return dataContract;
				}
			}
			return null;
		}

		protected XmlObjectSerializer serializer;

		protected DataContract rootTypeDataContract;

		internal ScopedKnownTypes scopedKnownTypes;

		protected Dictionary<XmlQualifiedName, DataContract> serializerKnownDataContracts;

		private bool isSerializerKnownDataContractsSetExplicit;

		protected IList<Type> serializerKnownTypeList;

		[SecurityCritical]
		private bool demandedSerializationFormatterPermission;

		[SecurityCritical]
		private bool demandedMemberAccessPermission;

		private int itemCount;

		private int maxItemsInObjectGraph;

		private StreamingContext streamingContext;

		private bool ignoreExtensionDataObject;

		private DataContractResolver dataContractResolver;

		private KnownTypeDataContractResolver knownTypeResolver;

		private static MethodInfo incrementItemCountMethod;
	}
}
