using System;
using System.CodeDom.Compiler;

namespace System.Xml.Serialization
{
	public class SoapSchemaImporter : SchemaImporter
	{
		public SoapSchemaImporter(XmlSchemas schemas)
		{
			this._importer = new XmlSchemaImporter(schemas);
			this._importer.UseEncodedFormat = true;
		}

		public SoapSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers)
		{
			this._importer = new XmlSchemaImporter(schemas, typeIdentifiers);
			this._importer.UseEncodedFormat = true;
		}

		public SoapSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, ImportContext context)
		{
			this._importer = new XmlSchemaImporter(schemas, options, context);
			this._importer.UseEncodedFormat = true;
		}

		public SoapSchemaImporter(XmlSchemas schemas, CodeIdentifiers typeIdentifiers, CodeGenerationOptions options)
		{
			this._importer = new XmlSchemaImporter(schemas, typeIdentifiers, options);
			this._importer.UseEncodedFormat = true;
		}

		public SoapSchemaImporter(XmlSchemas schemas, CodeGenerationOptions options, CodeDomProvider codeProvider, ImportContext context)
		{
			this._importer = new XmlSchemaImporter(schemas, options, codeProvider, context);
			this._importer.UseEncodedFormat = true;
		}

		public XmlTypeMapping ImportDerivedTypeMapping(XmlQualifiedName name, Type baseType, bool baseTypeCanBeIndirect)
		{
			return this._importer.ImportDerivedTypeMapping(name, baseType, baseTypeCanBeIndirect);
		}

		public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember member)
		{
			return this._importer.ImportEncodedMembersMapping(name, ns, member);
		}

		public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members)
		{
			return this._importer.ImportEncodedMembersMapping(name, ns, members, false);
		}

		public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement)
		{
			return this._importer.ImportEncodedMembersMapping(name, ns, members, hasWrapperElement);
		}

		[MonoTODO]
		public XmlMembersMapping ImportMembersMapping(string name, string ns, SoapSchemaMember[] members, bool hasWrapperElement, Type baseType, bool baseTypeCanBeIndirect)
		{
			throw new NotImplementedException();
		}

		private XmlSchemaImporter _importer;
	}
}
