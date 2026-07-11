using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Configuration;
using System.Security.Permissions;
using System.Xml.Serialization.Advanced;
using System.Xml.Serialization.Configuration;
using Microsoft.CSharp;
using Unity;

namespace System.Xml.Serialization
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public abstract class SchemaImporter
	{
		internal SchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, CodeDomProvider codeProvider, ImportContext context)
		{
			if (!schemas.Contains("http://www.w3.org/2001/XMLSchema"))
			{
				schemas.AddReference(XmlSchemas.XsdSchema);
				schemas.SchemaSet.Add(XmlSchemas.XsdSchema);
			}
			if (!schemas.Contains("http://www.w3.org/XML/1998/namespace"))
			{
				schemas.AddReference(XmlSchemas.XmlSchema);
				schemas.SchemaSet.Add(XmlSchemas.XmlSchema);
			}
			this.schemas = schemas;
			this.options = options;
			this.codeProvider = codeProvider;
			this.context = context;
			this.Schemas.SetCache(this.Context.Cache, this.Context.ShareTypes);
			SchemaImporterExtensionsSection schemaImporterExtensionsSection = PrivilegedConfigurationManager.GetSection(ConfigurationStrings.SchemaImporterExtensionsSectionPath) as SchemaImporterExtensionsSection;
			if (schemaImporterExtensionsSection != null)
			{
				this.extensions = schemaImporterExtensionsSection.SchemaImporterExtensionsInternal;
				return;
			}
			this.extensions = new SchemaImporterExtensionCollection();
		}

		internal ImportContext Context
		{
			get
			{
				if (this.context == null)
				{
					this.context = new ImportContext();
				}
				return this.context;
			}
		}

		internal CodeDomProvider CodeProvider
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

		public SchemaImporterExtensionCollection Extensions
		{
			get
			{
				if (this.extensions == null)
				{
					this.extensions = new SchemaImporterExtensionCollection();
				}
				return this.extensions;
			}
		}

		internal Hashtable ImportedElements
		{
			get
			{
				return this.Context.Elements;
			}
		}

		internal Hashtable ImportedMappings
		{
			get
			{
				return this.Context.Mappings;
			}
		}

		internal CodeIdentifiers TypeIdentifiers
		{
			get
			{
				return this.Context.TypeIdentifiers;
			}
		}

		internal XmlSchemas Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemas();
				}
				return this.schemas;
			}
		}

		internal TypeScope Scope
		{
			get
			{
				if (this.scope == null)
				{
					this.scope = new TypeScope();
				}
				return this.scope;
			}
		}

		internal NameTable GroupsInUse
		{
			get
			{
				if (this.groupsInUse == null)
				{
					this.groupsInUse = new NameTable();
				}
				return this.groupsInUse;
			}
		}

		internal NameTable TypesInUse
		{
			get
			{
				if (this.typesInUse == null)
				{
					this.typesInUse = new NameTable();
				}
				return this.typesInUse;
			}
		}

		internal CodeGenerationOptions Options
		{
			get
			{
				return this.options;
			}
		}

		internal void MakeDerived(StructMapping structMapping, Type baseType, bool baseTypeCanBeIndirect)
		{
			structMapping.ReferencedByTopLevelElement = true;
			if (baseType != null)
			{
				TypeDesc typeDesc = this.Scope.GetTypeDesc(baseType);
				if (typeDesc != null)
				{
					TypeDesc typeDesc2 = structMapping.TypeDesc;
					if (baseTypeCanBeIndirect)
					{
						while (typeDesc2.BaseTypeDesc != null && typeDesc2.BaseTypeDesc != typeDesc)
						{
							typeDesc2 = typeDesc2.BaseTypeDesc;
						}
					}
					if (typeDesc2.BaseTypeDesc != null && typeDesc2.BaseTypeDesc != typeDesc)
					{
						throw new InvalidOperationException(Res.GetString("Type {0} cannot derive from {1} because it already has base type {2}.", new object[]
						{
							structMapping.TypeDesc.FullName,
							baseType.FullName,
							typeDesc2.BaseTypeDesc.FullName
						}));
					}
					typeDesc2.BaseTypeDesc = typeDesc;
				}
			}
		}

		internal string GenerateUniqueTypeName(string typeName)
		{
			typeName = CodeIdentifier.MakeValid(typeName);
			return this.TypeIdentifiers.AddUnique(typeName, typeName);
		}

		private StructMapping CreateRootMapping()
		{
			TypeDesc typeDesc = this.Scope.GetTypeDesc(typeof(object));
			return new StructMapping
			{
				TypeDesc = typeDesc,
				Members = new MemberMapping[0],
				IncludeInSchema = false,
				TypeName = "anyType",
				Namespace = "http://www.w3.org/2001/XMLSchema"
			};
		}

		internal StructMapping GetRootMapping()
		{
			if (this.root == null)
			{
				this.root = this.CreateRootMapping();
			}
			return this.root;
		}

		internal StructMapping ImportRootMapping()
		{
			if (!this.rootImported)
			{
				this.rootImported = true;
				this.ImportDerivedTypes(XmlQualifiedName.Empty);
			}
			return this.GetRootMapping();
		}

		internal abstract void ImportDerivedTypes(XmlQualifiedName baseName);

		internal void AddReference(XmlQualifiedName name, NameTable references, string error)
		{
			if (name.Namespace == "http://www.w3.org/2001/XMLSchema")
			{
				return;
			}
			if (references[name] != null)
			{
				throw new InvalidOperationException(Res.GetString(error, new object[] { name.Name, name.Namespace }));
			}
			references[name] = name;
		}

		internal void RemoveReference(XmlQualifiedName name, NameTable references)
		{
			references[name] = null;
		}

		internal void AddReservedIdentifiersForDataBinding(CodeIdentifiers scope)
		{
			if ((this.options & CodeGenerationOptions.EnableDataBinding) != CodeGenerationOptions.None)
			{
				scope.AddReserved(CodeExporter.PropertyChangedEvent.Name);
				scope.AddReserved(CodeExporter.RaisePropertyChangedEventMethod.Name);
			}
		}

		internal SchemaImporter()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private XmlSchemas schemas;

		private StructMapping root;

		private CodeGenerationOptions options;

		private CodeDomProvider codeProvider;

		private TypeScope scope;

		private ImportContext context;

		private bool rootImported;

		private NameTable typesInUse;

		private NameTable groupsInUse;

		private SchemaImporterExtensionCollection extensions;
	}
}
