using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	public class XsdDataContractImporter
	{
		public XsdDataContractImporter()
			: this(null)
		{
		}

		public XsdDataContractImporter(CodeCompileUnit ccu)
		{
			this.ccu = ccu;
			this.imported_names = new Dictionary<XmlQualifiedName, XmlQualifiedName>();
		}

		public CodeCompileUnit CodeCompileUnit
		{
			get
			{
				if (this.ccu == null)
				{
					this.ccu = new CodeCompileUnit();
				}
				return this.ccu;
			}
		}

		public ImportOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		[MonoTODO]
		public ICollection<CodeTypeReference> GetKnownTypeReferences(XmlQualifiedName typeName)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName, XmlSchemaElement element)
		{
			throw new NotImplementedException();
		}

		public bool CanImport(XmlSchemaSet schemas)
		{
			foreach (object obj in schemas.GlobalElements)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj;
				if (!this.CanImport(schemas, xmlSchemaElement))
				{
					return false;
				}
			}
			return true;
		}

		public bool CanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
		{
			foreach (XmlQualifiedName xmlQualifiedName in typeNames)
			{
				if (!this.CanImport(schemas, xmlQualifiedName))
				{
					return false;
				}
			}
			return true;
		}

		public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName name)
		{
			return this.CanImport(schemas, (XmlSchemaElement)schemas.GlobalElements[name]);
		}

		[MonoTODO]
		public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void Import(XmlSchemaSet schemas)
		{
			if (schemas == null)
			{
				throw new ArgumentNullException("schemas");
			}
			schemas.Compile();
			foreach (object obj in schemas.GlobalElements.Values)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj;
				this.ImportInternal(schemas, xmlSchemaElement.QualifiedName);
			}
		}

		public void Import(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
		{
			if (schemas == null)
			{
				throw new ArgumentNullException("schemas");
			}
			if (typeNames == null)
			{
				throw new ArgumentNullException("typeNames");
			}
			schemas.Compile();
			foreach (XmlQualifiedName xmlQualifiedName in typeNames)
			{
				this.ImportInternal(schemas, xmlQualifiedName);
			}
		}

		public void Import(XmlSchemaSet schemas, XmlQualifiedName name)
		{
			if (schemas == null)
			{
				throw new ArgumentNullException("schemas");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			schemas.Compile();
			if (schemas.GlobalTypes[name] == null)
			{
				throw new InvalidDataContractException(string.Format("Type with name '{0}' not found in schema with namespace '{1}'", name.Name, name.Namespace));
			}
			this.ImportInternal(schemas, name);
		}

		[MonoTODO]
		public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
		{
			if (schemas == null)
			{
				throw new ArgumentNullException("schemas");
			}
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			schemas.Compile();
			XmlQualifiedName xmlQualifiedName = this.ImportInternal(schemas, element.QualifiedName);
			foreach (object obj in schemas.GlobalTypes.Names)
			{
				XmlQualifiedName xmlQualifiedName2 = (XmlQualifiedName)obj;
				this.ImportInternal(schemas, xmlQualifiedName2);
			}
			return xmlQualifiedName;
		}

		private XmlQualifiedName ImportInternal(XmlSchemaSet schemas, XmlQualifiedName qname)
		{
			if (qname.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				return qname;
			}
			if (this.imported_names.ContainsKey(qname))
			{
				return this.imported_names[qname];
			}
			XmlSchemas xmlSchemas = new XmlSchemas();
			foreach (object obj in schemas.Schemas())
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				xmlSchemas.Add(xmlSchema);
			}
			XmlSchemaImporter xmlSchemaImporter = new XmlSchemaImporter(xmlSchemas);
			XmlTypeMapping xmlTypeMapping = xmlSchemaImporter.ImportTypeMapping(qname);
			this.ImportFromTypeMapping(xmlTypeMapping);
			return qname;
		}

		private void ImportFromTypeMapping(XmlTypeMapping mapping)
		{
			if (mapping == null)
			{
				return;
			}
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(mapping.TypeName, mapping.Namespace);
			if (this.imported_names.ContainsKey(xmlQualifiedName))
			{
				return;
			}
			CodeNamespace codeNamespace = new CodeNamespace();
			codeNamespace.Name = this.FromXmlnsToClrName(mapping.Namespace);
			XmlCodeExporter xmlCodeExporter = new XmlCodeExporter(codeNamespace);
			xmlCodeExporter.ExportTypeMapping(mapping);
			List<CodeTypeDeclaration> list = new List<CodeTypeDeclaration>();
			foreach (object obj in codeNamespace.Types)
			{
				CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration)obj;
				string @namespace = this.GetNamespace(codeTypeDeclaration);
				if (@namespace != null)
				{
					XmlQualifiedName xmlQualifiedName2 = new XmlQualifiedName(codeTypeDeclaration.Name, @namespace);
					if (this.imported_names.ContainsKey(xmlQualifiedName2))
					{
						list.Add(codeTypeDeclaration);
					}
					else if (xmlQualifiedName2.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/Arrays")
					{
						list.Add(codeTypeDeclaration);
					}
					else
					{
						this.imported_names[xmlQualifiedName2] = xmlQualifiedName2;
						codeTypeDeclaration.Comments.Clear();
						codeTypeDeclaration.CustomAttributes.Clear();
						codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.CodeDom.Compiler.GeneratedCodeAttribute"), new CodeAttributeArgument[]
						{
							new CodeAttributeArgument(new CodePrimitiveExpression("System.Runtime.Serialization")),
							new CodeAttributeArgument(new CodePrimitiveExpression("3.0.0.0"))
						}));
						codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.Runtime.Serialization.DataContractAttribute")));
						if (!codeTypeDeclaration.IsEnum)
						{
							codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(typeof(object)));
							codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference("System.Runtime.Serialization.IExtensibleDataObject"));
							foreach (object obj2 in codeTypeDeclaration.Members)
							{
								CodeTypeMember codeTypeMember = (CodeTypeMember)obj2;
								CodeMemberProperty codeMemberProperty = codeTypeMember as CodeMemberProperty;
								if (codeMemberProperty != null)
								{
									if ((codeMemberProperty.Attributes & MemberAttributes.Public) == MemberAttributes.Public)
									{
										codeMemberProperty.CustomAttributes.Clear();
										codeMemberProperty.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference("System.Runtime.Serialization.DataMemberAttribute")));
										codeMemberProperty.Comments.Clear();
									}
								}
							}
							CodeMemberField codeMemberField = new CodeMemberField(new CodeTypeReference("System.Runtime.Serialization.ExtensionDataObject"), "extensionDataField");
							codeMemberField.Attributes = (MemberAttributes)20482;
							codeTypeDeclaration.Members.Add(codeMemberField);
							CodeMemberProperty codeMemberProperty2 = new CodeMemberProperty();
							codeMemberProperty2.Type = new CodeTypeReference("System.Runtime.Serialization.ExtensionDataObject");
							codeMemberProperty2.Name = "ExtensionData";
							codeMemberProperty2.Attributes = (MemberAttributes)24578;
							codeMemberProperty2.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "extensionDataField")));
							codeMemberProperty2.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "extensionDataField"), new CodePropertySetValueReferenceExpression()));
							codeTypeDeclaration.Members.Add(codeMemberProperty2);
						}
					}
				}
			}
			foreach (CodeTypeDeclaration codeTypeDeclaration2 in list)
			{
				codeNamespace.Types.Remove(codeTypeDeclaration2);
			}
			if (codeNamespace.Types.Count > 0)
			{
				this.CodeCompileUnit.Namespaces.Add(codeNamespace);
			}
		}

		private string FromXmlnsToClrName(string xns)
		{
			Uri uri;
			string text;
			if (xns.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal))
			{
				xns = xns.Substring("http://schemas.datacontract.org/2004/07/".Length);
			}
			else if (Uri.TryCreate(xns, UriKind.Absolute, out uri) && (text = this.MakeStringNamespaceComponentsValid(uri.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.Unescaped))).Length > 0)
			{
				xns = text;
			}
			return this.MakeStringNamespaceComponentsValid(xns);
		}

		private string MakeStringNamespaceComponentsValid(string ns)
		{
			string[] array = ns.Split(XsdDataContractImporter.split_tokens, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = CodeIdentifier.MakeValid(array[i]);
			}
			return string.Join(".", array);
		}

		private string GetNamespace(CodeTypeDeclaration type)
		{
			foreach (object obj in type.CustomAttributes)
			{
				CodeAttributeDeclaration codeAttributeDeclaration = (CodeAttributeDeclaration)obj;
				if (codeAttributeDeclaration.Name == "System.Xml.Serialization.XmlTypeAttribute" || codeAttributeDeclaration.Name == "System.Xml.Serialization.XmlRootAttribute")
				{
					foreach (object obj2 in codeAttributeDeclaration.Arguments)
					{
						CodeAttributeArgument codeAttributeArgument = (CodeAttributeArgument)obj2;
						if (codeAttributeArgument.Name == "Namespace")
						{
							return ((CodePrimitiveExpression)codeAttributeArgument.Value).Value as string;
						}
					}
					return null;
				}
			}
			return null;
		}

		private const string default_ns_prefix = "http://schemas.datacontract.org/2004/07/";

		private ImportOptions options;

		private CodeCompileUnit ccu;

		private Dictionary<XmlQualifiedName, XmlQualifiedName> imported_names = new Dictionary<XmlQualifiedName, XmlQualifiedName>();

		private static readonly char[] split_tokens = new char[] { '/', '.' };
	}
}
