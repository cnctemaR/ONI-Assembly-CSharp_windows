using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Runtime.Serialization.Formatters;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Serialization
{
	internal class XmlObjectSerializerReadContextComplex : XmlObjectSerializerReadContext
	{
		internal XmlObjectSerializerReadContextComplex(DataContractSerializer serializer, DataContract rootTypeDataContract, DataContractResolver dataContractResolver)
			: base(serializer, rootTypeDataContract, dataContractResolver)
		{
			this.mode = SerializationMode.SharedContract;
			this.preserveObjectReferences = serializer.PreserveObjectReferences;
			this.dataContractSurrogate = serializer.DataContractSurrogate;
		}

		internal XmlObjectSerializerReadContextComplex(NetDataContractSerializer serializer)
			: base(serializer)
		{
			this.mode = SerializationMode.SharedType;
			this.preserveObjectReferences = true;
			this.binder = serializer.Binder;
			this.surrogateSelector = serializer.SurrogateSelector;
			this.assemblyFormat = serializer.AssemblyFormat;
		}

		internal XmlObjectSerializerReadContextComplex(XmlObjectSerializer serializer, int maxItemsInObjectGraph, StreamingContext streamingContext, bool ignoreExtensionDataObject)
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

		internal override DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle)
		{
			DataContract dataContract = null;
			if (this.mode == SerializationMode.SharedType && this.surrogateSelector != null)
			{
				dataContract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, base.GetStreamingContext(), typeHandle, null, ref this.surrogateDataContracts);
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

		internal override DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type)
		{
			DataContract dataContract = null;
			if (this.mode == SerializationMode.SharedType && this.surrogateSelector != null)
			{
				dataContract = NetDataContractSerializer.GetDataContractFromSurrogateSelector(this.surrogateSelector, base.GetStreamingContext(), typeHandle, type, ref this.surrogateDataContracts);
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

		public override object InternalDeserialize(XmlReaderDelegator xmlReader, int declaredTypeID, RuntimeTypeHandle declaredTypeHandle, string name, string ns)
		{
			if (this.mode != SerializationMode.SharedContract)
			{
				return this.InternalDeserializeInSharedTypeMode(xmlReader, declaredTypeID, Type.GetTypeFromHandle(declaredTypeHandle), name, ns);
			}
			if (this.dataContractSurrogate == null)
			{
				return base.InternalDeserialize(xmlReader, declaredTypeID, declaredTypeHandle, name, ns);
			}
			return this.InternalDeserializeWithSurrogate(xmlReader, Type.GetTypeFromHandle(declaredTypeHandle), null, name, ns);
		}

		internal override object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, string name, string ns)
		{
			if (this.mode != SerializationMode.SharedContract)
			{
				return this.InternalDeserializeInSharedTypeMode(xmlReader, -1, declaredType, name, ns);
			}
			if (this.dataContractSurrogate == null)
			{
				return base.InternalDeserialize(xmlReader, declaredType, name, ns);
			}
			return this.InternalDeserializeWithSurrogate(xmlReader, declaredType, null, name, ns);
		}

		internal override object InternalDeserialize(XmlReaderDelegator xmlReader, Type declaredType, DataContract dataContract, string name, string ns)
		{
			if (this.mode != SerializationMode.SharedContract)
			{
				return this.InternalDeserializeInSharedTypeMode(xmlReader, -1, declaredType, name, ns);
			}
			if (this.dataContractSurrogate == null)
			{
				return base.InternalDeserialize(xmlReader, declaredType, dataContract, name, ns);
			}
			return this.InternalDeserializeWithSurrogate(xmlReader, declaredType, dataContract, name, ns);
		}

		private object InternalDeserializeInSharedTypeMode(XmlReaderDelegator xmlReader, int declaredTypeID, Type declaredType, string name, string ns)
		{
			object obj = null;
			if (base.TryHandleNullOrRef(xmlReader, declaredType, name, ns, ref obj))
			{
				return obj;
			}
			string clrAssembly = this.attributes.ClrAssembly;
			string clrType = this.attributes.ClrType;
			DataContract dataContract;
			if (clrAssembly != null && clrType != null)
			{
				Assembly assembly;
				Type type;
				dataContract = this.ResolveDataContractInSharedTypeMode(clrAssembly, clrType, out assembly, out type);
				if (dataContract == null)
				{
					if (assembly == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Assembly '{0}' was not found.", new object[] { clrAssembly })));
					}
					if (type == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("CLR type '{1}' in assembly '{0}' is not found.", new object[] { assembly.FullName, clrType })));
					}
				}
				if (declaredType != null && declaredType.IsArray)
				{
					dataContract = ((declaredTypeID < 0) ? base.GetDataContract(declaredType) : this.GetDataContract(declaredTypeID, declaredType.TypeHandle));
				}
			}
			else
			{
				if (clrAssembly != null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, global::System.Runtime.Serialization.SR.GetString("Attribute was not found for CLR type '{1}' in namespace '{0}'. XML reader node is on {2}, '{4}' node in '{3}' namespace.", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/", "Type", xmlReader.NodeType, xmlReader.NamespaceURI, xmlReader.LocalName }))));
				}
				if (clrType != null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, global::System.Runtime.Serialization.SR.GetString("Attribute was not found for CLR type '{1}' in namespace '{0}'. XML reader node is on {2}, '{4}' node in '{3}' namespace.", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/", "Assembly", xmlReader.NodeType, xmlReader.NamespaceURI, xmlReader.LocalName }))));
				}
				if (declaredType == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(XmlObjectSerializer.TryAddLineInfo(xmlReader, global::System.Runtime.Serialization.SR.GetString("Attribute was not found for CLR type '{1}' in namespace '{0}'. XML reader node is on {2}, '{4}' node in '{3}' namespace.", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/", "Type", xmlReader.NodeType, xmlReader.NamespaceURI, xmlReader.LocalName }))));
				}
				dataContract = ((declaredTypeID < 0) ? base.GetDataContract(declaredType) : this.GetDataContract(declaredTypeID, declaredType.TypeHandle));
			}
			return this.ReadDataContractValue(dataContract, xmlReader);
		}

		private object InternalDeserializeWithSurrogate(XmlReaderDelegator xmlReader, Type declaredType, DataContract surrogateDataContract, string name, string ns)
		{
			if (TD.DCDeserializeWithSurrogateStartIsEnabled())
			{
				TD.DCDeserializeWithSurrogateStart(declaredType.FullName);
			}
			DataContract dataContract = surrogateDataContract ?? base.GetDataContract(DataContractSurrogateCaller.GetDataContractType(this.dataContractSurrogate, declaredType));
			if (this.IsGetOnlyCollection && dataContract.UnderlyingType != declaredType)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Surrogates with get-only collections are not supported. Found on type '{0}'.", new object[] { DataContract.GetClrTypeFullName(declaredType) })));
			}
			this.ReadAttributes(xmlReader);
			string objectId = base.GetObjectId();
			object obj = base.InternalDeserialize(xmlReader, name, ns, declaredType, ref dataContract);
			object deserializedObject = DataContractSurrogateCaller.GetDeserializedObject(this.dataContractSurrogate, obj, dataContract.UnderlyingType, declaredType);
			base.ReplaceDeserializedObject(objectId, obj, deserializedObject);
			if (TD.DCDeserializeWithSurrogateStopIsEnabled())
			{
				TD.DCDeserializeWithSurrogateStop();
			}
			return deserializedObject;
		}

		private Type ResolveDataContractTypeInSharedTypeMode(string assemblyName, string typeName, out Assembly assembly)
		{
			assembly = null;
			Type type = null;
			if (this.binder != null)
			{
				type = this.binder.BindToType(assemblyName, typeName);
			}
			if (type == null)
			{
				XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeKey xmlObjectDataContractTypeKey = new XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeKey(assemblyName, typeName);
				XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeInfo xmlObjectDataContractTypeInfo = (XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeInfo)XmlObjectSerializerReadContextComplex.dataContractTypeCache[xmlObjectDataContractTypeKey];
				if (xmlObjectDataContractTypeInfo == null)
				{
					if (this.assemblyFormat == FormatterAssemblyStyle.Full)
					{
						if (assemblyName == "0")
						{
							assembly = Globals.TypeOfInt.Assembly;
						}
						else
						{
							assembly = Assembly.Load(assemblyName);
						}
						if (assembly != null)
						{
							type = assembly.GetType(typeName);
						}
					}
					else
					{
						assembly = XmlObjectSerializerReadContextComplex.ResolveSimpleAssemblyName(assemblyName);
						if (assembly != null)
						{
							try
							{
								type = assembly.GetType(typeName);
							}
							catch (TypeLoadException)
							{
							}
							catch (FileNotFoundException)
							{
							}
							catch (FileLoadException)
							{
							}
							catch (BadImageFormatException)
							{
							}
							if (type == null)
							{
								type = Type.GetType(typeName, new Func<AssemblyName, Assembly>(XmlObjectSerializerReadContextComplex.ResolveSimpleAssemblyName), new Func<Assembly, string, bool, Type>(new XmlObjectSerializerReadContextComplex.TopLevelAssemblyTypeResolver(assembly).ResolveType), false);
							}
						}
					}
					if (!(type != null))
					{
						return type;
					}
					XmlObjectSerializerReadContextComplex.CheckTypeForwardedTo(assembly, type.Assembly, type);
					xmlObjectDataContractTypeInfo = new XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeInfo(assembly, type);
					Hashtable hashtable = XmlObjectSerializerReadContextComplex.dataContractTypeCache;
					lock (hashtable)
					{
						if (!XmlObjectSerializerReadContextComplex.dataContractTypeCache.ContainsKey(xmlObjectDataContractTypeKey))
						{
							XmlObjectSerializerReadContextComplex.dataContractTypeCache[xmlObjectDataContractTypeKey] = xmlObjectDataContractTypeInfo;
						}
						return type;
					}
				}
				assembly = xmlObjectDataContractTypeInfo.Assembly;
				type = xmlObjectDataContractTypeInfo.Type;
			}
			return type;
		}

		private DataContract ResolveDataContractInSharedTypeMode(string assemblyName, string typeName, out Assembly assembly, out Type type)
		{
			type = this.ResolveDataContractTypeInSharedTypeMode(assemblyName, typeName, out assembly);
			if (type != null)
			{
				return base.GetDataContract(type);
			}
			return null;
		}

		protected override DataContract ResolveDataContractFromTypeName()
		{
			if (this.mode == SerializationMode.SharedContract)
			{
				return base.ResolveDataContractFromTypeName();
			}
			if (this.attributes.ClrAssembly != null && this.attributes.ClrType != null)
			{
				Assembly assembly;
				Type type;
				return this.ResolveDataContractInSharedTypeMode(this.attributes.ClrAssembly, this.attributes.ClrType, out assembly, out type);
			}
			return null;
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Unrestricted = true)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool CheckIfTypeSerializableForSharedTypeMode(Type memberType)
		{
			ISurrogateSelector surrogateSelector;
			return this.surrogateSelector.GetSurrogate(memberType, base.GetStreamingContext(), out surrogateSelector) != null;
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

		internal override int GetArraySize()
		{
			if (!this.preserveObjectReferences)
			{
				return -1;
			}
			return this.attributes.ArraySZSize;
		}

		private static Assembly ResolveSimpleAssemblyName(AssemblyName assemblyName)
		{
			return XmlObjectSerializerReadContextComplex.ResolveSimpleAssemblyName(assemblyName.FullName);
		}

		private static Assembly ResolveSimpleAssemblyName(string assemblyName)
		{
			Assembly assembly;
			if (assemblyName == "0")
			{
				assembly = Globals.TypeOfInt.Assembly;
			}
			else
			{
				assembly = Assembly.LoadWithPartialName(assemblyName);
				if (assembly == null)
				{
					assembly = Assembly.LoadWithPartialName(new AssemblyName(assemblyName)
					{
						Version = null
					}.FullName);
				}
			}
			return assembly;
		}

		[SecuritySafeCritical]
		private static void CheckTypeForwardedTo(Assembly sourceAssembly, Assembly destinationAssembly, Type resolvedType)
		{
			if (sourceAssembly != destinationAssembly && !NetDataContractSerializer.UnsafeTypeForwardingEnabled && !sourceAssembly.IsFullyTrusted && !destinationAssembly.PermissionSet.IsSubsetOf(sourceAssembly.PermissionSet))
			{
				TypeInformation typeInformation = NetDataContractSerializer.GetTypeInformation(resolvedType);
				if (typeInformation.HasTypeForwardedFrom)
				{
					Assembly assembly = null;
					try
					{
						assembly = Assembly.Load(typeInformation.AssemblyString);
					}
					catch
					{
					}
					if (assembly == sourceAssembly)
					{
						return;
					}
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Cannot deserialize forwarded type '{0}'.", new object[] { DataContract.GetClrTypeFullName(resolvedType) })));
			}
		}

		private static Hashtable dataContractTypeCache = new Hashtable();

		private bool preserveObjectReferences;

		protected IDataContractSurrogate dataContractSurrogate;

		private SerializationMode mode;

		private SerializationBinder binder;

		private ISurrogateSelector surrogateSelector;

		private FormatterAssemblyStyle assemblyFormat;

		private Hashtable surrogateDataContracts;

		private sealed class TopLevelAssemblyTypeResolver
		{
			public TopLevelAssemblyTypeResolver(Assembly topLevelAssembly)
			{
				this.topLevelAssembly = topLevelAssembly;
			}

			public Type ResolveType(Assembly assembly, string simpleTypeName, bool ignoreCase)
			{
				if (assembly == null)
				{
					assembly = this.topLevelAssembly;
				}
				return assembly.GetType(simpleTypeName, false, ignoreCase);
			}

			private Assembly topLevelAssembly;
		}

		private class XmlObjectDataContractTypeInfo
		{
			public XmlObjectDataContractTypeInfo(Assembly assembly, Type type)
			{
				this.assembly = assembly;
				this.type = type;
			}

			public Assembly Assembly
			{
				get
				{
					return this.assembly;
				}
			}

			public Type Type
			{
				get
				{
					return this.type;
				}
			}

			private Assembly assembly;

			private Type type;
		}

		private class XmlObjectDataContractTypeKey
		{
			public XmlObjectDataContractTypeKey(string assemblyName, string typeName)
			{
				this.assemblyName = assemblyName;
				this.typeName = typeName;
			}

			public override bool Equals(object obj)
			{
				if (this == obj)
				{
					return true;
				}
				XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeKey xmlObjectDataContractTypeKey = obj as XmlObjectSerializerReadContextComplex.XmlObjectDataContractTypeKey;
				return xmlObjectDataContractTypeKey != null && !(this.assemblyName != xmlObjectDataContractTypeKey.assemblyName) && !(this.typeName != xmlObjectDataContractTypeKey.typeName);
			}

			public override int GetHashCode()
			{
				int num = 0;
				if (this.assemblyName != null)
				{
					num = this.assemblyName.GetHashCode();
				}
				if (this.typeName != null)
				{
					num ^= this.typeName.GetHashCode();
				}
				return num;
			}

			private string assemblyName;

			private string typeName;
		}
	}
}
