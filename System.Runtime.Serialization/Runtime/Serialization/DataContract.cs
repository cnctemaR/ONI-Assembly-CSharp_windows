using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Configuration;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	internal abstract class DataContract
	{
		[SecuritySafeCritical]
		protected DataContract(DataContract.DataContractCriticalHelper helper)
		{
			this.helper = helper;
			this.name = helper.Name;
			this.ns = helper.Namespace;
		}

		internal static DataContract GetDataContract(Type type)
		{
			return DataContract.GetDataContract(type.TypeHandle, type, SerializationMode.SharedContract);
		}

		internal static DataContract GetDataContract(RuntimeTypeHandle typeHandle, Type type, SerializationMode mode)
		{
			return DataContract.GetDataContract(DataContract.GetId(typeHandle), typeHandle, mode);
		}

		internal static DataContract GetDataContract(int id, RuntimeTypeHandle typeHandle, SerializationMode mode)
		{
			return DataContract.GetDataContractSkipValidation(id, typeHandle, null).GetValidContract(mode);
		}

		[SecuritySafeCritical]
		internal static DataContract GetDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
		{
			return DataContract.DataContractCriticalHelper.GetDataContractSkipValidation(id, typeHandle, type);
		}

		internal static DataContract GetGetOnlyCollectionDataContract(int id, RuntimeTypeHandle typeHandle, Type type, SerializationMode mode)
		{
			DataContract dataContract = DataContract.GetGetOnlyCollectionDataContractSkipValidation(id, typeHandle, type);
			dataContract = dataContract.GetValidContract(mode);
			if (dataContract is ClassDataContract)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("For '{0}' type, class data contract was returned for get-only collection.", new object[] { DataContract.GetClrTypeFullName(dataContract.UnderlyingType) })));
			}
			return dataContract;
		}

		[SecuritySafeCritical]
		internal static DataContract GetGetOnlyCollectionDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
		{
			return DataContract.DataContractCriticalHelper.GetGetOnlyCollectionDataContractSkipValidation(id, typeHandle, type);
		}

		[SecuritySafeCritical]
		internal static DataContract GetDataContractForInitialization(int id)
		{
			return DataContract.DataContractCriticalHelper.GetDataContractForInitialization(id);
		}

		[SecuritySafeCritical]
		internal static int GetIdForInitialization(ClassDataContract classContract)
		{
			return DataContract.DataContractCriticalHelper.GetIdForInitialization(classContract);
		}

		[SecuritySafeCritical]
		internal static int GetId(RuntimeTypeHandle typeHandle)
		{
			return DataContract.DataContractCriticalHelper.GetId(typeHandle);
		}

		[SecuritySafeCritical]
		public static DataContract GetBuiltInDataContract(Type type)
		{
			return DataContract.DataContractCriticalHelper.GetBuiltInDataContract(type);
		}

		[SecuritySafeCritical]
		public static DataContract GetBuiltInDataContract(string name, string ns)
		{
			return DataContract.DataContractCriticalHelper.GetBuiltInDataContract(name, ns);
		}

		[SecuritySafeCritical]
		public static DataContract GetBuiltInDataContract(string typeName)
		{
			return DataContract.DataContractCriticalHelper.GetBuiltInDataContract(typeName);
		}

		[SecuritySafeCritical]
		internal static string GetNamespace(string key)
		{
			return DataContract.DataContractCriticalHelper.GetNamespace(key);
		}

		[SecuritySafeCritical]
		internal static XmlDictionaryString GetClrTypeString(string key)
		{
			return DataContract.DataContractCriticalHelper.GetClrTypeString(key);
		}

		[SecuritySafeCritical]
		internal static void ThrowInvalidDataContractException(string message, Type type)
		{
			DataContract.DataContractCriticalHelper.ThrowInvalidDataContractException(message, type);
		}

		protected DataContract.DataContractCriticalHelper Helper
		{
			[SecurityCritical]
			get
			{
				return this.helper;
			}
		}

		internal Type UnderlyingType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.UnderlyingType;
			}
		}

		internal Type OriginalUnderlyingType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.OriginalUnderlyingType;
			}
		}

		internal virtual bool IsBuiltInDataContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsBuiltInDataContract;
			}
		}

		internal Type TypeForInitialization
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TypeForInitialization;
			}
		}

		public virtual void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. Unexpected contract type '{0}' for type '{1}' encountered.", new object[]
			{
				DataContract.GetClrTypeFullName(base.GetType()),
				DataContract.GetClrTypeFullName(this.UnderlyingType)
			})));
		}

		public virtual object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. Unexpected contract type '{0}' for type '{1}' encountered.", new object[]
			{
				DataContract.GetClrTypeFullName(base.GetType()),
				DataContract.GetClrTypeFullName(this.UnderlyingType)
			})));
		}

		internal bool IsValueType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsValueType;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsValueType = value;
			}
		}

		internal bool IsReference
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsReference;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsReference = value;
			}
		}

		internal XmlQualifiedName StableName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.StableName;
			}
			[SecurityCritical]
			set
			{
				this.helper.StableName = value;
			}
		}

		internal GenericInfo GenericInfo
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.GenericInfo;
			}
			[SecurityCritical]
			set
			{
				this.helper.GenericInfo = value;
			}
		}

		internal virtual Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
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

		internal virtual bool IsISerializable
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

		internal XmlDictionaryString Name
		{
			[SecuritySafeCritical]
			get
			{
				return this.name;
			}
		}

		public virtual XmlDictionaryString Namespace
		{
			[SecuritySafeCritical]
			get
			{
				return this.ns;
			}
		}

		internal virtual bool HasRoot
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		internal virtual XmlDictionaryString TopLevelElementName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TopLevelElementName;
			}
			[SecurityCritical]
			set
			{
				this.helper.TopLevelElementName = value;
			}
		}

		internal virtual XmlDictionaryString TopLevelElementNamespace
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TopLevelElementNamespace;
			}
			[SecurityCritical]
			set
			{
				this.helper.TopLevelElementNamespace = value;
			}
		}

		internal virtual bool CanContainReferences
		{
			get
			{
				return true;
			}
		}

		internal virtual bool IsPrimitive
		{
			get
			{
				return false;
			}
		}

		internal virtual void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
		{
			if (ns == DictionaryGlobals.SerializationNamespace && !this.IsPrimitive)
			{
				writer.WriteStartElement("z", name, ns);
				return;
			}
			writer.WriteStartElement(name, ns);
		}

		internal virtual DataContract BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
		{
			return this;
		}

		internal virtual DataContract GetValidContract(SerializationMode mode)
		{
			return this;
		}

		internal virtual DataContract GetValidContract()
		{
			return this;
		}

		internal virtual bool IsValidContract(SerializationMode mode)
		{
			return true;
		}

		internal MethodInfo ParseMethod
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ParseMethod;
			}
		}

		internal static bool IsTypeSerializable(Type type)
		{
			return DataContract.IsTypeSerializable(type, new Dictionary<Type, object>());
		}

		private static bool IsTypeSerializable(Type type, Dictionary<Type, object> previousCollectionTypes)
		{
			if (type.IsSerializable || type.IsDefined(Globals.TypeOfDataContractAttribute, false) || type.IsInterface || type.IsPointer || Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
			{
				return true;
			}
			Type type2;
			if (CollectionDataContract.IsCollection(type, out type2))
			{
				DataContract.ValidatePreviousCollectionTypes(type, type2, previousCollectionTypes);
				if (DataContract.IsTypeSerializable(type2, previousCollectionTypes))
				{
					return true;
				}
			}
			return DataContract.GetBuiltInDataContract(type) != null || ClassDataContract.IsNonAttributedTypeValidForSerialization(type);
		}

		private static void ValidatePreviousCollectionTypes(Type collectionType, Type itemType, Dictionary<Type, object> previousCollectionTypes)
		{
			previousCollectionTypes.Add(collectionType, collectionType);
			while (itemType.IsArray)
			{
				itemType = itemType.GetElementType();
			}
			List<Type> list = new List<Type>();
			Queue<Type> queue = new Queue<Type>();
			queue.Enqueue(itemType);
			list.Add(itemType);
			while (queue.Count > 0)
			{
				itemType = queue.Dequeue();
				if (previousCollectionTypes.ContainsKey(itemType))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' involves recursive collection.", new object[] { DataContract.GetClrTypeFullName(itemType) })));
				}
				if (itemType.IsGenericType)
				{
					foreach (Type type in itemType.GetGenericArguments())
					{
						if (!list.Contains(type))
						{
							queue.Enqueue(type);
							list.Add(type);
						}
					}
				}
			}
		}

		internal static Type UnwrapRedundantNullableType(Type type)
		{
			Type type2 = type;
			while (type.IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
			{
				type2 = type;
				type = type.GetGenericArguments()[0];
			}
			return type2;
		}

		internal static Type UnwrapNullableType(Type type)
		{
			while (type.IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable)
			{
				type = type.GetGenericArguments()[0];
			}
			return type;
		}

		private static bool IsAlpha(char ch)
		{
			return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
		}

		private static bool IsDigit(char ch)
		{
			return ch >= '0' && ch <= '9';
		}

		private static bool IsAsciiLocalName(string localName)
		{
			if (localName.Length == 0)
			{
				return false;
			}
			if (!DataContract.IsAlpha(localName[0]))
			{
				return false;
			}
			for (int i = 1; i < localName.Length; i++)
			{
				char c = localName[i];
				if (!DataContract.IsAlpha(c) && !DataContract.IsDigit(c))
				{
					return false;
				}
			}
			return true;
		}

		internal static string EncodeLocalName(string localName)
		{
			if (DataContract.IsAsciiLocalName(localName))
			{
				return localName;
			}
			if (DataContract.IsValidNCName(localName))
			{
				return localName;
			}
			return XmlConvert.EncodeLocalName(localName);
		}

		internal static bool IsValidNCName(string name)
		{
			bool flag;
			try
			{
				XmlConvert.VerifyNCName(name);
				flag = true;
			}
			catch (XmlException)
			{
				flag = false;
			}
			return flag;
		}

		internal static XmlQualifiedName GetStableName(Type type)
		{
			bool flag;
			return DataContract.GetStableName(type, out flag);
		}

		internal static XmlQualifiedName GetStableName(Type type, out bool hasDataContract)
		{
			return DataContract.GetStableName(type, new Dictionary<Type, object>(), out hasDataContract);
		}

		private static XmlQualifiedName GetStableName(Type type, Dictionary<Type, object> previousCollectionTypes, out bool hasDataContract)
		{
			type = DataContract.UnwrapRedundantNullableType(type);
			XmlQualifiedName xmlQualifiedName;
			DataContractAttribute dataContractAttribute;
			if (DataContract.TryGetBuiltInXmlAndArrayTypeStableName(type, previousCollectionTypes, out xmlQualifiedName))
			{
				hasDataContract = false;
			}
			else if (DataContract.TryGetDCAttribute(type, out dataContractAttribute))
			{
				xmlQualifiedName = DataContract.GetDCTypeStableName(type, dataContractAttribute);
				hasDataContract = true;
			}
			else
			{
				xmlQualifiedName = DataContract.GetNonDCTypeStableName(type, previousCollectionTypes);
				hasDataContract = false;
			}
			return xmlQualifiedName;
		}

		private static XmlQualifiedName GetDCTypeStableName(Type type, DataContractAttribute dataContractAttribute)
		{
			string text;
			if (dataContractAttribute.IsNameSetExplicitly)
			{
				text = dataContractAttribute.Name;
				if (text == null || text.Length == 0)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have DataContractAttribute attribute Name set to null or empty string.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
				{
					text = DataContract.ExpandGenericParameters(text, type);
				}
				text = DataContract.EncodeLocalName(text);
			}
			else
			{
				text = DataContract.GetDefaultStableLocalName(type);
			}
			string text2;
			if (dataContractAttribute.IsNamespaceSetExplicitly)
			{
				text2 = dataContractAttribute.Namespace;
				if (text2 == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have DataContractAttribute attribute Namespace set to null.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				DataContract.CheckExplicitDataContractNamespaceUri(text2, type);
			}
			else
			{
				text2 = DataContract.GetDefaultDataContractNamespace(type);
			}
			return DataContract.CreateQualifiedName(text, text2);
		}

		private static XmlQualifiedName GetNonDCTypeStableName(Type type, Dictionary<Type, object> previousCollectionTypes)
		{
			Type type2;
			if (CollectionDataContract.IsCollection(type, out type2))
			{
				DataContract.ValidatePreviousCollectionTypes(type, type2, previousCollectionTypes);
				CollectionDataContractAttribute collectionDataContractAttribute;
				return DataContract.GetCollectionStableName(type, type2, previousCollectionTypes, out collectionDataContractAttribute);
			}
			string defaultStableLocalName = DataContract.GetDefaultStableLocalName(type);
			string text;
			if (ClassDataContract.IsNonAttributedTypeValidForSerialization(type))
			{
				text = DataContract.GetDefaultDataContractNamespace(type);
			}
			else
			{
				text = DataContract.GetDefaultStableNamespace(type);
			}
			return DataContract.CreateQualifiedName(defaultStableLocalName, text);
		}

		private static bool TryGetBuiltInXmlAndArrayTypeStableName(Type type, Dictionary<Type, object> previousCollectionTypes, out XmlQualifiedName stableName)
		{
			stableName = null;
			DataContract builtInDataContract = DataContract.GetBuiltInDataContract(type);
			if (builtInDataContract != null)
			{
				stableName = builtInDataContract.StableName;
			}
			else if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
			{
				XmlQualifiedName xmlQualifiedName;
				XmlSchemaType xmlSchemaType;
				bool flag;
				SchemaExporter.GetXmlTypeInfo(type, out xmlQualifiedName, out xmlSchemaType, out flag);
				stableName = xmlQualifiedName;
			}
			else if (type.IsArray)
			{
				Type elementType = type.GetElementType();
				DataContract.ValidatePreviousCollectionTypes(type, elementType, previousCollectionTypes);
				CollectionDataContractAttribute collectionDataContractAttribute;
				stableName = DataContract.GetCollectionStableName(type, elementType, previousCollectionTypes, out collectionDataContractAttribute);
			}
			return stableName != null;
		}

		[SecuritySafeCritical]
		internal static bool TryGetDCAttribute(Type type, out DataContractAttribute dataContractAttribute)
		{
			dataContractAttribute = null;
			object[] customAttributes = type.GetCustomAttributes(Globals.TypeOfDataContractAttribute, false);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				dataContractAttribute = (DataContractAttribute)customAttributes[0];
			}
			return dataContractAttribute != null;
		}

		internal static XmlQualifiedName GetCollectionStableName(Type type, Type itemType, out CollectionDataContractAttribute collectionContractAttribute)
		{
			return DataContract.GetCollectionStableName(type, itemType, new Dictionary<Type, object>(), out collectionContractAttribute);
		}

		private static XmlQualifiedName GetCollectionStableName(Type type, Type itemType, Dictionary<Type, object> previousCollectionTypes, out CollectionDataContractAttribute collectionContractAttribute)
		{
			object[] customAttributes = type.GetCustomAttributes(Globals.TypeOfCollectionDataContractAttribute, false);
			string text;
			string text2;
			if (customAttributes != null && customAttributes.Length != 0)
			{
				collectionContractAttribute = (CollectionDataContractAttribute)customAttributes[0];
				if (collectionContractAttribute.IsNameSetExplicitly)
				{
					text = collectionContractAttribute.Name;
					if (text == null || text.Length == 0)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have CollectionDataContractAttribute attribute Name set to null or empty string.", new object[] { DataContract.GetClrTypeFullName(type) })));
					}
					if (type.IsGenericType && !type.IsGenericTypeDefinition)
					{
						text = DataContract.ExpandGenericParameters(text, type);
					}
					text = DataContract.EncodeLocalName(text);
				}
				else
				{
					text = DataContract.GetDefaultStableLocalName(type);
				}
				if (collectionContractAttribute.IsNamespaceSetExplicitly)
				{
					text2 = collectionContractAttribute.Namespace;
					if (text2 == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot have CollectionDataContractAttribute attribute Namespace set to null.", new object[] { DataContract.GetClrTypeFullName(type) })));
					}
					DataContract.CheckExplicitDataContractNamespaceUri(text2, type);
				}
				else
				{
					text2 = DataContract.GetDefaultDataContractNamespace(type);
				}
			}
			else
			{
				collectionContractAttribute = null;
				string text3 = "ArrayOf" + DataContract.GetArrayPrefix(ref itemType);
				bool flag;
				XmlQualifiedName stableName = DataContract.GetStableName(itemType, previousCollectionTypes, out flag);
				text = text3 + stableName.Name;
				text2 = DataContract.GetCollectionNamespace(stableName.Namespace);
			}
			return DataContract.CreateQualifiedName(text, text2);
		}

		private static string GetArrayPrefix(ref Type itemType)
		{
			string text = string.Empty;
			while (itemType.IsArray && DataContract.GetBuiltInDataContract(itemType) == null)
			{
				text += "ArrayOf";
				itemType = itemType.GetElementType();
			}
			return text;
		}

		internal XmlQualifiedName GetArrayTypeName(bool isNullable)
		{
			XmlQualifiedName xmlQualifiedName;
			if (this.IsValueType && isNullable)
			{
				GenericInfo genericInfo = new GenericInfo(DataContract.GetStableName(Globals.TypeOfNullable), Globals.TypeOfNullable.FullName);
				genericInfo.Add(new GenericInfo(this.StableName, null));
				genericInfo.AddToLevel(0, 1);
				xmlQualifiedName = genericInfo.GetExpandedStableName();
			}
			else
			{
				xmlQualifiedName = this.StableName;
			}
			string collectionNamespace = DataContract.GetCollectionNamespace(xmlQualifiedName.Namespace);
			return new XmlQualifiedName("ArrayOf" + xmlQualifiedName.Name, collectionNamespace);
		}

		internal static string GetCollectionNamespace(string elementNs)
		{
			if (!DataContract.IsBuiltInNamespace(elementNs))
			{
				return elementNs;
			}
			return "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
		}

		internal static XmlQualifiedName GetDefaultStableName(Type type)
		{
			return DataContract.CreateQualifiedName(DataContract.GetDefaultStableLocalName(type), DataContract.GetDefaultStableNamespace(type));
		}

		private static string GetDefaultStableLocalName(Type type)
		{
			if (type.IsGenericParameter)
			{
				return "{" + type.GenericParameterPosition + "}";
			}
			string text = null;
			if (type.IsArray)
			{
				text = DataContract.GetArrayPrefix(ref type);
			}
			string text2;
			if (type.DeclaringType == null)
			{
				text2 = type.Name;
			}
			else
			{
				int num = ((type.Namespace == null) ? 0 : type.Namespace.Length);
				if (num > 0)
				{
					num++;
				}
				text2 = DataContract.GetClrTypeFullName(type).Substring(num).Replace('+', '.');
			}
			if (text != null)
			{
				text2 = text + text2;
			}
			if (type.IsGenericType)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				bool flag = true;
				int num2 = text2.IndexOf('[');
				if (num2 >= 0)
				{
					text2 = text2.Substring(0, num2);
				}
				IList<int> dataContractNameForGenericName = DataContract.GetDataContractNameForGenericName(text2, stringBuilder);
				bool isGenericTypeDefinition = type.IsGenericTypeDefinition;
				Type[] genericArguments = type.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					Type type2 = genericArguments[i];
					if (isGenericTypeDefinition)
					{
						stringBuilder.Append("{").Append(i).Append("}");
					}
					else
					{
						XmlQualifiedName stableName = DataContract.GetStableName(type2);
						stringBuilder.Append(stableName.Name);
						stringBuilder2.Append(" ").Append(stableName.Namespace);
						if (flag)
						{
							flag = DataContract.IsBuiltInNamespace(stableName.Namespace);
						}
					}
				}
				if (isGenericTypeDefinition)
				{
					stringBuilder.Append("{#}");
				}
				else if (dataContractNameForGenericName.Count > 1 || !flag)
				{
					foreach (int num3 in dataContractNameForGenericName)
					{
						stringBuilder2.Insert(0, num3).Insert(0, " ");
					}
					stringBuilder.Append(DataContract.GetNamespacesDigest(stringBuilder2.ToString()));
				}
				text2 = stringBuilder.ToString();
			}
			return DataContract.EncodeLocalName(text2);
		}

		private static string GetDefaultDataContractNamespace(Type type)
		{
			string text = type.Namespace;
			if (text == null)
			{
				text = string.Empty;
			}
			string text2 = DataContract.GetGlobalDataContractNamespace(text, type.Module);
			if (text2 == null)
			{
				text2 = DataContract.GetGlobalDataContractNamespace(text, type.Assembly);
			}
			if (text2 == null)
			{
				text2 = DataContract.GetDefaultStableNamespace(type);
			}
			else
			{
				DataContract.CheckExplicitDataContractNamespaceUri(text2, type);
			}
			return text2;
		}

		internal static IList<int> GetDataContractNameForGenericName(string typeName, StringBuilder localName)
		{
			List<int> list = new List<int>();
			int num = 0;
			int num2;
			for (;;)
			{
				num2 = typeName.IndexOf('`', num);
				if (num2 < 0)
				{
					break;
				}
				if (localName != null)
				{
					localName.Append(typeName.Substring(num, num2 - num));
				}
				while ((num = typeName.IndexOf('.', num + 1, num2 - num - 1)) >= 0)
				{
					list.Add(0);
				}
				num = typeName.IndexOf('.', num2);
				if (num < 0)
				{
					goto Block_5;
				}
				list.Add(int.Parse(typeName.Substring(num2 + 1, num - num2 - 1), CultureInfo.InvariantCulture));
			}
			if (localName != null)
			{
				localName.Append(typeName.Substring(num));
			}
			list.Add(0);
			goto IL_00AE;
			Block_5:
			list.Add(int.Parse(typeName.Substring(num2 + 1), CultureInfo.InvariantCulture));
			IL_00AE:
			if (localName != null)
			{
				localName.Append("Of");
			}
			return list;
		}

		internal static bool IsBuiltInNamespace(string ns)
		{
			return ns == "http://www.w3.org/2001/XMLSchema" || ns == "http://schemas.microsoft.com/2003/10/Serialization/";
		}

		internal static string GetDefaultStableNamespace(Type type)
		{
			if (type.IsGenericParameter)
			{
				return "{ns}";
			}
			return DataContract.GetDefaultStableNamespace(type.Namespace);
		}

		internal static XmlQualifiedName CreateQualifiedName(string localName, string ns)
		{
			return new XmlQualifiedName(localName, DataContract.GetNamespace(ns));
		}

		internal static string GetDefaultStableNamespace(string clrNs)
		{
			if (clrNs == null)
			{
				clrNs = string.Empty;
			}
			return new Uri(Globals.DataContractXsdBaseNamespaceUri, clrNs).AbsoluteUri;
		}

		private static void CheckExplicitDataContractNamespaceUri(string dataContractNs, Type type)
		{
			if (dataContractNs.Length > 0)
			{
				string text = dataContractNs.Trim();
				if (text.Length == 0 || text.IndexOf("##", StringComparison.Ordinal) != -1)
				{
					DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("DataContract namespace '{0}' is not a valid URI.", new object[] { dataContractNs }), type);
				}
				dataContractNs = text;
			}
			Uri uri;
			if (Uri.TryCreate(dataContractNs, UriKind.RelativeOrAbsolute, out uri))
			{
				if (uri.ToString() == "http://schemas.microsoft.com/2003/10/Serialization/")
				{
					DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("DataContract namespace '{0}' cannot be specified since it is reserved.", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/" }), type);
					return;
				}
			}
			else
			{
				DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("DataContract namespace '{0}' is not a valid URI.", new object[] { dataContractNs }), type);
			}
		}

		internal static string GetClrTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition || !type.ContainsGenericParameters)
			{
				return type.FullName;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", type.Namespace, type.Name);
		}

		internal static string GetClrAssemblyName(Type type, out bool hasTypeForwardedFrom)
		{
			hasTypeForwardedFrom = false;
			object[] customAttributes = type.GetCustomAttributes(typeof(TypeForwardedFromAttribute), false);
			if (customAttributes != null && customAttributes.Length != 0)
			{
				TypeForwardedFromAttribute typeForwardedFromAttribute = (TypeForwardedFromAttribute)customAttributes[0];
				hasTypeForwardedFrom = true;
				return typeForwardedFromAttribute.AssemblyFullName;
			}
			return type.Assembly.FullName;
		}

		internal static string GetClrTypeFullNameUsingTypeForwardedFromAttribute(Type type)
		{
			if (type.IsArray)
			{
				return DataContract.GetClrTypeFullNameForArray(type);
			}
			return DataContract.GetClrTypeFullNameForNonArrayTypes(type);
		}

		private static string GetClrTypeFullNameForArray(Type type)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", DataContract.GetClrTypeFullNameUsingTypeForwardedFromAttribute(type.GetElementType()), "[", "]");
		}

		private static string GetClrTypeFullNameForNonArrayTypes(Type type)
		{
			if (!type.IsGenericType)
			{
				return DataContract.GetClrTypeFullName(type);
			}
			Type[] genericArguments = type.GetGenericArguments();
			StringBuilder stringBuilder = new StringBuilder(type.GetGenericTypeDefinition().FullName).Append("[");
			foreach (Type type2 in genericArguments)
			{
				stringBuilder.Append("[").Append(DataContract.GetClrTypeFullNameUsingTypeForwardedFromAttribute(type2)).Append(",");
				bool flag;
				stringBuilder.Append(" ").Append(DataContract.GetClrAssemblyName(type2, out flag));
				stringBuilder.Append("]").Append(",");
			}
			return stringBuilder.Remove(stringBuilder.Length - 1, 1).Append("]").ToString();
		}

		internal static void GetClrNameAndNamespace(string fullTypeName, out string localName, out string ns)
		{
			int num = fullTypeName.LastIndexOf('.');
			if (num < 0)
			{
				ns = string.Empty;
				localName = fullTypeName.Replace('+', '.');
			}
			else
			{
				ns = fullTypeName.Substring(0, num);
				localName = fullTypeName.Substring(num + 1).Replace('+', '.');
			}
			int num2 = localName.IndexOf('[');
			if (num2 >= 0)
			{
				localName = localName.Substring(0, num2);
			}
		}

		internal static void GetDefaultStableName(string fullTypeName, out string localName, out string ns)
		{
			DataContract.GetDefaultStableName(new CodeTypeReference(fullTypeName), out localName, out ns);
		}

		private static void GetDefaultStableName(CodeTypeReference typeReference, out string localName, out string ns)
		{
			string baseType = typeReference.BaseType;
			DataContract builtInDataContract = DataContract.GetBuiltInDataContract(baseType);
			if (builtInDataContract != null)
			{
				localName = builtInDataContract.StableName.Name;
				ns = builtInDataContract.StableName.Namespace;
				return;
			}
			DataContract.GetClrNameAndNamespace(baseType, out localName, out ns);
			if (typeReference.TypeArguments.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				bool flag = true;
				IList<int> dataContractNameForGenericName = DataContract.GetDataContractNameForGenericName(localName, stringBuilder);
				foreach (object obj in typeReference.TypeArguments)
				{
					string text;
					string text2;
					DataContract.GetDefaultStableName((CodeTypeReference)obj, out text, out text2);
					stringBuilder.Append(text);
					stringBuilder2.Append(" ").Append(text2);
					if (flag)
					{
						flag = DataContract.IsBuiltInNamespace(text2);
					}
				}
				if (dataContractNameForGenericName.Count > 1 || !flag)
				{
					foreach (int num in dataContractNameForGenericName)
					{
						stringBuilder2.Insert(0, num).Insert(0, " ");
					}
					stringBuilder.Append(DataContract.GetNamespacesDigest(stringBuilder2.ToString()));
				}
				localName = stringBuilder.ToString();
			}
			localName = DataContract.EncodeLocalName(localName);
			ns = DataContract.GetDefaultStableNamespace(ns);
		}

		internal static string GetDataContractNamespaceFromUri(string uriString)
		{
			if (!uriString.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
			{
				return uriString;
			}
			return uriString.Substring("http://schemas.datacontract.org/2004/07/".Length);
		}

		private static string GetGlobalDataContractNamespace(string clrNs, ICustomAttributeProvider customAttribuetProvider)
		{
			object[] customAttributes = customAttribuetProvider.GetCustomAttributes(typeof(ContractNamespaceAttribute), false);
			string text = null;
			foreach (ContractNamespaceAttribute contractNamespaceAttribute in customAttributes)
			{
				string text2 = contractNamespaceAttribute.ClrNamespace;
				if (text2 == null)
				{
					text2 = string.Empty;
				}
				if (text2 == clrNs)
				{
					if (contractNamespaceAttribute.ContractNamespace == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("CLR namespace '{0}' cannot have ContractNamespace set to null.", new object[] { clrNs })));
					}
					if (text != null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("ContractNamespaceAttribute attribute maps CLR namespace '{2}' to multiple data contract namespaces '{0}' and '{1}'. You can map a CLR namespace to only one data contract namespace.", new object[] { text, contractNamespaceAttribute.ContractNamespace, clrNs })));
					}
					text = contractNamespaceAttribute.ContractNamespace;
				}
			}
			return text;
		}

		private static string GetNamespacesDigest(string namespaces)
		{
			byte[] array = HashHelper.ComputeHash(Encoding.UTF8.GetBytes(namespaces));
			char[] array2 = new char[24];
			int num = Convert.ToBase64CharArray(array, 0, 6, array2, 0);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < num; i++)
			{
				char c = array2[i];
				if (c != '+')
				{
					if (c != '/')
					{
						if (c != '=')
						{
							stringBuilder.Append(c);
						}
					}
					else
					{
						stringBuilder.Append("_S");
					}
				}
				else
				{
					stringBuilder.Append("_P");
				}
			}
			return stringBuilder.ToString();
		}

		private static string ExpandGenericParameters(string format, Type type)
		{
			GenericNameProvider genericNameProvider = new GenericNameProvider(type);
			return DataContract.ExpandGenericParameters(format, genericNameProvider);
		}

		internal static string ExpandGenericParameters(string format, IGenericNameProvider genericNameProvider)
		{
			string text = null;
			StringBuilder stringBuilder = new StringBuilder();
			IList<int> nestedParameterCounts = genericNameProvider.GetNestedParameterCounts();
			for (int i = 0; i < format.Length; i++)
			{
				char c = format[i];
				if (c == '{')
				{
					i++;
					int num = i;
					while (i < format.Length && format[i] != '}')
					{
						i++;
					}
					if (i == format.Length)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("The data contract name '{0}' for type '{1}' has a curly brace '{{' that is not matched with a closing curly brace. Curly braces have special meaning in data contract names - they are used to customize the naming of data contracts for generic types.", new object[]
						{
							format,
							genericNameProvider.GetGenericTypeName()
						})));
					}
					if (format[num] == '#' && i == num + 1)
					{
						if (nestedParameterCounts.Count > 1 || !genericNameProvider.ParametersFromBuiltInNamespaces)
						{
							if (text == null)
							{
								StringBuilder stringBuilder2 = new StringBuilder(genericNameProvider.GetNamespaces());
								foreach (int num2 in nestedParameterCounts)
								{
									stringBuilder2.Insert(0, num2).Insert(0, " ");
								}
								text = DataContract.GetNamespacesDigest(stringBuilder2.ToString());
							}
							stringBuilder.Append(text);
						}
					}
					else
					{
						int num3;
						if (!int.TryParse(format.Substring(num, i - num), out num3) || num3 < 0 || num3 >= genericNameProvider.GetParameterCount())
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("In the data contract name for type '{1}', there are curly braces with '{0}' inside, which is an invalid value. Curly braces have special meaning in data contract names - they are used to customize the naming of data contracts for generic types. Based on the number of generic parameters this type has, the contents of the curly braces must either be a number between 0 and '{2}' to insert the name of the generic parameter at that index or the '#' symbol to insert a digest of the generic parameter namespaces.", new object[]
							{
								format.Substring(num, i - num),
								genericNameProvider.GetGenericTypeName(),
								genericNameProvider.GetParameterCount() - 1
							})));
						}
						stringBuilder.Append(genericNameProvider.GetParameterName(num3));
					}
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		internal static bool IsTypeNullable(Type type)
		{
			return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == Globals.TypeOfNullable);
		}

		public static void ThrowTypeNotSerializable(Type type)
		{
			DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be serialized. Consider marking it with the DataContractAttribute attribute, and marking all of its members you want serialized with the DataMemberAttribute attribute. Alternatively, you can ensure that the type is public and has a parameterless constructor - all public members of the type will then be serialized, and no attributes will be required.", new object[] { type }), type);
		}

		private static DataContractSerializerSection ConfigSection
		{
			[SecurityCritical]
			get
			{
				if (DataContract.configSection == null)
				{
					DataContract.configSection = DataContractSerializerSection.UnsafeGetSection();
				}
				return DataContract.configSection;
			}
		}

		internal static Dictionary<XmlQualifiedName, DataContract> ImportKnownTypeAttributes(Type type)
		{
			Dictionary<XmlQualifiedName, DataContract> dictionary = null;
			Dictionary<Type, Type> dictionary2 = new Dictionary<Type, Type>();
			DataContract.ImportKnownTypeAttributes(type, dictionary2, ref dictionary);
			return dictionary;
		}

		private static void ImportKnownTypeAttributes(Type type, Dictionary<Type, Type> typesChecked, ref Dictionary<XmlQualifiedName, DataContract> knownDataContracts)
		{
			if (TD.ImportKnownTypesStartIsEnabled())
			{
				TD.ImportKnownTypesStart();
			}
			while (type != null && DataContract.IsTypeSerializable(type))
			{
				if (typesChecked.ContainsKey(type))
				{
					return;
				}
				typesChecked.Add(type, type);
				object[] customAttributes = type.GetCustomAttributes(Globals.TypeOfKnownTypeAttribute, false);
				if (customAttributes != null)
				{
					bool flag = false;
					bool flag2 = false;
					foreach (KnownTypeAttribute knownTypeAttribute in customAttributes)
					{
						if (knownTypeAttribute.Type != null)
						{
							if (flag)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}': If a KnownTypeAttribute attribute specifies a method it must be the only KnownTypeAttribute attribute on that type.", new object[] { DataContract.GetClrTypeFullName(type) }), type);
							}
							DataContract.CheckAndAdd(knownTypeAttribute.Type, typesChecked, ref knownDataContracts);
							flag2 = true;
						}
						else
						{
							if (flag || flag2)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}': If a KnownTypeAttribute attribute specifies a method it must be the only KnownTypeAttribute attribute on that type.", new object[] { DataContract.GetClrTypeFullName(type) }), type);
							}
							string methodName = knownTypeAttribute.MethodName;
							if (methodName == null)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("KnownTypeAttribute attribute on type '{0}' contains no data.", new object[] { DataContract.GetClrTypeFullName(type) }), type);
							}
							if (methodName.Length == 0)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Method name specified by KnownTypeAttribute attribute on type '{0}' cannot be the empty string.", new object[] { DataContract.GetClrTypeFullName(type) }), type);
							}
							MethodInfo method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
							if (method == null)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("KnownTypeAttribute attribute on type '{1}' specifies a method named '{0}' to provide known types. Static method '{0}()' was not found on this type. Ensure that the method exists and is marked as static.", new object[]
								{
									methodName,
									DataContract.GetClrTypeFullName(type)
								}), type);
							}
							if (!Globals.TypeOfTypeEnumerable.IsAssignableFrom(method.ReturnType))
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("KnownTypeAttribute attribute on type '{0}' specifies a method named '{1}' to provide known types. The return type of this method is invalid because it is not assignable to IEnumerable<Type>. Ensure that the method exists and has a valid signature.", new object[]
								{
									DataContract.GetClrTypeFullName(type),
									methodName
								}), type);
							}
							object obj = method.Invoke(null, Globals.EmptyObjectArray);
							if (obj == null)
							{
								DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Method specified by KnownTypeAttribute attribute on type '{0}' returned null.", new object[] { DataContract.GetClrTypeFullName(type) }), type);
							}
							foreach (Type type2 in ((IEnumerable<Type>)obj))
							{
								if (type2 == null)
								{
									DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Method specified by KnownTypeAttribute attribute on type '{0}' does not expose valid types.", new object[] { DataContract.GetClrTypeFullName(type) }), type);
								}
								DataContract.CheckAndAdd(type2, typesChecked, ref knownDataContracts);
							}
							flag = true;
						}
					}
				}
				DataContract.LoadKnownTypesFromConfig(type, typesChecked, ref knownDataContracts);
				type = type.BaseType;
			}
			if (TD.ImportKnownTypesStopIsEnabled())
			{
				TD.ImportKnownTypesStop();
			}
		}

		[SecuritySafeCritical]
		private static void LoadKnownTypesFromConfig(Type type, Dictionary<Type, Type> typesChecked, ref Dictionary<XmlQualifiedName, DataContract> knownDataContracts)
		{
			if (DataContract.ConfigSection != null)
			{
				DeclaredTypeElementCollection declaredTypes = DataContract.ConfigSection.DeclaredTypes;
				Type type2 = type;
				Type[] array = null;
				DataContract.CheckRootTypeInConfigIsGeneric(type, ref type2, ref array);
				DeclaredTypeElement declaredTypeElement = declaredTypes[type2.AssemblyQualifiedName];
				if (declaredTypeElement != null && DataContract.IsElemTypeNullOrNotEqualToRootType(declaredTypeElement.Type, type2))
				{
					declaredTypeElement = null;
				}
				if (declaredTypeElement == null)
				{
					for (int i = 0; i < declaredTypes.Count; i++)
					{
						if (DataContract.IsCollectionElementTypeEqualToRootType(declaredTypes[i].Type, type2))
						{
							declaredTypeElement = declaredTypes[i];
							break;
						}
					}
				}
				if (declaredTypeElement != null)
				{
					for (int j = 0; j < declaredTypeElement.KnownTypes.Count; j++)
					{
						Type type3 = declaredTypeElement.KnownTypes[j].GetType(declaredTypeElement.Type, array);
						if (type3 != null)
						{
							DataContract.CheckAndAdd(type3, typesChecked, ref knownDataContracts);
						}
					}
				}
			}
		}

		private static void CheckRootTypeInConfigIsGeneric(Type type, ref Type rootType, ref Type[] genArgs)
		{
			if (rootType.IsGenericType)
			{
				if (!rootType.ContainsGenericParameters)
				{
					genArgs = rootType.GetGenericArguments();
					rootType = rootType.GetGenericTypeDefinition();
					return;
				}
				DataContract.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Error while getting known types for Type '{0}'. The type must not be an open or partial generic class.", new object[] { type }), type);
			}
		}

		private static bool IsElemTypeNullOrNotEqualToRootType(string elemTypeName, Type rootType)
		{
			Type type = Type.GetType(elemTypeName, false);
			return type == null || !rootType.Equals(type);
		}

		private static bool IsCollectionElementTypeEqualToRootType(string collectionElementTypeName, Type rootType)
		{
			if (collectionElementTypeName.StartsWith(DataContract.GetClrTypeFullName(rootType), StringComparison.Ordinal))
			{
				Type type = Type.GetType(collectionElementTypeName, false);
				if (type != null)
				{
					if (type.IsGenericType && !DataContract.IsOpenGenericType(type))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Declared type '{0}' in config cannot be a closed or partial generic type.", new object[] { collectionElementTypeName })));
					}
					if (rootType.Equals(type))
					{
						return true;
					}
				}
			}
			return false;
		}

		[SecurityCritical]
		[SecurityTreatAsSafe]
		internal static void CheckAndAdd(Type type, Dictionary<Type, Type> typesChecked, ref Dictionary<XmlQualifiedName, DataContract> nameToDataContractTable)
		{
			type = DataContract.UnwrapNullableType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContract dataContract2;
			if (nameToDataContractTable == null)
			{
				nameToDataContractTable = new Dictionary<XmlQualifiedName, DataContract>();
			}
			else if (nameToDataContractTable.TryGetValue(dataContract.StableName, out dataContract2))
			{
				if (dataContract2.UnderlyingType != DataContract.DataContractCriticalHelper.GetDataContractAdapterType(type))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be added to list of known types since another type '{1}' with the same data contract name '{2}:{3}' is already present.", new object[]
					{
						type,
						dataContract2.UnderlyingType,
						dataContract.StableName.Namespace,
						dataContract.StableName.Name
					})));
				}
				return;
			}
			nameToDataContractTable.Add(dataContract.StableName, dataContract);
			DataContract.ImportKnownTypeAttributes(type, typesChecked, ref nameToDataContractTable);
		}

		private static bool IsOpenGenericType(Type t)
		{
			Type[] genericArguments = t.GetGenericArguments();
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (!genericArguments[i].IsGenericParameter)
				{
					return false;
				}
			}
			return true;
		}

		public sealed override bool Equals(object other)
		{
			return this == other || this.Equals(other, new Dictionary<DataContractPairKey, object>());
		}

		internal virtual bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			DataContract dataContract = other as DataContract;
			return dataContract != null && (this.StableName.Name == dataContract.StableName.Name && this.StableName.Namespace == dataContract.StableName.Namespace) && this.IsReference == dataContract.IsReference;
		}

		internal bool IsEqualOrChecked(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (this == other)
			{
				return true;
			}
			if (checkedContracts != null)
			{
				DataContractPairKey dataContractPairKey = new DataContractPairKey(this, other);
				if (checkedContracts.ContainsKey(dataContractPairKey))
				{
					return true;
				}
				checkedContracts.Add(dataContractPairKey, null);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal void ThrowInvalidDataContractException(string message)
		{
			DataContract.ThrowInvalidDataContractException(message, this.UnderlyingType);
		}

		internal static bool IsTypeVisible(Type t)
		{
			return true;
		}

		[SecurityCritical]
		private XmlDictionaryString name;

		[SecurityCritical]
		private XmlDictionaryString ns;

		[SecurityCritical]
		private DataContract.DataContractCriticalHelper helper;

		[SecurityCritical]
		private static DataContractSerializerSection configSection;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		protected class DataContractCriticalHelper
		{
			internal static DataContract GetDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
			{
				DataContract dataContract = DataContract.DataContractCriticalHelper.dataContractCache[id];
				if (dataContract == null)
				{
					return DataContract.DataContractCriticalHelper.CreateDataContract(id, typeHandle, type);
				}
				return dataContract.GetValidContract();
			}

			internal static DataContract GetGetOnlyCollectionDataContractSkipValidation(int id, RuntimeTypeHandle typeHandle, Type type)
			{
				DataContract dataContract = DataContract.DataContractCriticalHelper.dataContractCache[id];
				if (dataContract == null)
				{
					dataContract = DataContract.DataContractCriticalHelper.CreateGetOnlyCollectionDataContract(id, typeHandle, type);
					DataContract.DataContractCriticalHelper.AssignDataContractToId(dataContract, id);
				}
				return dataContract;
			}

			internal static DataContract GetDataContractForInitialization(int id)
			{
				DataContract dataContract = DataContract.DataContractCriticalHelper.dataContractCache[id];
				if (dataContract == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. DataContract cache overflow.")));
				}
				return dataContract;
			}

			internal static int GetIdForInitialization(ClassDataContract classContract)
			{
				int id = DataContract.GetId(classContract.TypeForInitialization.TypeHandle);
				if (id < DataContract.DataContractCriticalHelper.dataContractCache.Length && DataContract.DataContractCriticalHelper.ContractMatches(classContract, DataContract.DataContractCriticalHelper.dataContractCache[id]))
				{
					return id;
				}
				int num = DataContract.DataContractCriticalHelper.dataContractID;
				for (int i = 0; i < num; i++)
				{
					if (DataContract.DataContractCriticalHelper.ContractMatches(classContract, DataContract.DataContractCriticalHelper.dataContractCache[i]))
					{
						return i;
					}
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. DataContract cache overflow.")));
			}

			private static bool ContractMatches(DataContract contract, DataContract cachedContract)
			{
				return cachedContract != null && cachedContract.UnderlyingType == contract.UnderlyingType;
			}

			internal static int GetId(RuntimeTypeHandle typeHandle)
			{
				object obj = DataContract.DataContractCriticalHelper.cacheLock;
				int value;
				lock (obj)
				{
					typeHandle = DataContract.DataContractCriticalHelper.GetDataContractAdapterTypeHandle(typeHandle);
					DataContract.DataContractCriticalHelper.typeHandleRef.Value = typeHandle;
					IntRef nextId;
					if (!DataContract.DataContractCriticalHelper.typeToIDCache.TryGetValue(DataContract.DataContractCriticalHelper.typeHandleRef, out nextId))
					{
						nextId = DataContract.DataContractCriticalHelper.GetNextId();
						try
						{
							DataContract.DataContractCriticalHelper.typeToIDCache.Add(new TypeHandleRef(typeHandle), nextId);
						}
						catch (Exception ex)
						{
							if (Fx.IsFatal(ex))
							{
								throw;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
						}
					}
					value = nextId.Value;
				}
				return value;
			}

			private static IntRef GetNextId()
			{
				int num = DataContract.DataContractCriticalHelper.dataContractID++;
				if (num >= DataContract.DataContractCriticalHelper.dataContractCache.Length)
				{
					int num2 = ((num < 1073741823) ? (num * 2) : int.MaxValue);
					if (num2 <= num)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(global::System.Runtime.Serialization.SR.GetString("An internal error has occurred. DataContract cache overflow.")));
					}
					Array.Resize<DataContract>(ref DataContract.DataContractCriticalHelper.dataContractCache, num2);
				}
				return new IntRef(num);
			}

			private static DataContract CreateDataContract(int id, RuntimeTypeHandle typeHandle, Type type)
			{
				DataContract dataContract = DataContract.DataContractCriticalHelper.dataContractCache[id];
				if (dataContract == null)
				{
					object obj = DataContract.DataContractCriticalHelper.createDataContractLock;
					lock (obj)
					{
						dataContract = DataContract.DataContractCriticalHelper.dataContractCache[id];
						if (dataContract == null)
						{
							if (type == null)
							{
								type = Type.GetTypeFromHandle(typeHandle);
							}
							type = DataContract.UnwrapNullableType(type);
							type = DataContract.DataContractCriticalHelper.GetDataContractAdapterType(type);
							dataContract = DataContract.DataContractCriticalHelper.GetBuiltInDataContract(type);
							if (dataContract == null)
							{
								if (type.IsArray)
								{
									dataContract = new CollectionDataContract(type);
								}
								else if (type.IsEnum)
								{
									dataContract = new EnumDataContract(type);
								}
								else if (type.IsGenericParameter)
								{
									dataContract = new GenericParameterDataContract(type);
								}
								else if (Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
								{
									dataContract = new XmlDataContract(type);
								}
								else
								{
									if (type.IsPointer)
									{
										type = Globals.TypeOfReflectionPointer;
									}
									if (!CollectionDataContract.TryCreate(type, out dataContract))
									{
										if (type.IsSerializable || type.IsDefined(Globals.TypeOfDataContractAttribute, false) || ClassDataContract.IsNonAttributedTypeValidForSerialization(type))
										{
											dataContract = new ClassDataContract(type);
										}
										else
										{
											DataContract.DataContractCriticalHelper.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be serialized. Consider marking it with the DataContractAttribute attribute, and marking all of its members you want serialized with the DataMemberAttribute attribute. Alternatively, you can ensure that the type is public and has a parameterless constructor - all public members of the type will then be serialized, and no attributes will be required.", new object[] { type }), type);
										}
									}
								}
							}
							DataContract.DataContractCriticalHelper.AssignDataContractToId(dataContract, id);
						}
					}
				}
				return dataContract;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			private static void AssignDataContractToId(DataContract dataContract, int id)
			{
				object obj = DataContract.DataContractCriticalHelper.cacheLock;
				lock (obj)
				{
					DataContract.DataContractCriticalHelper.dataContractCache[id] = dataContract;
				}
			}

			private static DataContract CreateGetOnlyCollectionDataContract(int id, RuntimeTypeHandle typeHandle, Type type)
			{
				DataContract dataContract = null;
				object obj = DataContract.DataContractCriticalHelper.createDataContractLock;
				lock (obj)
				{
					dataContract = DataContract.DataContractCriticalHelper.dataContractCache[id];
					if (dataContract == null)
					{
						if (type == null)
						{
							type = Type.GetTypeFromHandle(typeHandle);
						}
						type = DataContract.UnwrapNullableType(type);
						type = DataContract.DataContractCriticalHelper.GetDataContractAdapterType(type);
						if (!CollectionDataContract.TryCreateGetOnlyCollectionDataContract(type, out dataContract))
						{
							DataContract.DataContractCriticalHelper.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be serialized. Consider marking it with the DataContractAttribute attribute, and marking all of its members you want serialized with the DataMemberAttribute attribute. Alternatively, you can ensure that the type is public and has a parameterless constructor - all public members of the type will then be serialized, and no attributes will be required.", new object[] { type }), type);
						}
					}
				}
				return dataContract;
			}

			internal static Type GetDataContractAdapterType(Type type)
			{
				if (type == Globals.TypeOfDateTimeOffset)
				{
					return Globals.TypeOfDateTimeOffsetAdapter;
				}
				return type;
			}

			internal static Type GetDataContractOriginalType(Type type)
			{
				if (type == Globals.TypeOfDateTimeOffsetAdapter)
				{
					return Globals.TypeOfDateTimeOffset;
				}
				return type;
			}

			private static RuntimeTypeHandle GetDataContractAdapterTypeHandle(RuntimeTypeHandle typeHandle)
			{
				if (Globals.TypeOfDateTimeOffset.TypeHandle.Equals(typeHandle))
				{
					return Globals.TypeOfDateTimeOffsetAdapter.TypeHandle;
				}
				return typeHandle;
			}

			public static DataContract GetBuiltInDataContract(Type type)
			{
				if (type.IsInterface && !CollectionDataContract.IsCollectionInterface(type))
				{
					type = Globals.TypeOfObject;
				}
				object obj = DataContract.DataContractCriticalHelper.initBuiltInContractsLock;
				DataContract dataContract2;
				lock (obj)
				{
					if (DataContract.DataContractCriticalHelper.typeToBuiltInContract == null)
					{
						DataContract.DataContractCriticalHelper.typeToBuiltInContract = new Dictionary<Type, DataContract>();
					}
					DataContract dataContract = null;
					if (!DataContract.DataContractCriticalHelper.typeToBuiltInContract.TryGetValue(type, out dataContract))
					{
						DataContract.DataContractCriticalHelper.TryCreateBuiltInDataContract(type, out dataContract);
						DataContract.DataContractCriticalHelper.typeToBuiltInContract.Add(type, dataContract);
					}
					dataContract2 = dataContract;
				}
				return dataContract2;
			}

			public static DataContract GetBuiltInDataContract(string name, string ns)
			{
				object obj = DataContract.DataContractCriticalHelper.initBuiltInContractsLock;
				DataContract dataContract2;
				lock (obj)
				{
					if (DataContract.DataContractCriticalHelper.nameToBuiltInContract == null)
					{
						DataContract.DataContractCriticalHelper.nameToBuiltInContract = new Dictionary<XmlQualifiedName, DataContract>();
					}
					DataContract dataContract = null;
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(name, ns);
					if (!DataContract.DataContractCriticalHelper.nameToBuiltInContract.TryGetValue(xmlQualifiedName, out dataContract) && DataContract.DataContractCriticalHelper.TryCreateBuiltInDataContract(name, ns, out dataContract))
					{
						DataContract.DataContractCriticalHelper.nameToBuiltInContract.Add(xmlQualifiedName, dataContract);
					}
					dataContract2 = dataContract;
				}
				return dataContract2;
			}

			public static DataContract GetBuiltInDataContract(string typeName)
			{
				if (!typeName.StartsWith("System.", StringComparison.Ordinal))
				{
					return null;
				}
				object obj = DataContract.DataContractCriticalHelper.initBuiltInContractsLock;
				DataContract dataContract2;
				lock (obj)
				{
					if (DataContract.DataContractCriticalHelper.typeNameToBuiltInContract == null)
					{
						DataContract.DataContractCriticalHelper.typeNameToBuiltInContract = new Dictionary<string, DataContract>();
					}
					DataContract dataContract = null;
					if (!DataContract.DataContractCriticalHelper.typeNameToBuiltInContract.TryGetValue(typeName, out dataContract))
					{
						Type type = null;
						string text = typeName.Substring(7);
						if (text == "Char")
						{
							type = typeof(char);
						}
						else if (text == "Boolean")
						{
							type = typeof(bool);
						}
						else if (text == "SByte")
						{
							type = typeof(sbyte);
						}
						else if (text == "Byte")
						{
							type = typeof(byte);
						}
						else if (text == "Int16")
						{
							type = typeof(short);
						}
						else if (text == "UInt16")
						{
							type = typeof(ushort);
						}
						else if (text == "Int32")
						{
							type = typeof(int);
						}
						else if (text == "UInt32")
						{
							type = typeof(uint);
						}
						else if (text == "Int64")
						{
							type = typeof(long);
						}
						else if (text == "UInt64")
						{
							type = typeof(ulong);
						}
						else if (text == "Single")
						{
							type = typeof(float);
						}
						else if (text == "Double")
						{
							type = typeof(double);
						}
						else if (text == "Decimal")
						{
							type = typeof(decimal);
						}
						else if (text == "DateTime")
						{
							type = typeof(DateTime);
						}
						else if (text == "String")
						{
							type = typeof(string);
						}
						else if (text == "Byte[]")
						{
							type = typeof(byte[]);
						}
						else if (text == "Object")
						{
							type = typeof(object);
						}
						else if (text == "TimeSpan")
						{
							type = typeof(TimeSpan);
						}
						else if (text == "Guid")
						{
							type = typeof(Guid);
						}
						else if (text == "Uri")
						{
							type = typeof(Uri);
						}
						else if (text == "Xml.XmlQualifiedName")
						{
							type = typeof(XmlQualifiedName);
						}
						else if (text == "Enum")
						{
							type = typeof(Enum);
						}
						else if (text == "ValueType")
						{
							type = typeof(ValueType);
						}
						else if (text == "Array")
						{
							type = typeof(Array);
						}
						else if (text == "Xml.XmlElement")
						{
							type = typeof(XmlElement);
						}
						else if (text == "Xml.XmlNode[]")
						{
							type = typeof(XmlNode[]);
						}
						if (type != null)
						{
							DataContract.DataContractCriticalHelper.TryCreateBuiltInDataContract(type, out dataContract);
						}
						DataContract.DataContractCriticalHelper.typeNameToBuiltInContract.Add(typeName, dataContract);
					}
					dataContract2 = dataContract;
				}
				return dataContract2;
			}

			public static bool TryCreateBuiltInDataContract(Type type, out DataContract dataContract)
			{
				if (type.IsEnum)
				{
					dataContract = null;
					return false;
				}
				dataContract = null;
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					dataContract = new BooleanDataContract();
					goto IL_024C;
				case TypeCode.Char:
					dataContract = new CharDataContract();
					goto IL_024C;
				case TypeCode.SByte:
					dataContract = new SignedByteDataContract();
					goto IL_024C;
				case TypeCode.Byte:
					dataContract = new UnsignedByteDataContract();
					goto IL_024C;
				case TypeCode.Int16:
					dataContract = new ShortDataContract();
					goto IL_024C;
				case TypeCode.UInt16:
					dataContract = new UnsignedShortDataContract();
					goto IL_024C;
				case TypeCode.Int32:
					dataContract = new IntDataContract();
					goto IL_024C;
				case TypeCode.UInt32:
					dataContract = new UnsignedIntDataContract();
					goto IL_024C;
				case TypeCode.Int64:
					dataContract = new LongDataContract();
					goto IL_024C;
				case TypeCode.UInt64:
					dataContract = new UnsignedLongDataContract();
					goto IL_024C;
				case TypeCode.Single:
					dataContract = new FloatDataContract();
					goto IL_024C;
				case TypeCode.Double:
					dataContract = new DoubleDataContract();
					goto IL_024C;
				case TypeCode.Decimal:
					dataContract = new DecimalDataContract();
					goto IL_024C;
				case TypeCode.DateTime:
					dataContract = new DateTimeDataContract();
					goto IL_024C;
				case TypeCode.String:
					dataContract = new StringDataContract();
					goto IL_024C;
				}
				if (type == typeof(byte[]))
				{
					dataContract = new ByteArrayDataContract();
				}
				else if (type == typeof(object))
				{
					dataContract = new ObjectDataContract();
				}
				else if (type == typeof(Uri))
				{
					dataContract = new UriDataContract();
				}
				else if (type == typeof(XmlQualifiedName))
				{
					dataContract = new QNameDataContract();
				}
				else if (type == typeof(TimeSpan))
				{
					dataContract = new TimeSpanDataContract();
				}
				else if (type == typeof(Guid))
				{
					dataContract = new GuidDataContract();
				}
				else if (type == typeof(Enum) || type == typeof(ValueType))
				{
					dataContract = new SpecialTypeDataContract(type, DictionaryGlobals.ObjectLocalName, DictionaryGlobals.SchemaNamespace);
				}
				else if (type == typeof(Array))
				{
					dataContract = new CollectionDataContract(type);
				}
				else if (type == typeof(XmlElement) || type == typeof(XmlNode[]))
				{
					dataContract = new XmlDataContract(type);
				}
				IL_024C:
				return dataContract != null;
			}

			public static bool TryCreateBuiltInDataContract(string name, string ns, out DataContract dataContract)
			{
				dataContract = null;
				if (ns == DictionaryGlobals.SchemaNamespace.Value)
				{
					if (DictionaryGlobals.BooleanLocalName.Value == name)
					{
						dataContract = new BooleanDataContract();
					}
					else if (DictionaryGlobals.SignedByteLocalName.Value == name)
					{
						dataContract = new SignedByteDataContract();
					}
					else if (DictionaryGlobals.UnsignedByteLocalName.Value == name)
					{
						dataContract = new UnsignedByteDataContract();
					}
					else if (DictionaryGlobals.ShortLocalName.Value == name)
					{
						dataContract = new ShortDataContract();
					}
					else if (DictionaryGlobals.UnsignedShortLocalName.Value == name)
					{
						dataContract = new UnsignedShortDataContract();
					}
					else if (DictionaryGlobals.IntLocalName.Value == name)
					{
						dataContract = new IntDataContract();
					}
					else if (DictionaryGlobals.UnsignedIntLocalName.Value == name)
					{
						dataContract = new UnsignedIntDataContract();
					}
					else if (DictionaryGlobals.LongLocalName.Value == name)
					{
						dataContract = new LongDataContract();
					}
					else if (DictionaryGlobals.integerLocalName.Value == name)
					{
						dataContract = new IntegerDataContract();
					}
					else if (DictionaryGlobals.positiveIntegerLocalName.Value == name)
					{
						dataContract = new PositiveIntegerDataContract();
					}
					else if (DictionaryGlobals.negativeIntegerLocalName.Value == name)
					{
						dataContract = new NegativeIntegerDataContract();
					}
					else if (DictionaryGlobals.nonPositiveIntegerLocalName.Value == name)
					{
						dataContract = new NonPositiveIntegerDataContract();
					}
					else if (DictionaryGlobals.nonNegativeIntegerLocalName.Value == name)
					{
						dataContract = new NonNegativeIntegerDataContract();
					}
					else if (DictionaryGlobals.UnsignedLongLocalName.Value == name)
					{
						dataContract = new UnsignedLongDataContract();
					}
					else if (DictionaryGlobals.FloatLocalName.Value == name)
					{
						dataContract = new FloatDataContract();
					}
					else if (DictionaryGlobals.DoubleLocalName.Value == name)
					{
						dataContract = new DoubleDataContract();
					}
					else if (DictionaryGlobals.DecimalLocalName.Value == name)
					{
						dataContract = new DecimalDataContract();
					}
					else if (DictionaryGlobals.DateTimeLocalName.Value == name)
					{
						dataContract = new DateTimeDataContract();
					}
					else if (DictionaryGlobals.StringLocalName.Value == name)
					{
						dataContract = new StringDataContract();
					}
					else if (DictionaryGlobals.timeLocalName.Value == name)
					{
						dataContract = new TimeDataContract();
					}
					else if (DictionaryGlobals.dateLocalName.Value == name)
					{
						dataContract = new DateDataContract();
					}
					else if (DictionaryGlobals.hexBinaryLocalName.Value == name)
					{
						dataContract = new HexBinaryDataContract();
					}
					else if (DictionaryGlobals.gYearMonthLocalName.Value == name)
					{
						dataContract = new GYearMonthDataContract();
					}
					else if (DictionaryGlobals.gYearLocalName.Value == name)
					{
						dataContract = new GYearDataContract();
					}
					else if (DictionaryGlobals.gMonthDayLocalName.Value == name)
					{
						dataContract = new GMonthDayDataContract();
					}
					else if (DictionaryGlobals.gDayLocalName.Value == name)
					{
						dataContract = new GDayDataContract();
					}
					else if (DictionaryGlobals.gMonthLocalName.Value == name)
					{
						dataContract = new GMonthDataContract();
					}
					else if (DictionaryGlobals.normalizedStringLocalName.Value == name)
					{
						dataContract = new NormalizedStringDataContract();
					}
					else if (DictionaryGlobals.tokenLocalName.Value == name)
					{
						dataContract = new TokenDataContract();
					}
					else if (DictionaryGlobals.languageLocalName.Value == name)
					{
						dataContract = new LanguageDataContract();
					}
					else if (DictionaryGlobals.NameLocalName.Value == name)
					{
						dataContract = new NameDataContract();
					}
					else if (DictionaryGlobals.NCNameLocalName.Value == name)
					{
						dataContract = new NCNameDataContract();
					}
					else if (DictionaryGlobals.XSDIDLocalName.Value == name)
					{
						dataContract = new IDDataContract();
					}
					else if (DictionaryGlobals.IDREFLocalName.Value == name)
					{
						dataContract = new IDREFDataContract();
					}
					else if (DictionaryGlobals.IDREFSLocalName.Value == name)
					{
						dataContract = new IDREFSDataContract();
					}
					else if (DictionaryGlobals.ENTITYLocalName.Value == name)
					{
						dataContract = new ENTITYDataContract();
					}
					else if (DictionaryGlobals.ENTITIESLocalName.Value == name)
					{
						dataContract = new ENTITIESDataContract();
					}
					else if (DictionaryGlobals.NMTOKENLocalName.Value == name)
					{
						dataContract = new NMTOKENDataContract();
					}
					else if (DictionaryGlobals.NMTOKENSLocalName.Value == name)
					{
						dataContract = new NMTOKENDataContract();
					}
					else if (DictionaryGlobals.ByteArrayLocalName.Value == name)
					{
						dataContract = new ByteArrayDataContract();
					}
					else if (DictionaryGlobals.ObjectLocalName.Value == name)
					{
						dataContract = new ObjectDataContract();
					}
					else if (DictionaryGlobals.TimeSpanLocalName.Value == name)
					{
						dataContract = new XsDurationDataContract();
					}
					else if (DictionaryGlobals.UriLocalName.Value == name)
					{
						dataContract = new UriDataContract();
					}
					else if (DictionaryGlobals.QNameLocalName.Value == name)
					{
						dataContract = new QNameDataContract();
					}
				}
				else if (ns == DictionaryGlobals.SerializationNamespace.Value)
				{
					if (DictionaryGlobals.TimeSpanLocalName.Value == name)
					{
						dataContract = new TimeSpanDataContract();
					}
					else if (DictionaryGlobals.GuidLocalName.Value == name)
					{
						dataContract = new GuidDataContract();
					}
					else if (DictionaryGlobals.CharLocalName.Value == name)
					{
						dataContract = new CharDataContract();
					}
					else if ("ArrayOfanyType" == name)
					{
						dataContract = new CollectionDataContract(typeof(Array));
					}
				}
				else if (ns == DictionaryGlobals.AsmxTypesNamespace.Value)
				{
					if (DictionaryGlobals.CharLocalName.Value == name)
					{
						dataContract = new AsmxCharDataContract();
					}
					else if (DictionaryGlobals.GuidLocalName.Value == name)
					{
						dataContract = new AsmxGuidDataContract();
					}
				}
				else if (ns == "http://schemas.datacontract.org/2004/07/System.Xml")
				{
					if (name == "XmlElement")
					{
						dataContract = new XmlDataContract(typeof(XmlElement));
					}
					else if (name == "ArrayOfXmlNode")
					{
						dataContract = new XmlDataContract(typeof(XmlNode[]));
					}
				}
				return dataContract != null;
			}

			internal static string GetNamespace(string key)
			{
				object obj = DataContract.DataContractCriticalHelper.namespacesLock;
				string text2;
				lock (obj)
				{
					if (DataContract.DataContractCriticalHelper.namespaces == null)
					{
						DataContract.DataContractCriticalHelper.namespaces = new Dictionary<string, string>();
					}
					string text;
					if (DataContract.DataContractCriticalHelper.namespaces.TryGetValue(key, out text))
					{
						text2 = text;
					}
					else
					{
						try
						{
							DataContract.DataContractCriticalHelper.namespaces.Add(key, key);
						}
						catch (Exception ex)
						{
							if (Fx.IsFatal(ex))
							{
								throw;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
						}
						text2 = key;
					}
				}
				return text2;
			}

			internal static XmlDictionaryString GetClrTypeString(string key)
			{
				object obj = DataContract.DataContractCriticalHelper.clrTypeStringsLock;
				XmlDictionaryString xmlDictionaryString2;
				lock (obj)
				{
					if (DataContract.DataContractCriticalHelper.clrTypeStrings == null)
					{
						DataContract.DataContractCriticalHelper.clrTypeStringsDictionary = new XmlDictionary();
						DataContract.DataContractCriticalHelper.clrTypeStrings = new Dictionary<string, XmlDictionaryString>();
						try
						{
							DataContract.DataContractCriticalHelper.clrTypeStrings.Add(Globals.TypeOfInt.Assembly.FullName, DataContract.DataContractCriticalHelper.clrTypeStringsDictionary.Add("0"));
						}
						catch (Exception ex)
						{
							if (Fx.IsFatal(ex))
							{
								throw;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
						}
					}
					XmlDictionaryString xmlDictionaryString;
					if (DataContract.DataContractCriticalHelper.clrTypeStrings.TryGetValue(key, out xmlDictionaryString))
					{
						xmlDictionaryString2 = xmlDictionaryString;
					}
					else
					{
						xmlDictionaryString = DataContract.DataContractCriticalHelper.clrTypeStringsDictionary.Add(key);
						try
						{
							DataContract.DataContractCriticalHelper.clrTypeStrings.Add(key, xmlDictionaryString);
						}
						catch (Exception ex2)
						{
							if (Fx.IsFatal(ex2))
							{
								throw;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex2.Message, ex2);
						}
						xmlDictionaryString2 = xmlDictionaryString;
					}
				}
				return xmlDictionaryString2;
			}

			internal static void ThrowInvalidDataContractException(string message, Type type)
			{
				if (type != null)
				{
					object obj = DataContract.DataContractCriticalHelper.cacheLock;
					lock (obj)
					{
						DataContract.DataContractCriticalHelper.typeHandleRef.Value = DataContract.DataContractCriticalHelper.GetDataContractAdapterTypeHandle(type.TypeHandle);
						try
						{
							DataContract.DataContractCriticalHelper.typeToIDCache.Remove(DataContract.DataContractCriticalHelper.typeHandleRef);
						}
						catch (Exception ex)
						{
							if (Fx.IsFatal(ex))
							{
								throw;
							}
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
						}
					}
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(message));
			}

			internal DataContractCriticalHelper()
			{
			}

			internal DataContractCriticalHelper(Type type)
			{
				this.underlyingType = type;
				this.SetTypeForInitialization(type);
				this.isValueType = type.IsValueType;
			}

			internal Type UnderlyingType
			{
				get
				{
					return this.underlyingType;
				}
			}

			internal Type OriginalUnderlyingType
			{
				get
				{
					if (this.originalUnderlyingType == null)
					{
						this.originalUnderlyingType = DataContract.DataContractCriticalHelper.GetDataContractOriginalType(this.underlyingType);
					}
					return this.originalUnderlyingType;
				}
			}

			internal virtual bool IsBuiltInDataContract
			{
				get
				{
					return false;
				}
			}

			internal Type TypeForInitialization
			{
				get
				{
					return this.typeForInitialization;
				}
			}

			[SecuritySafeCritical]
			private void SetTypeForInitialization(Type classType)
			{
				if (classType.IsSerializable || classType.IsDefined(Globals.TypeOfDataContractAttribute, false))
				{
					this.typeForInitialization = classType;
				}
			}

			internal bool IsReference
			{
				get
				{
					return this.isReference;
				}
				set
				{
					this.isReference = value;
				}
			}

			internal bool IsValueType
			{
				get
				{
					return this.isValueType;
				}
				set
				{
					this.isValueType = value;
				}
			}

			internal XmlQualifiedName StableName
			{
				get
				{
					return this.stableName;
				}
				set
				{
					this.stableName = value;
				}
			}

			internal GenericInfo GenericInfo
			{
				get
				{
					return this.genericInfo;
				}
				set
				{
					this.genericInfo = value;
				}
			}

			internal virtual Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
			{
				get
				{
					return null;
				}
				set
				{
				}
			}

			internal virtual bool IsISerializable
			{
				get
				{
					return false;
				}
				set
				{
					this.ThrowInvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("To set IsISerializable, class data cotnract is required."));
				}
			}

			internal XmlDictionaryString Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			public XmlDictionaryString Namespace
			{
				get
				{
					return this.ns;
				}
				set
				{
					this.ns = value;
				}
			}

			internal virtual bool HasRoot
			{
				get
				{
					return true;
				}
				set
				{
				}
			}

			internal virtual XmlDictionaryString TopLevelElementName
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			internal virtual XmlDictionaryString TopLevelElementNamespace
			{
				get
				{
					return this.ns;
				}
				set
				{
					this.ns = value;
				}
			}

			internal virtual bool CanContainReferences
			{
				get
				{
					return true;
				}
			}

			internal virtual bool IsPrimitive
			{
				get
				{
					return false;
				}
			}

			internal MethodInfo ParseMethod
			{
				get
				{
					if (!this.parseMethodSet)
					{
						MethodInfo method = this.UnderlyingType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, new Type[] { Globals.TypeOfString }, null);
						if (method != null && method.ReturnType == this.UnderlyingType)
						{
							this.parseMethod = method;
						}
						this.parseMethodSet = true;
					}
					return this.parseMethod;
				}
			}

			internal virtual void WriteRootElement(XmlWriterDelegator writer, XmlDictionaryString name, XmlDictionaryString ns)
			{
				if (ns == DictionaryGlobals.SerializationNamespace && !this.IsPrimitive)
				{
					writer.WriteStartElement("z", name, ns);
					return;
				}
				writer.WriteStartElement(name, ns);
			}

			internal void SetDataContractName(XmlQualifiedName stableName)
			{
				XmlDictionary xmlDictionary = new XmlDictionary(2);
				this.Name = xmlDictionary.Add(stableName.Name);
				this.Namespace = xmlDictionary.Add(stableName.Namespace);
				this.StableName = stableName;
			}

			internal void SetDataContractName(XmlDictionaryString name, XmlDictionaryString ns)
			{
				this.Name = name;
				this.Namespace = ns;
				this.StableName = DataContract.CreateQualifiedName(name.Value, ns.Value);
			}

			internal void ThrowInvalidDataContractException(string message)
			{
				DataContract.DataContractCriticalHelper.ThrowInvalidDataContractException(message, this.UnderlyingType);
			}

			private static Dictionary<TypeHandleRef, IntRef> typeToIDCache = new Dictionary<TypeHandleRef, IntRef>(new TypeHandleRefEqualityComparer());

			private static DataContract[] dataContractCache = new DataContract[32];

			private static int dataContractID = 0;

			private static Dictionary<Type, DataContract> typeToBuiltInContract;

			private static Dictionary<XmlQualifiedName, DataContract> nameToBuiltInContract;

			private static Dictionary<string, DataContract> typeNameToBuiltInContract;

			private static Dictionary<string, string> namespaces;

			private static Dictionary<string, XmlDictionaryString> clrTypeStrings;

			private static XmlDictionary clrTypeStringsDictionary;

			private static TypeHandleRef typeHandleRef = new TypeHandleRef();

			private static object cacheLock = new object();

			private static object createDataContractLock = new object();

			private static object initBuiltInContractsLock = new object();

			private static object namespacesLock = new object();

			private static object clrTypeStringsLock = new object();

			private readonly Type underlyingType;

			private Type originalUnderlyingType;

			private bool isReference;

			private bool isValueType;

			private XmlQualifiedName stableName;

			private GenericInfo genericInfo;

			private XmlDictionaryString name;

			private XmlDictionaryString ns;

			private Type typeForInitialization;

			private MethodInfo parseMethod;

			private bool parseMethodSet;
		}
	}
}
