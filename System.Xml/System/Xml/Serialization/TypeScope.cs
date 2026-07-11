using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class TypeScope
	{
		static TypeScope()
		{
			TypeScope.AddPrimitive(typeof(string), "string", "String", (TypeFlags)2106);
			TypeScope.AddPrimitive(typeof(int), "int", "Int32", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(bool), "boolean", "Boolean", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(short), "short", "Int16", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(long), "long", "Int64", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(float), "float", "Single", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(double), "double", "Double", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(decimal), "decimal", "Decimal", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(DateTime), "dateTime", "DateTime", (TypeFlags)4200);
			TypeScope.AddPrimitive(typeof(XmlQualifiedName), "QName", "XmlQualifiedName", (TypeFlags)5226);
			TypeScope.AddPrimitive(typeof(byte), "unsignedByte", "Byte", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(sbyte), "byte", "SByte", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(ushort), "unsignedShort", "UInt16", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(uint), "unsignedInt", "UInt32", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(ulong), "unsignedLong", "UInt64", (TypeFlags)4136);
			TypeScope.AddPrimitive(typeof(DateTime), "date", "Date", (TypeFlags)4328);
			TypeScope.AddPrimitive(typeof(DateTime), "time", "Time", (TypeFlags)4328);
			TypeScope.AddPrimitive(typeof(string), "Name", "XmlName", (TypeFlags)234);
			TypeScope.AddPrimitive(typeof(string), "NCName", "XmlNCName", (TypeFlags)234);
			TypeScope.AddPrimitive(typeof(string), "NMTOKEN", "XmlNmToken", (TypeFlags)234);
			TypeScope.AddPrimitive(typeof(string), "NMTOKENS", "XmlNmTokens", (TypeFlags)234);
			TypeScope.AddPrimitive(typeof(byte[]), "base64Binary", "ByteArrayBase64", (TypeFlags)6890);
			TypeScope.AddPrimitive(typeof(byte[]), "hexBinary", "ByteArrayHex", (TypeFlags)6890);
			XmlSchemaPatternFacet xmlSchemaPatternFacet = new XmlSchemaPatternFacet();
			xmlSchemaPatternFacet.Value = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
			TypeScope.AddNonXsdPrimitive(typeof(Guid), "guid", "http://microsoft.com/wsdl/types/", "Guid", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), new XmlSchemaFacet[] { xmlSchemaPatternFacet }, (TypeFlags)4648);
			TypeScope.AddNonXsdPrimitive(typeof(char), "char", "http://microsoft.com/wsdl/types/", "Char", new XmlQualifiedName("unsignedShort", "http://www.w3.org/2001/XMLSchema"), new XmlSchemaFacet[0], (TypeFlags)616);
			if (LocalAppContextSwitches.EnableTimeSpanSerialization)
			{
				TypeScope.AddNonXsdPrimitive(typeof(TimeSpan), "TimeSpan", "http://microsoft.com/wsdl/types/", "TimeSpan", new XmlQualifiedName("duration", "http://www.w3.org/2001/XMLSchema"), new XmlSchemaFacet[0], (TypeFlags)4136);
			}
			TypeScope.AddSoapEncodedTypes("http://schemas.xmlsoap.org/soap/encoding/");
			TypeScope.AddPrimitive(typeof(string), "normalizedString", "String", (TypeFlags)2234);
			for (int i = 0; i < TypeScope.unsupportedTypes.Length; i++)
			{
				TypeScope.AddPrimitive(typeof(string), TypeScope.unsupportedTypes[i], "String", (TypeFlags)32954);
			}
		}

		internal static bool IsKnownType(Type type)
		{
			if (type == typeof(object))
			{
				return true;
			}
			if (type.IsEnum)
			{
				return false;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return true;
			case TypeCode.Char:
				return true;
			case TypeCode.SByte:
				return true;
			case TypeCode.Byte:
				return true;
			case TypeCode.Int16:
				return true;
			case TypeCode.UInt16:
				return true;
			case TypeCode.Int32:
				return true;
			case TypeCode.UInt32:
				return true;
			case TypeCode.Int64:
				return true;
			case TypeCode.UInt64:
				return true;
			case TypeCode.Single:
				return true;
			case TypeCode.Double:
				return true;
			case TypeCode.Decimal:
				return true;
			case TypeCode.DateTime:
				return true;
			case TypeCode.String:
				return true;
			}
			return type == typeof(XmlQualifiedName) || type == typeof(byte[]) || type == typeof(Guid) || (LocalAppContextSwitches.EnableTimeSpanSerialization && type == typeof(TimeSpan)) || type == typeof(XmlNode[]);
		}

		private static void AddSoapEncodedTypes(string ns)
		{
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "normalizedString", ns, "String", new XmlQualifiedName("normalizedString", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)2218);
			for (int i = 0; i < TypeScope.unsupportedTypes.Length; i++)
			{
				TypeScope.AddSoapEncodedPrimitive(typeof(string), TypeScope.unsupportedTypes[i], ns, "String", new XmlQualifiedName(TypeScope.unsupportedTypes[i], "http://www.w3.org/2001/XMLSchema"), (TypeFlags)32938);
			}
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "string", ns, "String", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)58);
			TypeScope.AddSoapEncodedPrimitive(typeof(int), "int", ns, "Int32", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(bool), "boolean", ns, "Boolean", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(short), "short", ns, "Int16", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(long), "long", ns, "Int64", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(float), "float", ns, "Single", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(double), "double", ns, "Double", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(decimal), "decimal", ns, "Decimal", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(DateTime), "dateTime", ns, "DateTime", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4200);
			TypeScope.AddSoapEncodedPrimitive(typeof(XmlQualifiedName), "QName", ns, "XmlQualifiedName", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)5226);
			TypeScope.AddSoapEncodedPrimitive(typeof(byte), "unsignedByte", ns, "Byte", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(sbyte), "byte", ns, "SByte", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(ushort), "unsignedShort", ns, "UInt16", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(uint), "unsignedInt", ns, "UInt32", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(ulong), "unsignedLong", ns, "UInt64", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4136);
			TypeScope.AddSoapEncodedPrimitive(typeof(DateTime), "date", ns, "Date", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4328);
			TypeScope.AddSoapEncodedPrimitive(typeof(DateTime), "time", ns, "Time", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4328);
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "Name", ns, "XmlName", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)234);
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "NCName", ns, "XmlNCName", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)234);
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "NMTOKEN", ns, "XmlNmToken", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)234);
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "NMTOKENS", ns, "XmlNmTokens", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)234);
			TypeScope.AddSoapEncodedPrimitive(typeof(byte[]), "base64Binary", ns, "ByteArrayBase64", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4842);
			TypeScope.AddSoapEncodedPrimitive(typeof(byte[]), "hexBinary", ns, "ByteArrayHex", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)4842);
			TypeScope.AddSoapEncodedPrimitive(typeof(string), "arrayCoordinate", ns, "String", new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)40);
			TypeScope.AddSoapEncodedPrimitive(typeof(byte[]), "base64", ns, "ByteArrayBase64", new XmlQualifiedName("base64Binary", "http://www.w3.org/2001/XMLSchema"), (TypeFlags)554);
		}

		private static void AddPrimitive(Type type, string dataTypeName, string formatterName, TypeFlags flags)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
			xmlSchemaSimpleType.Name = dataTypeName;
			TypeDesc typeDesc = new TypeDesc(type, true, xmlSchemaSimpleType, formatterName, flags);
			if (TypeScope.primitiveTypes[type] == null)
			{
				TypeScope.primitiveTypes.Add(type, typeDesc);
			}
			TypeScope.primitiveDataTypes.Add(xmlSchemaSimpleType, typeDesc);
			TypeScope.primitiveNames.Add(dataTypeName, "http://www.w3.org/2001/XMLSchema", typeDesc);
		}

		private static void AddNonXsdPrimitive(Type type, string dataTypeName, string ns, string formatterName, XmlQualifiedName baseTypeName, XmlSchemaFacet[] facets, TypeFlags flags)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
			xmlSchemaSimpleType.Name = dataTypeName;
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
			xmlSchemaSimpleTypeRestriction.BaseTypeName = baseTypeName;
			foreach (XmlSchemaFacet xmlSchemaFacet in facets)
			{
				xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaFacet);
			}
			xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeRestriction;
			TypeDesc typeDesc = new TypeDesc(type, false, xmlSchemaSimpleType, formatterName, flags);
			if (TypeScope.primitiveTypes[type] == null)
			{
				TypeScope.primitiveTypes.Add(type, typeDesc);
			}
			TypeScope.primitiveDataTypes.Add(xmlSchemaSimpleType, typeDesc);
			TypeScope.primitiveNames.Add(dataTypeName, ns, typeDesc);
		}

		private static void AddSoapEncodedPrimitive(Type type, string dataTypeName, string ns, string formatterName, XmlQualifiedName baseTypeName, TypeFlags flags)
		{
			TypeScope.AddNonXsdPrimitive(type, dataTypeName, ns, formatterName, baseTypeName, new XmlSchemaFacet[0], flags);
		}

		internal TypeDesc GetTypeDesc(string name, string ns)
		{
			return this.GetTypeDesc(name, ns, (TypeFlags)56);
		}

		internal TypeDesc GetTypeDesc(string name, string ns, TypeFlags flags)
		{
			TypeDesc typeDesc = (TypeDesc)TypeScope.primitiveNames[name, ns];
			if (typeDesc != null && (typeDesc.Flags & flags) != TypeFlags.None)
			{
				return typeDesc;
			}
			return null;
		}

		internal TypeDesc GetTypeDesc(XmlSchemaSimpleType dataType)
		{
			return (TypeDesc)TypeScope.primitiveDataTypes[dataType];
		}

		internal TypeDesc GetTypeDesc(Type type)
		{
			return this.GetTypeDesc(type, null, true, true);
		}

		internal TypeDesc GetTypeDesc(Type type, MemberInfo source)
		{
			return this.GetTypeDesc(type, source, true, true);
		}

		internal TypeDesc GetTypeDesc(Type type, MemberInfo source, bool directReference)
		{
			return this.GetTypeDesc(type, source, directReference, true);
		}

		internal TypeDesc GetTypeDesc(Type type, MemberInfo source, bool directReference, bool throwOnError)
		{
			if (type.ContainsGenericParameters)
			{
				throw new InvalidOperationException(Res.GetString("Type {0} is not supported because it has unbound generic parameters.  Only instantiated generic types can be serialized.", new object[] { type.ToString() }));
			}
			TypeDesc typeDesc = (TypeDesc)TypeScope.primitiveTypes[type];
			if (typeDesc == null)
			{
				typeDesc = (TypeDesc)this.typeDescs[type];
				if (typeDesc == null)
				{
					typeDesc = this.ImportTypeDesc(type, source, directReference);
				}
			}
			if (throwOnError)
			{
				typeDesc.CheckSupported();
			}
			return typeDesc;
		}

		internal TypeDesc GetArrayTypeDesc(Type type)
		{
			TypeDesc typeDesc = (TypeDesc)this.arrayTypeDescs[type];
			if (typeDesc == null)
			{
				typeDesc = this.GetTypeDesc(type);
				if (!typeDesc.IsArrayLike)
				{
					typeDesc = this.ImportTypeDesc(type, null, false);
				}
				typeDesc.CheckSupported();
				this.arrayTypeDescs.Add(type, typeDesc);
			}
			return typeDesc;
		}

		internal TypeMapping GetTypeMappingFromTypeDesc(TypeDesc typeDesc)
		{
			foreach (object obj in this.TypeMappings)
			{
				TypeMapping typeMapping = (TypeMapping)obj;
				if (typeMapping.TypeDesc == typeDesc)
				{
					return typeMapping;
				}
			}
			return null;
		}

		internal Type GetTypeFromTypeDesc(TypeDesc typeDesc)
		{
			if (typeDesc.Type != null)
			{
				return typeDesc.Type;
			}
			foreach (object obj in this.typeDescs)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				if (dictionaryEntry.Value == typeDesc)
				{
					return dictionaryEntry.Key as Type;
				}
			}
			return null;
		}

		private TypeDesc ImportTypeDesc(Type type, MemberInfo memberInfo, bool directReference)
		{
			Type type2 = null;
			Type type3 = null;
			TypeFlags typeFlags = TypeFlags.None;
			Exception ex = null;
			if (!type.IsPublic && !type.IsNestedPublic)
			{
				typeFlags |= TypeFlags.Unsupported;
				ex = new InvalidOperationException(Res.GetString("{0} is inaccessible due to its protection level. Only public types can be processed.", new object[] { type.FullName }));
			}
			else if (directReference && type.IsAbstract && type.IsSealed)
			{
				typeFlags |= TypeFlags.Unsupported;
				ex = new InvalidOperationException(Res.GetString("{0} cannot be serialized. Static types cannot be used as parameters or return types.", new object[] { type.FullName }));
			}
			if (DynamicAssemblies.IsTypeDynamic(type))
			{
				typeFlags |= TypeFlags.UseReflection;
			}
			if (!type.IsValueType)
			{
				typeFlags |= TypeFlags.Reference;
			}
			TypeKind typeKind;
			if (type == typeof(object))
			{
				typeKind = TypeKind.Root;
				typeFlags |= TypeFlags.HasDefaultConstructor;
			}
			else if (type == typeof(ValueType))
			{
				typeKind = TypeKind.Enum;
				typeFlags |= TypeFlags.Unsupported;
				if (ex == null)
				{
					ex = new NotSupportedException(Res.GetString("{0} is an unsupported type. Please use [XmlIgnore] attribute to exclude members of this type from serialization graph.", new object[] { type.FullName }));
				}
			}
			else if (type == typeof(void))
			{
				typeKind = TypeKind.Void;
			}
			else if (typeof(IXmlSerializable).IsAssignableFrom(type))
			{
				typeKind = TypeKind.Serializable;
				typeFlags |= (TypeFlags)36;
				typeFlags |= TypeScope.GetConstructorFlags(type, ref ex);
			}
			else if (type.IsArray)
			{
				typeKind = TypeKind.Array;
				if (type.GetArrayRank() > 1)
				{
					typeFlags |= TypeFlags.Unsupported;
					if (ex == null)
					{
						ex = new NotSupportedException(Res.GetString("Cannot serialize object of type {0}. Multidimensional arrays are not supported.", new object[] { type.FullName }));
					}
				}
				type2 = type.GetElementType();
				typeFlags |= TypeFlags.HasDefaultConstructor;
			}
			else if (typeof(ICollection).IsAssignableFrom(type) && !TypeScope.IsArraySegment(type))
			{
				typeKind = TypeKind.Collection;
				type2 = TypeScope.GetCollectionElementType(type, (memberInfo == null) ? null : (memberInfo.DeclaringType.FullName + "." + memberInfo.Name));
				typeFlags |= TypeScope.GetConstructorFlags(type, ref ex);
			}
			else if (type == typeof(XmlQualifiedName))
			{
				typeKind = TypeKind.Primitive;
			}
			else if (type.IsPrimitive)
			{
				typeKind = TypeKind.Primitive;
				typeFlags |= TypeFlags.Unsupported;
				if (ex == null)
				{
					ex = new NotSupportedException(Res.GetString("{0} is an unsupported type. Please use [XmlIgnore] attribute to exclude members of this type from serialization graph.", new object[] { type.FullName }));
				}
			}
			else if (type.IsEnum)
			{
				typeKind = TypeKind.Enum;
			}
			else if (type.IsValueType)
			{
				typeKind = TypeKind.Struct;
				if (TypeScope.IsOptionalValue(type))
				{
					type3 = type.GetGenericArguments()[0];
					typeFlags |= TypeFlags.OptionalValue;
				}
				else
				{
					type3 = type.BaseType;
				}
				if (type.IsAbstract)
				{
					typeFlags |= TypeFlags.Abstract;
				}
			}
			else if (type.IsClass)
			{
				if (type == typeof(XmlAttribute))
				{
					typeKind = TypeKind.Attribute;
					typeFlags |= (TypeFlags)12;
				}
				else if (typeof(XmlNode).IsAssignableFrom(type))
				{
					typeKind = TypeKind.Node;
					type3 = type.BaseType;
					typeFlags |= (TypeFlags)52;
					if (typeof(XmlText).IsAssignableFrom(type))
					{
						typeFlags &= (TypeFlags)(-33);
					}
					else if (typeof(XmlElement).IsAssignableFrom(type))
					{
						typeFlags &= (TypeFlags)(-17);
					}
					else if (type.IsAssignableFrom(typeof(XmlAttribute)))
					{
						typeFlags |= TypeFlags.CanBeAttributeValue;
					}
				}
				else
				{
					typeKind = TypeKind.Class;
					type3 = type.BaseType;
					if (type.IsAbstract)
					{
						typeFlags |= TypeFlags.Abstract;
					}
				}
			}
			else if (type.IsInterface)
			{
				typeKind = TypeKind.Void;
				typeFlags |= TypeFlags.Unsupported;
				if (ex == null)
				{
					if (memberInfo == null)
					{
						ex = new NotSupportedException(Res.GetString("Cannot serialize interface {0}.", new object[] { type.FullName }));
					}
					else
					{
						ex = new NotSupportedException(Res.GetString("Cannot serialize member {0} of type {1} because it is an interface.", new object[]
						{
							memberInfo.DeclaringType.FullName + "." + memberInfo.Name,
							type.FullName
						}));
					}
				}
			}
			else
			{
				typeKind = TypeKind.Void;
				typeFlags |= TypeFlags.Unsupported;
				if (ex == null)
				{
					ex = new NotSupportedException(Res.GetString("{0} is an unsupported type. Please use [XmlIgnore] attribute to exclude members of this type from serialization graph.", new object[] { type.FullName }));
				}
			}
			if (typeKind == TypeKind.Class && !type.IsAbstract)
			{
				typeFlags |= TypeScope.GetConstructorFlags(type, ref ex);
			}
			if ((typeKind == TypeKind.Struct || typeKind == TypeKind.Class) && typeof(IEnumerable).IsAssignableFrom(type) && !TypeScope.IsArraySegment(type))
			{
				type2 = TypeScope.GetEnumeratorElementType(type, ref typeFlags);
				typeKind = TypeKind.Enumerable;
				typeFlags |= TypeScope.GetConstructorFlags(type, ref ex);
			}
			TypeDesc typeDesc = new TypeDesc(type, CodeIdentifier.MakeValid(TypeScope.TypeName(type)), type.ToString(), typeKind, null, typeFlags, null);
			typeDesc.Exception = ex;
			if (directReference && (typeDesc.IsClass || typeKind == TypeKind.Serializable))
			{
				typeDesc.CheckNeedConstructor();
			}
			if (typeDesc.IsUnsupported)
			{
				return typeDesc;
			}
			this.typeDescs.Add(type, typeDesc);
			if (type2 != null)
			{
				TypeDesc typeDesc2 = this.GetTypeDesc(type2, memberInfo, true, false);
				if (directReference && (typeDesc2.IsCollection || typeDesc2.IsEnumerable) && !typeDesc2.IsPrimitive)
				{
					typeDesc2.CheckNeedConstructor();
				}
				typeDesc.ArrayElementTypeDesc = typeDesc2;
			}
			if (type3 != null && type3 != typeof(object) && type3 != typeof(ValueType))
			{
				typeDesc.BaseTypeDesc = this.GetTypeDesc(type3, memberInfo, false, false);
			}
			if (type.IsNestedPublic)
			{
				Type type4 = type.DeclaringType;
				while (type4 != null && !type4.ContainsGenericParameters && (!type4.IsAbstract || !type4.IsSealed))
				{
					this.GetTypeDesc(type4, null, false);
					type4 = type4.DeclaringType;
				}
			}
			return typeDesc;
		}

		private static bool IsArraySegment(Type t)
		{
			return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ArraySegment<>);
		}

		internal static bool IsOptionalValue(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition();
		}

		internal static string TypeName(Type t)
		{
			if (t.IsArray)
			{
				return "ArrayOf" + TypeScope.TypeName(t.GetElementType());
			}
			if (t.IsGenericType)
			{
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				string text = t.Name;
				int num = text.IndexOf("`", StringComparison.Ordinal);
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				stringBuilder.Append(text);
				stringBuilder.Append("Of");
				Type[] genericArguments = t.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					stringBuilder.Append(TypeScope.TypeName(genericArguments[i]));
					stringBuilder2.Append(genericArguments[i].Namespace);
				}
				return stringBuilder.ToString();
			}
			return t.Name;
		}

		internal static Type GetArrayElementType(Type type, string memberInfo)
		{
			if (type.IsArray)
			{
				return type.GetElementType();
			}
			if (TypeScope.IsArraySegment(type))
			{
				return null;
			}
			if (typeof(ICollection).IsAssignableFrom(type))
			{
				return TypeScope.GetCollectionElementType(type, memberInfo);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				TypeFlags typeFlags = TypeFlags.None;
				return TypeScope.GetEnumeratorElementType(type, ref typeFlags);
			}
			return null;
		}

		internal static MemberMapping[] GetAllMembers(StructMapping mapping)
		{
			if (mapping.BaseMapping == null)
			{
				return mapping.Members;
			}
			ArrayList arrayList = new ArrayList();
			TypeScope.GetAllMembers(mapping, arrayList);
			return (MemberMapping[])arrayList.ToArray(typeof(MemberMapping));
		}

		internal static void GetAllMembers(StructMapping mapping, ArrayList list)
		{
			if (mapping.BaseMapping != null)
			{
				TypeScope.GetAllMembers(mapping.BaseMapping, list);
			}
			for (int i = 0; i < mapping.Members.Length; i++)
			{
				list.Add(mapping.Members[i]);
			}
		}

		internal static MemberMapping[] GetAllMembers(StructMapping mapping, Dictionary<string, MemberInfo> memberInfos)
		{
			MemberMapping[] allMembers = TypeScope.GetAllMembers(mapping);
			TypeScope.PopulateMemberInfos(mapping, allMembers, memberInfos);
			return allMembers;
		}

		internal static MemberMapping[] GetSettableMembers(StructMapping structMapping)
		{
			ArrayList arrayList = new ArrayList();
			TypeScope.GetSettableMembers(structMapping, arrayList);
			return (MemberMapping[])arrayList.ToArray(typeof(MemberMapping));
		}

		private static void GetSettableMembers(StructMapping mapping, ArrayList list)
		{
			if (mapping.BaseMapping != null)
			{
				TypeScope.GetSettableMembers(mapping.BaseMapping, list);
			}
			if (mapping.Members != null)
			{
				foreach (MemberMapping memberMapping in mapping.Members)
				{
					MemberInfo memberInfo = memberMapping.MemberInfo;
					if (memberInfo != null && memberInfo.MemberType == MemberTypes.Property)
					{
						PropertyInfo propertyInfo = memberInfo as PropertyInfo;
						if (propertyInfo != null && !TypeScope.CanWriteProperty(propertyInfo, memberMapping.TypeDesc))
						{
							throw new InvalidOperationException(Res.GetString("Cannot deserialize type '{0}' because it contains property '{1}' which has no public setter.", new object[] { propertyInfo.DeclaringType, propertyInfo.Name }));
						}
					}
					list.Add(memberMapping);
				}
			}
		}

		private static bool CanWriteProperty(PropertyInfo propertyInfo, TypeDesc typeDesc)
		{
			return typeDesc.Kind == TypeKind.Collection || typeDesc.Kind == TypeKind.Enumerable || (propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic);
		}

		internal static MemberMapping[] GetSettableMembers(StructMapping mapping, Dictionary<string, MemberInfo> memberInfos)
		{
			MemberMapping[] settableMembers = TypeScope.GetSettableMembers(mapping);
			TypeScope.PopulateMemberInfos(mapping, settableMembers, memberInfos);
			return settableMembers;
		}

		private static void PopulateMemberInfos(StructMapping structMapping, MemberMapping[] mappings, Dictionary<string, MemberInfo> memberInfos)
		{
			memberInfos.Clear();
			for (int i = 0; i < mappings.Length; i++)
			{
				memberInfos[mappings[i].Name] = mappings[i].MemberInfo;
				if (mappings[i].ChoiceIdentifier != null)
				{
					memberInfos[mappings[i].ChoiceIdentifier.MemberName] = mappings[i].ChoiceIdentifier.MemberInfo;
				}
				if (mappings[i].CheckSpecifiedMemberInfo != null)
				{
					memberInfos[mappings[i].Name + "Specified"] = mappings[i].CheckSpecifiedMemberInfo;
				}
			}
			Dictionary<string, MemberInfo> dictionary = null;
			MemberInfo memberInfo = null;
			foreach (KeyValuePair<string, MemberInfo> keyValuePair in memberInfos)
			{
				if (TypeScope.ShouldBeReplaced(keyValuePair.Value, structMapping.TypeDesc.Type, out memberInfo))
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<string, MemberInfo>();
					}
					dictionary.Add(keyValuePair.Key, memberInfo);
				}
			}
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, MemberInfo> keyValuePair2 in dictionary)
				{
					memberInfos[keyValuePair2.Key] = keyValuePair2.Value;
				}
				for (int j = 0; j < mappings.Length; j++)
				{
					MemberInfo memberInfo2;
					if (dictionary.TryGetValue(mappings[j].Name, out memberInfo2))
					{
						MemberMapping memberMapping = mappings[j].Clone();
						memberMapping.MemberInfo = memberInfo2;
						mappings[j] = memberMapping;
					}
				}
			}
		}

		private static bool ShouldBeReplaced(MemberInfo memberInfoToBeReplaced, Type derivedType, out MemberInfo replacedInfo)
		{
			replacedInfo = memberInfoToBeReplaced;
			Type type = derivedType;
			Type declaringType = memberInfoToBeReplaced.DeclaringType;
			if (declaringType.IsAssignableFrom(type))
			{
				while (type != declaringType)
				{
					TypeInfo typeInfo = type.GetTypeInfo();
					foreach (PropertyInfo propertyInfo in typeInfo.DeclaredProperties)
					{
						if (propertyInfo.Name == memberInfoToBeReplaced.Name)
						{
							replacedInfo = propertyInfo;
							if (replacedInfo != memberInfoToBeReplaced)
							{
								return true;
							}
						}
					}
					foreach (FieldInfo fieldInfo in typeInfo.DeclaredFields)
					{
						if (fieldInfo.Name == memberInfoToBeReplaced.Name)
						{
							replacedInfo = fieldInfo;
							if (replacedInfo != memberInfoToBeReplaced)
							{
								return true;
							}
						}
					}
					type = type.BaseType;
				}
			}
			return false;
		}

		private static TypeFlags GetConstructorFlags(Type type, ref Exception exception)
		{
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
			if (constructor != null)
			{
				TypeFlags typeFlags = TypeFlags.HasDefaultConstructor;
				if (!constructor.IsPublic)
				{
					typeFlags |= TypeFlags.CtorInaccessible;
				}
				else
				{
					object[] customAttributes = constructor.GetCustomAttributes(typeof(ObsoleteAttribute), false);
					if (customAttributes != null && customAttributes.Length != 0 && ((ObsoleteAttribute)customAttributes[0]).IsError)
					{
						typeFlags |= TypeFlags.CtorInaccessible;
					}
				}
				return typeFlags;
			}
			return TypeFlags.None;
		}

		private static Type GetEnumeratorElementType(Type type, ref TypeFlags flags)
		{
			if (!typeof(IEnumerable).IsAssignableFrom(type))
			{
				return null;
			}
			MethodInfo methodInfo = type.GetMethod("GetEnumerator", new Type[0]);
			if (methodInfo == null || !typeof(IEnumerator).IsAssignableFrom(methodInfo.ReturnType))
			{
				methodInfo = null;
				MemberInfo[] member = type.GetMember("System.Collections.Generic.IEnumerable<*", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < member.Length; i++)
				{
					methodInfo = member[i] as MethodInfo;
					if (methodInfo != null && typeof(IEnumerator).IsAssignableFrom(methodInfo.ReturnType))
					{
						flags |= TypeFlags.GenericInterface;
						break;
					}
					methodInfo = null;
				}
				if (methodInfo == null)
				{
					flags |= TypeFlags.UsePrivateImplementation;
					methodInfo = type.GetMethod("System.Collections.IEnumerable.GetEnumerator", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
				}
			}
			if (methodInfo == null || !typeof(IEnumerator).IsAssignableFrom(methodInfo.ReturnType))
			{
				return null;
			}
			if (new XmlAttributes(methodInfo).XmlIgnore)
			{
				return null;
			}
			PropertyInfo property = methodInfo.ReturnType.GetProperty("Current");
			Type type2 = ((property == null) ? typeof(object) : property.PropertyType);
			MethodInfo methodInfo2 = type.GetMethod("Add", new Type[] { type2 });
			if (methodInfo2 == null && type2 != typeof(object))
			{
				type2 = typeof(object);
				methodInfo2 = type.GetMethod("Add", new Type[] { type2 });
			}
			if (methodInfo2 == null)
			{
				throw new InvalidOperationException(Res.GetString("To be XML serializable, types which inherit from {2} must have an implementation of Add({1}) at all levels of their inheritance hierarchy. {0} does not implement Add({1}).", new object[] { type.FullName, type2, "IEnumerable" }));
			}
			return type2;
		}

		internal static PropertyInfo GetDefaultIndexer(Type type, string memberInfo)
		{
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				if (memberInfo == null)
				{
					throw new NotSupportedException(Res.GetString("The type {0} is not supported because it implements IDictionary.", new object[] { type.FullName }));
				}
				throw new NotSupportedException(Res.GetString("Cannot serialize member {0} of type {1}, because it implements IDictionary.", new object[] { memberInfo, type.FullName }));
			}
			else
			{
				MemberInfo[] defaultMembers = type.GetDefaultMembers();
				PropertyInfo propertyInfo = null;
				if (defaultMembers != null && defaultMembers.Length != 0)
				{
					Type type2 = type;
					while (type2 != null)
					{
						for (int i = 0; i < defaultMembers.Length; i++)
						{
							if (defaultMembers[i] is PropertyInfo)
							{
								PropertyInfo propertyInfo2 = (PropertyInfo)defaultMembers[i];
								if (!(propertyInfo2.DeclaringType != type2) && propertyInfo2.CanRead)
								{
									ParameterInfo[] parameters = propertyInfo2.GetGetMethod().GetParameters();
									if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
									{
										propertyInfo = propertyInfo2;
										break;
									}
								}
							}
						}
						if (propertyInfo != null)
						{
							break;
						}
						type2 = type2.BaseType;
					}
				}
				if (propertyInfo == null)
				{
					throw new InvalidOperationException(Res.GetString("You must implement a default accessor on {0} because it inherits from ICollection.", new object[] { type.FullName }));
				}
				if (type.GetMethod("Add", new Type[] { propertyInfo.PropertyType }) == null)
				{
					throw new InvalidOperationException(Res.GetString("To be XML serializable, types which inherit from {2} must have an implementation of Add({1}) at all levels of their inheritance hierarchy. {0} does not implement Add({1}).", new object[] { type.FullName, propertyInfo.PropertyType, "ICollection" }));
				}
				return propertyInfo;
			}
		}

		private static Type GetCollectionElementType(Type type, string memberInfo)
		{
			return TypeScope.GetDefaultIndexer(type, memberInfo).PropertyType;
		}

		internal static XmlQualifiedName ParseWsdlArrayType(string type, out string dims, XmlSchemaObject parent)
		{
			int num = type.LastIndexOf(':');
			string text;
			if (num <= 0)
			{
				text = "";
			}
			else
			{
				text = type.Substring(0, num);
			}
			int num2 = type.IndexOf('[', num + 1);
			if (num2 <= num)
			{
				throw new InvalidOperationException(Res.GetString("Invalid wsd:arrayType syntax: '{0}'.", new object[] { type }));
			}
			string text2 = type.Substring(num + 1, num2 - num - 1);
			dims = type.Substring(num2);
			while (parent != null)
			{
				if (parent.Namespaces != null)
				{
					string text3 = (string)parent.Namespaces.Namespaces[text];
					if (text3 != null)
					{
						text = text3;
						break;
					}
				}
				parent = parent.Parent;
			}
			return new XmlQualifiedName(text2, text);
		}

		internal ICollection Types
		{
			get
			{
				return this.typeDescs.Keys;
			}
		}

		internal void AddTypeMapping(TypeMapping typeMapping)
		{
			this.typeMappings.Add(typeMapping);
		}

		internal ICollection TypeMappings
		{
			get
			{
				return this.typeMappings;
			}
		}

		internal static Hashtable PrimtiveTypes
		{
			get
			{
				return TypeScope.primitiveTypes;
			}
		}

		private Hashtable typeDescs = new Hashtable();

		private Hashtable arrayTypeDescs = new Hashtable();

		private ArrayList typeMappings = new ArrayList();

		private static Hashtable primitiveTypes = new Hashtable();

		private static Hashtable primitiveDataTypes = new Hashtable();

		private static NameTable primitiveNames = new NameTable();

		private static string[] unsupportedTypes = new string[]
		{
			"anyURI", "duration", "ENTITY", "ENTITIES", "gDay", "gMonth", "gMonthDay", "gYear", "gYearMonth", "ID",
			"IDREF", "IDREFS", "integer", "language", "negativeInteger", "nonNegativeInteger", "nonPositiveInteger", "NOTATION", "positiveInteger", "token"
		};
	}
}
