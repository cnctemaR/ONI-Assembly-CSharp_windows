using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.CSharp;

namespace System.Xml.Serialization
{
	internal class MapCodeGenerator
	{
		public MapCodeGenerator(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options)
		{
			this.codeCompileUnit = codeCompileUnit;
			this.codeNamespace = codeNamespace;
			this.options = options;
			this.identifiers = new CodeIdentifiers();
		}

		public MapCodeGenerator(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable mappings)
		{
			this.codeCompileUnit = codeCompileUnit;
			this.codeNamespace = codeNamespace;
			this.options = options;
			this.codeProvider = codeProvider;
			this.identifiers = new CodeIdentifiers((codeProvider.LanguageOptions & LanguageOptions.CaseInsensitive) == LanguageOptions.None);
		}

		public CodeAttributeDeclarationCollection IncludeMetadata
		{
			get
			{
				if (this.includeMetadata != null)
				{
					return this.includeMetadata;
				}
				this.includeMetadata = new CodeAttributeDeclarationCollection();
				foreach (object obj in this.includeMaps.Values)
				{
					XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)obj;
					this.GenerateClassInclude(this.includeMetadata, xmlTypeMapping);
				}
				return this.includeMetadata;
			}
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
		{
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			this.ExportMembersMapCode(codeTypeDeclaration, (ClassMap)xmlMembersMapping.ObjectMap, xmlMembersMapping.Namespace, null);
		}

		public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping, bool isTopLevel)
		{
			this.ExportMapCode(xmlTypeMapping, isTopLevel);
			this.RemoveInclude(xmlTypeMapping);
		}

		private void ExportMapCode(XmlTypeMapping map, bool isTopLevel)
		{
			switch (map.TypeData.SchemaType)
			{
			case SchemaTypes.Enum:
				this.ExportEnumCode(map, isTopLevel);
				break;
			case SchemaTypes.Array:
				this.ExportArrayCode(map);
				break;
			case SchemaTypes.Class:
				this.ExportClassCode(map, isTopLevel);
				break;
			}
		}

		private void ExportClassCode(XmlTypeMapping map, bool isTopLevel)
		{
			CodeTypeDeclaration codeTypeDeclaration;
			if (this.IsMapExported(map))
			{
				codeTypeDeclaration = this.GetMapDeclaration(map);
				if (codeTypeDeclaration != null)
				{
					codeTypeDeclaration.CustomAttributes.Clear();
					this.AddClassAttributes(codeTypeDeclaration);
					this.GenerateClass(map, codeTypeDeclaration, isTopLevel);
					this.ExportDerivedTypeAttributes(map, codeTypeDeclaration);
				}
				return;
			}
			if (map.TypeData.Type == typeof(object))
			{
				this.exportedAnyType = map;
				this.SetMapExported(map, null);
				foreach (object obj in this.exportedAnyType.DerivedTypes)
				{
					XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)obj;
					if (!this.IsMapExported(xmlTypeMapping) && xmlTypeMapping.IncludeInSchema)
					{
						this.ExportTypeMapping(xmlTypeMapping, false);
						this.AddInclude(xmlTypeMapping);
					}
				}
				return;
			}
			codeTypeDeclaration = new CodeTypeDeclaration(map.TypeData.TypeName);
			this.SetMapExported(map, codeTypeDeclaration);
			this.AddCodeType(codeTypeDeclaration, map.Documentation);
			codeTypeDeclaration.Attributes = MemberAttributes.Public;
			codeTypeDeclaration.IsPartial = this.CodeProvider.Supports(GeneratorSupport.PartialTypes);
			this.AddClassAttributes(codeTypeDeclaration);
			this.GenerateClass(map, codeTypeDeclaration, isTopLevel);
			this.ExportDerivedTypeAttributes(map, codeTypeDeclaration);
			this.ExportMembersMapCode(codeTypeDeclaration, (ClassMap)map.ObjectMap, map.XmlTypeNamespace, map.BaseMap);
			if (map.BaseMap != null && map.BaseMap.TypeData.SchemaType != SchemaTypes.XmlNode)
			{
				CodeTypeReference domType = this.GetDomType(map.BaseMap.TypeData, false);
				codeTypeDeclaration.BaseTypes.Add(domType);
				if (map.BaseMap.IncludeInSchema)
				{
					this.ExportMapCode(map.BaseMap, false);
					this.AddInclude(map.BaseMap);
				}
			}
			this.ExportDerivedTypes(map, codeTypeDeclaration);
		}

		private void ExportDerivedTypeAttributes(XmlTypeMapping map, CodeTypeDeclaration codeClass)
		{
			foreach (object obj in map.DerivedTypes)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)obj;
				this.GenerateClassInclude(codeClass.CustomAttributes, xmlTypeMapping);
				this.ExportDerivedTypeAttributes(xmlTypeMapping, codeClass);
			}
		}

		private void ExportDerivedTypes(XmlTypeMapping map, CodeTypeDeclaration codeClass)
		{
			foreach (object obj in map.DerivedTypes)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)obj;
				if (codeClass.CustomAttributes == null)
				{
					codeClass.CustomAttributes = new CodeAttributeDeclarationCollection();
				}
				this.ExportMapCode(xmlTypeMapping, false);
				this.ExportDerivedTypes(xmlTypeMapping, codeClass);
			}
		}

		private void ExportMembersMapCode(CodeTypeDeclaration codeClass, ClassMap map, string defaultNamespace, XmlTypeMapping baseMap)
		{
			ICollection attributeMembers = map.AttributeMembers;
			ICollection collection = map.ElementMembers;
			if (attributeMembers != null)
			{
				foreach (object obj in attributeMembers)
				{
					XmlTypeMapMemberAttribute xmlTypeMapMemberAttribute = (XmlTypeMapMemberAttribute)obj;
					this.identifiers.AddUnique(xmlTypeMapMemberAttribute.Name, xmlTypeMapMemberAttribute);
				}
			}
			if (collection != null)
			{
				foreach (object obj2 in collection)
				{
					XmlTypeMapMemberElement xmlTypeMapMemberElement = (XmlTypeMapMemberElement)obj2;
					this.identifiers.AddUnique(xmlTypeMapMemberElement.Name, xmlTypeMapMemberElement);
				}
			}
			if (attributeMembers != null)
			{
				foreach (object obj3 in attributeMembers)
				{
					XmlTypeMapMemberAttribute xmlTypeMapMemberAttribute2 = (XmlTypeMapMemberAttribute)obj3;
					if (baseMap == null || !this.DefinedInBaseMap(baseMap, xmlTypeMapMemberAttribute2))
					{
						this.AddAttributeFieldMember(codeClass, xmlTypeMapMemberAttribute2, defaultNamespace);
					}
				}
			}
			collection = map.ElementMembers;
			if (collection != null)
			{
				foreach (object obj4 in collection)
				{
					XmlTypeMapMemberElement xmlTypeMapMemberElement2 = (XmlTypeMapMemberElement)obj4;
					if (baseMap == null || !this.DefinedInBaseMap(baseMap, xmlTypeMapMemberElement2))
					{
						Type type = xmlTypeMapMemberElement2.GetType();
						if (type == typeof(XmlTypeMapMemberList))
						{
							this.AddArrayElementFieldMember(codeClass, (XmlTypeMapMemberList)xmlTypeMapMemberElement2, defaultNamespace);
						}
						else if (type == typeof(XmlTypeMapMemberFlatList))
						{
							this.AddElementFieldMember(codeClass, xmlTypeMapMemberElement2, defaultNamespace);
						}
						else if (type == typeof(XmlTypeMapMemberAnyElement))
						{
							this.AddAnyElementFieldMember(codeClass, xmlTypeMapMemberElement2, defaultNamespace);
						}
						else
						{
							if (type != typeof(XmlTypeMapMemberElement))
							{
								throw new InvalidOperationException("Member type " + type + " not supported");
							}
							this.AddElementFieldMember(codeClass, xmlTypeMapMemberElement2, defaultNamespace);
						}
					}
				}
			}
			XmlTypeMapMember defaultAnyAttributeMember = map.DefaultAnyAttributeMember;
			if (defaultAnyAttributeMember != null)
			{
				CodeTypeMember codeTypeMember = this.CreateFieldMember(codeClass, defaultAnyAttributeMember.TypeData, defaultAnyAttributeMember.Name);
				MapCodeGenerator.AddComments(codeTypeMember, defaultAnyAttributeMember.Documentation);
				codeTypeMember.Attributes = MemberAttributes.Public;
				this.GenerateAnyAttribute(codeTypeMember);
			}
		}

		private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, Type type, string name)
		{
			return this.CreateFieldMember(codeClass, new CodeTypeReference(type), name, DBNull.Value, null, null);
		}

		private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, TypeData type, string name)
		{
			return this.CreateFieldMember(codeClass, this.GetDomType(type, false), name, DBNull.Value, null, null);
		}

		private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMember member)
		{
			return this.CreateFieldMember(codeClass, this.GetDomType(member.TypeData, member.RequiresNullable), member.Name, member.DefaultValue, member.TypeData, member.Documentation);
		}

		private CodeTypeMember CreateFieldMember(CodeTypeDeclaration codeClass, CodeTypeReference type, string name, object defaultValue, TypeData defaultType, string documentation)
		{
			CodeMemberField codeMemberField;
			CodeTypeMember codeTypeMember;
			if ((this.options & CodeGenerationOptions.GenerateProperties) > CodeGenerationOptions.None)
			{
				string text = this.identifiers.AddUnique(CodeIdentifier.MakeCamel(name + "Field"), name);
				codeMemberField = new CodeMemberField(type, text);
				codeMemberField.Attributes = MemberAttributes.Private;
				codeClass.Members.Add(codeMemberField);
				CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
				codeMemberProperty.Name = name;
				codeMemberProperty.Type = type;
				codeMemberProperty.Attributes = (MemberAttributes)24578;
				codeTypeMember = codeMemberProperty;
				CodeMemberProperty codeMemberProperty2 = codeMemberProperty;
				bool flag = true;
				codeMemberProperty.HasSet = flag;
				codeMemberProperty2.HasGet = flag;
				CodeExpression codeExpression = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), text);
				codeMemberProperty.SetStatements.Add(new CodeAssignStatement(codeExpression, new CodePropertySetValueReferenceExpression()));
				codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(codeExpression));
			}
			else
			{
				codeMemberField = new CodeMemberField(type, name);
				codeMemberField.Attributes = MemberAttributes.Public;
				codeTypeMember = codeMemberField;
			}
			if (defaultValue != DBNull.Value)
			{
				this.GenerateDefaultAttribute(codeMemberField, codeTypeMember, defaultType, defaultValue);
			}
			MapCodeGenerator.AddComments(codeTypeMember, documentation);
			codeClass.Members.Add(codeTypeMember);
			return codeTypeMember;
		}

		private void AddAttributeFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberAttribute attinfo, string defaultNamespace)
		{
			CodeTypeMember codeTypeMember = this.CreateFieldMember(codeClass, attinfo);
			CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = codeTypeMember.CustomAttributes;
			if (codeAttributeDeclarationCollection == null)
			{
				codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
			}
			this.GenerateAttributeMember(codeAttributeDeclarationCollection, attinfo, defaultNamespace, false);
			if (codeAttributeDeclarationCollection.Count > 0)
			{
				codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
			}
			if (attinfo.MappedType != null)
			{
				this.ExportMapCode(attinfo.MappedType, false);
				this.RemoveInclude(attinfo.MappedType);
			}
			if (attinfo.TypeData.IsValueType && attinfo.IsOptionalValueType)
			{
				codeTypeMember = this.CreateFieldMember(codeClass, typeof(bool), this.identifiers.MakeUnique(attinfo.Name + "Specified"));
				codeTypeMember.Attributes = MemberAttributes.Public;
				this.GenerateSpecifierMember(codeTypeMember);
			}
		}

		public void AddAttributeMemberAttributes(XmlTypeMapMemberAttribute attinfo, string defaultNamespace, CodeAttributeDeclarationCollection attributes, bool forceUseMemberName)
		{
			this.GenerateAttributeMember(attributes, attinfo, defaultNamespace, forceUseMemberName);
		}

		private void AddElementFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberElement member, string defaultNamespace)
		{
			CodeTypeMember codeTypeMember = this.CreateFieldMember(codeClass, member);
			CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = codeTypeMember.CustomAttributes;
			if (codeAttributeDeclarationCollection == null)
			{
				codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
			}
			this.AddElementMemberAttributes(member, defaultNamespace, codeAttributeDeclarationCollection, false);
			if (codeAttributeDeclarationCollection.Count > 0)
			{
				codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
			}
			if (member.TypeData.IsValueType && member.IsOptionalValueType)
			{
				codeTypeMember = this.CreateFieldMember(codeClass, typeof(bool), this.identifiers.MakeUnique(member.Name + "Specified"));
				codeTypeMember.Attributes = MemberAttributes.Public;
				this.GenerateSpecifierMember(codeTypeMember);
			}
		}

		public void AddElementMemberAttributes(XmlTypeMapMemberElement member, string defaultNamespace, CodeAttributeDeclarationCollection attributes, bool forceUseMemberName)
		{
			TypeData typeData = member.TypeData;
			bool flag = false;
			if (member is XmlTypeMapMemberFlatList)
			{
				typeData = typeData.ListItemTypeData;
				flag = true;
			}
			foreach (object obj in member.ElementInfo)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)obj;
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					this.ExportMapCode(xmlTypeMapElementInfo.MappedType, false);
					this.RemoveInclude(xmlTypeMapElementInfo.MappedType);
				}
				if (!this.ExportExtraElementAttributes(attributes, xmlTypeMapElementInfo, defaultNamespace, typeData))
				{
					this.GenerateElementInfoMember(attributes, member, xmlTypeMapElementInfo, typeData, defaultNamespace, flag, forceUseMemberName || flag);
				}
			}
			this.GenerateElementMember(attributes, member);
		}

		private void AddAnyElementFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberElement member, string defaultNamespace)
		{
			CodeTypeMember codeTypeMember = this.CreateFieldMember(codeClass, member);
			CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
			foreach (object obj in member.ElementInfo)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)obj;
				this.ExportExtraElementAttributes(codeAttributeDeclarationCollection, xmlTypeMapElementInfo, defaultNamespace, xmlTypeMapElementInfo.TypeData);
			}
			if (codeAttributeDeclarationCollection.Count > 0)
			{
				codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
			}
		}

		private bool DefinedInBaseMap(XmlTypeMapping map, XmlTypeMapMember member)
		{
			return ((ClassMap)map.ObjectMap).FindMember(member.Name) != null || (map.BaseMap != null && this.DefinedInBaseMap(map.BaseMap, member));
		}

		private void AddArrayElementFieldMember(CodeTypeDeclaration codeClass, XmlTypeMapMemberList member, string defaultNamespace)
		{
			CodeTypeMember codeTypeMember = this.CreateFieldMember(codeClass, member.TypeData, member.Name);
			CodeAttributeDeclarationCollection codeAttributeDeclarationCollection = new CodeAttributeDeclarationCollection();
			this.AddArrayAttributes(codeAttributeDeclarationCollection, member, defaultNamespace, false);
			ListMap listMap = (ListMap)member.ListTypeMapping.ObjectMap;
			this.AddArrayItemAttributes(codeAttributeDeclarationCollection, listMap, member.TypeData.ListItemTypeData, defaultNamespace, 0);
			if (codeAttributeDeclarationCollection.Count > 0)
			{
				codeTypeMember.CustomAttributes = codeAttributeDeclarationCollection;
			}
		}

		public void AddArrayAttributes(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, string defaultNamespace, bool forceUseMemberName)
		{
			this.GenerateArrayElement(attributes, member, defaultNamespace, forceUseMemberName);
		}

		public void AddArrayItemAttributes(CodeAttributeDeclarationCollection attributes, ListMap listMap, TypeData type, string defaultNamespace, int nestingLevel)
		{
			foreach (object obj in listMap.ItemInfo)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)obj;
				string text;
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					text = xmlTypeMapElementInfo.MappedType.ElementName;
				}
				else
				{
					text = xmlTypeMapElementInfo.TypeData.XmlType;
				}
				this.GenerateArrayItemAttributes(attributes, listMap, type, xmlTypeMapElementInfo, text, defaultNamespace, nestingLevel);
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					if (!this.IsMapExported(xmlTypeMapElementInfo.MappedType) && this.includeArrayTypes)
					{
						this.AddInclude(xmlTypeMapElementInfo.MappedType);
					}
					this.ExportMapCode(xmlTypeMapElementInfo.MappedType, false);
				}
			}
			if (listMap.IsMultiArray)
			{
				XmlTypeMapping nestedArrayMapping = listMap.NestedArrayMapping;
				this.AddArrayItemAttributes(attributes, (ListMap)nestedArrayMapping.ObjectMap, nestedArrayMapping.TypeData.ListItemTypeData, defaultNamespace, nestingLevel + 1);
			}
		}

		private void ExportArrayCode(XmlTypeMapping map)
		{
			ListMap listMap = (ListMap)map.ObjectMap;
			foreach (object obj in listMap.ItemInfo)
			{
				XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)obj;
				if (xmlTypeMapElementInfo.MappedType != null)
				{
					if (!this.IsMapExported(xmlTypeMapElementInfo.MappedType) && this.includeArrayTypes)
					{
						this.AddInclude(xmlTypeMapElementInfo.MappedType);
					}
					this.ExportMapCode(xmlTypeMapElementInfo.MappedType, false);
				}
			}
		}

		private bool ExportExtraElementAttributes(CodeAttributeDeclarationCollection attributes, XmlTypeMapElementInfo einfo, string defaultNamespace, TypeData defaultType)
		{
			if (einfo.IsTextElement)
			{
				this.GenerateTextElementAttribute(attributes, einfo, defaultType);
				return true;
			}
			if (einfo.IsUnnamedAnyElement)
			{
				this.GenerateUnnamedAnyElementAttribute(attributes, einfo, defaultNamespace);
				return true;
			}
			return false;
		}

		private void ExportEnumCode(XmlTypeMapping map, bool isTopLevel)
		{
			if (this.IsMapExported(map))
			{
				return;
			}
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(map.TypeData.TypeName);
			this.SetMapExported(map, codeTypeDeclaration);
			codeTypeDeclaration.Attributes = MemberAttributes.Public;
			codeTypeDeclaration.IsEnum = true;
			this.AddCodeType(codeTypeDeclaration, map.Documentation);
			EnumMap enumMap = (EnumMap)map.ObjectMap;
			if (enumMap.IsFlags)
			{
				codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("System.FlagsAttribute"));
			}
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)));
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("System.Xml")));
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("2.0.50727.1433")));
			codeTypeDeclaration.CustomAttributes.Add(codeAttributeDeclaration);
			codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
			this.GenerateEnum(map, codeTypeDeclaration, isTopLevel);
			int num = 1;
			foreach (EnumMap.EnumMapMember enumMapMember in enumMap.Members)
			{
				CodeMemberField codeMemberField = new CodeMemberField(string.Empty, enumMapMember.EnumName);
				if (enumMap.IsFlags)
				{
					codeMemberField.InitExpression = new CodePrimitiveExpression(num);
					num *= 2;
				}
				MapCodeGenerator.AddComments(codeMemberField, enumMapMember.Documentation);
				this.GenerateEnumItem(codeMemberField, enumMapMember);
				codeTypeDeclaration.Members.Add(codeMemberField);
			}
		}

		private void AddInclude(XmlTypeMapping map)
		{
			if (!this.includeMaps.ContainsKey(map.TypeData.FullTypeName))
			{
				this.includeMaps[map.TypeData.FullTypeName] = map;
			}
		}

		private void RemoveInclude(XmlTypeMapping map)
		{
			this.includeMaps.Remove(map.TypeData.FullTypeName);
		}

		private bool IsMapExported(XmlTypeMapping map)
		{
			return this.exportedMaps.Contains(map.TypeData.FullTypeName);
		}

		private void SetMapExported(XmlTypeMapping map, CodeTypeDeclaration declaration)
		{
			this.exportedMaps.Add(map.TypeData.FullTypeName, declaration);
		}

		private CodeTypeDeclaration GetMapDeclaration(XmlTypeMapping map)
		{
			return this.exportedMaps[map.TypeData.FullTypeName] as CodeTypeDeclaration;
		}

		public static void AddCustomAttribute(CodeTypeMember ctm, CodeAttributeDeclaration att, bool addIfNoParams)
		{
			if (att.Arguments.Count == 0 && !addIfNoParams)
			{
				return;
			}
			if (ctm.CustomAttributes == null)
			{
				ctm.CustomAttributes = new CodeAttributeDeclarationCollection();
			}
			ctm.CustomAttributes.Add(att);
		}

		public static void AddCustomAttribute(CodeTypeMember ctm, string name, params CodeAttributeArgument[] args)
		{
			if (ctm.CustomAttributes == null)
			{
				ctm.CustomAttributes = new CodeAttributeDeclarationCollection();
			}
			ctm.CustomAttributes.Add(new CodeAttributeDeclaration(name, args));
		}

		public static CodeAttributeArgument GetArg(string name, object value)
		{
			return new CodeAttributeArgument(name, new CodePrimitiveExpression(value));
		}

		public static CodeAttributeArgument GetArg(object value)
		{
			return new CodeAttributeArgument(new CodePrimitiveExpression(value));
		}

		public static CodeAttributeArgument GetTypeArg(string name, string typeName)
		{
			return new CodeAttributeArgument(name, new CodeTypeOfExpression(typeName));
		}

		public static CodeAttributeArgument GetEnumArg(string name, string enumType, string enumValue)
		{
			return new CodeAttributeArgument(name, new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(enumType), enumValue));
		}

		public static void AddComments(CodeTypeMember member, string comments)
		{
			if (comments == null || comments == string.Empty)
			{
				member.Comments.Add(new CodeCommentStatement("<remarks/>", true));
			}
			else
			{
				member.Comments.Add(new CodeCommentStatement("<remarks>\n" + comments + "\n</remarks>", true));
			}
		}

		private void AddCodeType(CodeTypeDeclaration type, string comments)
		{
			MapCodeGenerator.AddComments(type, comments);
			this.codeNamespace.Types.Add(type);
		}

		private void AddClassAttributes(CodeTypeDeclaration codeClass)
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)));
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("System.Xml")));
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("2.0.50727.1433")));
			codeClass.CustomAttributes.Add(codeAttributeDeclaration);
			codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
			codeClass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DebuggerStepThroughAttribute))));
			CodeAttributeDeclaration codeAttributeDeclaration2 = new CodeAttributeDeclaration(new CodeTypeReference(typeof(DesignerCategoryAttribute)));
			codeAttributeDeclaration2.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("code")));
			codeClass.CustomAttributes.Add(codeAttributeDeclaration2);
		}

		private CodeTypeReference GetDomType(TypeData data, bool requiresNullable)
		{
			if (data.IsValueType && (data.IsNullable || requiresNullable))
			{
				return new CodeTypeReference("System.Nullable", new CodeTypeReference[]
				{
					new CodeTypeReference(data.FullTypeName)
				});
			}
			if (data.SchemaType == SchemaTypes.Array)
			{
				return new CodeTypeReference(this.GetDomType(data.ListItemTypeData, false), 1);
			}
			return new CodeTypeReference(data.FullTypeName);
		}

		private CodeDomProvider CodeProvider
		{
			get
			{
				if (this.codeProvider == null)
				{
					this.codeProvider = new CSharpCodeProvider();
				}
				return this.codeProvider;
			}
		}

		protected virtual void GenerateClass(XmlTypeMapping map, CodeTypeDeclaration codeClass, bool isTopLevel)
		{
		}

		protected virtual void GenerateClassInclude(CodeAttributeDeclarationCollection attributes, XmlTypeMapping map)
		{
		}

		protected virtual void GenerateAnyAttribute(CodeTypeMember codeField)
		{
		}

		protected virtual void GenerateDefaultAttribute(CodeMemberField internalField, CodeTypeMember externalField, TypeData typeData, object defaultValue)
		{
			if (typeData.Type == null)
			{
				if (typeData.SchemaType != SchemaTypes.Enum)
				{
					throw new InvalidOperationException("Type " + typeData.TypeName + " not supported");
				}
				IFormattable formattable = defaultValue as IFormattable;
				CodeFieldReferenceExpression codeFieldReferenceExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.GetDomType(typeData, false)), (formattable == null) ? defaultValue.ToString() : formattable.ToString(null, CultureInfo.InvariantCulture));
				CodeAttributeArgument codeAttributeArgument = new CodeAttributeArgument(codeFieldReferenceExpression);
				MapCodeGenerator.AddCustomAttribute(externalField, "System.ComponentModel.DefaultValue", new CodeAttributeArgument[] { codeAttributeArgument });
			}
			else
			{
				MapCodeGenerator.AddCustomAttribute(externalField, "System.ComponentModel.DefaultValue", new CodeAttributeArgument[] { MapCodeGenerator.GetArg(defaultValue) });
			}
		}

		protected virtual void GenerateAttributeMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberAttribute attinfo, string defaultNamespace, bool forceUseMemberName)
		{
		}

		protected virtual void GenerateElementInfoMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, XmlTypeMapElementInfo einfo, TypeData defaultType, string defaultNamespace, bool addAlwaysAttr, bool forceUseMemberName)
		{
		}

		protected virtual void GenerateElementMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member)
		{
		}

		protected virtual void GenerateArrayElement(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, string defaultNamespace, bool forceUseMemberName)
		{
		}

		protected virtual void GenerateArrayItemAttributes(CodeAttributeDeclarationCollection attributes, ListMap listMap, TypeData type, XmlTypeMapElementInfo ainfo, string defaultName, string defaultNamespace, int nestingLevel)
		{
		}

		protected virtual void GenerateTextElementAttribute(CodeAttributeDeclarationCollection attributes, XmlTypeMapElementInfo einfo, TypeData defaultType)
		{
		}

		protected virtual void GenerateUnnamedAnyElementAttribute(CodeAttributeDeclarationCollection attributes, XmlTypeMapElementInfo einfo, string defaultNamespace)
		{
		}

		protected virtual void GenerateEnum(XmlTypeMapping map, CodeTypeDeclaration codeEnum, bool isTopLevel)
		{
		}

		protected virtual void GenerateEnumItem(CodeMemberField codeField, EnumMap.EnumMapMember emem)
		{
		}

		protected virtual void GenerateSpecifierMember(CodeTypeMember codeField)
		{
		}

		private CodeNamespace codeNamespace;

		private CodeCompileUnit codeCompileUnit;

		private CodeAttributeDeclarationCollection includeMetadata;

		private XmlTypeMapping exportedAnyType;

		protected bool includeArrayTypes;

		private CodeDomProvider codeProvider;

		private CodeGenerationOptions options;

		private CodeIdentifiers identifiers;

		private Hashtable exportedMaps = new Hashtable();

		private Hashtable includeMaps = new Hashtable();
	}
}
