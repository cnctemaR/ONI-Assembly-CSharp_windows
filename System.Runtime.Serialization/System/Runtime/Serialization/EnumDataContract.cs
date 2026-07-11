using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class EnumDataContract : DataContract
	{
		[SecuritySafeCritical]
		internal EnumDataContract()
			: base(new EnumDataContract.EnumDataContractCriticalHelper())
		{
			this.helper = base.Helper as EnumDataContract.EnumDataContractCriticalHelper;
		}

		[SecuritySafeCritical]
		internal EnumDataContract(Type type)
			: base(new EnumDataContract.EnumDataContractCriticalHelper(type))
		{
			this.helper = base.Helper as EnumDataContract.EnumDataContractCriticalHelper;
		}

		[SecuritySafeCritical]
		internal static XmlQualifiedName GetBaseContractName(Type type)
		{
			return EnumDataContract.EnumDataContractCriticalHelper.GetBaseContractName(type);
		}

		[SecuritySafeCritical]
		internal static Type GetBaseType(XmlQualifiedName baseContractName)
		{
			return EnumDataContract.EnumDataContractCriticalHelper.GetBaseType(baseContractName);
		}

		internal XmlQualifiedName BaseContractName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.BaseContractName;
			}
			[SecurityCritical]
			set
			{
				this.helper.BaseContractName = value;
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

		internal List<long> Values
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.Values;
			}
			[SecurityCritical]
			set
			{
				this.helper.Values = value;
			}
		}

		internal bool IsFlags
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsFlags;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsFlags = value;
			}
		}

		internal bool IsULong
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsULong;
			}
		}

		private XmlDictionaryString[] ChildElementNames
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ChildElementNames;
			}
		}

		internal override bool CanContainReferences
		{
			get
			{
				return false;
			}
		}

		internal void WriteEnumValue(XmlWriterDelegator writer, object value)
		{
			long num = (long)(this.IsULong ? ((IConvertible)value).ToUInt64(null) : ((ulong)((IConvertible)value).ToInt64(null)));
			for (int i = 0; i < this.Values.Count; i++)
			{
				if (num == this.Values[i])
				{
					writer.WriteString(this.ChildElementNames[i].Value);
					return;
				}
			}
			if (!this.IsFlags)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Enum value '{0}' is invalid for type '{1}' and cannot be serialized. Ensure that the necessary enum values are present and are marked with EnumMemberAttribute attribute if the type has DataContractAttribute attribute.", new object[]
				{
					value,
					DataContract.GetClrTypeFullName(base.UnderlyingType)
				})));
			}
			int num2 = -1;
			bool flag = true;
			for (int j = 0; j < this.Values.Count; j++)
			{
				long num3 = this.Values[j];
				if (num3 == 0L)
				{
					num2 = j;
				}
				else
				{
					if (num == 0L)
					{
						break;
					}
					if ((num3 & num) == num3)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							writer.WriteString(DictionaryGlobals.Space.Value);
						}
						writer.WriteString(this.ChildElementNames[j].Value);
						num &= ~num3;
					}
				}
			}
			if (num != 0L)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Enum value '{0}' is invalid for type '{1}' and cannot be serialized. Ensure that the necessary enum values are present and are marked with EnumMemberAttribute attribute if the type has DataContractAttribute attribute.", new object[]
				{
					value,
					DataContract.GetClrTypeFullName(base.UnderlyingType)
				})));
			}
			if (flag && num2 >= 0)
			{
				writer.WriteString(this.ChildElementNames[num2].Value);
				return;
			}
		}

		internal object ReadEnumValue(XmlReaderDelegator reader)
		{
			string text = reader.ReadElementContentAsString();
			long num = 0L;
			int i = 0;
			if (this.IsFlags)
			{
				while (i < text.Length && text[i] == ' ')
				{
					i++;
				}
				int num2 = i;
				int num3;
				while (i < text.Length)
				{
					if (text[i] == ' ')
					{
						num3 = i - num2;
						if (num3 > 0)
						{
							num |= this.ReadEnumValue(text, num2, num3);
						}
						i++;
						while (i < text.Length && text[i] == ' ')
						{
							i++;
						}
						num2 = i;
						if (i == text.Length)
						{
							break;
						}
					}
					i++;
				}
				num3 = i - num2;
				if (num3 > 0)
				{
					num |= this.ReadEnumValue(text, num2, num3);
				}
			}
			else
			{
				if (text.Length == 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid enum value '{0}' cannot be deserialized into type '{1}'. Ensure that the necessary enum values are present and are marked with EnumMemberAttribute attribute if the type has DataContractAttribute attribute.", new object[]
					{
						text,
						DataContract.GetClrTypeFullName(base.UnderlyingType)
					})));
				}
				num = this.ReadEnumValue(text, 0, text.Length);
			}
			if (this.IsULong)
			{
				return Enum.ToObject(base.UnderlyingType, (ulong)num);
			}
			return Enum.ToObject(base.UnderlyingType, num);
		}

		private long ReadEnumValue(string value, int index, int count)
		{
			for (int i = 0; i < this.Members.Count; i++)
			{
				string name = this.Members[i].Name;
				if (name.Length == count && string.CompareOrdinal(value, index, name, 0, count) == 0)
				{
					return this.Values[i];
				}
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid enum value '{0}' cannot be deserialized into type '{1}'. Ensure that the necessary enum values are present and are marked with EnumMemberAttribute attribute if the type has DataContractAttribute attribute.", new object[]
			{
				value.Substring(index, count),
				DataContract.GetClrTypeFullName(base.UnderlyingType)
			})));
		}

		internal string GetStringFromEnumValue(long value)
		{
			if (this.IsULong)
			{
				return XmlConvert.ToString((ulong)value);
			}
			return XmlConvert.ToString(value);
		}

		internal long GetEnumValueFromString(string value)
		{
			if (this.IsULong)
			{
				return (long)XmlConverter.ToUInt64(value);
			}
			return XmlConverter.ToInt64(value);
		}

		internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (base.IsEqualOrChecked(other, checkedContracts))
			{
				return true;
			}
			if (base.Equals(other, null))
			{
				EnumDataContract enumDataContract = other as EnumDataContract;
				if (enumDataContract != null)
				{
					if (this.Members.Count != enumDataContract.Members.Count || this.Values.Count != enumDataContract.Values.Count)
					{
						return false;
					}
					string[] array = new string[this.Members.Count];
					string[] array2 = new string[this.Members.Count];
					for (int i = 0; i < this.Members.Count; i++)
					{
						array[i] = this.Members[i].Name;
						array2[i] = enumDataContract.Members[i].Name;
					}
					Array.Sort<string>(array);
					Array.Sort<string>(array2);
					for (int j = 0; j < this.Members.Count; j++)
					{
						if (array[j] != array2[j])
						{
							return false;
						}
					}
					return this.IsFlags == enumDataContract.IsFlags;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			this.WriteEnumValue(xmlWriter, obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
		{
			object obj = this.ReadEnumValue(xmlReader);
			if (context != null)
			{
				context.AddNewObject(obj);
			}
			return obj;
		}

		[SecurityCritical]
		private EnumDataContract.EnumDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class EnumDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			static EnumDataContractCriticalHelper()
			{
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(sbyte), "byte");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(byte), "unsignedByte");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(short), "short");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(ushort), "unsignedShort");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(int), "int");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(uint), "unsignedInt");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(long), "long");
				EnumDataContract.EnumDataContractCriticalHelper.Add(typeof(ulong), "unsignedLong");
			}

			internal static void Add(Type type, string localName)
			{
				XmlQualifiedName xmlQualifiedName = DataContract.CreateQualifiedName(localName, "http://www.w3.org/2001/XMLSchema");
				EnumDataContract.EnumDataContractCriticalHelper.typeToName.Add(type, xmlQualifiedName);
				EnumDataContract.EnumDataContractCriticalHelper.nameToType.Add(xmlQualifiedName, type);
			}

			internal static XmlQualifiedName GetBaseContractName(Type type)
			{
				XmlQualifiedName xmlQualifiedName = null;
				EnumDataContract.EnumDataContractCriticalHelper.typeToName.TryGetValue(type, out xmlQualifiedName);
				return xmlQualifiedName;
			}

			internal static Type GetBaseType(XmlQualifiedName baseContractName)
			{
				Type type = null;
				EnumDataContract.EnumDataContractCriticalHelper.nameToType.TryGetValue(baseContractName, out type);
				return type;
			}

			internal EnumDataContractCriticalHelper()
			{
				base.IsValueType = true;
			}

			internal EnumDataContractCriticalHelper(Type type)
				: base(type)
			{
				base.StableName = DataContract.GetStableName(type, out this.hasDataContract);
				Type underlyingType = Enum.GetUnderlyingType(type);
				this.baseContractName = EnumDataContract.EnumDataContractCriticalHelper.GetBaseContractName(underlyingType);
				this.ImportBaseType(underlyingType);
				this.IsFlags = type.IsDefined(Globals.TypeOfFlagsAttribute, false);
				this.ImportDataMembers();
				XmlDictionary xmlDictionary = new XmlDictionary(2 + this.Members.Count);
				base.Name = xmlDictionary.Add(base.StableName.Name);
				base.Namespace = xmlDictionary.Add(base.StableName.Namespace);
				this.childElementNames = new XmlDictionaryString[this.Members.Count];
				for (int i = 0; i < this.Members.Count; i++)
				{
					this.childElementNames[i] = xmlDictionary.Add(this.Members[i].Name);
				}
				DataContractAttribute dataContractAttribute;
				if (DataContract.TryGetDCAttribute(type, out dataContractAttribute) && dataContractAttribute.IsReference)
				{
					DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Enum type '{0}' cannot have the IsReference setting of '{1}'. Either change the setting to '{2}', or remove it completely.", new object[]
					{
						DataContract.GetClrTypeFullName(type),
						dataContractAttribute.IsReference,
						false
					}), type);
				}
			}

			internal XmlQualifiedName BaseContractName
			{
				get
				{
					return this.baseContractName;
				}
				set
				{
					this.baseContractName = value;
					Type baseType = EnumDataContract.EnumDataContractCriticalHelper.GetBaseType(this.baseContractName);
					if (baseType == null)
					{
						base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Invalid enum base type is specified for type '{0}' in '{1}' namespace, element name is '{2}' in '{3}' namespace.", new object[]
						{
							value.Name,
							value.Namespace,
							base.StableName.Name,
							base.StableName.Namespace
						}));
					}
					this.ImportBaseType(baseType);
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

			internal List<long> Values
			{
				get
				{
					return this.values;
				}
				set
				{
					this.values = value;
				}
			}

			internal bool IsFlags
			{
				get
				{
					return this.isFlags;
				}
				set
				{
					this.isFlags = value;
				}
			}

			internal bool IsULong
			{
				get
				{
					return this.isULong;
				}
			}

			internal XmlDictionaryString[] ChildElementNames
			{
				get
				{
					return this.childElementNames;
				}
			}

			private void ImportBaseType(Type baseType)
			{
				this.isULong = baseType == Globals.TypeOfULong;
			}

			private void ImportDataMembers()
			{
				Type underlyingType = base.UnderlyingType;
				FieldInfo[] fields = underlyingType.GetFields(BindingFlags.Static | BindingFlags.Public);
				Dictionary<string, DataMember> dictionary = new Dictionary<string, DataMember>();
				List<DataMember> list = new List<DataMember>(fields.Length);
				List<long> list2 = new List<long>(fields.Length);
				foreach (FieldInfo fieldInfo in fields)
				{
					bool flag = false;
					if (this.hasDataContract)
					{
						object[] customAttributes = fieldInfo.GetCustomAttributes(Globals.TypeOfEnumMemberAttribute, false);
						if (customAttributes != null && customAttributes.Length != 0)
						{
							if (customAttributes.Length > 1)
							{
								base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Member '{0}.{1}' has more than one EnumMemberAttribute attribute.", new object[]
								{
									DataContract.GetClrTypeFullName(fieldInfo.DeclaringType),
									fieldInfo.Name
								}));
							}
							EnumMemberAttribute enumMemberAttribute = (EnumMemberAttribute)customAttributes[0];
							DataMember dataMember = new DataMember(fieldInfo);
							if (enumMemberAttribute.IsValueSetExplicitly)
							{
								if (enumMemberAttribute.Value == null || enumMemberAttribute.Value.Length == 0)
								{
									base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("'{0}' in type '{1}' cannot have EnumMemberAttribute attribute Value set to null or empty string.", new object[]
									{
										fieldInfo.Name,
										DataContract.GetClrTypeFullName(underlyingType)
									}));
								}
								dataMember.Name = enumMemberAttribute.Value;
							}
							else
							{
								dataMember.Name = fieldInfo.Name;
							}
							ClassDataContract.CheckAndAddMember(list, dataMember, dictionary);
							flag = true;
						}
						object[] customAttributes2 = fieldInfo.GetCustomAttributes(Globals.TypeOfDataMemberAttribute, false);
						if (customAttributes2 != null && customAttributes2.Length != 0)
						{
							base.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Member '{0}.{1}' has DataMemberAttribute attribute. Use EnumMemberAttribute attribute instead.", new object[]
							{
								DataContract.GetClrTypeFullName(fieldInfo.DeclaringType),
								fieldInfo.Name
							}));
						}
					}
					else if (!fieldInfo.IsNotSerialized)
					{
						ClassDataContract.CheckAndAddMember(list, new DataMember(fieldInfo)
						{
							Name = fieldInfo.Name
						}, dictionary);
						flag = true;
					}
					if (flag)
					{
						object value = fieldInfo.GetValue(null);
						if (this.isULong)
						{
							list2.Add((long)((IConvertible)value).ToUInt64(null));
						}
						else
						{
							list2.Add(((IConvertible)value).ToInt64(null));
						}
					}
				}
				Thread.MemoryBarrier();
				this.members = list;
				this.values = list2;
			}

			private static Dictionary<Type, XmlQualifiedName> typeToName = new Dictionary<Type, XmlQualifiedName>();

			private static Dictionary<XmlQualifiedName, Type> nameToType = new Dictionary<XmlQualifiedName, Type>();

			private XmlQualifiedName baseContractName;

			private List<DataMember> members;

			private List<long> values;

			private bool isULong;

			private bool isFlags;

			private bool hasDataContract;

			private XmlDictionaryString[] childElementNames;
		}
	}
}
