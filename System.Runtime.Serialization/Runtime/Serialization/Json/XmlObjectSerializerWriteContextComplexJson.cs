using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class XmlObjectSerializerWriteContextComplexJson : XmlObjectSerializerWriteContextComplex
	{
		public XmlObjectSerializerWriteContextComplexJson(DataContractJsonSerializer serializer, DataContract rootTypeDataContract)
			: base(serializer, serializer.MaxItemsInObjectGraph, new StreamingContext(StreamingContextStates.All), serializer.IgnoreExtensionDataObject)
		{
			this.emitXsiType = serializer.EmitTypeInformation;
			this.rootTypeDataContract = rootTypeDataContract;
			this.serializerKnownTypeList = serializer.knownTypeList;
			this.dataContractSurrogate = serializer.DataContractSurrogate;
			this.serializeReadOnlyTypes = serializer.SerializeReadOnlyTypes;
			this.useSimpleDictionaryFormat = serializer.UseSimpleDictionaryFormat;
		}

		internal static XmlObjectSerializerWriteContextComplexJson CreateContext(DataContractJsonSerializer serializer, DataContract rootTypeDataContract)
		{
			return new XmlObjectSerializerWriteContextComplexJson(serializer, rootTypeDataContract);
		}

		internal IList<Type> SerializerKnownTypeList
		{
			get
			{
				return this.serializerKnownTypeList;
			}
		}

		public bool UseSimpleDictionaryFormat
		{
			get
			{
				return this.useSimpleDictionaryFormat;
			}
		}

		internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, Type dataContractType, string clrTypeName, string clrAssemblyName)
		{
			return false;
		}

		internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, DataContract dataContract)
		{
			return false;
		}

		internal override void WriteArraySize(XmlWriterDelegator xmlWriter, int size)
		{
		}

		protected override void WriteTypeInfo(XmlWriterDelegator writer, string dataContractName, string dataContractNamespace)
		{
			if (this.emitXsiType != EmitTypeInformation.Never)
			{
				if (string.IsNullOrEmpty(dataContractNamespace))
				{
					this.WriteTypeInfo(writer, dataContractName);
					return;
				}
				this.WriteTypeInfo(writer, dataContractName + ":" + XmlObjectSerializerWriteContextComplexJson.TruncateDefaultDataContractNamespace(dataContractNamespace));
			}
		}

		internal static string TruncateDefaultDataContractNamespace(string dataContractNamespace)
		{
			if (!string.IsNullOrEmpty(dataContractNamespace))
			{
				if (dataContractNamespace[0] == '#')
				{
					return "\\" + dataContractNamespace;
				}
				if (dataContractNamespace[0] == '\\')
				{
					return "\\" + dataContractNamespace;
				}
				if (dataContractNamespace.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
				{
					return "#" + dataContractNamespace.Substring(JsonGlobals.DataContractXsdBaseNamespaceLength);
				}
			}
			return dataContractNamespace;
		}

		private static bool RequiresJsonTypeInfo(DataContract contract)
		{
			return contract is ClassDataContract;
		}

		private void WriteTypeInfo(XmlWriterDelegator writer, string typeInformation)
		{
			writer.WriteAttributeString(null, "__type", null, typeInformation);
		}

		protected override bool WriteTypeInfo(XmlWriterDelegator writer, DataContract contract, DataContract declaredContract)
		{
			if ((contract.Name != declaredContract.Name || contract.Namespace != declaredContract.Namespace) && (!(contract.Name.Value == declaredContract.Name.Value) || !(contract.Namespace.Value == declaredContract.Namespace.Value)) && contract.UnderlyingType != Globals.TypeOfObjectArray && this.emitXsiType != EmitTypeInformation.Never)
			{
				if (XmlObjectSerializerWriteContextComplexJson.RequiresJsonTypeInfo(contract))
				{
					this.perCallXsiTypeAlreadyEmitted = true;
					this.WriteTypeInfo(writer, contract.Name.Value, contract.Namespace.Value);
				}
				else if (declaredContract.UnderlyingType == typeof(Enum))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Enum type is not supported by DataContractJsonSerializer. The underlying type is '{0}'.", new object[] { declaredContract.UnderlyingType })));
				}
				return true;
			}
			return false;
		}

		internal void WriteJsonISerializable(XmlWriterDelegator xmlWriter, ISerializable obj)
		{
			Type type = obj.GetType();
			SerializationInfo serializationInfo = new SerializationInfo(type, XmlObjectSerializer.FormatterConverter);
			base.GetObjectData(obj, serializationInfo, base.GetStreamingContext());
			if (DataContract.GetClrTypeFullName(type) != serializationInfo.FullTypeName)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Changing full type name is not supported. Serialization type name: '{0}', data contract type name: '{1}'.", new object[]
				{
					serializationInfo.FullTypeName,
					DataContract.GetClrTypeFullName(type)
				})));
			}
			base.WriteSerializationInfo(xmlWriter, type, serializationInfo);
		}

		internal static DataContract GetRevisedItemContract(DataContract oldItemContract)
		{
			if (oldItemContract != null && oldItemContract.UnderlyingType.IsGenericType && oldItemContract.UnderlyingType.GetGenericTypeDefinition() == Globals.TypeOfKeyValue)
			{
				return DataContract.GetDataContract(oldItemContract.UnderlyingType);
			}
			return oldItemContract;
		}

		protected override void WriteDataContractValue(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
		{
			JsonDataContract jsonDataContract = JsonDataContract.GetJsonDataContract(dataContract);
			if (this.emitXsiType == EmitTypeInformation.Always && !this.perCallXsiTypeAlreadyEmitted && XmlObjectSerializerWriteContextComplexJson.RequiresJsonTypeInfo(dataContract))
			{
				this.WriteTypeInfo(xmlWriter, jsonDataContract.TypeName);
			}
			this.perCallXsiTypeAlreadyEmitted = false;
			DataContractJsonSerializer.WriteJsonValue(jsonDataContract, xmlWriter, obj, this, declaredTypeHandle);
		}

		protected override void WriteNull(XmlWriterDelegator xmlWriter)
		{
			DataContractJsonSerializer.WriteJsonNull(xmlWriter);
		}

		internal XmlDictionaryString CollectionItemName
		{
			get
			{
				return JsonGlobals.itemDictionaryString;
			}
		}

		internal static void WriteJsonNameWithMapping(XmlWriterDelegator xmlWriter, XmlDictionaryString[] memberNames, int index)
		{
			xmlWriter.WriteStartElement("a", "item", "item");
			xmlWriter.WriteAttributeString(null, "item", null, memberNames[index].Value);
		}

		internal override void WriteExtensionDataTypeInfo(XmlWriterDelegator xmlWriter, IDataNode dataNode)
		{
			Type dataType = dataNode.DataType;
			if (dataType == Globals.TypeOfClassDataNode || dataType == Globals.TypeOfISerializableDataNode)
			{
				xmlWriter.WriteAttributeString(null, "type", null, "object");
				base.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
				return;
			}
			if (dataType == Globals.TypeOfCollectionDataNode)
			{
				xmlWriter.WriteAttributeString(null, "type", null, "array");
				return;
			}
			if (!(dataType == Globals.TypeOfXmlDataNode) && dataType == Globals.TypeOfObject && dataNode.Value != null && XmlObjectSerializerWriteContextComplexJson.RequiresJsonTypeInfo(base.GetDataContract(dataNode.Value.GetType())))
			{
				base.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
			}
		}

		protected override void SerializeWithXsiType(XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle objectTypeHandle, Type objectType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
		{
			bool flag = false;
			bool isInterface = declaredType.IsInterface;
			DataContract dataContract;
			if (isInterface && CollectionDataContract.IsCollectionInterface(declaredType))
			{
				dataContract = this.GetDataContract(declaredTypeHandle, declaredType);
			}
			else if (declaredType.IsArray)
			{
				dataContract = this.GetDataContract(declaredTypeHandle, declaredType);
			}
			else
			{
				dataContract = this.GetDataContract(objectTypeHandle, objectType);
				DataContract dataContract2 = ((declaredTypeID >= 0) ? this.GetDataContract(declaredTypeID, declaredTypeHandle) : this.GetDataContract(declaredTypeHandle, declaredType));
				flag = this.WriteTypeInfo(xmlWriter, dataContract, dataContract2);
				this.HandleCollectionAssignedToObject(declaredType, ref dataContract, ref obj, ref flag);
			}
			if (isInterface)
			{
				XmlObjectSerializerWriteContextComplexJson.VerifyObjectCompatibilityWithInterface(dataContract, obj, declaredType);
			}
			base.SerializeAndVerifyType(dataContract, xmlWriter, obj, flag, declaredType.TypeHandle, declaredType);
		}

		private static void VerifyObjectCompatibilityWithInterface(DataContract contract, object graph, Type declaredType)
		{
			Type type = contract.GetType();
			if (type == typeof(XmlDataContract) && !Globals.TypeOfIXmlSerializable.IsAssignableFrom(declaredType))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Object of type '{0}' is assigned to an incompatible interface '{1}'.", new object[]
				{
					graph.GetType(),
					declaredType
				})));
			}
			if (type == typeof(CollectionDataContract) && !CollectionDataContract.IsCollectionInterface(declaredType))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Collection of type '{0}' is assigned to an incompatible interface '{1}'", new object[]
				{
					graph.GetType(),
					declaredType
				})));
			}
		}

		private void HandleCollectionAssignedToObject(Type declaredType, ref DataContract dataContract, ref object obj, ref bool verifyKnownType)
		{
			if (declaredType != dataContract.UnderlyingType && dataContract is CollectionDataContract)
			{
				if (verifyKnownType)
				{
					this.VerifyType(dataContract, declaredType);
					verifyKnownType = false;
				}
				if (((CollectionDataContract)dataContract).Kind == CollectionKind.Dictionary)
				{
					IDictionary dictionary = obj as IDictionary;
					Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
					foreach (object obj2 in dictionary)
					{
						DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
						dictionary2.Add(dictionaryEntry.Key, dictionaryEntry.Value);
					}
					obj = dictionary2;
				}
				dataContract = base.GetDataContract(Globals.TypeOfIEnumerable);
			}
		}

		internal override void SerializeWithXsiTypeAtTopLevel(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle originalDeclaredTypeHandle, Type graphType)
		{
			bool flag = false;
			Type underlyingType = this.rootTypeDataContract.UnderlyingType;
			bool isInterface = underlyingType.IsInterface;
			if ((!isInterface || !CollectionDataContract.IsCollectionInterface(underlyingType)) && !underlyingType.IsArray)
			{
				flag = this.WriteTypeInfo(xmlWriter, dataContract, this.rootTypeDataContract);
				this.HandleCollectionAssignedToObject(underlyingType, ref dataContract, ref obj, ref flag);
			}
			if (isInterface)
			{
				XmlObjectSerializerWriteContextComplexJson.VerifyObjectCompatibilityWithInterface(dataContract, obj, underlyingType);
			}
			base.SerializeAndVerifyType(dataContract, xmlWriter, obj, flag, underlyingType.TypeHandle, underlyingType);
		}

		private void VerifyType(DataContract dataContract, Type declaredType)
		{
			bool flag = false;
			if (dataContract.KnownDataContracts != null)
			{
				this.scopedKnownTypes.Push(dataContract.KnownDataContracts);
				flag = true;
			}
			if (!base.IsKnownType(dataContract, declaredType))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' with data contract name '{1}:{2}' is not expected. Add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to DataContractSerializer.", new object[]
				{
					DataContract.GetClrTypeFullName(dataContract.UnderlyingType),
					dataContract.StableName.Name,
					dataContract.StableName.Namespace
				})));
			}
			if (flag)
			{
				this.scopedKnownTypes.Pop();
			}
		}

		internal override DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContract = base.GetDataContract(typeHandle, type);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
			return dataContract;
		}

		internal override DataContract GetDataContractSkipValidation(int typeId, RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContractSkipValidation = base.GetDataContractSkipValidation(typeId, typeHandle, type);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContractSkipValidation);
			return dataContractSkipValidation;
		}

		internal override DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
		{
			DataContract dataContract = base.GetDataContract(id, typeHandle);
			DataContractJsonSerializer.CheckIfTypeIsReference(dataContract);
			return dataContract;
		}

		internal static DataContract ResolveJsonDataContractFromRootDataContract(XmlObjectSerializerContext context, XmlQualifiedName typeQName, DataContract rootTypeDataContract)
		{
			if (rootTypeDataContract.StableName == typeQName)
			{
				return rootTypeDataContract;
			}
			DataContract dataContract;
			for (CollectionDataContract collectionDataContract = rootTypeDataContract as CollectionDataContract; collectionDataContract != null; collectionDataContract = dataContract as CollectionDataContract)
			{
				if (collectionDataContract.ItemType.IsGenericType && collectionDataContract.ItemType.GetGenericTypeDefinition() == typeof(KeyValue<, >))
				{
					dataContract = context.GetDataContract(Globals.TypeOfKeyValuePair.MakeGenericType(collectionDataContract.ItemType.GetGenericArguments()));
				}
				else
				{
					dataContract = context.GetDataContract(context.GetSurrogatedType(collectionDataContract.ItemType));
				}
				if (dataContract.StableName == typeQName)
				{
					return dataContract;
				}
			}
			return null;
		}

		protected override DataContract ResolveDataContractFromRootDataContract(XmlQualifiedName typeQName)
		{
			return XmlObjectSerializerWriteContextComplexJson.ResolveJsonDataContractFromRootDataContract(this, typeQName, this.rootTypeDataContract);
		}

		private EmitTypeInformation emitXsiType;

		private bool perCallXsiTypeAlreadyEmitted;

		private bool useSimpleDictionaryFormat;
	}
}
