using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlObjectSerializerWriteContextComplex : XmlObjectSerializerWriteContext
	{
		internal XmlObjectSerializerWriteContextComplex(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
			: base(serializer, rootTypeDataContract, dataContractResolver)
		{
			this.mode = SerializationMode.SharedContract;
			this.preserveObjectReferences = serializer.PreserveObjectReferences;
			this.dataContractSurrogate = serializer.DataContractSurrogate;
		}

		internal XmlObjectSerializerWriteContextComplex(NetDataContractSerializer serializer, Hashtable surrogateDataContracts)
			: base(serializer)
		{
			this.mode = SerializationMode.SharedType;
			this.preserveObjectReferences = true;
			this.streamingContext = serializer.Context;
			this.binder = serializer.Binder;
			this.surrogateSelector = serializer.SurrogateSelector;
			this.surrogateDataContracts = surrogateDataContracts;
		}

		internal XmlObjectSerializerWriteContextComplex(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
			: base(serializer, maxItemsInObjectGraph, streamingContext, ignoreExtensionDataObject)
		{
		}

		internal override SerializationMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		internal override DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContract = null;
			if (this.mode == SerializationMode.SharedType && this.surrogateSelector != null)
			{
				dataContract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, this.streamingContext, typeHandle, type, ref this.surrogateDataContracts);
			}
			if (dataContract == null)
			{
				return base.GetDataContract(typeHandle, type);
			}
			if (this.IsGetOnlyCollection && dataContract is SurrogateDataContract)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Surrogates with get-only collections are not supported. Found on type '{0}'.", new object[] { DataContract.GetClrTypeFullName(dataContract.UnderlyingType) })));
			}
			return dataContract;
		}

		internal override DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
		{
			DataContract dataContract = null;
			if (this.mode == SerializationMode.SharedType && this.surrogateSelector != null)
			{
				dataContract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, this.streamingContext, typeHandle, null, ref this.surrogateDataContracts);
			}
			if (dataContract == null)
			{
				return base.GetDataContract(id, typeHandle);
			}
			if (this.IsGetOnlyCollection && dataContract is SurrogateDataContract)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Surrogates with get-only collections are not supported. Found on type '{0}'.", new object[] { DataContract.GetClrTypeFullName(dataContract.UnderlyingType) })));
			}
			return dataContract;
		}

		internal override DataContract GetDataContractSkipValidation(int typeId, RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContract = null;
			if (this.mode == SerializationMode.SharedType && this.surrogateSelector != null)
			{
				dataContract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, this.streamingContext, typeHandle, null, ref this.surrogateDataContracts);
			}
			if (dataContract == null)
			{
				return base.GetDataContractSkipValidation(typeId, typeHandle, type);
			}
			if (this.IsGetOnlyCollection && dataContract is SurrogateDataContract)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Surrogates with get-only collections are not supported. Found on type '{0}'.", new object[] { DataContract.GetClrTypeFullName(dataContract.UnderlyingType) })));
			}
			return dataContract;
		}

		internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, DataContract dataContract)
		{
			if (this.mode == SerializationMode.SharedType)
			{
				NetDataContractSerializer.WriteClrTypeInfo(xmlWriter, dataContract, this.binder);
				return true;
			}
			return false;
		}

		internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, Type dataContractType, string clrTypeName, string clrAssemblyName)
		{
			if (this.mode == SerializationMode.SharedType)
			{
				NetDataContractSerializer.WriteClrTypeInfo(xmlWriter, dataContractType, this.binder, clrTypeName, clrAssemblyName);
				return true;
			}
			return false;
		}

		internal override bool WriteClrTypeInfo(XmlWriterDelegator xmlWriter, Type dataContractType, SerializationInfo serInfo)
		{
			if (this.mode == SerializationMode.SharedType)
			{
				NetDataContractSerializer.WriteClrTypeInfo(xmlWriter, dataContractType, this.binder, serInfo);
				return true;
			}
			return false;
		}

		public override void WriteAnyType(XmlWriterDelegator xmlWriter, object value)
		{
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteAnyType(value);
			}
		}

		public override void WriteString(XmlWriterDelegator xmlWriter, string value)
		{
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteString(value);
			}
		}

		public override void WriteString(XmlWriterDelegator xmlWriter, string value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				base.WriteNull(xmlWriter, typeof(string), true, name, ns);
				return;
			}
			xmlWriter.WriteStartElementPrimitive(name, ns);
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteString(value);
			}
			xmlWriter.WriteEndElementPrimitive();
		}

		public override void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value)
		{
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteBase64(value);
			}
		}

		public override void WriteBase64(XmlWriterDelegator xmlWriter, byte[] value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				base.WriteNull(xmlWriter, typeof(byte[]), true, name, ns);
				return;
			}
			xmlWriter.WriteStartElementPrimitive(name, ns);
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteBase64(value);
			}
			xmlWriter.WriteEndElementPrimitive();
		}

		public override void WriteUri(XmlWriterDelegator xmlWriter, Uri value)
		{
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteUri(value);
			}
		}

		public override void WriteUri(XmlWriterDelegator xmlWriter, Uri value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				base.WriteNull(xmlWriter, typeof(Uri), true, name, ns);
				return;
			}
			xmlWriter.WriteStartElementPrimitive(name, ns);
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteUri(value);
			}
			xmlWriter.WriteEndElementPrimitive();
		}

		public override void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value)
		{
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteQName(value);
			}
		}

		public override void WriteQName(XmlWriterDelegator xmlWriter, XmlQualifiedName value, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (value == null)
			{
				base.WriteNull(xmlWriter, typeof(XmlQualifiedName), true, name, ns);
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
			if (!this.OnHandleReference(xmlWriter, value, false))
			{
				xmlWriter.WriteQName(value);
			}
			xmlWriter.WriteEndElement();
		}

		public override void InternalSerialize(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
		{
			if (this.dataContractSurrogate == null)
			{
				base.InternalSerialize(xmlWriter, obj, isDeclaredType, writeXsiType, declaredTypeID, declaredTypeHandle);
				return;
			}
			this.InternalSerializeWithSurrogate(xmlWriter, obj, isDeclaredType, writeXsiType, declaredTypeID, declaredTypeHandle);
		}

		internal override bool OnHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
		{
			if (this.preserveObjectReferences && !this.IsGetOnlyCollection)
			{
				bool flag = true;
				int id = base.SerializedObjects.GetId(obj, ref flag);
				if (flag)
				{
					xmlWriter.WriteAttributeInt("z", DictionaryGlobals.IdLocalName, DictionaryGlobals.SerializationNamespace, id);
				}
				else
				{
					xmlWriter.WriteAttributeInt("z", DictionaryGlobals.RefLocalName, DictionaryGlobals.SerializationNamespace, id);
					xmlWriter.WriteAttributeBool("i", DictionaryGlobals.XsiNilLocalName, DictionaryGlobals.SchemaInstanceNamespace, true);
				}
				return !flag;
			}
			return base.OnHandleReference(xmlWriter, obj, canContainCyclicReference);
		}

		internal override void OnEndHandleReference(XmlWriterDelegator xmlWriter, object obj, bool canContainCyclicReference)
		{
			if (this.preserveObjectReferences && !this.IsGetOnlyCollection)
			{
				return;
			}
			base.OnEndHandleReference(xmlWriter, obj, canContainCyclicReference);
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool CheckIfTypeSerializableForSharedTypeMode(Type memberType)
		{
			ISurrogateSelector surrogateSelector;
			return this.surrogateSelector.GetSurrogate(memberType, this.streamingContext, out surrogateSelector) != null;
		}

		internal override void CheckIfTypeSerializable(Type memberType, bool isMemberTypeSerializable)
		{
			if (this.mode == SerializationMode.SharedType && this.surrogateSelector != null && this.CheckIfTypeSerializableForSharedTypeMode(memberType))
			{
				return;
			}
			if (this.dataContractSurrogate == null)
			{
				base.CheckIfTypeSerializable(memberType, isMemberTypeSerializable);
				return;
			}
			while (memberType.IsArray)
			{
				memberType = memberType.GetElementType();
			}
			memberType = DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, memberType);
			if (!DataContract.IsTypeSerializable(memberType))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be serialized. Consider marking it with the DataContractAttribute attribute, and marking all of its members you want serialized with the DataMemberAttribute attribute. Alternatively, you can ensure that the type is public and has a parameterless constructor - all public members of the type will then be serialized, and no attributes will be required.", new object[] { memberType })));
			}
		}

		internal override Type GetSurrogatedType(Type type)
		{
			if (this.dataContractSurrogate == null)
			{
				return base.GetSurrogatedType(type);
			}
			type = DataContract.UnwrapNullableType(type);
			Type surrogatedType = DataContractSerializer.GetSurrogatedType(this.dataContractSurrogate, type);
			if (this.IsGetOnlyCollection && surrogatedType != type)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Surrogates with get-only collections are not supported. Found on type '{0}'.", new object[] { DataContract.GetClrTypeFullName(type) })));
			}
			return surrogatedType;
		}

		private void InternalSerializeWithSurrogate(XmlWriterDelegator xmlWriter, object obj, bool isDeclaredType, bool writeXsiType, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle)
		{
			RuntimeTypeHandle runtimeTypeHandle = (isDeclaredType ? declaredTypeHandle : Type.GetTypeHandle(obj));
			object obj2 = obj;
			int num = 0;
			Type typeFromHandle = Type.GetTypeFromHandle(runtimeTypeHandle);
			Type type = this.GetSurrogatedType(Type.GetTypeFromHandle(declaredTypeHandle));
			if (TD.DCSerializeWithSurrogateStartIsEnabled())
			{
				TD.DCSerializeWithSurrogateStart(type.FullName);
			}
			declaredTypeHandle = type.TypeHandle;
			obj = DataContractSerializer.SurrogateToDataContractType(this.dataContractSurrogate, obj, type, ref typeFromHandle);
			runtimeTypeHandle = typeFromHandle.TypeHandle;
			if (obj2 != obj)
			{
				num = base.SerializedObjects.ReassignId(0, obj2, obj);
			}
			if (writeXsiType)
			{
				type = Globals.TypeOfObject;
				this.SerializeWithXsiType(xmlWriter, obj, runtimeTypeHandle, typeFromHandle, -1, type.TypeHandle, type);
			}
			else if (declaredTypeHandle.Equals(runtimeTypeHandle))
			{
				DataContract dataContract = this.GetDataContract(runtimeTypeHandle, typeFromHandle);
				base.SerializeWithoutXsiType(dataContract, xmlWriter, obj, declaredTypeHandle);
			}
			else
			{
				this.SerializeWithXsiType(xmlWriter, obj, runtimeTypeHandle, typeFromHandle, -1, declaredTypeHandle, type);
			}
			if (obj2 != obj)
			{
				base.SerializedObjects.ReassignId(num, obj, obj2);
			}
			if (TD.DCSerializeWithSurrogateStopIsEnabled())
			{
				TD.DCSerializeWithSurrogateStop();
			}
		}

		internal override void WriteArraySize(XmlWriterDelegator xmlWriter, int size)
		{
			if (this.preserveObjectReferences && size > -1)
			{
				xmlWriter.WriteAttributeInt("z", DictionaryGlobals.ArraySizeLocalName, DictionaryGlobals.SerializationNamespace, size);
			}
		}

		protected IDataContractSurrogate dataContractSurrogate;

		private SerializationMode mode;

		private SerializationBinder binder;

		private ISurrogateSelector surrogateSelector;

		private StreamingContext streamingContext;

		private Hashtable surrogateDataContracts;
	}
}
