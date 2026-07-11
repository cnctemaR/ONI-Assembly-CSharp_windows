using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace System.Xml.Serialization
{
	internal class SoapMapCodeGenerator : MapCodeGenerator
	{
		public SoapMapCodeGenerator(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit)
			: base(codeNamespace, codeCompileUnit, CodeGenerationOptions.None)
		{
			this.includeArrayTypes = true;
		}

		public SoapMapCodeGenerator(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable mappings)
			: base(codeNamespace, codeCompileUnit, codeProvider, options, mappings)
		{
		}

		protected override void GenerateClass(XmlTypeMapping map, CodeTypeDeclaration codeClass, bool isTopLevel)
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.SoapType");
			if (map.XmlType != map.TypeData.TypeName)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg(map.XmlType));
			}
			if (map.XmlTypeNamespace != string.Empty)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Namespace", map.XmlTypeNamespace));
			}
			MapCodeGenerator.AddCustomAttribute(codeClass, codeAttributeDeclaration, false);
		}

		protected override void GenerateClassInclude(CodeAttributeDeclarationCollection attributes, XmlTypeMapping map)
		{
			attributes.Add(new CodeAttributeDeclaration("System.Xml.Serialization.SoapInclude")
			{
				Arguments = 
				{
					new CodeAttributeArgument(new CodeTypeOfExpression(map.TypeData.FullTypeName))
				}
			});
		}

		protected override void GenerateAttributeMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberAttribute attinfo, string defaultNamespace, bool forceUseMemberName)
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.SoapAttribute");
			if (attinfo.Name != attinfo.AttributeName)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg(attinfo.AttributeName));
			}
			if (attinfo.Namespace != defaultNamespace)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Namespace", attinfo.Namespace));
			}
			if (!TypeTranslator.IsDefaultPrimitiveTpeData(attinfo.TypeData))
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("DataType", attinfo.TypeData.XmlType));
			}
			attributes.Add(codeAttributeDeclaration);
		}

		protected override void GenerateElementInfoMember(CodeAttributeDeclarationCollection attributes, XmlTypeMapMemberElement member, XmlTypeMapElementInfo einfo, TypeData defaultType, string defaultNamespace, bool addAlwaysAttr, bool forceUseMemberName)
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.SoapElement");
			if (forceUseMemberName || einfo.ElementName != member.Name)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg(einfo.ElementName));
			}
			if (!TypeTranslator.IsDefaultPrimitiveTpeData(einfo.TypeData))
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("DataType", einfo.TypeData.XmlType));
			}
			if (addAlwaysAttr || codeAttributeDeclaration.Arguments.Count > 0)
			{
				attributes.Add(codeAttributeDeclaration);
			}
		}

		protected override void GenerateEnum(XmlTypeMapping map, CodeTypeDeclaration codeEnum, bool isTopLevel)
		{
			CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.SoapType");
			if (map.XmlType != map.TypeData.TypeName)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg(map.XmlType));
			}
			if (map.XmlTypeNamespace != string.Empty)
			{
				codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Namespace", map.XmlTypeNamespace));
			}
			MapCodeGenerator.AddCustomAttribute(codeEnum, codeAttributeDeclaration, false);
		}

		protected override void GenerateEnumItem(CodeMemberField codeField, EnumMap.EnumMapMember emem)
		{
			if (emem.EnumName != emem.XmlName)
			{
				MapCodeGenerator.AddCustomAttribute(codeField, new CodeAttributeDeclaration("System.Xml.Serialization.SoapEnum")
				{
					Arguments = { MapCodeGenerator.GetArg("Name", emem.XmlName) }
				}, true);
			}
		}

		protected override void GenerateSpecifierMember(CodeTypeMember codeField)
		{
			MapCodeGenerator.AddCustomAttribute(codeField, "System.Xml.Serialization.SoapIgnore", new CodeAttributeArgument[0]);
		}
	}
}
