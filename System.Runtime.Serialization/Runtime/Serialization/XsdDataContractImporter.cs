using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	public class XsdDataContractImporter
	{
		public XsdDataContractImporter()
		{
		}

		public XsdDataContractImporter(CodeCompileUnit codeCompileUnit)
		{
			this.codeCompileUnit = codeCompileUnit;
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

		public CodeCompileUnit CodeCompileUnit
		{
			get
			{
				return this.GetCodeCompileUnit();
			}
		}

		private CodeCompileUnit GetCodeCompileUnit()
		{
			if (this.codeCompileUnit == null)
			{
				this.codeCompileUnit = new CodeCompileUnit();
			}
			return this.codeCompileUnit;
		}

		private DataContractSet DataContractSet
		{
			get
			{
				if (this.dataContractSet == null)
				{
					this.dataContractSet = ((this.Options == null) ? new DataContractSet(null, null, null) : new DataContractSet(this.Options.DataContractSurrogate, this.Options.ReferencedTypes, this.Options.ReferencedCollectionTypes));
				}
				return this.dataContractSet;
			}
		}

		public void Import(XmlSchemaSet schemas)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			this.InternalImport(schemas, null, null, null);
		}

		public void Import(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			if (typeNames == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));
			}
			this.InternalImport(schemas, typeNames, XsdDataContractImporter.emptyElementArray, XsdDataContractImporter.emptyTypeNameArray);
		}

		public void Import(XmlSchemaSet schemas, XmlQualifiedName typeName)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			if (typeName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
			}
			this.SingleTypeNameArray[0] = typeName;
			this.InternalImport(schemas, this.SingleTypeNameArray, XsdDataContractImporter.emptyElementArray, XsdDataContractImporter.emptyTypeNameArray);
		}

		public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
			}
			this.SingleTypeNameArray[0] = null;
			this.SingleElementArray[0] = element;
			this.InternalImport(schemas, XsdDataContractImporter.emptyTypeNameArray, this.SingleElementArray, this.SingleTypeNameArray);
			return this.SingleTypeNameArray[0];
		}

		public bool CanImport(XmlSchemaSet schemas)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			return this.InternalCanImport(schemas, null, null, null);
		}

		public bool CanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			if (typeNames == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));
			}
			return this.InternalCanImport(schemas, typeNames, XsdDataContractImporter.emptyElementArray, XsdDataContractImporter.emptyTypeNameArray);
		}

		public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName typeName)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			if (typeName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
			}
			return this.InternalCanImport(schemas, new XmlQualifiedName[] { typeName }, XsdDataContractImporter.emptyElementArray, XsdDataContractImporter.emptyTypeNameArray);
		}

		public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
		{
			if (schemas == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
			}
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
			}
			this.SingleTypeNameArray[0] = null;
			this.SingleElementArray[0] = element;
			return this.InternalCanImport(schemas, XsdDataContractImporter.emptyTypeNameArray, this.SingleElementArray, this.SingleTypeNameArray);
		}

		public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName)
		{
			DataContract dataContract = this.FindDataContract(typeName);
			return new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit()).GetCodeTypeReference(dataContract);
		}

		public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName, XmlSchemaElement element)
		{
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
			}
			if (typeName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
			}
			DataContract dataContract = this.FindDataContract(typeName);
			return new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit()).GetElementTypeReference(dataContract, element.IsNillable);
		}

		internal DataContract FindDataContract(XmlQualifiedName typeName)
		{
			if (typeName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
			}
			DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
			if (dataContract == null)
			{
				dataContract = this.DataContractSet[typeName];
				if (dataContract == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' in '{1}' namespace has not been imported.", new object[] { typeName.Name, typeName.Namespace })));
				}
			}
			return dataContract;
		}

		public ICollection<CodeTypeReference> GetKnownTypeReferences(XmlQualifiedName typeName)
		{
			if (typeName == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
			}
			DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
			if (dataContract == null)
			{
				dataContract = this.DataContractSet[typeName];
				if (dataContract == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' in '{1}' namespace has not been imported.", new object[] { typeName.Name, typeName.Namespace })));
				}
			}
			return new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit()).GetKnownTypeReferences(dataContract);
		}

		private XmlQualifiedName[] SingleTypeNameArray
		{
			get
			{
				if (this.singleTypeNameArray == null)
				{
					this.singleTypeNameArray = new XmlQualifiedName[1];
				}
				return this.singleTypeNameArray;
			}
		}

		private XmlSchemaElement[] SingleElementArray
		{
			get
			{
				if (this.singleElementArray == null)
				{
					this.singleElementArray = new XmlSchemaElement[1];
				}
				return this.singleElementArray;
			}
		}

		[SecuritySafeCritical]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private void InternalImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
		{
			if (DiagnosticUtility.ShouldTraceInformation)
			{
				TraceUtility.Trace(TraceEventType.Information, 196618, global::System.Runtime.Serialization.SR.GetString("XSD import begins"));
			}
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			try
			{
				new SchemaImporter(schemas, typeNames, elements, elementTypeNames, this.DataContractSet, this.ImportXmlDataType).Import();
				new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit()).Export();
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceImportError(ex);
				throw;
			}
			if (DiagnosticUtility.ShouldTraceInformation)
			{
				TraceUtility.Trace(TraceEventType.Information, 196619, global::System.Runtime.Serialization.SR.GetString("XSD import ends"));
			}
		}

		private bool ImportXmlDataType
		{
			get
			{
				return this.Options != null && this.Options.ImportXmlType;
			}
		}

		private void TraceImportError(Exception exception)
		{
			if (DiagnosticUtility.ShouldTraceError)
			{
				TraceUtility.Trace(TraceEventType.Error, 196621, global::System.Runtime.Serialization.SR.GetString("XSD import error"), null, exception);
			}
		}

		private bool InternalCanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
		{
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			bool flag;
			try
			{
				new SchemaImporter(schemas, typeNames, elements, elementTypeNames, this.DataContractSet, this.ImportXmlDataType).Import();
				flag = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				flag = false;
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceImportError(ex);
				throw;
			}
			return flag;
		}

		private ImportOptions options;

		private CodeCompileUnit codeCompileUnit;

		private DataContractSet dataContractSet;

		private static readonly XmlQualifiedName[] emptyTypeNameArray = new XmlQualifiedName[0];

		private static readonly XmlSchemaElement[] emptyElementArray = new XmlSchemaElement[0];

		private XmlQualifiedName[] singleTypeNameArray;

		private XmlSchemaElement[] singleElementArray;
	}
}
