using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal sealed class KnownTypeCollection : Collection<Type>
	{
		static KnownTypeCollection()
		{
			string text = "http://schemas.microsoft.com/2003/10/Serialization/";
			KnownTypeCollection.any_type = new XmlQualifiedName("anyType", text);
			KnownTypeCollection.any_uri_type = new XmlQualifiedName("anyURI", text);
			KnownTypeCollection.bool_type = new XmlQualifiedName("boolean", text);
			KnownTypeCollection.base64_type = new XmlQualifiedName("base64Binary", text);
			KnownTypeCollection.date_type = new XmlQualifiedName("dateTime", text);
			KnownTypeCollection.duration_type = new XmlQualifiedName("duration", text);
			KnownTypeCollection.qname_type = new XmlQualifiedName("QName", text);
			KnownTypeCollection.decimal_type = new XmlQualifiedName("decimal", text);
			KnownTypeCollection.double_type = new XmlQualifiedName("double", text);
			KnownTypeCollection.float_type = new XmlQualifiedName("float", text);
			KnownTypeCollection.byte_type = new XmlQualifiedName("byte", text);
			KnownTypeCollection.short_type = new XmlQualifiedName("short", text);
			KnownTypeCollection.int_type = new XmlQualifiedName("int", text);
			KnownTypeCollection.long_type = new XmlQualifiedName("long", text);
			KnownTypeCollection.ubyte_type = new XmlQualifiedName("unsignedByte", text);
			KnownTypeCollection.ushort_type = new XmlQualifiedName("unsignedShort", text);
			KnownTypeCollection.uint_type = new XmlQualifiedName("unsignedInt", text);
			KnownTypeCollection.ulong_type = new XmlQualifiedName("unsignedLong", text);
			KnownTypeCollection.string_type = new XmlQualifiedName("string", text);
			KnownTypeCollection.guid_type = new XmlQualifiedName("guid", text);
			KnownTypeCollection.char_type = new XmlQualifiedName("char", text);
			KnownTypeCollection.dbnull_type = new XmlQualifiedName("DBNull", "http://schemas.microsoft.com/2003/10/Serialization/System");
		}

		internal XmlQualifiedName GetXmlName(Type type)
		{
			SerializationMap serializationMap = this.FindUserMap(type);
			if (serializationMap != null)
			{
				return serializationMap.XmlName;
			}
			return KnownTypeCollection.GetPredefinedTypeName(type);
		}

		internal static XmlQualifiedName GetPredefinedTypeName(Type type)
		{
			XmlQualifiedName primitiveTypeName = KnownTypeCollection.GetPrimitiveTypeName(type);
			if (primitiveTypeName != XmlQualifiedName.Empty)
			{
				return primitiveTypeName;
			}
			if (type == typeof(DBNull))
			{
				return KnownTypeCollection.dbnull_type;
			}
			return XmlQualifiedName.Empty;
		}

		internal static XmlQualifiedName GetPrimitiveTypeName(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return KnownTypeCollection.GetPrimitiveTypeName(type.GetGenericArguments()[0]);
			}
			if (type.IsEnum)
			{
				return XmlQualifiedName.Empty;
			}
			switch (Type.GetTypeCode(type))
			{
			default:
				if (type == typeof(object))
				{
					return KnownTypeCollection.any_type;
				}
				if (type == typeof(Guid))
				{
					return KnownTypeCollection.guid_type;
				}
				if (type == typeof(TimeSpan))
				{
					return KnownTypeCollection.duration_type;
				}
				if (type == typeof(byte[]))
				{
					return KnownTypeCollection.base64_type;
				}
				if (type == typeof(Uri))
				{
					return KnownTypeCollection.any_uri_type;
				}
				return XmlQualifiedName.Empty;
			case TypeCode.Boolean:
				return KnownTypeCollection.bool_type;
			case TypeCode.Char:
				return KnownTypeCollection.char_type;
			case TypeCode.SByte:
				return KnownTypeCollection.byte_type;
			case TypeCode.Byte:
				return KnownTypeCollection.ubyte_type;
			case TypeCode.Int16:
				return KnownTypeCollection.short_type;
			case TypeCode.UInt16:
				return KnownTypeCollection.ushort_type;
			case TypeCode.Int32:
				return KnownTypeCollection.int_type;
			case TypeCode.UInt32:
				return KnownTypeCollection.uint_type;
			case TypeCode.Int64:
				return KnownTypeCollection.long_type;
			case TypeCode.UInt64:
				return KnownTypeCollection.ulong_type;
			case TypeCode.Single:
				return KnownTypeCollection.float_type;
			case TypeCode.Double:
				return KnownTypeCollection.double_type;
			case TypeCode.Decimal:
				return KnownTypeCollection.decimal_type;
			case TypeCode.DateTime:
				return KnownTypeCollection.date_type;
			case TypeCode.String:
				return KnownTypeCollection.string_type;
			}
		}

		internal static string PredefinedTypeObjectToString(object obj)
		{
			Type type = obj.GetType();
			switch (Type.GetTypeCode(type))
			{
			default:
				if (type == typeof(object))
				{
					return string.Empty;
				}
				if (type == typeof(Guid))
				{
					return XmlConvert.ToString((Guid)obj);
				}
				if (type == typeof(TimeSpan))
				{
					return XmlConvert.ToString((TimeSpan)obj);
				}
				if (type == typeof(byte[]))
				{
					return Convert.ToBase64String((byte[])obj);
				}
				if (type == typeof(Uri))
				{
					return ((Uri)obj).ToString();
				}
				throw new Exception("Internal error: missing predefined type serialization for type " + type.FullName);
			case TypeCode.DBNull:
				return string.Empty;
			case TypeCode.Boolean:
				return XmlConvert.ToString((bool)obj);
			case TypeCode.Char:
				return XmlConvert.ToString((uint)((char)obj));
			case TypeCode.SByte:
				return XmlConvert.ToString((sbyte)obj);
			case TypeCode.Byte:
				return XmlConvert.ToString((int)((byte)obj));
			case TypeCode.Int16:
				return XmlConvert.ToString((short)obj);
			case TypeCode.UInt16:
				return XmlConvert.ToString((int)((ushort)obj));
			case TypeCode.Int32:
				return XmlConvert.ToString((int)obj);
			case TypeCode.UInt32:
				return XmlConvert.ToString((uint)obj);
			case TypeCode.Int64:
				return XmlConvert.ToString((long)obj);
			case TypeCode.UInt64:
				return XmlConvert.ToString((ulong)obj);
			case TypeCode.Single:
				return XmlConvert.ToString((float)obj);
			case TypeCode.Double:
				return XmlConvert.ToString((double)obj);
			case TypeCode.Decimal:
				return XmlConvert.ToString((decimal)obj);
			case TypeCode.DateTime:
				return XmlConvert.ToString((DateTime)obj, XmlDateTimeSerializationMode.RoundtripKind);
			case TypeCode.String:
				return (string)obj;
			}
		}

		internal static Type GetPrimitiveTypeFromName(string name)
		{
			switch (name)
			{
			case "anyURI":
				return typeof(Uri);
			case "boolean":
				return typeof(bool);
			case "base64Binary":
				return typeof(byte[]);
			case "dateTime":
				return typeof(DateTime);
			case "duration":
				return typeof(TimeSpan);
			case "QName":
				return typeof(XmlQualifiedName);
			case "decimal":
				return typeof(decimal);
			case "double":
				return typeof(double);
			case "float":
				return typeof(float);
			case "byte":
				return typeof(sbyte);
			case "short":
				return typeof(short);
			case "int":
				return typeof(int);
			case "long":
				return typeof(long);
			case "unsignedByte":
				return typeof(byte);
			case "unsignedShort":
				return typeof(ushort);
			case "unsignedInt":
				return typeof(uint);
			case "unsignedLong":
				return typeof(ulong);
			case "string":
				return typeof(string);
			case "anyType":
				return typeof(object);
			case "guid":
				return typeof(Guid);
			case "char":
				return typeof(char);
			}
			return null;
		}

		internal static object PredefinedTypeStringToObject(string s, string name, XmlReader reader)
		{
			switch (name)
			{
			case "anyURI":
				return new Uri(s, UriKind.RelativeOrAbsolute);
			case "boolean":
				return XmlConvert.ToBoolean(s);
			case "base64Binary":
				return Convert.FromBase64String(s);
			case "dateTime":
				return XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
			case "duration":
				return XmlConvert.ToTimeSpan(s);
			case "QName":
			{
				int num2 = s.IndexOf(':');
				string text = ((num2 >= 0) ? s.Substring(num2 + 1) : s);
				return (num2 >= 0) ? new XmlQualifiedName(text, reader.LookupNamespace(s.Substring(0, num2))) : new XmlQualifiedName(text);
			}
			case "decimal":
				return XmlConvert.ToDecimal(s);
			case "double":
				return XmlConvert.ToDouble(s);
			case "float":
				return XmlConvert.ToSingle(s);
			case "byte":
				return XmlConvert.ToSByte(s);
			case "short":
				return XmlConvert.ToInt16(s);
			case "int":
				return XmlConvert.ToInt32(s);
			case "long":
				return XmlConvert.ToInt64(s);
			case "unsignedByte":
				return XmlConvert.ToByte(s);
			case "unsignedShort":
				return XmlConvert.ToUInt16(s);
			case "unsignedInt":
				return XmlConvert.ToUInt32(s);
			case "unsignedLong":
				return XmlConvert.ToUInt64(s);
			case "string":
				return s;
			case "guid":
				return XmlConvert.ToGuid(s);
			case "anyType":
				return s;
			case "char":
				return (char)XmlConvert.ToUInt32(s);
			}
			throw new Exception("Unanticipated primitive type: " + name);
		}

		protected override void ClearItems()
		{
			base.Clear();
		}

		protected override void InsertItem(int index, Type type)
		{
			if (this.TryRegister(type))
			{
				base.InsertItem(index, type);
			}
		}

		protected override void RemoveItem(int index)
		{
			Type type = base[index];
			List<SerializationMap> list = new List<SerializationMap>();
			foreach (SerializationMap serializationMap in this.contracts)
			{
				if (serializationMap.RuntimeType == type)
				{
					list.Add(serializationMap);
				}
			}
			foreach (SerializationMap serializationMap2 in list)
			{
				this.contracts.Remove(serializationMap2);
				base.RemoveItem(index);
			}
		}

		protected override void SetItem(int index, Type type)
		{
			if (index == this.Count)
			{
				this.InsertItem(index, type);
			}
			else
			{
				this.RemoveItem(index);
				if (this.TryRegister(type))
				{
					base.InsertItem(index - 1, type);
				}
			}
		}

		internal SerializationMap FindUserMap(XmlQualifiedName qname)
		{
			for (int i = 0; i < this.contracts.Count; i++)
			{
				if (qname == this.contracts[i].XmlName)
				{
					return this.contracts[i];
				}
			}
			return null;
		}

		internal Type GetSerializedType(Type type)
		{
			if (KnownTypeCollection.GetCollectionElementType(type) == null)
			{
				return type;
			}
			XmlQualifiedName qname = this.GetQName(type);
			SerializationMap serializationMap = this.FindUserMap(qname);
			if (serializationMap != null)
			{
				return serializationMap.RuntimeType;
			}
			return type;
		}

		internal SerializationMap FindUserMap(Type type)
		{
			for (int i = 0; i < this.contracts.Count; i++)
			{
				if (type == this.contracts[i].RuntimeType)
				{
					return this.contracts[i];
				}
			}
			return null;
		}

		internal XmlQualifiedName GetQName(Type type)
		{
			if (this.IsPrimitiveNotEnum(type))
			{
				return KnownTypeCollection.GetPrimitiveTypeName(type);
			}
			SerializationMap serializationMap = this.FindUserMap(type);
			if (serializationMap != null)
			{
				return serializationMap.XmlName;
			}
			if (type.IsEnum)
			{
				return this.GetEnumQName(type);
			}
			XmlQualifiedName xmlQualifiedName = this.GetContractQName(type);
			if (xmlQualifiedName != null)
			{
				return xmlQualifiedName;
			}
			if (type.GetInterface("System.Xml.Serialization.IXmlSerializable") != null)
			{
				return this.GetSerializableQName(type);
			}
			xmlQualifiedName = this.GetCollectionContractQName(type);
			if (xmlQualifiedName != null)
			{
				return xmlQualifiedName;
			}
			Type collectionElementType = KnownTypeCollection.GetCollectionElementType(type);
			if (collectionElementType != null)
			{
				return this.GetCollectionQName(collectionElementType);
			}
			if (this.GetAttribute<SerializableAttribute>(type) != null)
			{
				return this.GetSerializableQName(type);
			}
			return XmlQualifiedName.Empty;
		}

		private XmlQualifiedName GetContractQName(Type type)
		{
			DataContractAttribute attribute = this.GetAttribute<DataContractAttribute>(type);
			return (attribute != null) ? KnownTypeCollection.GetContractQName(type, attribute.Name, attribute.Namespace) : null;
		}

		private XmlQualifiedName GetCollectionContractQName(Type type)
		{
			CollectionDataContractAttribute attribute = this.GetAttribute<CollectionDataContractAttribute>(type);
			return (attribute != null) ? KnownTypeCollection.GetContractQName(type, attribute.Name, attribute.Namespace) : null;
		}

		internal static XmlQualifiedName GetContractQName(Type type, string name, string ns)
		{
			if (name == null)
			{
				name = ((type.Namespace != null && type.Namespace.Length != 0) ? type.FullName.Substring(type.Namespace.Length + 1).Replace('+', '.') : type.Name);
				if (type.IsGenericType)
				{
					name = name.Substring(0, name.IndexOf('`')) + "Of";
					foreach (Type type2 in type.GetGenericArguments())
					{
						name += type2.Name;
					}
				}
			}
			if (ns == null)
			{
				ns = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
			}
			return new XmlQualifiedName(name, ns);
		}

		private XmlQualifiedName GetEnumQName(Type type)
		{
			string text = null;
			string text2 = null;
			if (!type.IsEnum)
			{
				return null;
			}
			DataContractAttribute attribute = this.GetAttribute<DataContractAttribute>(type);
			if (attribute != null)
			{
				text2 = attribute.Namespace;
				text = attribute.Name;
			}
			if (text2 == null)
			{
				text2 = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
			}
			if (text == null)
			{
				text = ((type.Namespace != null) ? type.FullName.Substring(type.Namespace.Length + 1).Replace('+', '.') : type.Name);
			}
			return new XmlQualifiedName(text, text2);
		}

		private XmlQualifiedName GetCollectionQName(Type element)
		{
			XmlQualifiedName qname = this.GetQName(element);
			string text = qname.Namespace;
			if (qname.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				text = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
			}
			return new XmlQualifiedName("ArrayOf" + XmlConvert.EncodeLocalName(qname.Name), text);
		}

		private XmlQualifiedName GetSerializableQName(Type type)
		{
			string text = type.Name;
			if (type.IsGenericType)
			{
				text = text.Substring(0, text.IndexOf('`')) + "Of";
				foreach (Type type2 in type.GetGenericArguments())
				{
					text += this.GetQName(type2).Name;
				}
			}
			string text2 = "http://schemas.datacontract.org/2004/07/" + type.Namespace;
			XmlRootAttribute attribute = this.GetAttribute<XmlRootAttribute>(type);
			if (attribute != null)
			{
				text = attribute.ElementName;
				text2 = attribute.Namespace;
			}
			return new XmlQualifiedName(XmlConvert.EncodeLocalName(text), text2);
		}

		internal bool IsPrimitiveNotEnum(Type type)
		{
			return !type.IsEnum && (Type.GetTypeCode(type) != TypeCode.Object || (type == typeof(Guid) || type == typeof(object) || type == typeof(TimeSpan) || type == typeof(byte[]) || type == typeof(Uri)) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && this.IsPrimitiveNotEnum(type.GetGenericArguments()[0])));
		}

		internal bool TryRegister(Type type)
		{
			if (this.IsPrimitiveNotEnum(type))
			{
				return false;
			}
			if (this.FindUserMap(type) != null)
			{
				return false;
			}
			if (this.RegisterEnum(type) != null)
			{
				return true;
			}
			if (this.RegisterContract(type) != null)
			{
				return true;
			}
			if (this.RegisterIXmlSerializable(type) != null)
			{
				return true;
			}
			if (this.RegisterDictionary(type) != null)
			{
				return true;
			}
			if (this.RegisterCollectionContract(type) != null)
			{
				return true;
			}
			if (this.RegisterCollection(type) != null)
			{
				return true;
			}
			if (this.GetAttribute<SerializableAttribute>(type) != null)
			{
				this.RegisterSerializable(type);
				return true;
			}
			this.RegisterDefaultTypeMap(type);
			return true;
		}

		internal static Type GetCollectionElementType(Type type)
		{
			if (type.IsArray)
			{
				return type.GetElementType();
			}
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type2 in interfaces)
			{
				if (type2.IsGenericType && type2.GetGenericTypeDefinition().Equals(typeof(ICollection<>)))
				{
					return type2.GetGenericArguments()[0];
				}
			}
			foreach (Type type3 in interfaces)
			{
				if (type3 == typeof(IList))
				{
					return typeof(object);
				}
			}
			return null;
		}

		internal T GetAttribute<T>(MemberInfo mi) where T : Attribute
		{
			object[] customAttributes = mi.GetCustomAttributes(typeof(T), false);
			return (customAttributes.Length != 0) ? ((T)((object)customAttributes[0])) : ((T)((object)null));
		}

		private CollectionContractTypeMap RegisterCollectionContract(Type type)
		{
			CollectionDataContractAttribute attribute = this.GetAttribute<CollectionDataContractAttribute>(type);
			if (attribute == null)
			{
				return null;
			}
			Type collectionElementType = KnownTypeCollection.GetCollectionElementType(type);
			if (collectionElementType == null)
			{
				throw new InvalidOperationException(string.Format("Type '{0}' is marked as collection contract, but it is not a collection", type));
			}
			this.TryRegister(collectionElementType);
			XmlQualifiedName collectionContractQName = this.GetCollectionContractQName(type);
			this.CheckStandardQName(collectionContractQName);
			if (this.FindUserMap(collectionContractQName) != null)
			{
				throw new InvalidOperationException(string.Format("Failed to add type {0} to known type collection. There already is a registered type for XML name {1}", type, collectionContractQName));
			}
			CollectionContractTypeMap collectionContractTypeMap = new CollectionContractTypeMap(type, attribute, collectionElementType, collectionContractQName, this);
			this.contracts.Add(collectionContractTypeMap);
			return collectionContractTypeMap;
		}

		private CollectionTypeMap RegisterCollection(Type type)
		{
			Type collectionElementType = KnownTypeCollection.GetCollectionElementType(type);
			if (collectionElementType == null)
			{
				return null;
			}
			this.TryRegister(collectionElementType);
			XmlQualifiedName collectionQName = this.GetCollectionQName(collectionElementType);
			SerializationMap serializationMap = this.FindUserMap(collectionQName);
			if (serializationMap == null)
			{
				CollectionTypeMap collectionTypeMap = new CollectionTypeMap(type, collectionElementType, collectionQName, this);
				this.contracts.Add(collectionTypeMap);
				return collectionTypeMap;
			}
			CollectionTypeMap collectionTypeMap2 = serializationMap as CollectionTypeMap;
			if (collectionTypeMap2 == null || collectionTypeMap2.RuntimeType != type)
			{
				throw new InvalidOperationException(string.Format("Failed to add type {0} to known type collection. There already is a registered type for XML name {1}", type, collectionQName));
			}
			return collectionTypeMap2;
		}

		private static bool TypeImplementsIDictionary(Type type)
		{
			foreach (Type type2 in type.GetInterfaces())
			{
				if (type2 == typeof(IDictionary) || (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(IDictionary<, >)))
				{
					return true;
				}
			}
			return false;
		}

		private DictionaryTypeMap RegisterDictionary(Type type)
		{
			if (!KnownTypeCollection.TypeImplementsIDictionary(type))
			{
				return null;
			}
			CollectionDataContractAttribute attribute = this.GetAttribute<CollectionDataContractAttribute>(type);
			DictionaryTypeMap dictionaryTypeMap = new DictionaryTypeMap(type, attribute, this);
			if (this.FindUserMap(dictionaryTypeMap.XmlName) != null)
			{
				throw new InvalidOperationException(string.Format("Failed to add type {0} to known type collection. There already is a registered type for XML name {1}", type, dictionaryTypeMap.XmlName));
			}
			this.contracts.Add(dictionaryTypeMap);
			this.TryRegister(dictionaryTypeMap.KeyType);
			this.TryRegister(dictionaryTypeMap.ValueType);
			return dictionaryTypeMap;
		}

		private SerializationMap RegisterSerializable(Type type)
		{
			XmlQualifiedName serializableQName = this.GetSerializableQName(type);
			if (this.FindUserMap(serializableQName) != null)
			{
				throw new InvalidOperationException(string.Format("There is already a registered type for XML name {0}", serializableQName));
			}
			SharedTypeMap sharedTypeMap = new SharedTypeMap(type, serializableQName, this);
			this.contracts.Add(sharedTypeMap);
			return sharedTypeMap;
		}

		private SerializationMap RegisterIXmlSerializable(Type type)
		{
			if (type.GetInterface("System.Xml.Serialization.IXmlSerializable") == null)
			{
				return null;
			}
			XmlQualifiedName serializableQName = this.GetSerializableQName(type);
			if (this.FindUserMap(serializableQName) != null)
			{
				throw new InvalidOperationException(string.Format("There is already a registered type for XML name {0}", serializableQName));
			}
			XmlSerializableMap xmlSerializableMap = new XmlSerializableMap(type, serializableQName, this);
			this.contracts.Add(xmlSerializableMap);
			return xmlSerializableMap;
		}

		private void CheckStandardQName(XmlQualifiedName qname)
		{
			string @namespace = qname.Namespace;
			if (@namespace != null)
			{
				if (KnownTypeCollection.<>f__switch$map2 == null)
				{
					KnownTypeCollection.<>f__switch$map2 = new Dictionary<string, int>(4)
					{
						{ "http://www.w3.org/2001/XMLSchema", 0 },
						{ "http://www.w3.org/2001/XMLSchema-instance", 0 },
						{ "http://schemas.microsoft.com/2003/10/Serialization/", 0 },
						{ "http://schemas.microsoft.com/2003/10/Serialization/Arrays", 0 }
					};
				}
				int num;
				if (KnownTypeCollection.<>f__switch$map2.TryGetValue(@namespace, out num))
				{
					if (num == 0)
					{
						throw new InvalidOperationException(string.Format("Namespace {0} is reserved and cannot be used for user serialization", qname.Namespace));
					}
				}
			}
		}

		private SharedContractMap RegisterContract(Type type)
		{
			XmlQualifiedName contractQName = this.GetContractQName(type);
			if (contractQName == null)
			{
				return null;
			}
			this.CheckStandardQName(contractQName);
			if (this.FindUserMap(contractQName) != null)
			{
				throw new InvalidOperationException(string.Format("There is already a registered type for XML name {0}", contractQName));
			}
			SharedContractMap sharedContractMap = new SharedContractMap(type, contractQName, this);
			this.contracts.Add(sharedContractMap);
			sharedContractMap.Initialize();
			foreach (KnownTypeAttribute knownTypeAttribute in type.GetCustomAttributes(typeof(KnownTypeAttribute), true))
			{
				this.TryRegister(knownTypeAttribute.Type);
			}
			return sharedContractMap;
		}

		private DefaultTypeMap RegisterDefaultTypeMap(Type type)
		{
			DefaultTypeMap defaultTypeMap = new DefaultTypeMap(type, this);
			this.contracts.Add(defaultTypeMap);
			return defaultTypeMap;
		}

		private EnumMap RegisterEnum(Type type)
		{
			XmlQualifiedName enumQName = this.GetEnumQName(type);
			if (enumQName == null)
			{
				return null;
			}
			if (this.FindUserMap(enumQName) != null)
			{
				throw new InvalidOperationException(string.Format("There is already a registered type for XML name {0}", enumQName));
			}
			EnumMap enumMap = new EnumMap(type, enumQName, this);
			this.contracts.Add(enumMap);
			return enumMap;
		}

		internal const string MSSimpleNamespace = "http://schemas.microsoft.com/2003/10/Serialization/";

		internal const string MSArraysNamespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

		internal const string DefaultClrNamespaceBase = "http://schemas.datacontract.org/2004/07/";

		private static XmlQualifiedName any_type;

		private static XmlQualifiedName bool_type;

		private static XmlQualifiedName byte_type;

		private static XmlQualifiedName date_type;

		private static XmlQualifiedName decimal_type;

		private static XmlQualifiedName double_type;

		private static XmlQualifiedName float_type;

		private static XmlQualifiedName string_type;

		private static XmlQualifiedName short_type;

		private static XmlQualifiedName int_type;

		private static XmlQualifiedName long_type;

		private static XmlQualifiedName ubyte_type;

		private static XmlQualifiedName ushort_type;

		private static XmlQualifiedName uint_type;

		private static XmlQualifiedName ulong_type;

		private static XmlQualifiedName any_uri_type;

		private static XmlQualifiedName base64_type;

		private static XmlQualifiedName duration_type;

		private static XmlQualifiedName qname_type;

		private static XmlQualifiedName char_type;

		private static XmlQualifiedName guid_type;

		private static XmlQualifiedName dbnull_type;

		private List<SerializationMap> contracts = new List<SerializationMap>();
	}
}
