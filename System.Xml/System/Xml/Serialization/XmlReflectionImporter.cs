using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlReflectionImporter
	{
		public XmlReflectionImporter()
			: this(null, null)
		{
		}

		public XmlReflectionImporter(string defaultNamespace)
			: this(null, defaultNamespace)
		{
		}

		public XmlReflectionImporter(XmlAttributeOverrides attributeOverrides)
			: this(attributeOverrides, null)
		{
		}

		public XmlReflectionImporter(XmlAttributeOverrides attributeOverrides, string defaultNamespace)
		{
			if (defaultNamespace == null)
			{
				defaultNamespace = string.Empty;
			}
			if (attributeOverrides == null)
			{
				attributeOverrides = new XmlAttributeOverrides();
			}
			this.attributeOverrides = attributeOverrides;
			this.defaultNs = defaultNamespace;
			this.typeScope = new TypeScope();
			this.modelScope = new ModelScope(this.typeScope);
		}

		public void IncludeTypes(ICustomAttributeProvider provider)
		{
			this.IncludeTypes(provider, new RecursionLimiter());
		}

		private void IncludeTypes(ICustomAttributeProvider provider, RecursionLimiter limiter)
		{
			object[] customAttributes = provider.GetCustomAttributes(typeof(XmlIncludeAttribute), false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Type type = ((XmlIncludeAttribute)customAttributes[i]).Type;
				this.IncludeType(type, limiter);
			}
		}

		public void IncludeType(Type type)
		{
			this.IncludeType(type, new RecursionLimiter());
		}

		private void IncludeType(Type type, RecursionLimiter limiter)
		{
			int num = this.arrayNestingLevel;
			XmlArrayItemAttributes xmlArrayItemAttributes = this.savedArrayItemAttributes;
			string text = this.savedArrayNamespace;
			this.arrayNestingLevel = 0;
			this.savedArrayItemAttributes = null;
			this.savedArrayNamespace = null;
			TypeMapping typeMapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(type), this.defaultNs, XmlReflectionImporter.ImportContext.Element, string.Empty, null, limiter);
			if (typeMapping.IsAnonymousType && !typeMapping.TypeDesc.IsSpecial)
			{
				throw new InvalidOperationException(Res.GetString("Cannot include anonymous type '{0}'.", new object[] { type.FullName }));
			}
			this.arrayNestingLevel = num;
			this.savedArrayItemAttributes = xmlArrayItemAttributes;
			this.savedArrayNamespace = text;
		}

		public XmlTypeMapping ImportTypeMapping(Type type)
		{
			return this.ImportTypeMapping(type, null, null);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, string defaultNamespace)
		{
			return this.ImportTypeMapping(type, null, defaultNamespace);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, XmlRootAttribute root)
		{
			return this.ImportTypeMapping(type, root, null);
		}

		public XmlTypeMapping ImportTypeMapping(Type type, XmlRootAttribute root, string defaultNamespace)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			XmlTypeMapping xmlTypeMapping = new XmlTypeMapping(this.typeScope, this.ImportElement(this.modelScope.GetTypeModel(type), root, defaultNamespace, new RecursionLimiter()));
			xmlTypeMapping.SetKeyInternal(XmlMapping.GenerateKey(type, root, defaultNamespace));
			xmlTypeMapping.GenerateSerializer = true;
			return xmlTypeMapping;
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement)
		{
			return this.ImportMembersMapping(elementName, ns, members, hasWrapperElement, false);
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool rpc)
		{
			return this.ImportMembersMapping(elementName, ns, members, hasWrapperElement, rpc, false);
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool rpc, bool openModel)
		{
			return this.ImportMembersMapping(elementName, ns, members, hasWrapperElement, rpc, openModel, XmlMappingAccess.Read | XmlMappingAccess.Write);
		}

		public XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool rpc, bool openModel, XmlMappingAccess access)
		{
			ElementAccessor elementAccessor = new ElementAccessor();
			elementAccessor.Name = ((elementName == null || elementName.Length == 0) ? elementName : XmlConvert.EncodeLocalName(elementName));
			elementAccessor.Namespace = ns;
			MembersMapping membersMapping = this.ImportMembersMapping(members, ns, hasWrapperElement, rpc, openModel, new RecursionLimiter());
			elementAccessor.Mapping = membersMapping;
			elementAccessor.Form = XmlSchemaForm.Qualified;
			if (!rpc)
			{
				if (hasWrapperElement)
				{
					elementAccessor = (ElementAccessor)this.ReconcileAccessor(elementAccessor, this.elements);
				}
				else
				{
					foreach (MemberMapping memberMapping in membersMapping.Members)
					{
						if (memberMapping.Elements != null && memberMapping.Elements.Length != 0)
						{
							memberMapping.Elements[0] = (ElementAccessor)this.ReconcileAccessor(memberMapping.Elements[0], this.elements);
						}
					}
				}
			}
			return new XmlMembersMapping(this.typeScope, elementAccessor, access)
			{
				GenerateSerializer = true
			};
		}

		private XmlAttributes GetAttributes(Type type, bool canBeSimpleType)
		{
			XmlAttributes xmlAttributes = this.attributeOverrides[type];
			if (xmlAttributes != null)
			{
				return xmlAttributes;
			}
			if (canBeSimpleType && TypeScope.IsKnownType(type))
			{
				return this.defaultAttributes;
			}
			return new XmlAttributes(type);
		}

		private XmlAttributes GetAttributes(MemberInfo memberInfo)
		{
			XmlAttributes xmlAttributes = this.attributeOverrides[memberInfo.DeclaringType, memberInfo.Name];
			if (xmlAttributes != null)
			{
				return xmlAttributes;
			}
			return new XmlAttributes(memberInfo);
		}

		private ElementAccessor ImportElement(TypeModel model, XmlRootAttribute root, string defaultNamespace, RecursionLimiter limiter)
		{
			XmlAttributes attributes = this.GetAttributes(model.Type, true);
			if (root == null)
			{
				root = attributes.XmlRoot;
			}
			string text = ((root == null) ? null : root.Namespace);
			if (text == null)
			{
				text = defaultNamespace;
			}
			if (text == null)
			{
				text = this.defaultNs;
			}
			this.arrayNestingLevel = -1;
			this.savedArrayItemAttributes = null;
			this.savedArrayNamespace = null;
			ElementAccessor elementAccessor = XmlReflectionImporter.CreateElementAccessor(this.ImportTypeMapping(model, text, XmlReflectionImporter.ImportContext.Element, string.Empty, attributes, limiter), text);
			if (root != null)
			{
				if (root.ElementName.Length > 0)
				{
					elementAccessor.Name = XmlConvert.EncodeLocalName(root.ElementName);
				}
				if (root.IsNullableSpecified && !root.IsNullable && model.TypeDesc.IsOptionalValue)
				{
					throw new InvalidOperationException(Res.GetString("IsNullable may not be set to 'false' for a Nullable<{0}> type. Consider using '{0}' type or removing the IsNullable property from the {1} attribute.", new object[]
					{
						model.TypeDesc.BaseTypeDesc.FullName,
						"XmlRoot"
					}));
				}
				elementAccessor.IsNullable = (root.IsNullableSpecified ? root.IsNullable : (model.TypeDesc.IsNullable || model.TypeDesc.IsOptionalValue));
				XmlReflectionImporter.CheckNullable(elementAccessor.IsNullable, model.TypeDesc, elementAccessor.Mapping);
			}
			else
			{
				elementAccessor.IsNullable = model.TypeDesc.IsNullable || model.TypeDesc.IsOptionalValue;
			}
			elementAccessor.Form = XmlSchemaForm.Qualified;
			return (ElementAccessor)this.ReconcileAccessor(elementAccessor, this.elements);
		}

		private static string GetMappingName(Mapping mapping)
		{
			if (mapping is MembersMapping)
			{
				return "(method)";
			}
			if (mapping is TypeMapping)
			{
				return ((TypeMapping)mapping).TypeDesc.FullName;
			}
			throw new ArgumentException(Res.GetString("Internal error."), "mapping");
		}

		private ElementAccessor ReconcileLocalAccessor(ElementAccessor accessor, string ns)
		{
			if (accessor.Namespace == ns)
			{
				return accessor;
			}
			return (ElementAccessor)this.ReconcileAccessor(accessor, this.elements);
		}

		private Accessor ReconcileAccessor(Accessor accessor, NameTable accessors)
		{
			if (accessor.Any && accessor.Name.Length == 0)
			{
				return accessor;
			}
			Accessor accessor2 = (Accessor)accessors[accessor.Name, accessor.Namespace];
			if (accessor2 == null)
			{
				accessor.IsTopLevelInSchema = true;
				accessors.Add(accessor.Name, accessor.Namespace, accessor);
				return accessor;
			}
			if (accessor2.Mapping == accessor.Mapping)
			{
				return accessor2;
			}
			if (!(accessor.Mapping is MembersMapping) && !(accessor2.Mapping is MembersMapping) && (accessor.Mapping.TypeDesc == accessor2.Mapping.TypeDesc || (accessor2.Mapping is NullableMapping && accessor.Mapping.TypeDesc == ((NullableMapping)accessor2.Mapping).BaseMapping.TypeDesc) || (accessor.Mapping is NullableMapping && ((NullableMapping)accessor.Mapping).BaseMapping.TypeDesc == accessor2.Mapping.TypeDesc)))
			{
				string text = Convert.ToString(accessor.Default, CultureInfo.InvariantCulture);
				string text2 = Convert.ToString(accessor2.Default, CultureInfo.InvariantCulture);
				if (text == text2)
				{
					return accessor2;
				}
				throw new InvalidOperationException(Res.GetString("The global XML item '{0}' from namespace '{1}' has mismatch default value attributes: '{2}' and '{3}' and cannot be mapped to the same schema item. Use XML attributes to specify another XML name or namespace for one of the items, or make sure that the default values match.", new object[] { accessor.Name, accessor.Namespace, text, text2 }));
			}
			else
			{
				if (accessor.Mapping is MembersMapping || accessor2.Mapping is MembersMapping)
				{
					throw new InvalidOperationException(Res.GetString("The XML element '{0}' from namespace '{1}' references a method and a type. Change the method's message name using WebMethodAttribute or change the type's root element using the XmlRootAttribute.", new object[] { accessor.Name, accessor.Namespace }));
				}
				if (accessor.Mapping is ArrayMapping)
				{
					if (!(accessor2.Mapping is ArrayMapping))
					{
						throw new InvalidOperationException(Res.GetString("The top XML element '{0}' from namespace '{1}' references distinct types {2} and {3}. Use XML attributes to specify another XML name or namespace for the element or types.", new object[]
						{
							accessor.Name,
							accessor.Namespace,
							XmlReflectionImporter.GetMappingName(accessor2.Mapping),
							XmlReflectionImporter.GetMappingName(accessor.Mapping)
						}));
					}
					ArrayMapping arrayMapping = (ArrayMapping)accessor.Mapping;
					ArrayMapping arrayMapping2 = (arrayMapping.IsAnonymousType ? null : ((ArrayMapping)this.types[accessor2.Mapping.TypeName, accessor2.Mapping.Namespace]));
					ArrayMapping arrayMapping3 = arrayMapping2;
					while (arrayMapping2 != null)
					{
						if (arrayMapping2 == accessor.Mapping)
						{
							return accessor2;
						}
						arrayMapping2 = arrayMapping2.Next;
					}
					arrayMapping.Next = arrayMapping3;
					if (!arrayMapping.IsAnonymousType)
					{
						this.types[accessor2.Mapping.TypeName, accessor2.Mapping.Namespace] = arrayMapping;
					}
					return accessor2;
				}
				else
				{
					if (accessor is AttributeAccessor)
					{
						throw new InvalidOperationException(Res.GetString("The global XML attribute '{0}' from namespace '{1}' references distinct types {2} and {3}. Use XML attributes to specify another XML name or namespace for the attribute or types.", new object[]
						{
							accessor.Name,
							accessor.Namespace,
							XmlReflectionImporter.GetMappingName(accessor2.Mapping),
							XmlReflectionImporter.GetMappingName(accessor.Mapping)
						}));
					}
					throw new InvalidOperationException(Res.GetString("The top XML element '{0}' from namespace '{1}' references distinct types {2} and {3}. Use XML attributes to specify another XML name or namespace for the element or types.", new object[]
					{
						accessor.Name,
						accessor.Namespace,
						XmlReflectionImporter.GetMappingName(accessor2.Mapping),
						XmlReflectionImporter.GetMappingName(accessor.Mapping)
					}));
				}
			}
		}

		private Exception CreateReflectionException(string context, Exception e)
		{
			return new InvalidOperationException(Res.GetString("There was an error reflecting '{0}'.", new object[] { context }), e);
		}

		private Exception CreateTypeReflectionException(string context, Exception e)
		{
			return new InvalidOperationException(Res.GetString("There was an error reflecting type '{0}'.", new object[] { context }), e);
		}

		private Exception CreateMemberReflectionException(FieldModel model, Exception e)
		{
			return new InvalidOperationException(Res.GetString(model.IsProperty ? "There was an error reflecting property '{0}'." : "There was an error reflecting field '{0}'.", new object[] { model.Name }), e);
		}

		private TypeMapping ImportTypeMapping(TypeModel model, string ns, XmlReflectionImporter.ImportContext context, string dataType, XmlAttributes a, RecursionLimiter limiter)
		{
			return this.ImportTypeMapping(model, ns, context, dataType, a, false, false, limiter);
		}

		private TypeMapping ImportTypeMapping(TypeModel model, string ns, XmlReflectionImporter.ImportContext context, string dataType, XmlAttributes a, bool repeats, bool openModel, RecursionLimiter limiter)
		{
			TypeMapping typeMapping2;
			try
			{
				if (dataType.Length > 0)
				{
					TypeDesc typeDesc = (TypeScope.IsOptionalValue(model.Type) ? model.TypeDesc.BaseTypeDesc : model.TypeDesc);
					if (!typeDesc.IsPrimitive)
					{
						throw new InvalidOperationException(Res.GetString("'{0}' is an invalid value for the {1} property. The property may only be specified for primitive types.", new object[] { dataType, "XmlElementAttribute.DataType" }));
					}
					TypeDesc typeDesc2 = this.typeScope.GetTypeDesc(dataType, "http://www.w3.org/2001/XMLSchema");
					if (typeDesc2 == null)
					{
						throw new InvalidOperationException(Res.GetString("Value '{0}' cannot be used for the {1} property. The datatype '{2}' is missing.", new object[]
						{
							dataType,
							"XmlElementAttribute.DataType",
							new XmlQualifiedName(dataType, "http://www.w3.org/2001/XMLSchema").ToString()
						}));
					}
					if (typeDesc.FullName != typeDesc2.FullName)
					{
						throw new InvalidOperationException(Res.GetString("'{0}' is an invalid value for the {1} property. {0} cannot be converted to {2}.", new object[] { dataType, "XmlElementAttribute.DataType", typeDesc.FullName }));
					}
				}
				if (a == null)
				{
					a = this.GetAttributes(model.Type, false);
				}
				if ((a.XmlFlags & (XmlAttributeFlags)(-193)) != (XmlAttributeFlags)0)
				{
					throw new InvalidOperationException(Res.GetString("XmlRoot and XmlType attributes may not be specified for the type {0}.", new object[] { model.Type.FullName }));
				}
				switch (model.TypeDesc.Kind)
				{
				case TypeKind.Root:
				case TypeKind.Struct:
				case TypeKind.Class:
					if (context != XmlReflectionImporter.ImportContext.Element)
					{
						throw XmlReflectionImporter.UnsupportedException(model.TypeDesc, context);
					}
					if (model.TypeDesc.IsOptionalValue)
					{
						TypeDesc typeDesc3 = (string.IsNullOrEmpty(dataType) ? model.TypeDesc.BaseTypeDesc : this.typeScope.GetTypeDesc(dataType, "http://www.w3.org/2001/XMLSchema"));
						string text = ((typeDesc3.DataType == null) ? typeDesc3.Name : typeDesc3.DataType.Name);
						TypeMapping typeMapping = this.GetTypeMapping(text, ns, typeDesc3, this.types, null);
						if (typeMapping == null)
						{
							typeMapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(model.TypeDesc.BaseTypeDesc.Type), ns, context, dataType, null, repeats, openModel, limiter);
						}
						typeMapping2 = this.CreateNullableMapping(typeMapping, model.TypeDesc.Type);
					}
					else
					{
						typeMapping2 = this.ImportStructLikeMapping((StructModel)model, ns, openModel, a, limiter);
					}
					break;
				case TypeKind.Primitive:
					if (a.XmlFlags != (XmlAttributeFlags)0)
					{
						throw XmlReflectionImporter.InvalidAttributeUseException(model.Type);
					}
					typeMapping2 = this.ImportPrimitiveMapping((PrimitiveModel)model, context, dataType, repeats);
					break;
				case TypeKind.Enum:
					typeMapping2 = this.ImportEnumMapping((EnumModel)model, ns, repeats);
					break;
				case TypeKind.Array:
				case TypeKind.Collection:
				case TypeKind.Enumerable:
				{
					if (context != XmlReflectionImporter.ImportContext.Element)
					{
						throw XmlReflectionImporter.UnsupportedException(model.TypeDesc, context);
					}
					this.arrayNestingLevel++;
					TypeMapping typeMapping3 = this.ImportArrayLikeMapping((ArrayModel)model, ns, limiter);
					this.arrayNestingLevel--;
					typeMapping2 = typeMapping3;
					break;
				}
				default:
					if (model.TypeDesc.Kind == TypeKind.Serializable)
					{
						if ((a.XmlFlags & (XmlAttributeFlags)(-65)) != (XmlAttributeFlags)0)
						{
							throw new InvalidOperationException(Res.GetString("Only XmlRoot attribute may be specified for the type {0}. Please use {1} to specify schema type.", new object[]
							{
								model.TypeDesc.FullName,
								typeof(XmlSchemaProviderAttribute).Name
							}));
						}
					}
					else if (a.XmlFlags != (XmlAttributeFlags)0)
					{
						throw XmlReflectionImporter.InvalidAttributeUseException(model.Type);
					}
					if (!model.TypeDesc.IsSpecial)
					{
						throw XmlReflectionImporter.UnsupportedException(model.TypeDesc, context);
					}
					typeMapping2 = this.ImportSpecialMapping(model.Type, model.TypeDesc, ns, context, limiter);
					break;
				}
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
				{
					throw;
				}
				throw this.CreateTypeReflectionException(model.TypeDesc.FullName, ex);
			}
			return typeMapping2;
		}

		internal static MethodInfo GetMethodFromSchemaProvider(XmlSchemaProviderAttribute provider, Type type)
		{
			if (provider.IsAny)
			{
				return null;
			}
			if (provider.MethodName == null)
			{
				throw new ArgumentNullException("MethodName");
			}
			if (!CodeGenerator.IsValidLanguageIndependentIdentifier(provider.MethodName))
			{
				throw new ArgumentException(Res.GetString("'{0}' is an invalid language identifier.", new object[] { provider.MethodName }), "MethodName");
			}
			MethodInfo method = type.GetMethod(provider.MethodName, BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(XmlSchemaSet) }, null);
			if (method == null)
			{
				throw new InvalidOperationException(Res.GetString("You must implement public static {0}({1}) method on {2}.", new object[]
				{
					provider.MethodName,
					typeof(XmlSchemaSet).Name,
					type.FullName
				}));
			}
			if (!typeof(XmlQualifiedName).IsAssignableFrom(method.ReturnType) && !typeof(XmlSchemaType).IsAssignableFrom(method.ReturnType))
			{
				throw new InvalidOperationException(Res.GetString("Method {0}.{1}() specified by {2} has invalid signature: return type must be compatible with {3}.", new object[]
				{
					type.Name,
					provider.MethodName,
					typeof(XmlSchemaProviderAttribute).Name,
					typeof(XmlQualifiedName).FullName,
					typeof(XmlSchemaType).FullName
				}));
			}
			return method;
		}

		private SpecialMapping ImportSpecialMapping(Type type, TypeDesc typeDesc, string ns, XmlReflectionImporter.ImportContext context, RecursionLimiter limiter)
		{
			if (this.specials == null)
			{
				this.specials = new Hashtable();
			}
			SpecialMapping specialMapping = (SpecialMapping)this.specials[type];
			if (specialMapping != null)
			{
				this.CheckContext(specialMapping.TypeDesc, context);
				return specialMapping;
			}
			if (typeDesc.Kind == TypeKind.Serializable)
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(XmlSchemaProviderAttribute), false);
				SerializableMapping serializableMapping;
				if (customAttributes.Length != 0)
				{
					XmlSchemaProviderAttribute xmlSchemaProviderAttribute = (XmlSchemaProviderAttribute)customAttributes[0];
					serializableMapping = new SerializableMapping(XmlReflectionImporter.GetMethodFromSchemaProvider(xmlSchemaProviderAttribute, type), xmlSchemaProviderAttribute.IsAny, ns);
					XmlQualifiedName xsiType = serializableMapping.XsiType;
					if (xsiType != null && !xsiType.IsEmpty)
					{
						if (this.serializables == null)
						{
							this.serializables = new NameTable();
						}
						SerializableMapping serializableMapping2 = (SerializableMapping)this.serializables[xsiType];
						if (serializableMapping2 != null)
						{
							if (serializableMapping2.Type == null)
							{
								serializableMapping = serializableMapping2;
							}
							else if (serializableMapping2.Type != type)
							{
								SerializableMapping next = serializableMapping2.Next;
								serializableMapping2.Next = serializableMapping;
								serializableMapping.Next = next;
							}
						}
						else
						{
							XmlSchemaType xsdType = serializableMapping.XsdType;
							if (xsdType != null)
							{
								this.SetBase(serializableMapping, xsdType.DerivedFrom);
							}
							this.serializables[xsiType] = serializableMapping;
						}
						serializableMapping.TypeName = xsiType.Name;
						serializableMapping.Namespace = xsiType.Namespace;
					}
					serializableMapping.TypeDesc = typeDesc;
					serializableMapping.Type = type;
					this.IncludeTypes(type);
				}
				else
				{
					serializableMapping = new SerializableMapping();
					serializableMapping.TypeDesc = typeDesc;
					serializableMapping.Type = type;
				}
				specialMapping = serializableMapping;
			}
			else
			{
				specialMapping = new SpecialMapping();
				specialMapping.TypeDesc = typeDesc;
			}
			this.CheckContext(typeDesc, context);
			this.specials.Add(type, specialMapping);
			this.typeScope.AddTypeMapping(specialMapping);
			return specialMapping;
		}

		internal static void ValidationCallbackWithErrorCode(object sender, ValidationEventArgs args)
		{
			if (args.Severity == XmlSeverityType.Error)
			{
				throw new InvalidOperationException(Res.GetString("Schema type information provided by {0} is invalid: {1}", new object[]
				{
					typeof(IXmlSerializable).Name,
					args.Message
				}));
			}
		}

		internal void SetBase(SerializableMapping mapping, XmlQualifiedName baseQname)
		{
			if (baseQname.IsEmpty)
			{
				return;
			}
			if (baseQname.Namespace == "http://www.w3.org/2001/XMLSchema")
			{
				return;
			}
			XmlSchemaSet schemas = mapping.Schemas;
			ArrayList arrayList = (ArrayList)schemas.Schemas(baseQname.Namespace);
			if (arrayList.Count == 0)
			{
				throw new InvalidOperationException(Res.GetString("Missing schema targetNamespace=\"{0}\".", new object[] { baseQname.Namespace }));
			}
			if (arrayList.Count > 1)
			{
				throw new InvalidOperationException(Res.GetString("Multiple schemas with targetNamespace='{0}' returned by {1}.{2}().  Please use only the main (parent) schema, and add the others to the schema Includes.", new object[]
				{
					baseQname.Namespace,
					typeof(IXmlSerializable).Name,
					"GetSchema"
				}));
			}
			XmlSchemaType xmlSchemaType = (XmlSchemaType)((XmlSchema)arrayList[0]).SchemaTypes[baseQname];
			xmlSchemaType = ((xmlSchemaType.Redefined != null) ? xmlSchemaType.Redefined : xmlSchemaType);
			if (this.serializables[baseQname] == null)
			{
				SerializableMapping serializableMapping = new SerializableMapping(baseQname, schemas);
				this.SetBase(serializableMapping, xmlSchemaType.DerivedFrom);
				this.serializables.Add(baseQname, serializableMapping);
			}
			mapping.SetBaseMapping((SerializableMapping)this.serializables[baseQname]);
		}

		private static string GetContextName(XmlReflectionImporter.ImportContext context)
		{
			switch (context)
			{
			case XmlReflectionImporter.ImportContext.Text:
				return "text";
			case XmlReflectionImporter.ImportContext.Attribute:
				return "attribute";
			case XmlReflectionImporter.ImportContext.Element:
				return "element";
			default:
				throw new ArgumentException(Res.GetString("Internal error."), "context");
			}
		}

		private static Exception InvalidAttributeUseException(Type type)
		{
			return new InvalidOperationException(Res.GetString("XML attributes may not be specified for the type {0}.", new object[] { type.FullName }));
		}

		private static Exception UnsupportedException(TypeDesc typeDesc, XmlReflectionImporter.ImportContext context)
		{
			return new InvalidOperationException(Res.GetString("{0} cannot be used as: 'xml {1}'.", new object[]
			{
				typeDesc.FullName,
				XmlReflectionImporter.GetContextName(context)
			}));
		}

		private StructMapping CreateRootMapping()
		{
			TypeDesc typeDesc = this.typeScope.GetTypeDesc(typeof(object));
			return new StructMapping
			{
				TypeDesc = typeDesc,
				TypeName = "anyType",
				Namespace = "http://www.w3.org/2001/XMLSchema",
				Members = new MemberMapping[0],
				IncludeInSchema = false
			};
		}

		private NullableMapping CreateNullableMapping(TypeMapping baseMapping, Type type)
		{
			TypeDesc nullableTypeDesc = baseMapping.TypeDesc.GetNullableTypeDesc(type);
			TypeMapping typeMapping;
			if (!baseMapping.IsAnonymousType)
			{
				typeMapping = (TypeMapping)this.nullables[baseMapping.TypeName, baseMapping.Namespace];
			}
			else
			{
				typeMapping = (TypeMapping)this.anonymous[type];
			}
			NullableMapping nullableMapping;
			if (typeMapping == null)
			{
				nullableMapping = new NullableMapping();
				nullableMapping.BaseMapping = baseMapping;
				nullableMapping.TypeDesc = nullableTypeDesc;
				nullableMapping.TypeName = baseMapping.TypeName;
				nullableMapping.Namespace = baseMapping.Namespace;
				nullableMapping.IncludeInSchema = baseMapping.IncludeInSchema;
				if (!baseMapping.IsAnonymousType)
				{
					this.nullables.Add(baseMapping.TypeName, baseMapping.Namespace, nullableMapping);
				}
				else
				{
					this.anonymous[type] = nullableMapping;
				}
				this.typeScope.AddTypeMapping(nullableMapping);
				return nullableMapping;
			}
			if (!(typeMapping is NullableMapping))
			{
				throw new InvalidOperationException(Res.GetString("Types '{0}' and '{1}' both use the XML type name, '{2}', from namespace '{3}'. Use XML attributes to specify a unique XML name and/or namespace for the type.", new object[]
				{
					nullableTypeDesc.FullName,
					typeMapping.TypeDesc.FullName,
					nullableTypeDesc.Name,
					typeMapping.Namespace
				}));
			}
			nullableMapping = (NullableMapping)typeMapping;
			if (nullableMapping.BaseMapping is PrimitiveMapping && baseMapping is PrimitiveMapping)
			{
				return nullableMapping;
			}
			if (nullableMapping.BaseMapping == baseMapping)
			{
				return nullableMapping;
			}
			throw new InvalidOperationException(Res.GetString("Types '{0}' and '{1}' both use the XML type name, '{2}', from namespace '{3}'. Use XML attributes to specify a unique XML name and/or namespace for the type.", new object[]
			{
				nullableTypeDesc.FullName,
				typeMapping.TypeDesc.FullName,
				nullableTypeDesc.Name,
				typeMapping.Namespace
			}));
		}

		private StructMapping GetRootMapping()
		{
			if (this.root == null)
			{
				this.root = this.CreateRootMapping();
				this.typeScope.AddTypeMapping(this.root);
			}
			return this.root;
		}

		private TypeMapping GetTypeMapping(string typeName, string ns, TypeDesc typeDesc, NameTable typeLib, Type type)
		{
			TypeMapping typeMapping;
			if (typeName == null || typeName.Length == 0)
			{
				typeMapping = ((type == null) ? null : ((TypeMapping)this.anonymous[type]));
			}
			else
			{
				typeMapping = (TypeMapping)typeLib[typeName, ns];
			}
			if (typeMapping == null)
			{
				return null;
			}
			if (!typeMapping.IsAnonymousType && typeMapping.TypeDesc != typeDesc)
			{
				throw new InvalidOperationException(Res.GetString("Types '{0}' and '{1}' both use the XML type name, '{2}', from namespace '{3}'. Use XML attributes to specify a unique XML name and/or namespace for the type.", new object[]
				{
					typeDesc.FullName,
					typeMapping.TypeDesc.FullName,
					typeName,
					ns
				}));
			}
			return typeMapping;
		}

		private StructMapping ImportStructLikeMapping(StructModel model, string ns, bool openModel, XmlAttributes a, RecursionLimiter limiter)
		{
			if (model.TypeDesc.Kind == TypeKind.Root)
			{
				return this.GetRootMapping();
			}
			if (a == null)
			{
				a = this.GetAttributes(model.Type, false);
			}
			string text = ns;
			if (a.XmlType != null && a.XmlType.Namespace != null)
			{
				text = a.XmlType.Namespace;
			}
			else if (a.XmlRoot != null && a.XmlRoot.Namespace != null)
			{
				text = a.XmlRoot.Namespace;
			}
			string text2 = (XmlReflectionImporter.IsAnonymousType(a, ns) ? null : this.XsdTypeName(model.Type, a, model.TypeDesc.Name));
			text2 = XmlConvert.EncodeLocalName(text2);
			StructMapping structMapping = (StructMapping)this.GetTypeMapping(text2, text, model.TypeDesc, this.types, model.Type);
			if (structMapping == null)
			{
				structMapping = new StructMapping();
				structMapping.TypeDesc = model.TypeDesc;
				structMapping.Namespace = text;
				structMapping.TypeName = text2;
				if (!structMapping.IsAnonymousType)
				{
					this.types.Add(text2, text, structMapping);
				}
				else
				{
					this.anonymous[model.Type] = structMapping;
				}
				if (a.XmlType != null)
				{
					structMapping.IncludeInSchema = a.XmlType.IncludeInSchema;
				}
				if (limiter.IsExceededLimit)
				{
					limiter.DeferredWorkItems.Add(new ImportStructWorkItem(model, structMapping));
					return structMapping;
				}
				int num = limiter.Depth;
				limiter.Depth = num + 1;
				this.InitializeStructMembers(structMapping, model, openModel, text2, limiter);
				while (limiter.DeferredWorkItems.Count > 0)
				{
					int num2 = limiter.DeferredWorkItems.Count - 1;
					ImportStructWorkItem importStructWorkItem = limiter.DeferredWorkItems[num2];
					if (this.InitializeStructMembers(importStructWorkItem.Mapping, importStructWorkItem.Model, openModel, text2, limiter))
					{
						limiter.DeferredWorkItems.RemoveAt(num2);
					}
				}
				num = limiter.Depth;
				limiter.Depth = num - 1;
			}
			return structMapping;
		}

		private bool InitializeStructMembers(StructMapping mapping, StructModel model, bool openModel, string typeName, RecursionLimiter limiter)
		{
			if (mapping.IsFullyInitialized)
			{
				return true;
			}
			if (model.TypeDesc.BaseTypeDesc != null)
			{
				TypeModel typeModel = this.modelScope.GetTypeModel(model.Type.BaseType, false);
				if (!(typeModel is StructModel))
				{
					throw new NotSupportedException(Res.GetString("Using {0} as a base type for a class is not supported by XmlSerializer.", new object[] { model.Type.BaseType.FullName }));
				}
				StructMapping structMapping = this.ImportStructLikeMapping((StructModel)typeModel, mapping.Namespace, openModel, null, limiter);
				int num = limiter.DeferredWorkItems.IndexOf(structMapping);
				if (num < 0)
				{
					mapping.BaseMapping = structMapping;
					foreach (object obj in mapping.BaseMapping.LocalAttributes.Values)
					{
						AttributeAccessor attributeAccessor = (AttributeAccessor)obj;
						XmlReflectionImporter.AddUniqueAccessor(mapping.LocalAttributes, attributeAccessor);
					}
					if (mapping.BaseMapping.HasExplicitSequence())
					{
						goto IL_01CF;
					}
					using (IEnumerator enumerator = mapping.BaseMapping.LocalElements.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							ElementAccessor elementAccessor = (ElementAccessor)obj2;
							XmlReflectionImporter.AddUniqueAccessor(mapping.LocalElements, elementAccessor);
						}
						goto IL_01CF;
					}
				}
				if (!limiter.DeferredWorkItems.Contains(mapping))
				{
					limiter.DeferredWorkItems.Add(new ImportStructWorkItem(model, mapping));
				}
				int num2 = limiter.DeferredWorkItems.Count - 1;
				if (num < num2)
				{
					ImportStructWorkItem importStructWorkItem = limiter.DeferredWorkItems[num];
					limiter.DeferredWorkItems[num] = limiter.DeferredWorkItems[num2];
					limiter.DeferredWorkItems[num2] = importStructWorkItem;
				}
				return false;
			}
			IL_01CF:
			ArrayList arrayList = new ArrayList();
			TextAccessor textAccessor = null;
			bool flag = false;
			bool flag2 = false;
			foreach (MemberInfo memberInfo in model.GetMemberInfos())
			{
				if ((memberInfo.MemberType & (MemberTypes.Field | MemberTypes.Property)) != (MemberTypes)0)
				{
					XmlAttributes attributes = this.GetAttributes(memberInfo);
					if (!attributes.XmlIgnore)
					{
						FieldModel fieldModel = model.GetFieldModel(memberInfo);
						if (fieldModel != null)
						{
							try
							{
								MemberMapping memberMapping = this.ImportFieldMapping(model, fieldModel, attributes, mapping.Namespace, limiter);
								if (memberMapping != null)
								{
									if (mapping.BaseMapping == null || !mapping.BaseMapping.Declares(memberMapping, mapping.TypeName))
									{
										flag2 |= memberMapping.IsSequence;
										XmlReflectionImporter.AddUniqueAccessor(memberMapping, mapping.LocalElements, mapping.LocalAttributes, flag2);
										if (memberMapping.Text != null)
										{
											if (!memberMapping.Text.Mapping.TypeDesc.CanBeTextValue && memberMapping.Text.Mapping.IsList)
											{
												throw new InvalidOperationException(Res.GetString("Cannot serialize object of type '{0}'. Consider changing type of XmlText member '{0}.{1}' from {2} to string or string array.", new object[]
												{
													typeName,
													memberMapping.Text.Name,
													memberMapping.Text.Mapping.TypeDesc.FullName
												}));
											}
											if (textAccessor != null)
											{
												throw new InvalidOperationException(Res.GetString("Cannot serialize object of type '{0}' because it has multiple XmlText attributes. Consider using an array of strings with XmlTextAttribute for serialization of a mixed complex type.", new object[] { model.Type.FullName }));
											}
											textAccessor = memberMapping.Text;
										}
										if (memberMapping.Xmlns != null)
										{
											if (mapping.XmlnsMember != null)
											{
												throw new InvalidOperationException(Res.GetString("Cannot serialize object of type '{0}' because it has multiple XmlNamespaceDeclarations attributes.", new object[] { model.Type.FullName }));
											}
											mapping.XmlnsMember = memberMapping;
										}
										if (memberMapping.Elements != null && memberMapping.Elements.Length != 0)
										{
											flag = true;
										}
										arrayList.Add(memberMapping);
									}
								}
							}
							catch (Exception ex)
							{
								if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
								{
									throw;
								}
								throw this.CreateMemberReflectionException(fieldModel, ex);
							}
						}
					}
				}
			}
			mapping.SetContentModel(textAccessor, flag);
			if (flag2)
			{
				Hashtable hashtable = new Hashtable();
				for (int j = 0; j < arrayList.Count; j++)
				{
					MemberMapping memberMapping2 = (MemberMapping)arrayList[j];
					if (memberMapping2.IsParticle)
					{
						if (!memberMapping2.IsSequence)
						{
							throw new InvalidOperationException(Res.GetString("Inconsistent sequencing: if used on one of the class's members, the '{0}' property is required on all particle-like members, please explicitly set '{0}' using XmlElement, XmlAnyElement or XmlArray custom attribute on class member '{1}'.", new object[] { "Order", memberMapping2.Name }));
						}
						if (hashtable[memberMapping2.SequenceId] != null)
						{
							string text = "'{1}' values must be unique within the same scope. Value '{0}' is in use. Please change '{1}' property on '{2}'.";
							object[] array = new object[3];
							int num3 = 0;
							int i = memberMapping2.SequenceId;
							array[num3] = i.ToString(CultureInfo.InvariantCulture);
							array[1] = "Order";
							array[2] = memberMapping2.Name;
							throw new InvalidOperationException(Res.GetString(text, array));
						}
						hashtable[memberMapping2.SequenceId] = memberMapping2;
					}
				}
				arrayList.Sort(new MemberMappingComparer());
			}
			mapping.Members = (MemberMapping[])arrayList.ToArray(typeof(MemberMapping));
			if (mapping.BaseMapping == null)
			{
				mapping.BaseMapping = this.GetRootMapping();
			}
			if (mapping.XmlnsMember != null && mapping.BaseMapping.HasXmlnsMember)
			{
				throw new InvalidOperationException(Res.GetString("Cannot serialize object of type '{0}' because it has multiple XmlNamespaceDeclarations attributes.", new object[] { model.Type.FullName }));
			}
			this.IncludeTypes(model.Type, limiter);
			this.typeScope.AddTypeMapping(mapping);
			if (openModel)
			{
				mapping.IsOpenModel = true;
			}
			return true;
		}

		private static bool IsAnonymousType(XmlAttributes a, string contextNs)
		{
			if (a.XmlType != null && a.XmlType.AnonymousType)
			{
				string @namespace = a.XmlType.Namespace;
				return string.IsNullOrEmpty(@namespace) || @namespace == contextNs;
			}
			return false;
		}

		internal string XsdTypeName(Type type)
		{
			if (type == typeof(object))
			{
				return "anyType";
			}
			TypeDesc typeDesc = this.typeScope.GetTypeDesc(type);
			if (typeDesc.IsPrimitive && typeDesc.DataType != null && typeDesc.DataType.Name != null && typeDesc.DataType.Name.Length > 0)
			{
				return typeDesc.DataType.Name;
			}
			return this.XsdTypeName(type, this.GetAttributes(type, false), typeDesc.Name);
		}

		internal string XsdTypeName(Type type, XmlAttributes a, string name)
		{
			string text = name;
			if (a.XmlType != null && a.XmlType.TypeName.Length > 0)
			{
				text = a.XmlType.TypeName;
			}
			if (type.IsGenericType && text.IndexOf('{') >= 0)
			{
				Type[] genericArguments = type.GetGenericTypeDefinition().GetGenericArguments();
				Type[] genericArguments2 = type.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					string text2 = "{" + genericArguments[i] + "}";
					if (text.Contains(text2))
					{
						text = text.Replace(text2, this.XsdTypeName(genericArguments2[i]));
						if (text.IndexOf('{') < 0)
						{
							break;
						}
					}
				}
			}
			return text;
		}

		private static int CountAtLevel(XmlArrayItemAttributes attributes, int level)
		{
			int num = 0;
			for (int i = 0; i < attributes.Count; i++)
			{
				if (attributes[i].NestingLevel == level)
				{
					num++;
				}
			}
			return num;
		}

		private void SetArrayMappingType(ArrayMapping mapping, string defaultNs, Type type)
		{
			XmlAttributes attributes = this.GetAttributes(type, false);
			if (XmlReflectionImporter.IsAnonymousType(attributes, defaultNs))
			{
				mapping.TypeName = null;
				mapping.Namespace = defaultNs;
				return;
			}
			ElementAccessor elementAccessor = null;
			TypeMapping typeMapping;
			if (mapping.Elements.Length == 1)
			{
				elementAccessor = mapping.Elements[0];
				typeMapping = elementAccessor.Mapping;
			}
			else
			{
				typeMapping = null;
			}
			bool flag = true;
			string text;
			string text2;
			if (attributes.XmlType != null)
			{
				text = attributes.XmlType.Namespace;
				text2 = this.XsdTypeName(type, attributes, attributes.XmlType.TypeName);
				text2 = XmlConvert.EncodeLocalName(text2);
				flag = text2 == null;
			}
			else if (typeMapping is EnumMapping)
			{
				text = typeMapping.Namespace;
				text2 = typeMapping.DefaultElementName;
			}
			else if (typeMapping is PrimitiveMapping)
			{
				text = defaultNs;
				text2 = typeMapping.TypeDesc.DataType.Name;
			}
			else if (typeMapping is StructMapping && typeMapping.TypeDesc.IsRoot)
			{
				text = defaultNs;
				text2 = "anyType";
			}
			else if (typeMapping != null)
			{
				text = ((typeMapping.Namespace == "http://www.w3.org/2001/XMLSchema") ? defaultNs : typeMapping.Namespace);
				text2 = typeMapping.DefaultElementName;
			}
			else
			{
				text = defaultNs;
				object obj = "Choice";
				int num = this.choiceNum;
				this.choiceNum = num + 1;
				text2 = obj + num;
			}
			if (text2 == null)
			{
				text2 = "Any";
			}
			if (elementAccessor != null)
			{
				text = elementAccessor.Namespace;
			}
			if (text == null)
			{
				text = defaultNs;
			}
			string text3;
			text2 = (text3 = (flag ? ("ArrayOf" + CodeIdentifier.MakePascal(text2)) : text2));
			int num2 = 1;
			TypeMapping typeMapping2 = (TypeMapping)this.types[text3, text];
			while (typeMapping2 != null && (!(typeMapping2 is ArrayMapping) || !AccessorMapping.ElementsMatch(((ArrayMapping)typeMapping2).Elements, mapping.Elements)))
			{
				text3 = text2 + num2.ToString(CultureInfo.InvariantCulture);
				typeMapping2 = (TypeMapping)this.types[text3, text];
				num2++;
			}
			mapping.TypeName = text3;
			mapping.Namespace = text;
		}

		private ArrayMapping ImportArrayLikeMapping(ArrayModel model, string ns, RecursionLimiter limiter)
		{
			ArrayMapping arrayMapping = new ArrayMapping();
			arrayMapping.TypeDesc = model.TypeDesc;
			if (this.savedArrayItemAttributes == null)
			{
				this.savedArrayItemAttributes = new XmlArrayItemAttributes();
			}
			if (XmlReflectionImporter.CountAtLevel(this.savedArrayItemAttributes, this.arrayNestingLevel) == 0)
			{
				this.savedArrayItemAttributes.Add(XmlReflectionImporter.CreateArrayItemAttribute(this.typeScope.GetTypeDesc(model.Element.Type), this.arrayNestingLevel));
			}
			this.CreateArrayElementsFromAttributes(arrayMapping, this.savedArrayItemAttributes, model.Element.Type, (this.savedArrayNamespace == null) ? ns : this.savedArrayNamespace, limiter);
			this.SetArrayMappingType(arrayMapping, ns, model.Type);
			for (int i = 0; i < arrayMapping.Elements.Length; i++)
			{
				arrayMapping.Elements[i] = this.ReconcileLocalAccessor(arrayMapping.Elements[i], arrayMapping.Namespace);
			}
			this.IncludeTypes(model.Type);
			ArrayMapping arrayMapping2 = (ArrayMapping)this.types[arrayMapping.TypeName, arrayMapping.Namespace];
			if (arrayMapping2 != null)
			{
				ArrayMapping arrayMapping3 = arrayMapping2;
				while (arrayMapping2 != null)
				{
					if (arrayMapping2.TypeDesc == model.TypeDesc)
					{
						return arrayMapping2;
					}
					arrayMapping2 = arrayMapping2.Next;
				}
				arrayMapping.Next = arrayMapping3;
				if (!arrayMapping.IsAnonymousType)
				{
					this.types[arrayMapping.TypeName, arrayMapping.Namespace] = arrayMapping;
				}
				else
				{
					this.anonymous[model.Type] = arrayMapping;
				}
				return arrayMapping;
			}
			this.typeScope.AddTypeMapping(arrayMapping);
			if (!arrayMapping.IsAnonymousType)
			{
				this.types.Add(arrayMapping.TypeName, arrayMapping.Namespace, arrayMapping);
			}
			else
			{
				this.anonymous[model.Type] = arrayMapping;
			}
			return arrayMapping;
		}

		private void CheckContext(TypeDesc typeDesc, XmlReflectionImporter.ImportContext context)
		{
			switch (context)
			{
			case XmlReflectionImporter.ImportContext.Text:
				if (typeDesc.CanBeTextValue || typeDesc.IsEnum || typeDesc.IsPrimitive)
				{
					return;
				}
				break;
			case XmlReflectionImporter.ImportContext.Attribute:
				if (typeDesc.CanBeAttributeValue)
				{
					return;
				}
				break;
			case XmlReflectionImporter.ImportContext.Element:
				if (typeDesc.CanBeElementValue)
				{
					return;
				}
				break;
			default:
				throw new ArgumentException(Res.GetString("Internal error."), "context");
			}
			throw XmlReflectionImporter.UnsupportedException(typeDesc, context);
		}

		private PrimitiveMapping ImportPrimitiveMapping(PrimitiveModel model, XmlReflectionImporter.ImportContext context, string dataType, bool repeats)
		{
			PrimitiveMapping primitiveMapping = new PrimitiveMapping();
			if (dataType.Length > 0)
			{
				primitiveMapping.TypeDesc = this.typeScope.GetTypeDesc(dataType, "http://www.w3.org/2001/XMLSchema");
				if (primitiveMapping.TypeDesc == null)
				{
					primitiveMapping.TypeDesc = this.typeScope.GetTypeDesc(dataType, "http://microsoft.com/wsdl/types/");
					if (primitiveMapping.TypeDesc == null)
					{
						throw new InvalidOperationException(Res.GetString("The type, {0}, is undeclared.", new object[] { dataType }));
					}
				}
			}
			else
			{
				primitiveMapping.TypeDesc = model.TypeDesc;
			}
			primitiveMapping.TypeName = primitiveMapping.TypeDesc.DataType.Name;
			primitiveMapping.Namespace = (primitiveMapping.TypeDesc.IsXsdType ? "http://www.w3.org/2001/XMLSchema" : "http://microsoft.com/wsdl/types/");
			primitiveMapping.IsList = repeats;
			this.CheckContext(primitiveMapping.TypeDesc, context);
			return primitiveMapping;
		}

		private EnumMapping ImportEnumMapping(EnumModel model, string ns, bool repeats)
		{
			XmlAttributes attributes = this.GetAttributes(model.Type, false);
			string text = ns;
			if (attributes.XmlType != null && attributes.XmlType.Namespace != null)
			{
				text = attributes.XmlType.Namespace;
			}
			string text2 = (XmlReflectionImporter.IsAnonymousType(attributes, ns) ? null : this.XsdTypeName(model.Type, attributes, model.TypeDesc.Name));
			text2 = XmlConvert.EncodeLocalName(text2);
			EnumMapping enumMapping = (EnumMapping)this.GetTypeMapping(text2, text, model.TypeDesc, this.types, model.Type);
			if (enumMapping == null)
			{
				enumMapping = new EnumMapping();
				enumMapping.TypeDesc = model.TypeDesc;
				enumMapping.TypeName = text2;
				enumMapping.Namespace = text;
				enumMapping.IsFlags = model.Type.IsDefined(typeof(FlagsAttribute), false);
				if (enumMapping.IsFlags && repeats)
				{
					throw new InvalidOperationException(Res.GetString("XmlAttribute cannot be used to encode array of {1}, because it is marked with FlagsAttribute.", new object[] { model.TypeDesc.FullName }));
				}
				enumMapping.IsList = repeats;
				enumMapping.IncludeInSchema = attributes.XmlType == null || attributes.XmlType.IncludeInSchema;
				if (!enumMapping.IsAnonymousType)
				{
					this.types.Add(text2, text, enumMapping);
				}
				else
				{
					this.anonymous[model.Type] = enumMapping;
				}
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < model.Constants.Length; i++)
				{
					ConstantMapping constantMapping = this.ImportConstantMapping(model.Constants[i]);
					if (constantMapping != null)
					{
						arrayList.Add(constantMapping);
					}
				}
				if (arrayList.Count == 0)
				{
					throw new InvalidOperationException(Res.GetString("Cannot serialize object of type '{0}'. The object does not have serializable members.", new object[] { model.TypeDesc.FullName }));
				}
				enumMapping.Constants = (ConstantMapping[])arrayList.ToArray(typeof(ConstantMapping));
				this.typeScope.AddTypeMapping(enumMapping);
			}
			return enumMapping;
		}

		private ConstantMapping ImportConstantMapping(ConstantModel model)
		{
			XmlAttributes attributes = this.GetAttributes(model.FieldInfo);
			if (attributes.XmlIgnore)
			{
				return null;
			}
			if ((attributes.XmlFlags & (XmlAttributeFlags)(-2)) != (XmlAttributeFlags)0)
			{
				throw new InvalidOperationException(Res.GetString("Only XmlEnum may be used on enumerated constants."));
			}
			if (attributes.XmlEnum == null)
			{
				attributes.XmlEnum = new XmlEnumAttribute();
			}
			return new ConstantMapping
			{
				XmlName = ((attributes.XmlEnum.Name == null) ? model.Name : attributes.XmlEnum.Name),
				Name = model.Name,
				Value = model.Value
			};
		}

		private MembersMapping ImportMembersMapping(XmlReflectionMember[] xmlReflectionMembers, string ns, bool hasWrapperElement, bool rpc, bool openModel, RecursionLimiter limiter)
		{
			MembersMapping membersMapping = new MembersMapping();
			membersMapping.TypeDesc = this.typeScope.GetTypeDesc(typeof(object[]));
			MemberMapping[] array = new MemberMapping[xmlReflectionMembers.Length];
			NameTable nameTable = new NameTable();
			NameTable nameTable2 = new NameTable();
			TextAccessor textAccessor = null;
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					MemberMapping memberMapping = this.ImportMemberMapping(xmlReflectionMembers[i], ns, xmlReflectionMembers, rpc, openModel, limiter);
					if (!hasWrapperElement && memberMapping.Attribute != null)
					{
						if (rpc)
						{
							throw new InvalidOperationException(Res.GetString("XmlAttribute and XmlAnyAttribute cannot be used with rpc\\literal SOAP, you may use the following attributes: XmlArray, XmlArrayItem, or single XmlElement."));
						}
						throw new InvalidOperationException(Res.GetString("{0} may not be used on parameters or return values when they are not wrapped.", new object[] { "XmlAttribute" }));
					}
					else
					{
						if (rpc && xmlReflectionMembers[i].IsReturnValue)
						{
							if (i > 0)
							{
								throw new InvalidOperationException(Res.GetString("The return value must be the first member."));
							}
							memberMapping.IsReturnValue = true;
						}
						array[i] = memberMapping;
						flag |= memberMapping.IsSequence;
						if (!xmlReflectionMembers[i].XmlAttributes.XmlIgnore)
						{
							XmlReflectionImporter.AddUniqueAccessor(memberMapping, nameTable, nameTable2, flag);
						}
						array[i] = memberMapping;
						if (memberMapping.Text != null)
						{
							if (textAccessor != null)
							{
								throw new InvalidOperationException(Res.GetString("XmlText may not be used on multiple parameters or return values."));
							}
							textAccessor = memberMapping.Text;
						}
						if (memberMapping.Xmlns != null)
						{
							if (membersMapping.XmlnsMember != null)
							{
								throw new InvalidOperationException(Res.GetString("XmlNamespaceDeclarations may not be used on multiple parameters or return values."));
							}
							membersMapping.XmlnsMember = memberMapping;
						}
					}
				}
				catch (Exception ex)
				{
					if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
					{
						throw;
					}
					throw this.CreateReflectionException(xmlReflectionMembers[i].MemberName, ex);
				}
			}
			if (flag)
			{
				throw new InvalidOperationException(Res.GetString("Explicit sequencing may not be used on parameters or return values.  Please remove {0} property from custom attributes.", new object[] { "Order" }));
			}
			membersMapping.Members = array;
			membersMapping.HasWrapperElement = hasWrapperElement;
			return membersMapping;
		}

		private MemberMapping ImportMemberMapping(XmlReflectionMember xmlReflectionMember, string ns, XmlReflectionMember[] xmlReflectionMembers, bool rpc, bool openModel, RecursionLimiter limiter)
		{
			XmlSchemaForm xmlSchemaForm = (rpc ? XmlSchemaForm.Unqualified : XmlSchemaForm.Qualified);
			XmlAttributes xmlAttributes = xmlReflectionMember.XmlAttributes;
			TypeDesc typeDesc = this.typeScope.GetTypeDesc(xmlReflectionMember.MemberType);
			if (xmlAttributes.XmlFlags == (XmlAttributeFlags)0)
			{
				if (typeDesc.IsArrayLike)
				{
					XmlArrayAttribute xmlArrayAttribute = XmlReflectionImporter.CreateArrayAttribute(typeDesc);
					xmlArrayAttribute.ElementName = xmlReflectionMember.MemberName;
					xmlArrayAttribute.Namespace = (rpc ? null : ns);
					xmlArrayAttribute.Form = xmlSchemaForm;
					xmlAttributes.XmlArray = xmlArrayAttribute;
				}
				else
				{
					XmlElementAttribute xmlElementAttribute = XmlReflectionImporter.CreateElementAttribute(typeDesc);
					if (typeDesc.IsStructLike)
					{
						XmlAttributes xmlAttributes2 = new XmlAttributes(xmlReflectionMember.MemberType);
						if (xmlAttributes2.XmlRoot != null)
						{
							if (xmlAttributes2.XmlRoot.ElementName.Length > 0)
							{
								xmlElementAttribute.ElementName = xmlAttributes2.XmlRoot.ElementName;
							}
							if (rpc)
							{
								xmlElementAttribute.Namespace = null;
								if (xmlAttributes2.XmlRoot.IsNullableSpecified)
								{
									xmlElementAttribute.IsNullable = xmlAttributes2.XmlRoot.IsNullable;
								}
							}
							else
							{
								xmlElementAttribute.Namespace = xmlAttributes2.XmlRoot.Namespace;
								xmlElementAttribute.IsNullable = xmlAttributes2.XmlRoot.IsNullable;
							}
						}
					}
					if (xmlElementAttribute.ElementName.Length == 0)
					{
						xmlElementAttribute.ElementName = xmlReflectionMember.MemberName;
					}
					if (xmlElementAttribute.Namespace == null && !rpc)
					{
						xmlElementAttribute.Namespace = ns;
					}
					xmlElementAttribute.Form = xmlSchemaForm;
					xmlAttributes.XmlElements.Add(xmlElementAttribute);
				}
			}
			else if (xmlAttributes.XmlRoot != null)
			{
				XmlReflectionImporter.CheckNullable(xmlAttributes.XmlRoot.IsNullable, typeDesc, null);
			}
			MemberMapping memberMapping = new MemberMapping();
			memberMapping.Name = xmlReflectionMember.MemberName;
			bool flag = XmlReflectionImporter.FindSpecifiedMember(xmlReflectionMember.MemberName, xmlReflectionMembers) != null;
			FieldModel fieldModel = new FieldModel(xmlReflectionMember.MemberName, xmlReflectionMember.MemberType, this.typeScope.GetTypeDesc(xmlReflectionMember.MemberType), flag, false);
			memberMapping.CheckShouldPersist = fieldModel.CheckShouldPersist;
			memberMapping.CheckSpecified = fieldModel.CheckSpecified;
			memberMapping.ReadOnly = fieldModel.ReadOnly;
			Type type = null;
			if (xmlAttributes.XmlChoiceIdentifier != null)
			{
				type = this.GetChoiceIdentifierType(xmlAttributes.XmlChoiceIdentifier, xmlReflectionMembers, typeDesc.IsArrayLike, fieldModel.Name);
			}
			this.ImportAccessorMapping(memberMapping, fieldModel, xmlAttributes, ns, type, rpc, openModel, limiter);
			if (xmlReflectionMember.OverrideIsNullable && memberMapping.Elements.Length != 0)
			{
				memberMapping.Elements[0].IsNullable = false;
			}
			return memberMapping;
		}

		internal static XmlReflectionMember FindSpecifiedMember(string memberName, XmlReflectionMember[] reflectionMembers)
		{
			for (int i = 0; i < reflectionMembers.Length; i++)
			{
				if (string.Compare(reflectionMembers[i].MemberName, memberName + "Specified", StringComparison.Ordinal) == 0)
				{
					return reflectionMembers[i];
				}
			}
			return null;
		}

		private MemberMapping ImportFieldMapping(StructModel parent, FieldModel model, XmlAttributes a, string ns, RecursionLimiter limiter)
		{
			MemberMapping memberMapping = new MemberMapping();
			memberMapping.Name = model.Name;
			memberMapping.CheckShouldPersist = model.CheckShouldPersist;
			memberMapping.CheckSpecified = model.CheckSpecified;
			memberMapping.MemberInfo = model.MemberInfo;
			memberMapping.CheckSpecifiedMemberInfo = model.CheckSpecifiedMemberInfo;
			memberMapping.CheckShouldPersistMethodInfo = model.CheckShouldPersistMethodInfo;
			memberMapping.ReadOnly = model.ReadOnly;
			Type type = null;
			if (a.XmlChoiceIdentifier != null)
			{
				type = this.GetChoiceIdentifierType(a.XmlChoiceIdentifier, parent, model.FieldTypeDesc.IsArrayLike, model.Name);
			}
			this.ImportAccessorMapping(memberMapping, model, a, ns, type, false, false, limiter);
			return memberMapping;
		}

		private Type CheckChoiceIdentifierType(Type type, bool isArrayLike, string identifierName, string memberName)
		{
			if (type.IsArray)
			{
				if (!isArrayLike)
				{
					throw new InvalidOperationException(Res.GetString("Type of choice identifier '{0}' is inconsistent with type of '{1}'. Please use {2}.", new object[]
					{
						identifierName,
						memberName,
						type.GetElementType().FullName
					}));
				}
				type = type.GetElementType();
			}
			else if (isArrayLike)
			{
				throw new InvalidOperationException(Res.GetString("Type of choice identifier '{0}' is inconsistent with type of '{1}'. Please use array of {2}.", new object[] { identifierName, memberName, type.FullName }));
			}
			if (!type.IsEnum)
			{
				throw new InvalidOperationException(Res.GetString("Choice identifier '{0}' must be an enum.", new object[] { identifierName }));
			}
			return type;
		}

		private Type GetChoiceIdentifierType(XmlChoiceIdentifierAttribute choice, XmlReflectionMember[] xmlReflectionMembers, bool isArrayLike, string accessorName)
		{
			for (int i = 0; i < xmlReflectionMembers.Length; i++)
			{
				if (choice.MemberName == xmlReflectionMembers[i].MemberName)
				{
					return this.CheckChoiceIdentifierType(xmlReflectionMembers[i].MemberType, isArrayLike, choice.MemberName, accessorName);
				}
			}
			throw new InvalidOperationException(Res.GetString("Missing '{0}' member needed for serialization of choice '{1}'.", new object[] { choice.MemberName, accessorName }));
		}

		private Type GetChoiceIdentifierType(XmlChoiceIdentifierAttribute choice, StructModel structModel, bool isArrayLike, string accessorName)
		{
			MemberInfo[] array = structModel.Type.GetMember(choice.MemberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			if (array == null || array.Length == 0)
			{
				PropertyInfo property = structModel.Type.GetProperty(choice.MemberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
				if (property == null)
				{
					throw new InvalidOperationException(Res.GetString("Missing '{0}' member needed for serialization of choice '{1}'.", new object[] { choice.MemberName, accessorName }));
				}
				array = new MemberInfo[] { property };
			}
			else if (array.Length > 1)
			{
				throw new InvalidOperationException(Res.GetString("Ambiguous choice identifier. There are several members named '{0}'.", new object[] { choice.MemberName }));
			}
			FieldModel fieldModel = structModel.GetFieldModel(array[0]);
			if (fieldModel == null)
			{
				throw new InvalidOperationException(Res.GetString("Missing '{0}' member needed for serialization of choice '{1}'.", new object[] { choice.MemberName, accessorName }));
			}
			choice.MemberInfo = fieldModel.MemberInfo;
			Type fieldType = fieldModel.FieldType;
			return this.CheckChoiceIdentifierType(fieldType, isArrayLike, choice.MemberName, accessorName);
		}

		private void CreateArrayElementsFromAttributes(ArrayMapping arrayMapping, XmlArrayItemAttributes attributes, Type arrayElementType, string arrayElementNs, RecursionLimiter limiter)
		{
			NameTable nameTable = new NameTable();
			int num = 0;
			while (attributes != null && num < attributes.Count)
			{
				XmlArrayItemAttribute xmlArrayItemAttribute = attributes[num];
				if (xmlArrayItemAttribute.NestingLevel == this.arrayNestingLevel)
				{
					Type type = ((xmlArrayItemAttribute.Type != null) ? xmlArrayItemAttribute.Type : arrayElementType);
					TypeDesc typeDesc = this.typeScope.GetTypeDesc(type);
					ElementAccessor elementAccessor = new ElementAccessor();
					elementAccessor.Namespace = ((xmlArrayItemAttribute.Namespace == null) ? arrayElementNs : xmlArrayItemAttribute.Namespace);
					elementAccessor.Mapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(type), elementAccessor.Namespace, XmlReflectionImporter.ImportContext.Element, xmlArrayItemAttribute.DataType, null, limiter);
					elementAccessor.Name = ((xmlArrayItemAttribute.ElementName.Length == 0) ? elementAccessor.Mapping.DefaultElementName : XmlConvert.EncodeLocalName(xmlArrayItemAttribute.ElementName));
					elementAccessor.IsNullable = (xmlArrayItemAttribute.IsNullableSpecified ? xmlArrayItemAttribute.IsNullable : (typeDesc.IsNullable || typeDesc.IsOptionalValue));
					elementAccessor.Form = ((xmlArrayItemAttribute.Form == XmlSchemaForm.None) ? XmlSchemaForm.Qualified : xmlArrayItemAttribute.Form);
					XmlReflectionImporter.CheckForm(elementAccessor.Form, arrayElementNs != elementAccessor.Namespace);
					XmlReflectionImporter.CheckNullable(elementAccessor.IsNullable, typeDesc, elementAccessor.Mapping);
					XmlReflectionImporter.AddUniqueAccessor(nameTable, elementAccessor);
				}
				num++;
			}
			arrayMapping.Elements = (ElementAccessor[])nameTable.ToArray(typeof(ElementAccessor));
		}

		private void ImportAccessorMapping(MemberMapping accessor, FieldModel model, XmlAttributes a, string ns, Type choiceIdentifierType, bool rpc, bool openModel, RecursionLimiter limiter)
		{
			XmlSchemaForm xmlSchemaForm = XmlSchemaForm.Qualified;
			int num = this.arrayNestingLevel;
			int num2 = -1;
			XmlArrayItemAttributes xmlArrayItemAttributes = this.savedArrayItemAttributes;
			string text = this.savedArrayNamespace;
			this.arrayNestingLevel = 0;
			this.savedArrayItemAttributes = null;
			this.savedArrayNamespace = null;
			Type fieldType = model.FieldType;
			string name = model.Name;
			ArrayList arrayList = new ArrayList();
			NameTable nameTable = new NameTable();
			accessor.TypeDesc = this.typeScope.GetTypeDesc(fieldType);
			XmlAttributeFlags xmlFlags = a.XmlFlags;
			accessor.Ignore = a.XmlIgnore;
			if (rpc)
			{
				this.CheckTopLevelAttributes(a, name);
			}
			else
			{
				this.CheckAmbiguousChoice(a, fieldType, name);
			}
			XmlAttributeFlags xmlAttributeFlags = (XmlAttributeFlags)1300;
			XmlAttributeFlags xmlAttributeFlags2 = (XmlAttributeFlags)544;
			XmlAttributeFlags xmlAttributeFlags3 = (XmlAttributeFlags)10;
			if ((xmlFlags & xmlAttributeFlags3) != (XmlAttributeFlags)0 && fieldType == typeof(byte[]))
			{
				accessor.TypeDesc = this.typeScope.GetArrayTypeDesc(fieldType);
			}
			if (a.XmlChoiceIdentifier != null)
			{
				accessor.ChoiceIdentifier = new ChoiceIdentifierAccessor();
				accessor.ChoiceIdentifier.MemberName = a.XmlChoiceIdentifier.MemberName;
				accessor.ChoiceIdentifier.MemberInfo = a.XmlChoiceIdentifier.MemberInfo;
				accessor.ChoiceIdentifier.Mapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(choiceIdentifierType), ns, XmlReflectionImporter.ImportContext.Element, string.Empty, null, limiter);
				this.CheckChoiceIdentifierMapping((EnumMapping)accessor.ChoiceIdentifier.Mapping);
			}
			if (accessor.TypeDesc.IsArrayLike)
			{
				Type arrayElementType = TypeScope.GetArrayElementType(fieldType, model.FieldTypeDesc.FullName + "." + model.Name);
				if ((xmlFlags & xmlAttributeFlags2) != (XmlAttributeFlags)0)
				{
					if ((xmlFlags & xmlAttributeFlags2) != xmlFlags)
					{
						throw new InvalidOperationException(Res.GetString("XmlAttribute and XmlAnyAttribute cannot be used in conjunction with XmlElement, XmlText, XmlAnyElement, XmlArray, or XmlArrayItem."));
					}
					if (a.XmlAttribute != null && !accessor.TypeDesc.ArrayElementTypeDesc.IsPrimitive && !accessor.TypeDesc.ArrayElementTypeDesc.IsEnum)
					{
						if (accessor.TypeDesc.ArrayElementTypeDesc.Kind == TypeKind.Serializable)
						{
							throw new InvalidOperationException(Res.GetString("Cannot serialize member '{0}' of type {1}. XmlAttribute/XmlText cannot be used to encode types implementing {2}.", new object[]
							{
								name,
								accessor.TypeDesc.ArrayElementTypeDesc.FullName,
								typeof(IXmlSerializable).Name
							}));
						}
						throw new InvalidOperationException(Res.GetString("Cannot serialize member '{0}' of type {1}. XmlAttribute/XmlText cannot be used to encode complex types.", new object[]
						{
							name,
							accessor.TypeDesc.ArrayElementTypeDesc.FullName
						}));
					}
					else
					{
						bool flag = a.XmlAttribute != null && (accessor.TypeDesc.ArrayElementTypeDesc.IsPrimitive || accessor.TypeDesc.ArrayElementTypeDesc.IsEnum);
						if (a.XmlAnyAttribute != null)
						{
							a.XmlAttribute = new XmlAttributeAttribute();
						}
						AttributeAccessor attributeAccessor = new AttributeAccessor();
						Type type = ((a.XmlAttribute.Type == null) ? arrayElementType : a.XmlAttribute.Type);
						this.typeScope.GetTypeDesc(type);
						attributeAccessor.Name = Accessor.EscapeQName((a.XmlAttribute.AttributeName.Length == 0) ? name : a.XmlAttribute.AttributeName);
						attributeAccessor.Namespace = ((a.XmlAttribute.Namespace == null) ? ns : a.XmlAttribute.Namespace);
						attributeAccessor.Form = a.XmlAttribute.Form;
						if (attributeAccessor.Form == XmlSchemaForm.None && ns != attributeAccessor.Namespace)
						{
							attributeAccessor.Form = XmlSchemaForm.Qualified;
						}
						attributeAccessor.CheckSpecial();
						XmlReflectionImporter.CheckForm(attributeAccessor.Form, ns != attributeAccessor.Namespace);
						attributeAccessor.Mapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(type), ns, XmlReflectionImporter.ImportContext.Attribute, a.XmlAttribute.DataType, null, flag, false, limiter);
						attributeAccessor.IsList = flag;
						attributeAccessor.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
						attributeAccessor.Any = a.XmlAnyAttribute != null;
						if (attributeAccessor.Form == XmlSchemaForm.Qualified && attributeAccessor.Namespace != ns)
						{
							if (this.xsdAttributes == null)
							{
								this.xsdAttributes = new NameTable();
							}
							attributeAccessor = (AttributeAccessor)this.ReconcileAccessor(attributeAccessor, this.xsdAttributes);
						}
						accessor.Attribute = attributeAccessor;
					}
				}
				else if ((xmlFlags & xmlAttributeFlags) != (XmlAttributeFlags)0)
				{
					if ((xmlFlags & xmlAttributeFlags) != xmlFlags)
					{
						throw new InvalidOperationException(Res.GetString("XmlElement, XmlText, and XmlAnyElement cannot be used in conjunction with XmlAttribute, XmlAnyAttribute, XmlArray, or XmlArrayItem."));
					}
					if (a.XmlText != null)
					{
						TextAccessor textAccessor = new TextAccessor();
						Type type2 = ((a.XmlText.Type == null) ? arrayElementType : a.XmlText.Type);
						TypeDesc typeDesc = this.typeScope.GetTypeDesc(type2);
						textAccessor.Name = name;
						textAccessor.Mapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(type2), ns, XmlReflectionImporter.ImportContext.Text, a.XmlText.DataType, null, true, false, limiter);
						if (!(textAccessor.Mapping is SpecialMapping) && typeDesc != this.typeScope.GetTypeDesc(typeof(string)))
						{
							throw new InvalidOperationException(Res.GetString("Member '{0}' cannot be encoded using the XmlText attribute. You may use the XmlText attribute to encode primitives, enumerations, arrays of strings, or arrays of XmlNode.", new object[] { name }));
						}
						accessor.Text = textAccessor;
					}
					if (a.XmlText == null && a.XmlElements.Count == 0 && a.XmlAnyElements.Count == 0)
					{
						a.XmlElements.Add(XmlReflectionImporter.CreateElementAttribute(accessor.TypeDesc));
					}
					for (int i = 0; i < a.XmlElements.Count; i++)
					{
						XmlElementAttribute xmlElementAttribute = a.XmlElements[i];
						Type type3 = ((xmlElementAttribute.Type == null) ? arrayElementType : xmlElementAttribute.Type);
						TypeDesc typeDesc2 = this.typeScope.GetTypeDesc(type3);
						TypeModel typeModel = this.modelScope.GetTypeModel(type3);
						ElementAccessor elementAccessor = new ElementAccessor();
						elementAccessor.Namespace = (rpc ? null : ((xmlElementAttribute.Namespace == null) ? ns : xmlElementAttribute.Namespace));
						elementAccessor.Mapping = this.ImportTypeMapping(typeModel, rpc ? ns : elementAccessor.Namespace, XmlReflectionImporter.ImportContext.Element, xmlElementAttribute.DataType, null, limiter);
						if (a.XmlElements.Count == 1)
						{
							elementAccessor.Name = XmlConvert.EncodeLocalName((xmlElementAttribute.ElementName.Length == 0) ? name : xmlElementAttribute.ElementName);
						}
						else
						{
							elementAccessor.Name = ((xmlElementAttribute.ElementName.Length == 0) ? elementAccessor.Mapping.DefaultElementName : XmlConvert.EncodeLocalName(xmlElementAttribute.ElementName));
						}
						elementAccessor.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
						if (xmlElementAttribute.IsNullableSpecified && !xmlElementAttribute.IsNullable && typeModel.TypeDesc.IsOptionalValue)
						{
							throw new InvalidOperationException(Res.GetString("IsNullable may not be set to 'false' for a Nullable<{0}> type. Consider using '{0}' type or removing the IsNullable property from the {1} attribute.", new object[]
							{
								typeModel.TypeDesc.BaseTypeDesc.FullName,
								"XmlElement"
							}));
						}
						elementAccessor.IsNullable = (xmlElementAttribute.IsNullableSpecified ? xmlElementAttribute.IsNullable : typeModel.TypeDesc.IsOptionalValue);
						elementAccessor.Form = (rpc ? XmlSchemaForm.Unqualified : ((xmlElementAttribute.Form == XmlSchemaForm.None) ? xmlSchemaForm : xmlElementAttribute.Form));
						XmlReflectionImporter.CheckNullable(elementAccessor.IsNullable, typeDesc2, elementAccessor.Mapping);
						if (!rpc)
						{
							XmlReflectionImporter.CheckForm(elementAccessor.Form, ns != elementAccessor.Namespace);
							elementAccessor = this.ReconcileLocalAccessor(elementAccessor, ns);
						}
						if (xmlElementAttribute.Order != -1)
						{
							if (xmlElementAttribute.Order != num2 && num2 != -1)
							{
								throw new InvalidOperationException(Res.GetString("If multiple custom attributes specified on a single member only one of them have to have explicit '{0}' property, however if more that one attribute has the explicit '{0}', all values have to match.", new object[] { "Order" }));
							}
							num2 = xmlElementAttribute.Order;
						}
						XmlReflectionImporter.AddUniqueAccessor(nameTable, elementAccessor);
						arrayList.Add(elementAccessor);
					}
					NameTable nameTable2 = new NameTable();
					for (int j = 0; j < a.XmlAnyElements.Count; j++)
					{
						XmlAnyElementAttribute xmlAnyElementAttribute = a.XmlAnyElements[j];
						Type type4 = (typeof(IXmlSerializable).IsAssignableFrom(arrayElementType) ? arrayElementType : (typeof(XmlNode).IsAssignableFrom(arrayElementType) ? arrayElementType : typeof(XmlElement)));
						if (!arrayElementType.IsAssignableFrom(type4))
						{
							throw new InvalidOperationException(Res.GetString("Cannot serialize member of type {0}: XmlAnyElement can only be used with classes of type XmlNode or a type deriving from XmlNode.", new object[] { arrayElementType.FullName }));
						}
						string text2 = ((xmlAnyElementAttribute.Name.Length == 0) ? xmlAnyElementAttribute.Name : XmlConvert.EncodeLocalName(xmlAnyElementAttribute.Name));
						string text3 = (xmlAnyElementAttribute.NamespaceSpecified ? xmlAnyElementAttribute.Namespace : null);
						if (nameTable2[text2, text3] == null)
						{
							nameTable2[text2, text3] = xmlAnyElementAttribute;
							if (nameTable[text2, (text3 == null) ? ns : text3] != null)
							{
								throw new InvalidOperationException(Res.GetString("The element '{0}' has been attributed with duplicate XmlAnyElementAttribute(Name=\"{1}\", Namespace=\"{2}\").", new object[]
								{
									name,
									xmlAnyElementAttribute.Name,
									(xmlAnyElementAttribute.Namespace == null) ? "null" : xmlAnyElementAttribute.Namespace
								}));
							}
							ElementAccessor elementAccessor2 = new ElementAccessor();
							elementAccessor2.Name = text2;
							elementAccessor2.Namespace = ((text3 == null) ? ns : text3);
							elementAccessor2.Any = true;
							elementAccessor2.AnyNamespaces = text3;
							TypeDesc typeDesc3 = this.typeScope.GetTypeDesc(type4);
							TypeModel typeModel2 = this.modelScope.GetTypeModel(type4);
							if (elementAccessor2.Name.Length > 0)
							{
								typeModel2.TypeDesc.IsMixed = true;
							}
							elementAccessor2.Mapping = this.ImportTypeMapping(typeModel2, elementAccessor2.Namespace, XmlReflectionImporter.ImportContext.Element, string.Empty, null, limiter);
							elementAccessor2.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
							elementAccessor2.IsNullable = false;
							elementAccessor2.Form = xmlSchemaForm;
							XmlReflectionImporter.CheckNullable(elementAccessor2.IsNullable, typeDesc3, elementAccessor2.Mapping);
							if (!rpc)
							{
								XmlReflectionImporter.CheckForm(elementAccessor2.Form, ns != elementAccessor2.Namespace);
								elementAccessor2 = this.ReconcileLocalAccessor(elementAccessor2, ns);
							}
							nameTable.Add(elementAccessor2.Name, elementAccessor2.Namespace, elementAccessor2);
							arrayList.Add(elementAccessor2);
							if (xmlAnyElementAttribute.Order != -1)
							{
								if (xmlAnyElementAttribute.Order != num2 && num2 != -1)
								{
									throw new InvalidOperationException(Res.GetString("If multiple custom attributes specified on a single member only one of them have to have explicit '{0}' property, however if more that one attribute has the explicit '{0}', all values have to match.", new object[] { "Order" }));
								}
								num2 = xmlAnyElementAttribute.Order;
							}
						}
					}
				}
				else
				{
					if ((xmlFlags & xmlAttributeFlags3) != (XmlAttributeFlags)0 && (xmlFlags & xmlAttributeFlags3) != xmlFlags)
					{
						throw new InvalidOperationException(Res.GetString("XmlArray and XmlArrayItem cannot be used in conjunction with XmlAttribute, XmlAnyAttribute, XmlElement, XmlText, or XmlAnyElement."));
					}
					TypeDesc typeDesc4 = this.typeScope.GetTypeDesc(arrayElementType);
					if (a.XmlArray == null)
					{
						a.XmlArray = XmlReflectionImporter.CreateArrayAttribute(accessor.TypeDesc);
					}
					if (XmlReflectionImporter.CountAtLevel(a.XmlArrayItems, this.arrayNestingLevel) == 0)
					{
						a.XmlArrayItems.Add(XmlReflectionImporter.CreateArrayItemAttribute(typeDesc4, this.arrayNestingLevel));
					}
					ElementAccessor elementAccessor3 = new ElementAccessor();
					elementAccessor3.Name = XmlConvert.EncodeLocalName((a.XmlArray.ElementName.Length == 0) ? name : a.XmlArray.ElementName);
					elementAccessor3.Namespace = (rpc ? null : ((a.XmlArray.Namespace == null) ? ns : a.XmlArray.Namespace));
					this.savedArrayItemAttributes = a.XmlArrayItems;
					this.savedArrayNamespace = elementAccessor3.Namespace;
					ArrayMapping arrayMapping = this.ImportArrayLikeMapping(this.modelScope.GetArrayModel(fieldType), ns, limiter);
					elementAccessor3.Mapping = arrayMapping;
					elementAccessor3.IsNullable = a.XmlArray.IsNullable;
					elementAccessor3.Form = (rpc ? XmlSchemaForm.Unqualified : ((a.XmlArray.Form == XmlSchemaForm.None) ? xmlSchemaForm : a.XmlArray.Form));
					num2 = a.XmlArray.Order;
					XmlReflectionImporter.CheckNullable(elementAccessor3.IsNullable, accessor.TypeDesc, elementAccessor3.Mapping);
					if (!rpc)
					{
						XmlReflectionImporter.CheckForm(elementAccessor3.Form, ns != elementAccessor3.Namespace);
						elementAccessor3 = this.ReconcileLocalAccessor(elementAccessor3, ns);
					}
					this.savedArrayItemAttributes = null;
					this.savedArrayNamespace = null;
					XmlReflectionImporter.AddUniqueAccessor(nameTable, elementAccessor3);
					arrayList.Add(elementAccessor3);
				}
			}
			else if (!accessor.TypeDesc.IsVoid)
			{
				XmlAttributeFlags xmlAttributeFlags4 = (XmlAttributeFlags)3380;
				if ((xmlFlags & xmlAttributeFlags4) != xmlFlags)
				{
					throw new InvalidOperationException(Res.GetString("For non-array types, you may use the following attributes: XmlAttribute, XmlText, XmlElement, or XmlAnyElement."));
				}
				if (accessor.TypeDesc.IsPrimitive || accessor.TypeDesc.IsEnum)
				{
					if (a.XmlAnyElements.Count > 0)
					{
						throw new InvalidOperationException(Res.GetString("Cannot serialize member of type {0}: XmlAnyElement can only be used with classes of type XmlNode or a type deriving from XmlNode.", new object[] { accessor.TypeDesc.FullName }));
					}
					if (a.XmlAttribute != null)
					{
						if (a.XmlElements.Count > 0)
						{
							throw new InvalidOperationException(Res.GetString("For non-array types, you may use the following attributes: XmlAttribute, XmlText, XmlElement, or XmlAnyElement."));
						}
						if (a.XmlAttribute.Type != null)
						{
							throw new InvalidOperationException(Res.GetString("The type for {0} may not be specified for primitive types.", new object[] { "XmlAttribute" }));
						}
						AttributeAccessor attributeAccessor2 = new AttributeAccessor();
						attributeAccessor2.Name = Accessor.EscapeQName((a.XmlAttribute.AttributeName.Length == 0) ? name : a.XmlAttribute.AttributeName);
						attributeAccessor2.Namespace = ((a.XmlAttribute.Namespace == null) ? ns : a.XmlAttribute.Namespace);
						attributeAccessor2.Form = a.XmlAttribute.Form;
						if (attributeAccessor2.Form == XmlSchemaForm.None && ns != attributeAccessor2.Namespace)
						{
							attributeAccessor2.Form = XmlSchemaForm.Qualified;
						}
						attributeAccessor2.CheckSpecial();
						XmlReflectionImporter.CheckForm(attributeAccessor2.Form, ns != attributeAccessor2.Namespace);
						attributeAccessor2.Mapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(fieldType), ns, XmlReflectionImporter.ImportContext.Attribute, a.XmlAttribute.DataType, null, limiter);
						attributeAccessor2.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
						attributeAccessor2.Any = a.XmlAnyAttribute != null;
						if (attributeAccessor2.Form == XmlSchemaForm.Qualified && attributeAccessor2.Namespace != ns)
						{
							if (this.xsdAttributes == null)
							{
								this.xsdAttributes = new NameTable();
							}
							attributeAccessor2 = (AttributeAccessor)this.ReconcileAccessor(attributeAccessor2, this.xsdAttributes);
						}
						accessor.Attribute = attributeAccessor2;
					}
					else
					{
						if (a.XmlText != null)
						{
							if (a.XmlText.Type != null && a.XmlText.Type != fieldType)
							{
								throw new InvalidOperationException(Res.GetString("The type for {0} may not be specified for primitive types.", new object[] { "XmlText" }));
							}
							accessor.Text = new TextAccessor
							{
								Name = name,
								Mapping = this.ImportTypeMapping(this.modelScope.GetTypeModel(fieldType), ns, XmlReflectionImporter.ImportContext.Text, a.XmlText.DataType, null, limiter)
							};
						}
						else if (a.XmlElements.Count == 0)
						{
							a.XmlElements.Add(XmlReflectionImporter.CreateElementAttribute(accessor.TypeDesc));
						}
						for (int k = 0; k < a.XmlElements.Count; k++)
						{
							XmlElementAttribute xmlElementAttribute2 = a.XmlElements[k];
							if (xmlElementAttribute2.Type != null && this.typeScope.GetTypeDesc(xmlElementAttribute2.Type) != accessor.TypeDesc)
							{
								throw new InvalidOperationException(Res.GetString("The type for {0} may not be specified for primitive types.", new object[] { "XmlElement" }));
							}
							ElementAccessor elementAccessor4 = new ElementAccessor();
							elementAccessor4.Name = XmlConvert.EncodeLocalName((xmlElementAttribute2.ElementName.Length == 0) ? name : xmlElementAttribute2.ElementName);
							elementAccessor4.Namespace = (rpc ? null : ((xmlElementAttribute2.Namespace == null) ? ns : xmlElementAttribute2.Namespace));
							TypeModel typeModel3 = this.modelScope.GetTypeModel(fieldType);
							elementAccessor4.Mapping = this.ImportTypeMapping(typeModel3, rpc ? ns : elementAccessor4.Namespace, XmlReflectionImporter.ImportContext.Element, xmlElementAttribute2.DataType, null, limiter);
							if (elementAccessor4.Mapping.TypeDesc.Kind == TypeKind.Node)
							{
								elementAccessor4.Any = true;
							}
							elementAccessor4.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
							if (xmlElementAttribute2.IsNullableSpecified && !xmlElementAttribute2.IsNullable && typeModel3.TypeDesc.IsOptionalValue)
							{
								throw new InvalidOperationException(Res.GetString("IsNullable may not be set to 'false' for a Nullable<{0}> type. Consider using '{0}' type or removing the IsNullable property from the {1} attribute.", new object[]
								{
									typeModel3.TypeDesc.BaseTypeDesc.FullName,
									"XmlElement"
								}));
							}
							elementAccessor4.IsNullable = (xmlElementAttribute2.IsNullableSpecified ? xmlElementAttribute2.IsNullable : typeModel3.TypeDesc.IsOptionalValue);
							elementAccessor4.Form = (rpc ? XmlSchemaForm.Unqualified : ((xmlElementAttribute2.Form == XmlSchemaForm.None) ? xmlSchemaForm : xmlElementAttribute2.Form));
							XmlReflectionImporter.CheckNullable(elementAccessor4.IsNullable, accessor.TypeDesc, elementAccessor4.Mapping);
							if (!rpc)
							{
								XmlReflectionImporter.CheckForm(elementAccessor4.Form, ns != elementAccessor4.Namespace);
								elementAccessor4 = this.ReconcileLocalAccessor(elementAccessor4, ns);
							}
							if (xmlElementAttribute2.Order != -1)
							{
								if (xmlElementAttribute2.Order != num2 && num2 != -1)
								{
									throw new InvalidOperationException(Res.GetString("If multiple custom attributes specified on a single member only one of them have to have explicit '{0}' property, however if more that one attribute has the explicit '{0}', all values have to match.", new object[] { "Order" }));
								}
								num2 = xmlElementAttribute2.Order;
							}
							XmlReflectionImporter.AddUniqueAccessor(nameTable, elementAccessor4);
							arrayList.Add(elementAccessor4);
						}
					}
				}
				else if (a.Xmlns)
				{
					if (xmlFlags != XmlAttributeFlags.XmlnsDeclarations)
					{
						throw new InvalidOperationException(Res.GetString("XmlNamespaceDeclarations attribute cannot be used in conjunction with any other custom attributes."));
					}
					if (fieldType != typeof(XmlSerializerNamespaces))
					{
						throw new InvalidOperationException(Res.GetString("Cannot use XmlNamespaceDeclarations attribute on member '{0}' of type {1}.  This attribute is only valid on members of type {2}.", new object[]
						{
							name,
							fieldType.FullName,
							typeof(XmlSerializerNamespaces).FullName
						}));
					}
					accessor.Xmlns = new XmlnsAccessor();
					accessor.Ignore = true;
				}
				else if (a.XmlAttribute != null || a.XmlText != null)
				{
					if (accessor.TypeDesc.Kind == TypeKind.Serializable)
					{
						throw new InvalidOperationException(Res.GetString("Cannot serialize member '{0}' of type {1}. XmlAttribute/XmlText cannot be used to encode types implementing {2}.", new object[]
						{
							name,
							accessor.TypeDesc.FullName,
							typeof(IXmlSerializable).Name
						}));
					}
					throw new InvalidOperationException(Res.GetString("Cannot serialize member '{0}' of type {1}. XmlAttribute/XmlText cannot be used to encode complex types.", new object[] { name, accessor.TypeDesc }));
				}
				else
				{
					if (a.XmlElements.Count == 0 && a.XmlAnyElements.Count == 0)
					{
						a.XmlElements.Add(XmlReflectionImporter.CreateElementAttribute(accessor.TypeDesc));
					}
					for (int l = 0; l < a.XmlElements.Count; l++)
					{
						XmlElementAttribute xmlElementAttribute3 = a.XmlElements[l];
						Type type5 = ((xmlElementAttribute3.Type == null) ? fieldType : xmlElementAttribute3.Type);
						TypeDesc typeDesc5 = this.typeScope.GetTypeDesc(type5);
						ElementAccessor elementAccessor5 = new ElementAccessor();
						TypeModel typeModel4 = this.modelScope.GetTypeModel(type5);
						elementAccessor5.Namespace = (rpc ? null : ((xmlElementAttribute3.Namespace == null) ? ns : xmlElementAttribute3.Namespace));
						elementAccessor5.Mapping = this.ImportTypeMapping(typeModel4, rpc ? ns : elementAccessor5.Namespace, XmlReflectionImporter.ImportContext.Element, xmlElementAttribute3.DataType, null, false, openModel, limiter);
						if (a.XmlElements.Count == 1)
						{
							elementAccessor5.Name = XmlConvert.EncodeLocalName((xmlElementAttribute3.ElementName.Length == 0) ? name : xmlElementAttribute3.ElementName);
						}
						else
						{
							elementAccessor5.Name = ((xmlElementAttribute3.ElementName.Length == 0) ? elementAccessor5.Mapping.DefaultElementName : XmlConvert.EncodeLocalName(xmlElementAttribute3.ElementName));
						}
						elementAccessor5.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
						if (xmlElementAttribute3.IsNullableSpecified && !xmlElementAttribute3.IsNullable && typeModel4.TypeDesc.IsOptionalValue)
						{
							throw new InvalidOperationException(Res.GetString("IsNullable may not be set to 'false' for a Nullable<{0}> type. Consider using '{0}' type or removing the IsNullable property from the {1} attribute.", new object[]
							{
								typeModel4.TypeDesc.BaseTypeDesc.FullName,
								"XmlElement"
							}));
						}
						elementAccessor5.IsNullable = (xmlElementAttribute3.IsNullableSpecified ? xmlElementAttribute3.IsNullable : typeModel4.TypeDesc.IsOptionalValue);
						elementAccessor5.Form = (rpc ? XmlSchemaForm.Unqualified : ((xmlElementAttribute3.Form == XmlSchemaForm.None) ? xmlSchemaForm : xmlElementAttribute3.Form));
						XmlReflectionImporter.CheckNullable(elementAccessor5.IsNullable, typeDesc5, elementAccessor5.Mapping);
						if (!rpc)
						{
							XmlReflectionImporter.CheckForm(elementAccessor5.Form, ns != elementAccessor5.Namespace);
							elementAccessor5 = this.ReconcileLocalAccessor(elementAccessor5, ns);
						}
						if (xmlElementAttribute3.Order != -1)
						{
							if (xmlElementAttribute3.Order != num2 && num2 != -1)
							{
								throw new InvalidOperationException(Res.GetString("If multiple custom attributes specified on a single member only one of them have to have explicit '{0}' property, however if more that one attribute has the explicit '{0}', all values have to match.", new object[] { "Order" }));
							}
							num2 = xmlElementAttribute3.Order;
						}
						XmlReflectionImporter.AddUniqueAccessor(nameTable, elementAccessor5);
						arrayList.Add(elementAccessor5);
					}
					NameTable nameTable3 = new NameTable();
					for (int m = 0; m < a.XmlAnyElements.Count; m++)
					{
						XmlAnyElementAttribute xmlAnyElementAttribute2 = a.XmlAnyElements[m];
						Type type6 = (typeof(IXmlSerializable).IsAssignableFrom(fieldType) ? fieldType : (typeof(XmlNode).IsAssignableFrom(fieldType) ? fieldType : typeof(XmlElement)));
						if (!fieldType.IsAssignableFrom(type6))
						{
							throw new InvalidOperationException(Res.GetString("Cannot serialize member of type {0}: XmlAnyElement can only be used with classes of type XmlNode or a type deriving from XmlNode.", new object[] { fieldType.FullName }));
						}
						string text4 = ((xmlAnyElementAttribute2.Name.Length == 0) ? xmlAnyElementAttribute2.Name : XmlConvert.EncodeLocalName(xmlAnyElementAttribute2.Name));
						string text5 = (xmlAnyElementAttribute2.NamespaceSpecified ? xmlAnyElementAttribute2.Namespace : null);
						if (nameTable3[text4, text5] == null)
						{
							nameTable3[text4, text5] = xmlAnyElementAttribute2;
							if (nameTable[text4, (text5 == null) ? ns : text5] != null)
							{
								throw new InvalidOperationException(Res.GetString("The element '{0}' has been attributed with duplicate XmlAnyElementAttribute(Name=\"{1}\", Namespace=\"{2}\").", new object[]
								{
									name,
									xmlAnyElementAttribute2.Name,
									(xmlAnyElementAttribute2.Namespace == null) ? "null" : xmlAnyElementAttribute2.Namespace
								}));
							}
							ElementAccessor elementAccessor6 = new ElementAccessor();
							elementAccessor6.Name = text4;
							elementAccessor6.Namespace = ((text5 == null) ? ns : text5);
							elementAccessor6.Any = true;
							elementAccessor6.AnyNamespaces = text5;
							TypeDesc typeDesc6 = this.typeScope.GetTypeDesc(type6);
							TypeModel typeModel5 = this.modelScope.GetTypeModel(type6);
							if (elementAccessor6.Name.Length > 0)
							{
								typeModel5.TypeDesc.IsMixed = true;
							}
							elementAccessor6.Mapping = this.ImportTypeMapping(typeModel5, elementAccessor6.Namespace, XmlReflectionImporter.ImportContext.Element, string.Empty, null, false, openModel, limiter);
							elementAccessor6.Default = this.GetDefaultValue(model.FieldTypeDesc, model.FieldType, a);
							elementAccessor6.IsNullable = false;
							elementAccessor6.Form = xmlSchemaForm;
							XmlReflectionImporter.CheckNullable(elementAccessor6.IsNullable, typeDesc6, elementAccessor6.Mapping);
							if (!rpc)
							{
								XmlReflectionImporter.CheckForm(elementAccessor6.Form, ns != elementAccessor6.Namespace);
								elementAccessor6 = this.ReconcileLocalAccessor(elementAccessor6, ns);
							}
							if (xmlAnyElementAttribute2.Order != -1)
							{
								if (xmlAnyElementAttribute2.Order != num2 && num2 != -1)
								{
									throw new InvalidOperationException(Res.GetString("If multiple custom attributes specified on a single member only one of them have to have explicit '{0}' property, however if more that one attribute has the explicit '{0}', all values have to match.", new object[] { "Order" }));
								}
								num2 = xmlAnyElementAttribute2.Order;
							}
							nameTable.Add(elementAccessor6.Name, elementAccessor6.Namespace, elementAccessor6);
							arrayList.Add(elementAccessor6);
						}
					}
				}
			}
			accessor.Elements = (ElementAccessor[])arrayList.ToArray(typeof(ElementAccessor));
			accessor.SequenceId = num2;
			if (rpc)
			{
				if (accessor.TypeDesc.IsArrayLike && accessor.Elements.Length != 0 && !(accessor.Elements[0].Mapping is ArrayMapping))
				{
					throw new InvalidOperationException(Res.GetString("Input or output values of an rpc\\literal method cannot have maxOccurs > 1. Provide a wrapper element for '{0}' by using XmlArray or XmlArrayItem instead of XmlElement attribute.", new object[] { accessor.Elements[0].Name }));
				}
				if (accessor.Xmlns != null)
				{
					throw new InvalidOperationException(Res.GetString("Input or output values of an rpc\\literal method cannot have an XmlNamespaceDeclarations attribute (member '{0}').", new object[] { accessor.Name }));
				}
			}
			if (accessor.ChoiceIdentifier != null)
			{
				accessor.ChoiceIdentifier.MemberIds = new string[accessor.Elements.Length];
				int n = 0;
				while (n < accessor.Elements.Length)
				{
					bool flag2 = false;
					ElementAccessor elementAccessor7 = accessor.Elements[n];
					EnumMapping enumMapping = (EnumMapping)accessor.ChoiceIdentifier.Mapping;
					for (int num3 = 0; num3 < enumMapping.Constants.Length; num3++)
					{
						string xmlName = enumMapping.Constants[num3].XmlName;
						if (elementAccessor7.Any && elementAccessor7.Name.Length == 0)
						{
							string text6 = ((elementAccessor7.AnyNamespaces == null) ? "##any" : elementAccessor7.AnyNamespaces);
							if (xmlName.Substring(0, xmlName.Length - 1) == text6)
							{
								accessor.ChoiceIdentifier.MemberIds[n] = enumMapping.Constants[num3].Name;
								flag2 = true;
								break;
							}
						}
						else
						{
							int num4 = xmlName.LastIndexOf(':');
							string text7 = ((num4 < 0) ? enumMapping.Namespace : xmlName.Substring(0, num4));
							string text8 = ((num4 < 0) ? xmlName : xmlName.Substring(num4 + 1));
							if (elementAccessor7.Name == text8 && ((elementAccessor7.Form == XmlSchemaForm.Unqualified && string.IsNullOrEmpty(text7)) || elementAccessor7.Namespace == text7))
							{
								accessor.ChoiceIdentifier.MemberIds[n] = enumMapping.Constants[num3].Name;
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						if (elementAccessor7.Any && elementAccessor7.Name.Length == 0)
						{
							throw new InvalidOperationException(Res.GetString("Type {0} is missing enumeration value '##any:' corresponding to XmlAnyElementAttribute.", new object[] { accessor.ChoiceIdentifier.Mapping.TypeDesc.FullName }));
						}
						string text9 = ((elementAccessor7.Namespace != null && elementAccessor7.Namespace.Length > 0) ? (elementAccessor7.Namespace + ":" + elementAccessor7.Name) : elementAccessor7.Name);
						throw new InvalidOperationException(Res.GetString("Type {0} is missing enumeration value '{1}' for element '{2}' from namespace '{3}'.", new object[]
						{
							accessor.ChoiceIdentifier.Mapping.TypeDesc.FullName,
							text9,
							elementAccessor7.Name,
							elementAccessor7.Namespace
						}));
					}
					else
					{
						n++;
					}
				}
			}
			this.arrayNestingLevel = num;
			this.savedArrayItemAttributes = xmlArrayItemAttributes;
			this.savedArrayNamespace = text;
		}

		private void CheckTopLevelAttributes(XmlAttributes a, string accessorName)
		{
			XmlAttributeFlags xmlFlags = a.XmlFlags;
			if ((xmlFlags & (XmlAttributeFlags)544) != (XmlAttributeFlags)0)
			{
				throw new InvalidOperationException(Res.GetString("XmlAttribute and XmlAnyAttribute cannot be used with rpc\\literal SOAP, you may use the following attributes: XmlArray, XmlArrayItem, or single XmlElement."));
			}
			if ((xmlFlags & (XmlAttributeFlags)1284) != (XmlAttributeFlags)0)
			{
				throw new InvalidOperationException(Res.GetString("XmlText, XmlAnyElement, or XmlChoiceIdentifier cannot be used with rpc\\literal SOAP, you may use the following attributes: XmlArray, XmlArrayItem, or single XmlElement."));
			}
			if (a.XmlElements != null && a.XmlElements.Count > 0)
			{
				if (a.XmlElements.Count > 1)
				{
					throw new InvalidOperationException(Res.GetString("Multiple accessors are not supported with rpc\\literal SOAP, you may use the following attributes: XmlArray, XmlArrayItem, or single XmlElement."));
				}
				XmlElementAttribute xmlElementAttribute = a.XmlElements[0];
				if (xmlElementAttribute.Namespace != null)
				{
					throw new InvalidOperationException(Res.GetString("{0}='{1}' is not supported with rpc\\literal SOAP. The wrapper element has to be unqualified.", new object[] { "Namespace", xmlElementAttribute.Namespace }));
				}
				if (xmlElementAttribute.IsNullable)
				{
					throw new InvalidOperationException(Res.GetString("{0}='{1}' is not supported with rpc\\literal SOAP. The wrapper element cannot be nullable.", new object[] { "IsNullable", "true" }));
				}
			}
			if (a.XmlArray != null && a.XmlArray.Namespace != null)
			{
				throw new InvalidOperationException(Res.GetString("{0}='{1}' is not supported with rpc\\literal SOAP. The wrapper element has to be unqualified.", new object[]
				{
					"Namespace",
					a.XmlArray.Namespace
				}));
			}
		}

		private void CheckAmbiguousChoice(XmlAttributes a, Type accessorType, string accessorName)
		{
			Hashtable hashtable = new Hashtable();
			XmlElementAttributes xmlElements = a.XmlElements;
			if (xmlElements != null && xmlElements.Count >= 2 && a.XmlChoiceIdentifier == null)
			{
				for (int i = 0; i < xmlElements.Count; i++)
				{
					Type type = ((xmlElements[i].Type == null) ? accessorType : xmlElements[i].Type);
					if (hashtable.Contains(type))
					{
						throw new InvalidOperationException(Res.GetString("You need to add {0} to the '{1}' member.", new object[]
						{
							typeof(XmlChoiceIdentifierAttribute).Name,
							accessorName
						}));
					}
					hashtable.Add(type, false);
				}
			}
			if (hashtable.Contains(typeof(XmlElement)) && a.XmlAnyElements.Count > 0)
			{
				throw new InvalidOperationException(Res.GetString("You need to add {0} to the '{1}' member.", new object[]
				{
					typeof(XmlChoiceIdentifierAttribute).Name,
					accessorName
				}));
			}
			XmlArrayItemAttributes xmlArrayItems = a.XmlArrayItems;
			if (xmlArrayItems != null && xmlArrayItems.Count >= 2)
			{
				NameTable nameTable = new NameTable();
				for (int j = 0; j < xmlArrayItems.Count; j++)
				{
					Type type2 = ((xmlArrayItems[j].Type == null) ? accessorType : xmlArrayItems[j].Type);
					string text = xmlArrayItems[j].NestingLevel.ToString(CultureInfo.InvariantCulture);
					XmlArrayItemAttribute xmlArrayItemAttribute = (XmlArrayItemAttribute)nameTable[type2.FullName, text];
					if (xmlArrayItemAttribute != null)
					{
						throw new InvalidOperationException(Res.GetString("Ambiguous types specified for member '{0}'.  Items '{1}' and '{2}' have the same type.  Please consider using {3} with {4} instead.", new object[]
						{
							accessorName,
							xmlArrayItemAttribute.ElementName,
							xmlArrayItems[j].ElementName,
							typeof(XmlElementAttribute).Name,
							typeof(XmlChoiceIdentifierAttribute).Name,
							accessorName
						}));
					}
					nameTable[type2.FullName, text] = xmlArrayItems[j];
				}
			}
		}

		private void CheckChoiceIdentifierMapping(EnumMapping choiceMapping)
		{
			NameTable nameTable = new NameTable();
			for (int i = 0; i < choiceMapping.Constants.Length; i++)
			{
				string xmlName = choiceMapping.Constants[i].XmlName;
				int num = xmlName.LastIndexOf(':');
				string text = ((num < 0) ? xmlName : xmlName.Substring(num + 1));
				string text2 = ((num < 0) ? "" : xmlName.Substring(0, num));
				if (nameTable[text, text2] != null)
				{
					throw new InvalidOperationException(Res.GetString("Enum values in the XmlChoiceIdentifier '{0}' have to be unique.  Value '{1}' already present.", new object[] { choiceMapping.TypeName, xmlName }));
				}
				nameTable.Add(text, text2, choiceMapping.Constants[i]);
			}
		}

		private object GetDefaultValue(TypeDesc fieldTypeDesc, Type t, XmlAttributes a)
		{
			if (a.XmlDefaultValue == null || a.XmlDefaultValue == DBNull.Value)
			{
				return null;
			}
			if (fieldTypeDesc.Kind != TypeKind.Primitive && fieldTypeDesc.Kind != TypeKind.Enum)
			{
				a.XmlDefaultValue = null;
				return a.XmlDefaultValue;
			}
			if (fieldTypeDesc.Kind != TypeKind.Enum)
			{
				return a.XmlDefaultValue;
			}
			string text = Enum.Format(t, a.XmlDefaultValue, "G").Replace(",", " ");
			string text2 = Enum.Format(t, a.XmlDefaultValue, "D");
			if (text == text2)
			{
				throw new InvalidOperationException(Res.GetString("Value '{0}' cannot be converted to {1}.", new object[]
				{
					text,
					a.XmlDefaultValue.GetType().FullName
				}));
			}
			return text;
		}

		private static XmlArrayItemAttribute CreateArrayItemAttribute(TypeDesc typeDesc, int nestingLevel)
		{
			return new XmlArrayItemAttribute
			{
				NestingLevel = nestingLevel
			};
		}

		private static XmlArrayAttribute CreateArrayAttribute(TypeDesc typeDesc)
		{
			return new XmlArrayAttribute();
		}

		private static XmlElementAttribute CreateElementAttribute(TypeDesc typeDesc)
		{
			return new XmlElementAttribute
			{
				IsNullable = typeDesc.IsOptionalValue
			};
		}

		private static void AddUniqueAccessor(INameScope scope, Accessor accessor)
		{
			Accessor accessor2 = (Accessor)scope[accessor.Name, accessor.Namespace];
			if (accessor2 == null)
			{
				scope[accessor.Name, accessor.Namespace] = accessor;
				return;
			}
			if (accessor is ElementAccessor)
			{
				throw new InvalidOperationException(Res.GetString("The XML element '{0}' from namespace '{1}' is already present in the current scope. Use XML attributes to specify another XML name or namespace for the element.", new object[] { accessor2.Name, accessor2.Namespace }));
			}
			throw new InvalidOperationException(Res.GetString("The XML attribute '{0}' from namespace '{1}' is already present in the current scope. Use XML attributes to specify another XML name or namespace for the attribute.", new object[] { accessor2.Name, accessor2.Namespace }));
		}

		private static void AddUniqueAccessor(MemberMapping member, INameScope elements, INameScope attributes, bool isSequence)
		{
			if (member.Attribute != null)
			{
				XmlReflectionImporter.AddUniqueAccessor(attributes, member.Attribute);
				return;
			}
			if (!isSequence && member.Elements != null && member.Elements.Length != 0)
			{
				for (int i = 0; i < member.Elements.Length; i++)
				{
					XmlReflectionImporter.AddUniqueAccessor(elements, member.Elements[i]);
				}
			}
		}

		private static void CheckForm(XmlSchemaForm form, bool isQualified)
		{
			if (isQualified && form == XmlSchemaForm.Unqualified)
			{
				throw new InvalidOperationException(Res.GetString("The Form property may not be 'Unqualified' when an explicit Namespace property is present."));
			}
		}

		private static void CheckNullable(bool isNullable, TypeDesc typeDesc, TypeMapping mapping)
		{
			if (mapping is NullableMapping)
			{
				return;
			}
			if (mapping is SerializableMapping)
			{
				return;
			}
			if (isNullable && !typeDesc.IsNullable)
			{
				throw new InvalidOperationException(Res.GetString("IsNullable may not be 'true' for value type {0}.  Please consider using Nullable<{0}> instead.", new object[] { typeDesc.FullName }));
			}
		}

		private static ElementAccessor CreateElementAccessor(TypeMapping mapping, string ns)
		{
			ElementAccessor elementAccessor = new ElementAccessor();
			bool flag = mapping.TypeDesc.Kind == TypeKind.Node;
			if (!flag && mapping is SerializableMapping)
			{
				flag = ((SerializableMapping)mapping).IsAny;
			}
			if (flag)
			{
				elementAccessor.Any = true;
			}
			else
			{
				elementAccessor.Name = mapping.DefaultElementName;
				elementAccessor.Namespace = ns;
			}
			elementAccessor.Mapping = mapping;
			return elementAccessor;
		}

		internal static XmlTypeMapping GetTopLevelMapping(Type type, string defaultNamespace)
		{
			XmlAttributes xmlAttributes = new XmlAttributes(type);
			TypeDesc typeDesc = new TypeScope().GetTypeDesc(type);
			ElementAccessor elementAccessor = new ElementAccessor();
			if (typeDesc.Kind == TypeKind.Node)
			{
				elementAccessor.Any = true;
			}
			else
			{
				string text = ((xmlAttributes.XmlRoot == null) ? defaultNamespace : xmlAttributes.XmlRoot.Namespace);
				string text2 = string.Empty;
				if (xmlAttributes.XmlType != null)
				{
					text2 = xmlAttributes.XmlType.TypeName;
				}
				if (text2.Length == 0)
				{
					text2 = type.Name;
				}
				elementAccessor.Name = XmlConvert.EncodeLocalName(text2);
				elementAccessor.Namespace = text;
			}
			XmlTypeMapping xmlTypeMapping = new XmlTypeMapping(null, elementAccessor);
			xmlTypeMapping.SetKeyInternal(XmlMapping.GenerateKey(type, xmlAttributes.XmlRoot, defaultNamespace));
			return xmlTypeMapping;
		}

		private TypeScope typeScope;

		private XmlAttributeOverrides attributeOverrides;

		private XmlAttributes defaultAttributes = new XmlAttributes();

		private NameTable types = new NameTable();

		private NameTable nullables = new NameTable();

		private NameTable elements = new NameTable();

		private NameTable xsdAttributes;

		private Hashtable specials;

		private Hashtable anonymous = new Hashtable();

		private NameTable serializables;

		private StructMapping root;

		private string defaultNs;

		private ModelScope modelScope;

		private int arrayNestingLevel;

		private XmlArrayItemAttributes savedArrayItemAttributes;

		private string savedArrayNamespace;

		private int choiceNum = 1;

		private enum ImportContext
		{
			Text,
			Attribute,
			Element
		}
	}
}
