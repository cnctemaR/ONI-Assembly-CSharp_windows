using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

namespace System.Xml.Serialization
{
	public class XmlCodeExporter : CodeExporter
	{
		public XmlCodeExporter(CodeNamespace codeNamespace)
			: this(codeNamespace, null)
		{
		}

		public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit)
		{
			this.codeGenerator = new XmlMapCodeGenerator(codeNamespace, codeCompileUnit, CodeGenerationOptions.GenerateProperties);
		}

		public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options)
			: this(codeNamespace, codeCompileUnit, null, options, null)
		{
		}

		public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeGenerationOptions options, Hashtable mappings)
			: this(codeNamespace, codeCompileUnit, null, options, mappings)
		{
		}

		[MonoTODO]
		public XmlCodeExporter(CodeNamespace codeNamespace, CodeCompileUnit codeCompileUnit, CodeDomProvider codeProvider, CodeGenerationOptions options, Hashtable mappings)
		{
			this.codeGenerator = new XmlMapCodeGenerator(codeNamespace, codeCompileUnit, codeProvider, options, mappings);
		}

		public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlMemberMapping member, string ns)
		{
			this.AddMappingMetadata(metadata, member, ns, false);
		}

		public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlTypeMapping member, string ns)
		{
			if ((member.TypeData.SchemaType == SchemaTypes.Primitive || member.TypeData.SchemaType == SchemaTypes.Array) && member.Namespace != "http://www.w3.org/2001/XMLSchema")
			{
				metadata.Add(new CodeAttributeDeclaration("System.Xml.Serialization.XmlRoot")
				{
					Arguments = 
					{
						MapCodeGenerator.GetArg(member.ElementName),
						MapCodeGenerator.GetArg("Namespace", member.Namespace),
						MapCodeGenerator.GetArg("IsNullable", member.IsNullable)
					}
				});
			}
		}

		public void AddMappingMetadata(CodeAttributeDeclarationCollection metadata, XmlMemberMapping member, string ns, bool forceUseMemberName)
		{
			TypeData typeData = member.TypeMapMember.TypeData;
			if (member.Any)
			{
				XmlTypeMapElementInfoList elementInfo = ((XmlTypeMapMemberElement)member.TypeMapMember).ElementInfo;
				foreach (object obj in elementInfo)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)obj;
					if (xmlTypeMapElementInfo.IsTextElement)
					{
						metadata.Add(new CodeAttributeDeclaration("System.Xml.Serialization.XmlText"));
					}
					else
					{
						CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration("System.Xml.Serialization.XmlAnyElement");
						if (!xmlTypeMapElementInfo.IsUnnamedAnyElement)
						{
							codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Name", xmlTypeMapElementInfo.ElementName));
							if (xmlTypeMapElementInfo.Namespace != ns)
							{
								codeAttributeDeclaration.Arguments.Add(MapCodeGenerator.GetArg("Namespace", member.Namespace));
							}
						}
						metadata.Add(codeAttributeDeclaration);
					}
				}
			}
			else if (member.TypeMapMember is XmlTypeMapMemberList)
			{
				XmlTypeMapMemberList xmlTypeMapMemberList = member.TypeMapMember as XmlTypeMapMemberList;
				ListMap listMap = (ListMap)xmlTypeMapMemberList.ListTypeMapping.ObjectMap;
				this.codeGenerator.AddArrayAttributes(metadata, xmlTypeMapMemberList, ns, forceUseMemberName);
				this.codeGenerator.AddArrayItemAttributes(metadata, listMap, typeData.ListItemTypeData, xmlTypeMapMemberList.Namespace, 0);
			}
			else if (member.TypeMapMember is XmlTypeMapMemberElement)
			{
				this.codeGenerator.AddElementMemberAttributes((XmlTypeMapMemberElement)member.TypeMapMember, ns, metadata, forceUseMemberName);
			}
			else
			{
				if (!(member.TypeMapMember is XmlTypeMapMemberAttribute))
				{
					throw new NotSupportedException("Schema type not supported");
				}
				this.codeGenerator.AddAttributeMemberAttributes((XmlTypeMapMemberAttribute)member.TypeMapMember, ns, metadata, forceUseMemberName);
			}
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
		{
			this.codeGenerator.ExportMembersMapping(xmlMembersMapping);
		}

		public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
		{
			this.codeGenerator.ExportTypeMapping(xmlTypeMapping, true);
		}
	}
}
