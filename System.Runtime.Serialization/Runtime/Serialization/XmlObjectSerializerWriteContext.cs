using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Diagnostics;
using System.Security;
using System.Xml;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal class XmlObjectSerializerWriteContext : XmlObjectSerializerContext
	{
		internal static XmlObjectSerializerWriteContext CreateContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
		{
			if (!serializer.PreserveObjectReferences && serializer.DataContractSurrogate == null)
			{
				return new XmlObjectSerializerWriteContext(serializer, rootTypeDataContract, dataContractResolver);
			}
			return new XmlObjectSerializerWriteContextComplex(serializer, rootTypeDataContract, dataContractResolver);
		}

		internal static XmlObjectSerializerWriteContext CreateContext(NetDataContractSerializer serializer, Hashtable surrogateDataContracts)
		{
			return new XmlObjectSerializerWriteContextComplex(serializer, surrogateDataContracts);
		}

		protected XmlObjectSerializerWriteContext(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver resolver)
			: base(serializer, rootTypeDataContract, resolver)
		{
			this.serializeReadOnlyTypes = serializer.SerializeReadOnlyTypes;
			this.unsafeTypeForwardingEnabled = true;
		}

		protected XmlObjectSerializerWriteContext(NetDataContractSerializer serializer)
			: base(serializer)
		{
			this.unsafeTypeForwardingEnabled = NetDataContractSerializer.UnsafeTypeForwardingEnabled;
		}

		internal XmlObjectSerializerWriteContext(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
			: base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
		{
			this.unsafeTypeForwardingEnabled = true;
		}

		protected ObjectToIdCache SerializedObjects
		{
			get
			{
				if (this.serializedObjects == null)
				{
					this.serializedObjects = new ObjectToIdCache();
				}
				return this.serializedObjects;
			}
		}

		internal override bool IsGetOnlyCollection
		{
			get
			{
				return this.isGetOnlyCollection;
			}
			set
			{
				this.isGetOnlyCollection = value;
			}
		}

		internal bool SerializeReadOnlyTypes
		{
			get
			{
				return this.serializeReadOnlyTypes;
			}
		}

		internal bool UnsafeTypeForwardingEnabled
		{
			get
			{
				return this.unsafeTypeForwardingEnabled;
			}
		}

		internal void StoreIsGetOnlyCollection()
		{
			this.isGetOnlyCollection = true;
		}

		public void InternalSerializeReference(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
		{
			if (!this.OnHandleReference(xmlWriter, obj, true))
			{
				this.InternalSerialize(xmlWriter, obj, isDeclaredType, writeXsiType, declaredTypeID, declaredTypeHandle);
			}
			this.OnEndHandleReference(xmlWriter, obj, true);
		}

		public virtual void InternalSerialize(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
		{
			if (writeXsiType)
			{
				Type typeOfObject = Globals.TypeOfObject;
				this.SerializeWithXsiType(xmlWriter, obj, Type.GetTypeHandle(obj), null, -1, typeOfObject.TypeHandle, typeOfObject);
				return;
			}
			if (isDeclaredType)
			{
				DataContract dataContract = this.GetDataContract(declaredTypeID, declaredTypeHandle);
				this.SerializeWithoutXsiType(dataContract, xmlWriter, obj, declaredTypeHandle);
				return;
			}
			RuntimeTypeHandle typeHandle = Type.GetTypeHandle(obj);
			if (declaredTypeHandle.Equals(typeHandle))
			{
				DataContract dataContract2 = ((declaredTypeID >= 0) ? this.GetDataContract(declaredTypeID, declaredTypeHandle) : this.GetDataContract(declaredTypeHandle, null));
				this.SerializeWithoutXsiType(dataContract2, xmlWriter, obj, declaredTypeHandle);
				return;
			}
			this.SerializeWithXsiType(xmlWriter, obj, typeHandle, null, declaredTypeID, declaredTypeHandle, Type.GetTypeFromHandle(declaredTypeHandle));
		}

		internal void SerializeWithoutXsiType(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
		{
			if (this.OnHandleIsReference(xmlWriter, dataContract, obj))
			{
				return;
			}
			if (dataContract.KnownDataContracts != null)
			{
				this.scopedKnownTypes.Push(dataContract.KnownDataContracts);
				this.WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
				this.scopedKnownTypes.Pop();
				return;
			}
			this.WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
		}

		internal virtual void SerializeWithXsiTypeAtTopLevel(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle originalDeclaredTypeHandle, Type graphType)
		{
			bool flag = false;
			Type originalUnderlyingType = this.rootTypeDataContract.OriginalUnderlyingType;
			if (originalUnderlyingType.IsInterface && CollectionDataContract.IsCollectionInterface(originalUnderlyingType))
			{
				if (base.DataContractResolver != null)
				{
					this.WriteResolvedTypeInfo(xmlWriter, graphType, originalUnderlyingType);
				}
			}
			else if (!originalUnderlyingType.IsArray)
			{
				flag = this.WriteTypeInfo(xmlWriter, dataContract, this.rootTypeDataContract);
			}
			this.SerializeAndVerifyType(dataContract, xmlWriter, obj, flag, originalDeclaredTypeHandle, originalUnderlyingType);
		}

		protected virtual void SerializeWithXsiType(XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle objectTypeHandle, Type objectType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
		{
			bool flag = false;
			DataContract dataContract;
			if (declaredType.IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
			{
				dataContract = this.GetDataContractSkipValidation(DataContract.GetId(objectTypeHandle), objectTypeHandle, objectType);
				if (this.OnHandleIsReference(xmlWriter, dataContract, obj))
				{
					return;
				}
				if (this.Mode == SerializationMode.SharedType && dataContract.IsValidContract(this.Mode))
				{
					dataContract = dataContract.GetValidContract(this.Mode);
				}
				else
				{
					dataContract = this.GetDataContract(declaredTypeHandle, declaredType);
				}
				if (!this.WriteClrTypeInfo(xmlWriter, dataContract) && base.DataContractResolver != null)
				{
					if (objectType == null)
					{
						objectType = Type.GetTypeFromHandle(objectTypeHandle);
					}
					this.WriteResolvedTypeInfo(xmlWriter, objectType, declaredType);
				}
			}
			else if (declaredType.IsArray)
			{
				dataContract = this.GetDataContract(objectTypeHandle, objectType);
				this.WriteClrTypeInfo(xmlWriter, dataContract);
				dataContract = this.GetDataContract(declaredTypeHandle, declaredType);
			}
			else
			{
				dataContract = this.GetDataContract(objectTypeHandle, objectType);
				if (this.OnHandleIsReference(xmlWriter, dataContract, obj))
				{
					return;
				}
				if (!this.WriteClrTypeInfo(xmlWriter, dataContract))
				{
					DataContract dataContract2 = ((declaredTypeID >= 0) ? this.GetDataContract(declaredTypeID, declaredTypeHandle) : this.GetDataContract(declaredTypeHandle, declaredType));
					flag = this.WriteTypeInfo(xmlWriter, dataContract, dataContract2);
				}
			}
			this.SerializeAndVerifyType(dataContract, xmlWriter, obj, flag, declaredTypeHandle, declaredType);
		}

		internal bool OnHandleIsReference(XmlWriterDelegator xmlWriter, DataContract contract, object obj)
		{
			if (this.preserveObjectReferences || !contract.IsReference || this.isGetOnlyCollection)
			{
				return false;
			}
			bool flag = true;
			int id = this.SerializedObjects.GetId(obj, ref flag);
			this.byValObjectsInScope.EnsureSetAsIsReference(obj);
			if (flag)
			{
				xmlWriter.WriteAttributeString("z", DictionaryGlobals.IdLocalName, DictionaryGlobals.SerializationNamespace, string.Format(CultureInfo.InvariantCulture, "{0}{1}", "i", id));
				return false;
			}
			xmlWriter.WriteAttributeString("z", DictionaryGlobals.RefLocalName, DictionaryGlobals.SerializationNamespace, string.Format(CultureInfo.InvariantCulture, "{0}{1}", "i", id));
			return true;
		}

		protected void SerializeAndVerifyType(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, bool verifyKnownType, RuntimeTypeHandle declaredTypeHandle, Type declaredType)
		{
			bool flag = false;
			if (dataContract.KnownDataContracts != null)
			{
				this.scopedKnownTypes.Push(dataContract.KnownDataContracts);
				flag = true;
			}
			if (verifyKnownType && !base.IsKnownType(dataContract, declaredType))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' with data contract name '{1}:{2}' is not expected. Add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to DataContractSerializer.", new object[]
				{
					DataContract.GetClrTypeFullName(dataContract.UnderlyingType),
					dataContract.StableName.Name,
					dataContract.StableName.Namespace
				})));
			}
			this.WriteDataContractValue(dataContract, xmlWriter, obj, declaredTypeHandle);
			if (flag)
			{
				this.scopedKnownTypes.Pop();
			}
		}

		internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, DataContract dataContract)
		{
			return false;
		}

		internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, Type dataContractType, string clrTypeName, string clrAssemblyName)
		{
			return false;
		}

		internal virtual bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, Type dataContractType, SerializationInfo serInfo)
		{
			return false;
		}

		public virtual void WriteAnyType(XmlWriterDelegator xmlWriter, object value)
		{
			xmlWriter.WriteAnyType(value);
		}

		public virtual void WriteString(XmlWriterDelegator xmlWriter, string value)
		{
			xmlWriter.WriteString(value);
		}

		public virtual void WriteString(XmlWriterDelegator xmlWriter, string value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				this.WriteNull(xmlWriter, typeof(string), true, name, ns);
				return;
			}
			xmlWriter.WriteStartElementPrimitive(name, ns);
			xmlWriter.WriteString(value);
			xmlWriter.WriteEndElementPrimitive();
		}

		public virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value)
		{
			xmlWriter.WriteBase64(value);
		}

		public virtual void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				this.WriteNull(xmlWriter, typeof(byte[]), true, name, ns);
				return;
			}
			xmlWriter.WriteStartElementPrimitive(name, ns);
			xmlWriter.WriteBase64(value);
			xmlWriter.WriteEndElementPrimitive();
		}

		public virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value)
		{
			xmlWriter.WriteUri(value);
		}

		public virtual void WriteUri(XmlWriterDelegator xmlWriter, Uri value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				this.WriteNull(xmlWriter, typeof(Uri), true, name, ns);
				return;
			}
			xmlWriter.WriteStartElementPrimitive(name, ns);
			xmlWriter.WriteUri(value);
			xmlWriter.WriteEndElementPrimitive();
		}

		public virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value)
		{
			xmlWriter.WriteQName(value);
		}

		public virtual void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				this.WriteNull(xmlWriter, typeof(XmlQualifiedName), true, name, ns);
				return;
			}
			if (ns != null && ns.Value != null && ns.Value.Length > 0)
			{
				xmlWriter.WriteStartElement("q", name, ns);
			}
			else
			{
				xmlWriter.WriteStartElement(name, ns);
			}
			xmlWriter.WriteQName(value);
			xmlWriter.WriteEndElement();
		}

		internal void HandleGraphAtTopLevel(XmlWriterDelegator writer, object obj, DataContract contract)
		{
			writer.WriteXmlnsAttribute("i", DictionaryGlobals.SchemaInstanceNamespace);
			if (contract.IsISerializable)
			{
				writer.WriteXmlnsAttribute("x", DictionaryGlobals.SchemaNamespace);
			}
			this.OnHandleReference(writer, obj, true);
		}

		internal virtual bool OnHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
		{
			if (xmlWriter.depth < 512)
			{
				return false;
			}
			if (canContainCyclicReference)
			{
				if (this.byValObjectsInScope.Count == 0 && DiagnosticUtility.ShouldTraceWarning)
				{
					TraceUtility.Trace(TraceEventType.Warning, 196626, global::System.Runtime.Serialization.SR.GetString("Object with large depth"));
				}
				if (this.byValObjectsInScope.Contains(obj))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Object graph for type '{0}' contains cycles and cannot be serialized if references are not tracked. Consider using the DataContractAttribute with the IsReference property set to true.", new object[] { DataContract.GetClrTypeFullName(obj.GetType()) })));
				}
				this.byValObjectsInScope.Push(obj);
			}
			return false;
		}

		internal virtual void OnEndHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
		{
			if (xmlWriter.depth < 512)
			{
				return;
			}
			if (canContainCyclicReference)
			{
				this.byValObjectsInScope.Pop(obj);
			}
		}

		public void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable)
		{
			this.CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
			this.WriteNull(xmlWriter);
		}

		internal void WriteNull(XmlWriterDelegator xmlWriter, Type memberType, bool isMemberTypeSerializable, XmlDictionaryString name, XmlDictionaryString ns)
		{
			xmlWriter.WriteStartElement(name, ns);
			this.WriteNull(xmlWriter, memberType, isMemberTypeSerializable);
			xmlWriter.WriteEndElement();
		}

		public void IncrementArrayCount(XmlWriterDelegator xmlWriter, Array array)
		{
			this.IncrementCollectionCount(xmlWriter, array.GetLength(0));
		}

		public void IncrementCollectionCount(XmlWriterDelegator xmlWriter, ICollection collection)
		{
			this.IncrementCollectionCount(xmlWriter, collection.Count);
		}

		public void IncrementCollectionCountGeneric<T>(XmlWriterDelegator xmlWriter, ICollection<T> collection)
		{
			this.IncrementCollectionCount(xmlWriter, collection.Count);
		}

		private void IncrementCollectionCount(XmlWriterDelegator xmlWriter, int size)
		{
			base.IncrementItemCount(size);
			this.WriteArraySize(xmlWriter, size);
		}

		internal virtual void WriteArraySize(XmlWriterDelegator xmlWriter, int size)
		{
		}

		public static T GetDefaultValue<T>()
		{
			return default(T);
		}

		public static T GetNullableValue<T>(T? value) where T : struct
		{
			return value.Value;
		}

		public static void ThrowRequiredMemberMustBeEmitted(string memberName, Type type)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("Member {0} in type {1} cannot be serialized. This exception is usually caused by trying to use a null value where a null value is not allowed. The '{0}' member is set to its default value (usually null or zero). The member's EmitDefault setting is 'false', indicating that the member should not be serialized. However, the member's IsRequired setting is 'true', indicating that it must be serialized. This conflict cannot be resolved.  Consider setting '{0}' to a non-default value. Alternatively, you can change the EmitDefaultValue property on the DataMemberAttribute attribute to true, or changing the IsRequired property to false.", new object[] { memberName, type.FullName })));
		}

		public static bool GetHasValue<T>(T? value) where T : struct
		{
			return value != null;
		}

		internal void WriteIXmlSerializable(XmlWriterDelegator xmlWriter, object obj)
		{
			if (this.xmlSerializableWriter == null)
			{
				this.xmlSerializableWriter = new XmlSerializableWriter();
			}
			XmlObjectSerializerWriteContext.WriteIXmlSerializable(xmlWriter, obj, this.xmlSerializableWriter);
		}

		internal static void WriteRootIXmlSerializable(XmlWriterDelegator xmlWriter, object obj)
		{
			XmlObjectSerializerWriteContext.WriteIXmlSerializable(xmlWriter, obj, new XmlSerializableWriter());
		}

		private static void WriteIXmlSerializable(XmlWriterDelegator xmlWriter, object obj, XmlSerializableWriter xmlSerializableWriter)
		{
			xmlSerializableWriter.BeginWrite(xmlWriter.Writer, obj);
			IXmlSerializable xmlSerializable = obj as IXmlSerializable;
			if (xmlSerializable != null)
			{
				xmlSerializable.WriteXml(xmlSerializableWriter);
			}
			else
			{
				XmlElement xmlElement = obj as XmlElement;
				if (xmlElement != null)
				{
					xmlElement.WriteTo(xmlSerializableWriter);
				}
				else
				{
					XmlNode[] array = obj as XmlNode[];
					if (array == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Unknown XML type: '{0}'.", new object[] { DataContract.GetClrTypeFullName(obj.GetType()) })));
					}
					XmlNode[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].WriteTo(xmlSerializableWriter);
					}
				}
			}
			xmlSerializableWriter.EndWrite();
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal void GetObjectData(ISerializable obj, SerializationInfo serInfo, StreamingContext context)
		{
			obj.GetObjectData(serInfo, context);
		}

		public void WriteISerializable(XmlWriterDelegator xmlWriter, ISerializable obj)
		{
			Type type = obj.GetType();
			SerializationInfo serializationInfo = new SerializationInfo(type, XmlObjectSerializer.FormatterConverter, !this.UnsafeTypeForwardingEnabled);
			this.GetObjectData(obj, serializationInfo, base.GetStreamingContext());
			if (!this.UnsafeTypeForwardingEnabled && serializationInfo.AssemblyName == "0")
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("ISerializable AssemblyName is set to \"0\" for type '{0}'.", new object[] { DataContract.GetClrTypeFullName(obj.GetType()) })));
			}
			this.WriteSerializationInfo(xmlWriter, type, serializationInfo);
		}

		internal void WriteSerializationInfo(XmlWriterDelegator xmlWriter, Type objType, SerializationInfo serInfo)
		{
			if (DataContract.GetClrTypeFullName(objType) != serInfo.FullTypeName)
			{
				if (base.DataContractResolver != null)
				{
					XmlDictionaryString xmlDictionaryString;
					XmlDictionaryString xmlDictionaryString2;
					if (this.ResolveType(serInfo.ObjectType, objType, out xmlDictionaryString, out xmlDictionaryString2))
					{
						xmlWriter.WriteAttributeQualifiedName("z", DictionaryGlobals.ISerializableFactoryTypeLocalName, DictionaryGlobals.SerializationNamespace, xmlDictionaryString, xmlDictionaryString2);
					}
				}
				else
				{
					string text;
					string text2;
					DataContract.GetDefaultStableName(serInfo.FullTypeName, out text, out text2);
					xmlWriter.WriteAttributeQualifiedName("z", DictionaryGlobals.ISerializableFactoryTypeLocalName, DictionaryGlobals.SerializationNamespace, DataContract.GetClrTypeString(text), DataContract.GetClrTypeString(text2));
				}
			}
			this.WriteClrTypeInfo(xmlWriter, objType, serInfo);
			base.IncrementItemCount(serInfo.MemberCount);
			foreach (SerializationEntry serializationEntry in serInfo)
			{
				XmlDictionaryString clrTypeString = DataContract.GetClrTypeString(DataContract.EncodeLocalName(serializationEntry.Name));
				xmlWriter.WriteStartElement(clrTypeString, DictionaryGlobals.EmptyString);
				object value = serializationEntry.Value;
				if (value == null)
				{
					this.WriteNull(xmlWriter);
				}
				else
				{
					this.InternalSerializeReference(xmlWriter, value, false, false, -1, Globals.TypeOfObject.TypeHandle);
				}
				xmlWriter.WriteEndElement();
			}
		}

		public void WriteExtensionData(XmlWriterDelegator xmlWriter, ExtensionDataObject extensionData, int memberIndex)
		{
			if (base.IgnoreExtensionDataObject || extensionData == null)
			{
				return;
			}
			if (extensionData.Members != null)
			{
				for (int i = 0; i < extensionData.Members.Count; i++)
				{
					ExtensionDataMember extensionDataMember = extensionData.Members[i];
					if (extensionDataMember.MemberIndex == memberIndex)
					{
						this.WriteExtensionDataMember(xmlWriter, extensionDataMember);
					}
				}
			}
		}

		private void WriteExtensionDataMember(XmlWriterDelegator xmlWriter, ExtensionDataMember member)
		{
			xmlWriter.WriteStartElement(member.Name, member.Namespace);
			IDataNode value = member.Value;
			this.WriteExtensionDataValue(xmlWriter, value);
			xmlWriter.WriteEndElement();
		}

		internal virtual void WriteExtensionDataTypeInfo(XmlWriterDelegator xmlWriter, IDataNode dataNode)
		{
			if (dataNode.DataContractName != null)
			{
				this.WriteTypeInfo(xmlWriter, dataNode.DataContractName, dataNode.DataContractNamespace);
			}
			this.WriteClrTypeInfo(xmlWriter, dataNode.DataType, dataNode.ClrTypeName, dataNode.ClrAssemblyName);
		}

		internal void WriteExtensionDataValue(XmlWriterDelegator xmlWriter, IDataNode dataNode)
		{
			base.IncrementItemCount(1);
			if (dataNode == null)
			{
				this.WriteNull(xmlWriter);
				return;
			}
			if (dataNode.PreservesReferences && this.OnHandleReference(xmlWriter, (dataNode.Value == null) ? dataNode : dataNode.Value, true))
			{
				return;
			}
			Type dataType = dataNode.DataType;
			if (dataType == Globals.TypeOfClassDataNode)
			{
				this.WriteExtensionClassData(xmlWriter, (ClassDataNode)dataNode);
			}
			else if (dataType == Globals.TypeOfCollectionDataNode)
			{
				this.WriteExtensionCollectionData(xmlWriter, (CollectionDataNode)dataNode);
			}
			else if (dataType == Globals.TypeOfXmlDataNode)
			{
				this.WriteExtensionXmlData(xmlWriter, (XmlDataNode)dataNode);
			}
			else if (dataType == Globals.TypeOfISerializableDataNode)
			{
				this.WriteExtensionISerializableData(xmlWriter, (ISerializableDataNode)dataNode);
			}
			else
			{
				this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
				if (dataType == Globals.TypeOfObject)
				{
					object value = dataNode.Value;
					if (value != null)
					{
						this.InternalSerialize(xmlWriter, value, false, false, -1, value.GetType().TypeHandle);
					}
				}
				else
				{
					xmlWriter.WriteExtensionData(dataNode);
				}
			}
			if (dataNode.PreservesReferences)
			{
				this.OnEndHandleReference(xmlWriter, (dataNode.Value == null) ? dataNode : dataNode.Value, true);
			}
		}

		internal bool TryWriteDeserializedExtensionData(XmlWriterDelegator xmlWriter, IDataNode dataNode)
		{
			object value = dataNode.Value;
			if (value == null)
			{
				return false;
			}
			Type type = ((dataNode.DataContractName == null) ? value.GetType() : Globals.TypeOfObject);
			this.InternalSerialize(xmlWriter, value, false, false, -1, type.TypeHandle);
			return true;
		}

		private void WriteExtensionClassData(XmlWriterDelegator xmlWriter, ClassDataNode dataNode)
		{
			if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
			{
				this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
				IList<ExtensionDataMember> members = dataNode.Members;
				if (members != null)
				{
					for (int i = 0; i < members.Count; i++)
					{
						this.WriteExtensionDataMember(xmlWriter, members[i]);
					}
				}
			}
		}

		private void WriteExtensionCollectionData(XmlWriterDelegator xmlWriter, CollectionDataNode dataNode)
		{
			if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
			{
				this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
				this.WriteArraySize(xmlWriter, dataNode.Size);
				IList<IDataNode> items = dataNode.Items;
				if (items != null)
				{
					for (int i = 0; i < items.Count; i++)
					{
						xmlWriter.WriteStartElement(dataNode.ItemName, dataNode.ItemNamespace);
						this.WriteExtensionDataValue(xmlWriter, items[i]);
						xmlWriter.WriteEndElement();
					}
				}
			}
		}

		private void WriteExtensionISerializableData(XmlWriterDelegator xmlWriter, ISerializableDataNode dataNode)
		{
			if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
			{
				this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
				if (dataNode.FactoryTypeName != null)
				{
					xmlWriter.WriteAttributeQualifiedName("z", DictionaryGlobals.ISerializableFactoryTypeLocalName, DictionaryGlobals.SerializationNamespace, dataNode.FactoryTypeName, dataNode.FactoryTypeNamespace);
				}
				IList<ISerializableDataMember> members = dataNode.Members;
				if (members != null)
				{
					for (int i = 0; i < members.Count; i++)
					{
						ISerializableDataMember serializableDataMember = members[i];
						xmlWriter.WriteStartElement(serializableDataMember.Name, string.Empty);
						this.WriteExtensionDataValue(xmlWriter, serializableDataMember.Value);
						xmlWriter.WriteEndElement();
					}
				}
			}
		}

		private void WriteExtensionXmlData(XmlWriterDelegator xmlWriter, XmlDataNode dataNode)
		{
			if (!this.TryWriteDeserializedExtensionData(xmlWriter, dataNode))
			{
				IList<XmlAttribute> xmlAttributes = dataNode.XmlAttributes;
				if (xmlAttributes != null)
				{
					foreach (XmlAttribute xmlAttribute in xmlAttributes)
					{
						xmlAttribute.WriteTo(xmlWriter.Writer);
					}
				}
				this.WriteExtensionDataTypeInfo(xmlWriter, dataNode);
				IList<XmlNode> xmlChildNodes = dataNode.XmlChildNodes;
				if (xmlChildNodes != null)
				{
					foreach (XmlNode xmlNode in xmlChildNodes)
					{
						xmlNode.WriteTo(xmlWriter.Writer);
					}
				}
			}
		}

		protected virtual void WriteDataContractValue(DataContract dataContract, XmlWriterDelegator xmlWriter, object obj, RuntimeTypeHandle declaredTypeHandle)
		{
			dataContract.WriteXmlValue(xmlWriter, obj, this);
		}

		protected virtual void WriteNull(XmlWriterDelegator xmlWriter)
		{
			XmlObjectSerializer.WriteNull(xmlWriter);
		}

		private void WriteResolvedTypeInfo(XmlWriterDelegator writer, Type objectType, Type declaredType)
		{
			XmlDictionaryString xmlDictionaryString;
			XmlDictionaryString xmlDictionaryString2;
			if (this.ResolveType(objectType, declaredType, out xmlDictionaryString, out xmlDictionaryString2))
			{
				this.WriteTypeInfo(writer, xmlDictionaryString, xmlDictionaryString2);
			}
		}

		private bool ResolveType(Type objectType, Type declaredType, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
		{
			if (!base.DataContractResolver.TryResolveType(objectType, declaredType, base.KnownTypeResolver, out typeName, out typeNamespace))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An object of type '{0}' which derives from DataContractResolver returned false from its TryResolveType method when attempting to resolve the name for an object of type '{1}', indicating that the resolution failed. Change the TryResolveType implementation to return true.", new object[]
				{
					DataContract.GetClrTypeFullName(base.DataContractResolver.GetType()),
					DataContract.GetClrTypeFullName(objectType)
				})));
			}
			if (typeName == null)
			{
				if (typeNamespace == null)
				{
					return false;
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An object of type '{0}' which derives from DataContractResolver returned a null typeName or typeNamespace but not both from its TryResolveType method when attempting to resolve the name for an object of type '{1}'. Change the TryResolveType implementation to return non-null values, or to return null values for both typeName and typeNamespace in order to serialize as the declared type.", new object[]
				{
					DataContract.GetClrTypeFullName(base.DataContractResolver.GetType()),
					DataContract.GetClrTypeFullName(objectType)
				})));
			}
			else
			{
				if (typeNamespace == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("An object of type '{0}' which derives from DataContractResolver returned a null typeName or typeNamespace but not both from its TryResolveType method when attempting to resolve the name for an object of type '{1}'. Change the TryResolveType implementation to return non-null values, or to return null values for both typeName and typeNamespace in order to serialize as the declared type.", new object[]
					{
						DataContract.GetClrTypeFullName(base.DataContractResolver.GetType()),
						DataContract.GetClrTypeFullName(objectType)
					})));
				}
				return true;
			}
		}

		protected virtual bool WriteTypeInfo(XmlWriterDelegator writer, DataContract contract, DataContract declaredContract)
		{
			if (XmlObjectSerializer.IsContractDeclared(contract, declaredContract))
			{
				return false;
			}
			if (base.DataContractResolver == null)
			{
				this.WriteTypeInfo(writer, contract.Name, contract.Namespace);
				return true;
			}
			this.WriteResolvedTypeInfo(writer, contract.OriginalUnderlyingType, declaredContract.OriginalUnderlyingType);
			return false;
		}

		protected virtual void WriteTypeInfo(XmlWriterDelegator writer, string dataContractName, string dataContractNamespace)
		{
			writer.WriteAttributeQualifiedName("i", DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, dataContractName, dataContractNamespace);
		}

		protected virtual void WriteTypeInfo(XmlWriterDelegator writer, XmlDictionaryString dataContractName, XmlDictionaryString dataContractNamespace)
		{
			writer.WriteAttributeQualifiedName("i", DictionaryGlobals.XsiTypeLocalName, DictionaryGlobals.SchemaInstanceNamespace, dataContractName, dataContractNamespace);
		}

		private ObjectReferenceStack byValObjectsInScope;

		private XmlSerializableWriter xmlSerializableWriter;

		private const int depthToCheckCyclicReference = 512;

		protected bool preserveObjectReferences;

		private ObjectToIdCache serializedObjects;

		private bool isGetOnlyCollection;

		private readonly bool unsafeTypeForwardingEnabled;

		protected bool serializeReadOnlyTypes;
	}
}
