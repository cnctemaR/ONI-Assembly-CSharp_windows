using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class ClassDataContract : DataContract
	{
		[SecuritySafeCritical]
		internal ClassDataContract()
			: base(new ClassDataContract.ClassDataContractCriticalHelper())
		{
			this.InitClassDataContract();
		}

		[SecuritySafeCritical]
		internal ClassDataContract(Type type)
			: base(new ClassDataContract.ClassDataContractCriticalHelper(type))
		{
			this.InitClassDataContract();
		}

		[SecuritySafeCritical]
		private ClassDataContract(Type type, XmlDictionaryString ns, string[] memberNames)
			: base(new ClassDataContract.ClassDataContractCriticalHelper(type, ns, memberNames))
		{
			this.InitClassDataContract();
		}

		[SecurityCritical]
		private void InitClassDataContract()
		{
			this.helper = base.Helper as ClassDataContract.ClassDataContractCriticalHelper;
			this.ContractNamespaces = this.helper.ContractNamespaces;
			this.MemberNames = this.helper.MemberNames;
			this.MemberNamespaces = this.helper.MemberNamespaces;
		}

		internal ClassDataContract BaseContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.BaseContract;
			}
			[SecurityCritical]
			set
			{
				this.helper.BaseContract = value;
			}
		}

		internal List<DataMember> Members
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.Members;
			}
			[SecurityCritical]
			set
			{
				this.helper.Members = value;
			}
		}

		public XmlDictionaryString[] ChildElementNamespaces
		{
			[SecuritySafeCritical]
			get
			{
				if (this.childElementNamespaces == null)
				{
					lock (this)
					{
						if (this.childElementNamespaces == null)
						{
							if (this.helper.ChildElementNamespaces == null)
							{
								XmlDictionaryString[] array = this.CreateChildElementNamespaces();
								Thread.MemoryBarrier();
								this.helper.ChildElementNamespaces = array;
							}
							this.childElementNamespaces = this.helper.ChildElementNamespaces;
						}
					}
				}
				return this.childElementNamespaces;
			}
		}

		internal MethodInfo OnSerializing
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.OnSerializing;
			}
		}

		internal MethodInfo OnSerialized
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.OnSerialized;
			}
		}

		internal MethodInfo OnDeserializing
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.OnDeserializing;
			}
		}

		internal MethodInfo OnDeserialized
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.OnDeserialized;
			}
		}

		internal MethodInfo ExtensionDataSetMethod
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ExtensionDataSetMethod;
			}
		}

		internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.KnownDataContracts;
			}
			[SecurityCritical]
			set
			{
				this.helper.KnownDataContracts = value;
			}
		}

		internal override bool IsISerializable
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsISerializable;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsISerializable = value;
			}
		}

		internal bool IsNonAttributedType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsNonAttributedType;
			}
		}

		internal bool HasDataContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.HasDataContract;
			}
		}

		internal bool HasExtensionData
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.HasExtensionData;
			}
		}

		internal string SerializationExceptionMessage
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.SerializationExceptionMessage;
			}
		}

		internal string DeserializationExceptionMessage
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.DeserializationExceptionMessage;
			}
		}

		internal bool IsReadOnlyContract
		{
			get
			{
				return this.DeserializationExceptionMessage != null;
			}
		}

		[SecuritySafeCritical]
		internal ConstructorInfo GetISerializableConstructor()
		{
			return this.helper.GetISerializableConstructor();
		}

		[SecuritySafeCritical]
		internal ConstructorInfo GetNonAttributedTypeConstructor()
		{
			return this.helper.GetNonAttributedTypeConstructor();
		}

		internal XmlFormatClassWriterDelegate XmlFormatWriterDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.XmlFormatWriterDelegate == null)
				{
					lock (this)
					{
						if (this.helper.XmlFormatWriterDelegate == null)
						{
							XmlFormatClassWriterDelegate xmlFormatClassWriterDelegate = new XmlFormatWriterGenerator().GenerateClassWriter(this);
							Thread.MemoryBarrier();
							this.helper.XmlFormatWriterDelegate = xmlFormatClassWriterDelegate;
						}
					}
				}
				return this.helper.XmlFormatWriterDelegate;
			}
		}

		internal XmlFormatClassReaderDelegate XmlFormatReaderDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.XmlFormatReaderDelegate == null)
				{
					lock (this)
					{
						if (this.helper.XmlFormatReaderDelegate == null)
						{
							if (this.IsReadOnlyContract)
							{
								DataContract.ThrowInvalidDataContractException(this.helper.DeserializationExceptionMessage, null);
							}
							XmlFormatClassReaderDelegate xmlFormatClassReaderDelegate = new XmlFormatReaderGenerator().GenerateClassReader(this);
							Thread.MemoryBarrier();
							this.helper.XmlFormatReaderDelegate = xmlFormatClassReaderDelegate;
						}
					}
				}
				return this.helper.XmlFormatReaderDelegate;
			}
		}

		internal static ClassDataContract CreateClassDataContractForKeyValue(Type type, XmlDictionaryString ns, string[] memberNames)
		{
			return new ClassDataContract(type, ns, memberNames);
		}

		internal static void CheckAndAddMember(List<DataMember> members, DataMember memberContract, Dictionary<string, DataMember> memberNamesTable)
		{
			DataMember dataMember;
			if (memberNamesTable.TryGetValue(memberContract.Name, out dataMember))
			{
				Type declaringType = memberContract.MemberInfo.DeclaringType;
				DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString(declaringType.IsEnum ? "Type '{2}' contains two members '{0}' 'and '{1}' with the same name '{3}'. Multiple members with the same name in one type are not supported. Consider changing one of the member names using EnumMemberAttribute attribute." : "Type '{2}' contains two members '{0}' 'and '{1}' with the same data member name '{3}'. Multiple members with the same name in one type are not supported. Consider changing one of the member names using DataMemberAttribute attribute.", new object[]
				{
					dataMember.MemberInfo.Name,
					memberContract.MemberInfo.Name,
					DataContract.GetClrTypeFullName(declaringType),
					memberContract.Name
				}), declaringType);
			}
			memberNamesTable.Add(memberContract.Name, memberContract);
			members.Add(memberContract);
		}

		internal static XmlDictionaryString GetChildNamespaceToDeclare(DataContract dataContract, Type childType, XmlDictionary dictionary)
		{
			childType = DataContract.UnwrapNullableType(childType);
			if (!childType.IsEnum && !Globals.TypeOfIXmlSerializable.IsAssignableFrom(childType) && DataContract.GetBuiltInDataContract(childType) == null && childType != Globals.TypeOfDBNull)
			{
				string @namespace = DataContract.GetStableName(childType).Namespace;
				if (@namespace.Length > 0 && @namespace != dataContract.Namespace.Value)
				{
					return dictionary.Add(@namespace);
				}
			}
			return null;
		}

		internal static bool IsNonAttributedTypeValidForSerialization(Type type)
		{
			if (type.IsArray)
			{
				return false;
			}
			if (type.IsEnum)
			{
				return false;
			}
			if (type.IsGenericParameter)
			{
				return false;
			}
			if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
			{
				return false;
			}
			if (type.IsPointer)
			{
				return false;
			}
			if (type.IsDefined(Globals.TypeOfCollectionDataContractAttribute, false))
			{
				return false;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (CollectionDataContract.IsCollectionInterface(interfaces[i]))
				{
					return false;
				}
			}
			if (type.IsSerializable)
			{
				return false;
			}
			if (Globals.TypeOfISerializable.IsAssignableFrom(type))
			{
				return false;
			}
			if (type.IsDefined(Globals.TypeOfDataContractAttribute, false))
			{
				return false;
			}
			if (type == Globals.TypeOfExtensionDataObject)
			{
				return false;
			}
			if (type.IsValueType)
			{
				return type.IsVisible;
			}
			return type.IsVisible && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null) != null;
		}

		private XmlDictionaryString[] CreateChildElementNamespaces()
		{
			if (this.Members == null)
			{
				return null;
			}
			XmlDictionaryString[] array = null;
			if (this.BaseContract != null)
			{
				array = this.BaseContract.ChildElementNamespaces;
			}
			int num = ((array != null) ? array.Length : 0);
			XmlDictionaryString[] array2 = new XmlDictionaryString[this.Members.Count + num];
			if (num > 0)
			{
				Array.Copy(array, 0, array2, 0, array.Length);
			}
			XmlDictionary xmlDictionary = new XmlDictionary();
			for (int i = 0; i < this.Members.Count; i++)
			{
				array2[i + num] = ClassDataContract.GetChildNamespaceToDeclare(this, this.Members[i].MemberType, xmlDictionary);
			}
			return array2;
		}

		[SecuritySafeCritical]
		private void EnsureMethodsImported()
		{
			this.helper.EnsureMethodsImported();
		}

		public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			this.XmlFormatWriterDelegate(xmlWriter, obj, context, this);
		}

		public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
		{
			xmlReader.Read();
			object obj = this.XmlFormatReaderDelegate(xmlReader, context, this.MemberNames, this.MemberNamespaces);
			xmlReader.ReadEndElement();
			return obj;
		}

		[SecuritySafeCritical]
		internal override DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
		{
			Type underlyingType = base.UnderlyingType;
			if (!underlyingType.IsGenericType || !underlyingType.ContainsGenericParameters)
			{
				return this;
			}
			DataContract dataContract2;
			lock (this)
			{
				DataContract dataContract;
				if (boundContracts.TryGetValue(this, out dataContract))
				{
					dataContract2 = dataContract;
				}
				else
				{
					ClassDataContract classDataContract = new ClassDataContract();
					boundContracts.Add(this, classDataContract);
					XmlQualifiedName xmlQualifiedName;
					object[] array;
					if (underlyingType.IsGenericTypeDefinition)
					{
						xmlQualifiedName = base.StableName;
						array = paramContracts;
					}
					else
					{
						xmlQualifiedName = DataContract.GetStableName(underlyingType.GetGenericTypeDefinition());
						Type[] genericArguments = underlyingType.GetGenericArguments();
						array = new object[genericArguments.Length];
						for (int i = 0; i < genericArguments.Length; i++)
						{
							Type type = genericArguments[i];
							if (type.IsGenericParameter)
							{
								array[i] = paramContracts[type.GenericParameterPosition];
							}
							else
							{
								array[i] = type;
							}
						}
					}
					classDataContract.StableName = DataContract.CreateQualifiedName(DataContract.ExpandGenericParameters(XmlConvert.DecodeName(xmlQualifiedName.Name), new GenericNameProvider(DataContract.GetClrTypeFullName(base.UnderlyingType), array)), xmlQualifiedName.Namespace);
					if (this.BaseContract != null)
					{
						classDataContract.BaseContract = (ClassDataContract)this.BaseContract.BindGenericParameters(paramContracts, boundContracts);
					}
					classDataContract.IsISerializable = this.IsISerializable;
					classDataContract.IsValueType = base.IsValueType;
					classDataContract.IsReference = base.IsReference;
					if (this.Members != null)
					{
						classDataContract.Members = new List<DataMember>(this.Members.Count);
						foreach (DataMember dataMember in this.Members)
						{
							classDataContract.Members.Add(dataMember.BindGenericParameters(paramContracts, boundContracts));
						}
					}
					dataContract2 = classDataContract;
				}
			}
			return dataContract2;
		}

		internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (base.IsEqualOrChecked(other, checkedContracts))
			{
				return true;
			}
			if (base.Equals(other, checkedContracts))
			{
				ClassDataContract classDataContract = other as ClassDataContract;
				if (classDataContract != null)
				{
					if (this.IsISerializable)
					{
						if (!classDataContract.IsISerializable)
						{
							return false;
						}
					}
					else
					{
						if (classDataContract.IsISerializable)
						{
							return false;
						}
						if (this.Members == null)
						{
							if (classDataContract.Members != null && !this.IsEveryDataMemberOptional(classDataContract.Members))
							{
								return false;
							}
						}
						else if (classDataContract.Members == null)
						{
							if (!this.IsEveryDataMemberOptional(this.Members))
							{
								return false;
							}
						}
						else
						{
							Dictionary<string, DataMember> dictionary = new Dictionary<string, DataMember>(this.Members.Count);
							List<DataMember> list = new List<DataMember>();
							for (int i = 0; i < this.Members.Count; i++)
							{
								dictionary.Add(this.Members[i].Name, this.Members[i]);
							}
							for (int j = 0; j < classDataContract.Members.Count; j++)
							{
								DataMember dataMember;
								if (dictionary.TryGetValue(classDataContract.Members[j].Name, out dataMember))
								{
									if (!dataMember.Equals(classDataContract.Members[j], checkedContracts))
									{
										return false;
									}
									dictionary.Remove(dataMember.Name);
								}
								else
								{
									list.Add(classDataContract.Members[j]);
								}
							}
							if (!this.IsEveryDataMemberOptional(dictionary.Values))
							{
								return false;
							}
							if (!this.IsEveryDataMemberOptional(list))
							{
								return false;
							}
						}
					}
					if (this.BaseContract == null)
					{
						return classDataContract.BaseContract == null;
					}
					return classDataContract.BaseContract != null && this.BaseContract.Equals(classDataContract.BaseContract, checkedContracts);
				}
			}
			return false;
		}

		private bool IsEveryDataMemberOptional(IEnumerable<DataMember> dataMembers)
		{
			using (IEnumerator<DataMember> enumerator = dataMembers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsRequired)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public XmlDictionaryString[] ContractNamespaces;

		public XmlDictionaryString[] MemberNames;

		public XmlDictionaryString[] MemberNamespaces;

		[SecurityCritical]
		private XmlDictionaryString[] childElementNamespaces;

		[SecurityCritical]
		private ClassDataContract.ClassDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class ClassDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal ClassDataContractCriticalHelper()
			{
			}

			internal ClassDataContractCriticalHelper(Type type)
				: base(type)
			{
				XmlQualifiedName stableNameAndSetHasDataContract = this.GetStableNameAndSetHasDataContract(type);
				if (type == Globals.TypeOfDBNull)
				{
					base.StableName = stableNameAndSetHasDataContract;
					this.members = new List<DataMember>();
					XmlDictionary xmlDictionary = new XmlDictionary(2);
					base.Name = xmlDictionary.Add(base.StableName.Name);
					base.Namespace = xmlDictionary.Add(base.StableName.Namespace);
					this.ContractNamespaces = (this.MemberNames = (this.MemberNamespaces = new XmlDictionaryString[0]));
					this.EnsureMethodsImported();
					return;
				}
				Type type2 = type.BaseType;
				this.isISerializable = Globals.TypeOfISerializable.IsAssignableFrom(type);
				this.SetIsNonAttributedType(type);
				if (this.isISerializable)
				{
					if (this.HasDataContract)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("ISerializable type '{0}' cannot have DataContract.", new object[] { DataContract.GetClrTypeFullName(type) })));
					}
					if (type2 != null && (!type2.IsSerializable || !Globals.TypeOfISerializable.IsAssignableFrom(type2)))
					{
						type2 = null;
					}
				}
				base.IsValueType = type.IsValueType;
				if (type2 != null && type2 != Globals.TypeOfObject && type2 != Globals.TypeOfValueType && type2 != Globals.TypeOfUri)
				{
					DataContract dataContract = DataContract.GetDataContract(type2);
					if (dataContract is CollectionDataContract)
					{
						this.BaseContract = ((CollectionDataContract)dataContract).SharedTypeContract as ClassDataContract;
					}
					else
					{
						this.BaseContract = dataContract as ClassDataContract;
					}
					if (this.BaseContract != null && this.BaseContract.IsNonAttributedType && !this.isNonAttributedType)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot inherit from a type that is not marked with DataContractAttribute or SerializableAttribute.  Consider marking the base type '{1}' with DataContractAttribute or SerializableAttribute, or removing them from the derived type.", new object[]
						{
							DataContract.GetClrTypeFullName(type),
							DataContract.GetClrTypeFullName(type2)
						})));
					}
				}
				else
				{
					this.BaseContract = null;
				}
				this.hasExtensionData = Globals.TypeOfIExtensibleDataObject.IsAssignableFrom(type);
				if (this.hasExtensionData && !this.HasDataContract && !this.IsNonAttributedType)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On '{0}' type, only DataContract types can have extension data.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				if (this.isISerializable)
				{
					base.SetDataContractName(stableNameAndSetHasDataContract);
				}
				else
				{
					base.StableName = stableNameAndSetHasDataContract;
					this.ImportDataMembers();
					XmlDictionary xmlDictionary2 = new XmlDictionary(2 + this.Members.Count);
					base.Name = xmlDictionary2.Add(base.StableName.Name);
					base.Namespace = xmlDictionary2.Add(base.StableName.Namespace);
					int num = 0;
					int num2 = 0;
					if (this.BaseContract == null)
					{
						this.MemberNames = new XmlDictionaryString[this.Members.Count];
						this.MemberNamespaces = new XmlDictionaryString[this.Members.Count];
						this.ContractNamespaces = new XmlDictionaryString[1];
					}
					else
					{
						if (this.BaseContract.IsReadOnlyContract)
						{
							this.serializationExceptionMessage = this.BaseContract.SerializationExceptionMessage;
						}
						num = this.BaseContract.MemberNames.Length;
						this.MemberNames = new XmlDictionaryString[this.Members.Count + num];
						Array.Copy(this.BaseContract.MemberNames, this.MemberNames, num);
						this.MemberNamespaces = new XmlDictionaryString[this.Members.Count + num];
						Array.Copy(this.BaseContract.MemberNamespaces, this.MemberNamespaces, num);
						num2 = this.BaseContract.ContractNamespaces.Length;
						this.ContractNamespaces = new XmlDictionaryString[1 + num2];
						Array.Copy(this.BaseContract.ContractNamespaces, this.ContractNamespaces, num2);
					}
					this.ContractNamespaces[num2] = base.Namespace;
					for (int i = 0; i < this.Members.Count; i++)
					{
						this.MemberNames[i + num] = xmlDictionary2.Add(this.Members[i].Name);
						this.MemberNamespaces[i + num] = base.Namespace;
					}
				}
				this.EnsureMethodsImported();
			}

			internal ClassDataContractCriticalHelper(Type type, XmlDictionaryString ns, string[] memberNames)
				: base(type)
			{
				base.StableName = new XmlQualifiedName(this.GetStableNameAndSetHasDataContract(type).Name, ns.Value);
				this.ImportDataMembers();
				XmlDictionary xmlDictionary = new XmlDictionary(1 + this.Members.Count);
				base.Name = xmlDictionary.Add(base.StableName.Name);
				base.Namespace = ns;
				this.ContractNamespaces = new XmlDictionaryString[] { base.Namespace };
				this.MemberNames = new XmlDictionaryString[this.Members.Count];
				this.MemberNamespaces = new XmlDictionaryString[this.Members.Count];
				for (int i = 0; i < this.Members.Count; i++)
				{
					this.Members[i].Name = memberNames[i];
					this.MemberNames[i] = xmlDictionary.Add(this.Members[i].Name);
					this.MemberNamespaces[i] = base.Namespace;
				}
				this.EnsureMethodsImported();
			}

			private void EnsureIsReferenceImported(Type type)
			{
				bool flag = false;
				DataContractAttribute dataContractAttribute;
				bool flag2 = DataContract.TryGetDCAttribute(type, out dataContractAttribute);
				if (this.BaseContract != null)
				{
					if (flag2 && dataContractAttribute.IsReferenceSetExplicitly)
					{
						bool isReference = this.BaseContract.IsReference;
						if ((isReference && !dataContractAttribute.IsReference) || (!isReference && dataContractAttribute.IsReference))
						{
							DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("The IsReference setting for type '{0}' is '{1}', but the same setting for its parent class '{2}' is '{3}'. Derived types must have the same value for IsReference as the base type. Change the setting on type '{0}' to '{3}', or on type '{2}' to '{1}', or do not set IsReference explicitly.", new object[]
							{
								DataContract.GetClrTypeFullName(type),
								dataContractAttribute.IsReference,
								DataContract.GetClrTypeFullName(this.BaseContract.UnderlyingType),
								this.BaseContract.IsReference
							}), type);
						}
						else
						{
							flag = dataContractAttribute.IsReference;
						}
					}
					else
					{
						flag = this.BaseContract.IsReference;
					}
				}
				else if (flag2 && dataContractAttribute.IsReference)
				{
					flag = dataContractAttribute.IsReference;
				}
				if (flag && type.IsValueType)
				{
					DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Value type '{0}' cannot have the IsReference setting of '{1}'. Either change the setting to '{2}', or remove it completely.", new object[]
					{
						DataContract.GetClrTypeFullName(type),
						true,
						false
					}), type);
					return;
				}
				base.IsReference = flag;
			}

			private void ImportDataMembers()
			{
				Type underlyingType = base.UnderlyingType;
				this.EnsureIsReferenceImported(underlyingType);
				List<DataMember> list = new List<DataMember>();
				Dictionary<string, DataMember> dictionary = new Dictionary<string, DataMember>();
				MemberInfo[] array;
				if (this.isNonAttributedType)
				{
					array = underlyingType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
				}
				else
				{
					array = underlyingType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
				foreach (MemberInfo memberInfo in array)
				{
					if (this.HasDataContract)
					{
						object[] customAttributes = memberInfo.GetCustomAttributes(typeof(DataMemberAttribute), false);
						if (customAttributes != null && customAttributes.Length != 0)
						{
							if (customAttributes.Length > 1)
							{
								base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Member '{0}.{1}' has more than one DataMemberAttribute attribute.", new object[]
								{
									DataContract.GetClrTypeFullName(memberInfo.DeclaringType),
									memberInfo.Name
								}));
							}
							DataMember dataMember = new DataMember(memberInfo);
							if (memberInfo.MemberType == MemberTypes.Property)
							{
								PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
								MethodInfo getMethod = propertyInfo.GetGetMethod(true);
								if (getMethod != null && ClassDataContract.ClassDataContractCriticalHelper.IsMethodOverriding(getMethod))
								{
									goto IL_053D;
								}
								MethodInfo setMethod = propertyInfo.GetSetMethod(true);
								if (setMethod != null && ClassDataContract.ClassDataContractCriticalHelper.IsMethodOverriding(setMethod))
								{
									goto IL_053D;
								}
								if (getMethod == null)
								{
									base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("No get method for property '{1}' in type '{0}'.", new object[] { propertyInfo.DeclaringType, propertyInfo.Name }));
								}
								if (setMethod == null && !this.SetIfGetOnlyCollection(dataMember, false))
								{
									this.serializationExceptionMessage = global::System.Runtime.Serialization.SR.GetString("No set method for property '{1}' in type '{0}'.", new object[] { propertyInfo.DeclaringType, propertyInfo.Name });
								}
								if (getMethod.GetParameters().Length != 0)
								{
									base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Property '{1}' in type '{0}' cannot be serialized because serialization of indexed properties is not supported.", new object[] { propertyInfo.DeclaringType, propertyInfo.Name }));
								}
							}
							else if (memberInfo.MemberType != MemberTypes.Field)
							{
								base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Member '{0}.{1}' cannot be serialized since it is neither a field nor a property, and therefore cannot be marked with the DataMemberAttribute attribute. Remove the DataMemberAttribute attribute from the '{1}' member.", new object[]
								{
									DataContract.GetClrTypeFullName(underlyingType),
									memberInfo.Name
								}));
							}
							DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)customAttributes[0];
							if (dataMemberAttribute.IsNameSetExplicitly)
							{
								if (dataMemberAttribute.Name == null || dataMemberAttribute.Name.Length == 0)
								{
									base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Member '{0}' in type '{1}' cannot have DataMemberAttribute attribute Name set to null or empty string.", new object[]
									{
										memberInfo.Name,
										DataContract.GetClrTypeFullName(underlyingType)
									}));
								}
								dataMember.Name = dataMemberAttribute.Name;
							}
							else
							{
								dataMember.Name = memberInfo.Name;
							}
							dataMember.Name = DataContract.EncodeLocalName(dataMember.Name);
							dataMember.IsNullable = DataContract.IsTypeNullable(dataMember.MemberType);
							dataMember.IsRequired = dataMemberAttribute.IsRequired;
							if (dataMemberAttribute.IsRequired && base.IsReference)
							{
								DataContract.DataContractCriticalHelper.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("'{0}.{1}' has the IsRequired setting of '{2}. However, '{0}' has the IsReference setting of '{2}', because either it is set explicitly, or it is derived from a base class. Set IsRequired on '{0}.{1}' to false, or disable IsReference on '{0}'.", new object[]
								{
									DataContract.GetClrTypeFullName(memberInfo.DeclaringType),
									memberInfo.Name,
									true
								}), underlyingType);
							}
							dataMember.EmitDefaultValue = dataMemberAttribute.EmitDefaultValue;
							dataMember.Order = dataMemberAttribute.Order;
							ClassDataContract.CheckAndAddMember(list, dataMember, dictionary);
						}
					}
					else if (this.isNonAttributedType)
					{
						FieldInfo fieldInfo = memberInfo as FieldInfo;
						PropertyInfo propertyInfo2 = memberInfo as PropertyInfo;
						if ((!(fieldInfo == null) || !(propertyInfo2 == null)) && (!(fieldInfo != null) || !fieldInfo.IsInitOnly))
						{
							object[] customAttributes2 = memberInfo.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false);
							if (customAttributes2 != null && customAttributes2.Length != 0)
							{
								if (customAttributes2.Length <= 1)
								{
									goto IL_053D;
								}
								base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Member '{0}.{1}' has more than one IgnoreDataMemberAttribute attribute.", new object[]
								{
									DataContract.GetClrTypeFullName(memberInfo.DeclaringType),
									memberInfo.Name
								}));
							}
							DataMember dataMember2 = new DataMember(memberInfo);
							if (propertyInfo2 != null)
							{
								MethodInfo getMethod2 = propertyInfo2.GetGetMethod();
								if (getMethod2 == null || ClassDataContract.ClassDataContractCriticalHelper.IsMethodOverriding(getMethod2) || getMethod2.GetParameters().Length != 0)
								{
									goto IL_053D;
								}
								MethodInfo setMethod2 = propertyInfo2.GetSetMethod(true);
								if (setMethod2 == null)
								{
									if (!this.SetIfGetOnlyCollection(dataMember2, true))
									{
										goto IL_053D;
									}
								}
								else if (!setMethod2.IsPublic || ClassDataContract.ClassDataContractCriticalHelper.IsMethodOverriding(setMethod2))
								{
									goto IL_053D;
								}
								if (this.hasExtensionData && dataMember2.MemberType == Globals.TypeOfExtensionDataObject && memberInfo.Name == "ExtensionData")
								{
									goto IL_053D;
								}
							}
							dataMember2.Name = DataContract.EncodeLocalName(memberInfo.Name);
							dataMember2.IsNullable = DataContract.IsTypeNullable(dataMember2.MemberType);
							ClassDataContract.CheckAndAddMember(list, dataMember2, dictionary);
						}
					}
					else
					{
						FieldInfo fieldInfo2 = memberInfo as FieldInfo;
						if (fieldInfo2 != null && !fieldInfo2.IsNotSerialized)
						{
							DataMember dataMember3 = new DataMember(memberInfo);
							dataMember3.Name = DataContract.EncodeLocalName(memberInfo.Name);
							object[] customAttributes3 = fieldInfo2.GetCustomAttributes(Globals.TypeOfOptionalFieldAttribute, false);
							if (customAttributes3 == null || customAttributes3.Length == 0)
							{
								if (base.IsReference)
								{
									DataContract.DataContractCriticalHelper.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("For type '{0}', non-optional field member '{1}' is on the Serializable type that has IsReference as {2}.", new object[]
									{
										DataContract.GetClrTypeFullName(memberInfo.DeclaringType),
										memberInfo.Name,
										true
									}), underlyingType);
								}
								dataMember3.IsRequired = true;
							}
							dataMember3.IsNullable = DataContract.IsTypeNullable(dataMember3.MemberType);
							ClassDataContract.CheckAndAddMember(list, dataMember3, dictionary);
						}
					}
					IL_053D:;
				}
				if (list.Count > 1)
				{
					list.Sort(ClassDataContract.DataMemberComparer.Singleton);
				}
				this.SetIfMembersHaveConflict(list);
				Thread.MemoryBarrier();
				this.members = list;
			}

			private bool SetIfGetOnlyCollection(DataMember memberContract, bool skipIfReadOnlyContract)
			{
				if (CollectionDataContract.IsCollection(memberContract.MemberType, false, skipIfReadOnlyContract) && !memberContract.MemberType.IsValueType)
				{
					memberContract.IsGetOnlyCollection = true;
					return true;
				}
				return false;
			}

			private void SetIfMembersHaveConflict(List<DataMember> members)
			{
				if (this.BaseContract == null)
				{
					return;
				}
				int num = 0;
				List<ClassDataContract.ClassDataContractCriticalHelper.Member> list = new List<ClassDataContract.ClassDataContractCriticalHelper.Member>();
				foreach (DataMember dataMember in members)
				{
					list.Add(new ClassDataContract.ClassDataContractCriticalHelper.Member(dataMember, base.StableName.Namespace, num));
				}
				for (ClassDataContract classDataContract = this.BaseContract; classDataContract != null; classDataContract = classDataContract.BaseContract)
				{
					num++;
					foreach (DataMember dataMember2 in classDataContract.Members)
					{
						list.Add(new ClassDataContract.ClassDataContractCriticalHelper.Member(dataMember2, classDataContract.StableName.Namespace, num));
					}
				}
				IComparer<ClassDataContract.ClassDataContractCriticalHelper.Member> singleton = ClassDataContract.ClassDataContractCriticalHelper.DataMemberConflictComparer.Singleton;
				list.Sort(singleton);
				for (int i = 0; i < list.Count - 1; i++)
				{
					int num2 = i;
					int num3 = i;
					bool flag = false;
					while (num3 < list.Count - 1 && string.CompareOrdinal(list[num3].member.Name, list[num3 + 1].member.Name) == 0 && string.CompareOrdinal(list[num3].ns, list[num3 + 1].ns) == 0)
					{
						list[num3].member.ConflictingMember = list[num3 + 1].member;
						if (!flag)
						{
							flag = list[num3 + 1].member.HasConflictingNameAndType || list[num3].member.MemberType != list[num3 + 1].member.MemberType;
						}
						num3++;
					}
					if (flag)
					{
						for (int j = num2; j <= num3; j++)
						{
							list[j].member.HasConflictingNameAndType = true;
						}
					}
					i = num3 + 1;
				}
			}

			[SecuritySafeCritical]
			private XmlQualifiedName GetStableNameAndSetHasDataContract(Type type)
			{
				return DataContract.GetStableName(type, out this.hasDataContract);
			}

			private void SetIsNonAttributedType(Type type)
			{
				this.isNonAttributedType = !type.IsSerializable && !this.hasDataContract && ClassDataContract.IsNonAttributedTypeValidForSerialization(type);
			}

			private static bool IsMethodOverriding(MethodInfo method)
			{
				return method.IsVirtual && (method.Attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.PrivateScope;
			}

			internal void EnsureMethodsImported()
			{
				if (!this.isMethodChecked && base.UnderlyingType != null)
				{
					lock (this)
					{
						if (!this.isMethodChecked)
						{
							foreach (MethodInfo methodInfo in base.UnderlyingType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
							{
								Type type = null;
								ParameterInfo[] parameters = methodInfo.GetParameters();
								if (this.HasExtensionData && this.IsValidExtensionDataSetMethod(methodInfo, parameters))
								{
									if (methodInfo.Name == "System.Runtime.Serialization.IExtensibleDataObject.set_ExtensionData" || !methodInfo.IsPublic)
									{
										this.extensionDataSetMethod = XmlFormatGeneratorStatics.ExtensionDataSetExplicitMethodInfo;
									}
									else
									{
										this.extensionDataSetMethod = methodInfo;
									}
								}
								if (ClassDataContract.ClassDataContractCriticalHelper.IsValidCallback(methodInfo, parameters, Globals.TypeOfOnSerializingAttribute, this.onSerializing, ref type))
								{
									this.onSerializing = methodInfo;
								}
								if (ClassDataContract.ClassDataContractCriticalHelper.IsValidCallback(methodInfo, parameters, Globals.TypeOfOnSerializedAttribute, this.onSerialized, ref type))
								{
									this.onSerialized = methodInfo;
								}
								if (ClassDataContract.ClassDataContractCriticalHelper.IsValidCallback(methodInfo, parameters, Globals.TypeOfOnDeserializingAttribute, this.onDeserializing, ref type))
								{
									this.onDeserializing = methodInfo;
								}
								if (ClassDataContract.ClassDataContractCriticalHelper.IsValidCallback(methodInfo, parameters, Globals.TypeOfOnDeserializedAttribute, this.onDeserialized, ref type))
								{
									this.onDeserialized = methodInfo;
								}
							}
							Thread.MemoryBarrier();
							this.isMethodChecked = true;
						}
					}
				}
			}

			private bool IsValidExtensionDataSetMethod(MethodInfo method, ParameterInfo[] parameters)
			{
				if (method.Name == "System.Runtime.Serialization.IExtensibleDataObject.set_ExtensionData" || method.Name == "set_ExtensionData")
				{
					if (this.extensionDataSetMethod != null)
					{
						base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Duplicate extension data set method was found, for method '{0}', existing method is '{1}', on data contract type '{2}'.", new object[]
						{
							method,
							this.extensionDataSetMethod,
							DataContract.GetClrTypeFullName(method.DeclaringType)
						}));
					}
					if (method.ReturnType != Globals.TypeOfVoid)
					{
						DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("For type '{0}' method '{1}', extension data set method must return void.", new object[]
						{
							DataContract.GetClrTypeFullName(method.DeclaringType),
							method
						}), method.DeclaringType);
					}
					if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != Globals.TypeOfExtensionDataObject)
					{
						DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("For type '{0}' method '{1}', extension data set method has invalid type of parameter '{2}'.", new object[]
						{
							DataContract.GetClrTypeFullName(method.DeclaringType),
							method,
							Globals.TypeOfExtensionDataObject
						}), method.DeclaringType);
					}
					return true;
				}
				return false;
			}

			private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
			{
				if (method.IsDefined(attributeType, false))
				{
					if (currentCallback != null)
					{
						DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.", new object[]
						{
							method,
							currentCallback,
							DataContract.GetClrTypeFullName(method.DeclaringType),
							attributeType
						}), method.DeclaringType);
					}
					else if (prevAttributeType != null)
					{
						DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.", new object[]
						{
							prevAttributeType,
							attributeType,
							DataContract.GetClrTypeFullName(method.DeclaringType),
							method
						}), method.DeclaringType);
					}
					else if (method.IsVirtual)
					{
						DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.", new object[]
						{
							method,
							DataContract.GetClrTypeFullName(method.DeclaringType),
							attributeType
						}), method.DeclaringType);
					}
					else
					{
						if (method.ReturnType != Globals.TypeOfVoid)
						{
							DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Serialization Callback '{1}' in type '{0}' must return void.", new object[]
							{
								DataContract.GetClrTypeFullName(method.DeclaringType),
								method
							}), method.DeclaringType);
						}
						if (parameters == null || parameters.Length != 1 || parameters[0].ParameterType != Globals.TypeOfStreamingContext)
						{
							DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.", new object[]
							{
								DataContract.GetClrTypeFullName(method.DeclaringType),
								method,
								Globals.TypeOfStreamingContext
							}), method.DeclaringType);
						}
						prevAttributeType = attributeType;
					}
					return true;
				}
				return false;
			}

			internal ClassDataContract BaseContract
			{
				get
				{
					return this.baseContract;
				}
				set
				{
					this.baseContract = value;
					if (this.baseContract != null && base.IsValueType)
					{
						base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Data contract '{0}' from namespace '{1}' is a value type and cannot have base contract '{2}' from namespace '{3}'.", new object[]
						{
							base.StableName.Name,
							base.StableName.Namespace,
							this.baseContract.StableName.Name,
							this.baseContract.StableName.Namespace
						}));
					}
				}
			}

			internal List<DataMember> Members
			{
				get
				{
					return this.members;
				}
				set
				{
					this.members = value;
				}
			}

			internal MethodInfo OnSerializing
			{
				get
				{
					this.EnsureMethodsImported();
					return this.onSerializing;
				}
			}

			internal MethodInfo OnSerialized
			{
				get
				{
					this.EnsureMethodsImported();
					return this.onSerialized;
				}
			}

			internal MethodInfo OnDeserializing
			{
				get
				{
					this.EnsureMethodsImported();
					return this.onDeserializing;
				}
			}

			internal MethodInfo OnDeserialized
			{
				get
				{
					this.EnsureMethodsImported();
					return this.onDeserialized;
				}
			}

			internal MethodInfo ExtensionDataSetMethod
			{
				get
				{
					this.EnsureMethodsImported();
					return this.extensionDataSetMethod;
				}
			}

			internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
			{
				get
				{
					if (!this.isKnownTypeAttributeChecked && base.UnderlyingType != null)
					{
						lock (this)
						{
							if (!this.isKnownTypeAttributeChecked)
							{
								this.knownDataContracts = DataContract.ImportKnownTypeAttributes(base.UnderlyingType);
								Thread.MemoryBarrier();
								this.isKnownTypeAttributeChecked = true;
							}
						}
					}
					return this.knownDataContracts;
				}
				set
				{
					this.knownDataContracts = value;
				}
			}

			internal string SerializationExceptionMessage
			{
				get
				{
					return this.serializationExceptionMessage;
				}
			}

			internal string DeserializationExceptionMessage
			{
				get
				{
					if (this.serializationExceptionMessage == null)
					{
						return null;
					}
					return global::System.Runtime.Serialization.SR.GetString("Error on deserializing read-only members in the class: {0}", new object[] { this.serializationExceptionMessage });
				}
			}

			internal override bool IsISerializable
			{
				get
				{
					return this.isISerializable;
				}
				set
				{
					this.isISerializable = value;
				}
			}

			internal bool HasDataContract
			{
				get
				{
					return this.hasDataContract;
				}
			}

			internal bool HasExtensionData
			{
				get
				{
					return this.hasExtensionData;
				}
			}

			internal bool IsNonAttributedType
			{
				get
				{
					return this.isNonAttributedType;
				}
			}

			internal ConstructorInfo GetISerializableConstructor()
			{
				if (!this.IsISerializable)
				{
					return null;
				}
				ConstructorInfo constructor = base.UnderlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, ClassDataContract.ClassDataContractCriticalHelper.SerInfoCtorArgs, null);
				if (constructor == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Constructor that takes SerializationInfo and StreamingContext is not found for '{0}'.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
				}
				return constructor;
			}

			internal ConstructorInfo GetNonAttributedTypeConstructor()
			{
				if (!this.IsNonAttributedType)
				{
					return null;
				}
				Type underlyingType = base.UnderlyingType;
				if (underlyingType.IsValueType)
				{
					return null;
				}
				ConstructorInfo constructor = underlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
				if (constructor == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("The Type '{0}' must have a parameterless constructor.", new object[] { DataContract.GetClrTypeFullName(underlyingType) })));
				}
				return constructor;
			}

			internal XmlFormatClassWriterDelegate XmlFormatWriterDelegate
			{
				get
				{
					return this.xmlFormatWriterDelegate;
				}
				set
				{
					this.xmlFormatWriterDelegate = value;
				}
			}

			internal XmlFormatClassReaderDelegate XmlFormatReaderDelegate
			{
				get
				{
					return this.xmlFormatReaderDelegate;
				}
				set
				{
					this.xmlFormatReaderDelegate = value;
				}
			}

			public XmlDictionaryString[] ChildElementNamespaces
			{
				get
				{
					return this.childElementNamespaces;
				}
				set
				{
					this.childElementNamespaces = value;
				}
			}

			private static Type[] SerInfoCtorArgs
			{
				get
				{
					if (ClassDataContract.ClassDataContractCriticalHelper.serInfoCtorArgs == null)
					{
						ClassDataContract.ClassDataContractCriticalHelper.serInfoCtorArgs = new Type[]
						{
							typeof(SerializationInfo),
							typeof(StreamingContext)
						};
					}
					return ClassDataContract.ClassDataContractCriticalHelper.serInfoCtorArgs;
				}
			}

			private ClassDataContract baseContract;

			private List<DataMember> members;

			private MethodInfo onSerializing;

			private MethodInfo onSerialized;

			private MethodInfo onDeserializing;

			private MethodInfo onDeserialized;

			private MethodInfo extensionDataSetMethod;

			private Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

			private string serializationExceptionMessage;

			private bool isISerializable;

			private bool isKnownTypeAttributeChecked;

			private bool isMethodChecked;

			private bool hasExtensionData;

			private bool isNonAttributedType;

			private bool hasDataContract;

			private XmlDictionaryString[] childElementNamespaces;

			private XmlFormatClassReaderDelegate xmlFormatReaderDelegate;

			private XmlFormatClassWriterDelegate xmlFormatWriterDelegate;

			public XmlDictionaryString[] ContractNamespaces;

			public XmlDictionaryString[] MemberNames;

			public XmlDictionaryString[] MemberNamespaces;

			private static Type[] serInfoCtorArgs;

			internal struct Member
			{
				internal Member(DataMember member, string ns, int baseTypeIndex)
				{
					this.member = member;
					this.ns = ns;
					this.baseTypeIndex = baseTypeIndex;
				}

				internal DataMember member;

				internal string ns;

				internal int baseTypeIndex;
			}

			internal class DataMemberConflictComparer : IComparer<ClassDataContract.ClassDataContractCriticalHelper.Member>
			{
				public int Compare(ClassDataContract.ClassDataContractCriticalHelper.Member x, ClassDataContract.ClassDataContractCriticalHelper.Member y)
				{
					int num = string.CompareOrdinal(x.ns, y.ns);
					if (num != 0)
					{
						return num;
					}
					int num2 = string.CompareOrdinal(x.member.Name, y.member.Name);
					if (num2 != 0)
					{
						return num2;
					}
					return x.baseTypeIndex - y.baseTypeIndex;
				}

				internal static ClassDataContract.ClassDataContractCriticalHelper.DataMemberConflictComparer Singleton = new ClassDataContract.ClassDataContractCriticalHelper.DataMemberConflictComparer();
			}
		}

		internal class DataMemberComparer : IComparer<DataMember>
		{
			public int Compare(DataMember x, DataMember y)
			{
				int num = x.Order - y.Order;
				if (num != 0)
				{
					return num;
				}
				return string.CompareOrdinal(x.Name, y.Name);
			}

			internal static ClassDataContract.DataMemberComparer Singleton = new ClassDataContract.DataMemberComparer();
		}
	}
}
