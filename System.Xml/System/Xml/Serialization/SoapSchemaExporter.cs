using System;
using System.Collections;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class SoapSchemaExporter
	{
		public SoapSchemaExporter(XmlSchemas schemas)
		{
			this.schemas = schemas;
		}

		public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
		{
			this.CheckScope(xmlTypeMapping.Scope);
			this.ExportTypeMapping(xmlTypeMapping.Mapping, null);
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
		{
			this.ExportMembersMapping(xmlMembersMapping, false);
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping, bool exportEnclosingType)
		{
			this.CheckScope(xmlMembersMapping.Scope);
			MembersMapping membersMapping = (MembersMapping)xmlMembersMapping.Accessor.Mapping;
			if (exportEnclosingType)
			{
				this.ExportTypeMapping(membersMapping, null);
				return;
			}
			foreach (MemberMapping memberMapping in membersMapping.Members)
			{
				if (memberMapping.Elements.Length != 0)
				{
					this.ExportTypeMapping(memberMapping.Elements[0].Mapping, null);
				}
			}
		}

		private void CheckScope(TypeScope scope)
		{
			if (this.scope == null)
			{
				this.scope = scope;
				return;
			}
			if (this.scope != scope)
			{
				throw new InvalidOperationException(Res.GetString("Exported mappings must come from the same importer."));
			}
		}

		internal XmlDocument Document
		{
			get
			{
				if (this.document == null)
				{
					this.document = new XmlDocument();
				}
				return this.document;
			}
		}

		private void CheckForDuplicateType(string newTypeName, string newNamespace)
		{
			XmlSchema xmlSchema = this.schemas[newNamespace];
			if (xmlSchema != null)
			{
				foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				{
					XmlSchemaType xmlSchemaType = xmlSchemaObject as XmlSchemaType;
					if (xmlSchemaType != null && xmlSchemaType.Name == newTypeName)
					{
						throw new InvalidOperationException(Res.GetString("A type with the name {0} has already been added in namespace {1}.", new object[] { newTypeName, newNamespace }));
					}
				}
			}
		}

		private void AddSchemaItem(XmlSchemaObject item, string ns, string referencingNs)
		{
			if (!this.SchemaContainsItem(item, ns))
			{
				XmlSchema xmlSchema = this.schemas[ns];
				if (xmlSchema == null)
				{
					xmlSchema = new XmlSchema();
					xmlSchema.TargetNamespace = ((ns == null || ns.Length == 0) ? null : ns);
					xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
					this.schemas.Add(xmlSchema);
				}
				xmlSchema.Items.Add(item);
			}
			if (referencingNs != null)
			{
				this.AddSchemaImport(ns, referencingNs);
			}
		}

		private void AddSchemaImport(string ns, string referencingNs)
		{
			if (referencingNs == null || ns == null)
			{
				return;
			}
			if (ns == referencingNs)
			{
				return;
			}
			XmlSchema xmlSchema = this.schemas[referencingNs];
			if (xmlSchema == null)
			{
				throw new InvalidOperationException(Res.GetString("Missing schema targetNamespace=\"{0}\".", new object[] { referencingNs }));
			}
			if (ns != null && ns.Length > 0 && this.FindImport(xmlSchema, ns) == null)
			{
				XmlSchemaImport xmlSchemaImport = new XmlSchemaImport();
				xmlSchemaImport.Namespace = ns;
				xmlSchema.Includes.Add(xmlSchemaImport);
			}
		}

		private bool SchemaContainsItem(XmlSchemaObject item, string ns)
		{
			XmlSchema xmlSchema = this.schemas[ns];
			return xmlSchema != null && xmlSchema.Items.Contains(item);
		}

		private XmlSchemaImport FindImport(XmlSchema schema, string ns)
		{
			foreach (object obj in schema.Includes)
			{
				if (obj is XmlSchemaImport)
				{
					XmlSchemaImport xmlSchemaImport = (XmlSchemaImport)obj;
					if (xmlSchemaImport.Namespace == ns)
					{
						return xmlSchemaImport;
					}
				}
			}
			return null;
		}

		private XmlQualifiedName ExportTypeMapping(TypeMapping mapping, string ns)
		{
			if (mapping is ArrayMapping)
			{
				return this.ExportArrayMapping((ArrayMapping)mapping, ns);
			}
			if (mapping is EnumMapping)
			{
				return this.ExportEnumMapping((EnumMapping)mapping, ns);
			}
			if (mapping is PrimitiveMapping)
			{
				PrimitiveMapping primitiveMapping = (PrimitiveMapping)mapping;
				if (primitiveMapping.TypeDesc.IsXsdType)
				{
					return this.ExportPrimitiveMapping(primitiveMapping);
				}
				return this.ExportNonXsdPrimitiveMapping(primitiveMapping, ns);
			}
			else
			{
				if (mapping is StructMapping)
				{
					return this.ExportStructMapping((StructMapping)mapping, ns);
				}
				if (mapping is NullableMapping)
				{
					return this.ExportTypeMapping(((NullableMapping)mapping).BaseMapping, ns);
				}
				if (mapping is MembersMapping)
				{
					return this.ExportMembersMapping((MembersMapping)mapping, ns);
				}
				throw new ArgumentException(Res.GetString("Internal error."), "mapping");
			}
		}

		private XmlQualifiedName ExportNonXsdPrimitiveMapping(PrimitiveMapping mapping, string ns)
		{
			XmlSchemaType dataType = mapping.TypeDesc.DataType;
			if (!this.SchemaContainsItem(dataType, "http://microsoft.com/wsdl/types/"))
			{
				this.AddSchemaItem(dataType, "http://microsoft.com/wsdl/types/", ns);
			}
			else
			{
				this.AddSchemaImport("http://microsoft.com/wsdl/types/", ns);
			}
			return new XmlQualifiedName(mapping.TypeDesc.DataType.Name, "http://microsoft.com/wsdl/types/");
		}

		private XmlQualifiedName ExportPrimitiveMapping(PrimitiveMapping mapping)
		{
			return new XmlQualifiedName(mapping.TypeDesc.DataType.Name, "http://www.w3.org/2001/XMLSchema");
		}

		private XmlQualifiedName ExportArrayMapping(ArrayMapping mapping, string ns)
		{
			while (mapping.Next != null)
			{
				mapping = mapping.Next;
			}
			if ((XmlSchemaComplexType)this.types[mapping] == null)
			{
				this.CheckForDuplicateType(mapping.TypeName, mapping.Namespace);
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = mapping.TypeName;
				this.types.Add(mapping, xmlSchemaComplexType);
				this.AddSchemaItem(xmlSchemaComplexType, mapping.Namespace, ns);
				this.AddSchemaImport("http://schemas.xmlsoap.org/soap/encoding/", mapping.Namespace);
				this.AddSchemaImport("http://schemas.xmlsoap.org/wsdl/", mapping.Namespace);
				XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = new XmlSchemaComplexContentRestriction();
				XmlQualifiedName xmlQualifiedName = this.ExportTypeMapping(mapping.Elements[0].Mapping, mapping.Namespace);
				if (xmlQualifiedName.IsEmpty)
				{
					xmlQualifiedName = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
				}
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.RefName = SoapSchemaExporter.ArrayTypeQName;
				xmlSchemaAttribute.UnhandledAttributes = new XmlAttribute[]
				{
					new XmlAttribute("wsdl", "arrayType", "http://schemas.xmlsoap.org/wsdl/", this.Document)
					{
						Value = xmlQualifiedName.Namespace + ":" + xmlQualifiedName.Name + "[]"
					}
				};
				xmlSchemaComplexContentRestriction.Attributes.Add(xmlSchemaAttribute);
				xmlSchemaComplexContentRestriction.BaseTypeName = SoapSchemaExporter.ArrayQName;
				xmlSchemaComplexType.ContentModel = new XmlSchemaComplexContent
				{
					Content = xmlSchemaComplexContentRestriction
				};
				if (xmlQualifiedName.Namespace != "http://www.w3.org/2001/XMLSchema")
				{
					this.AddSchemaImport(xmlQualifiedName.Namespace, mapping.Namespace);
				}
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			return new XmlQualifiedName(mapping.TypeName, mapping.Namespace);
		}

		private void ExportElementAccessors(XmlSchemaGroupBase group, ElementAccessor[] accessors, bool repeats, bool valueTypeOptional, string ns)
		{
			if (accessors.Length == 0)
			{
				return;
			}
			if (accessors.Length == 1)
			{
				this.ExportElementAccessor(group, accessors[0], repeats, valueTypeOptional, ns);
				return;
			}
			XmlSchemaChoice xmlSchemaChoice = new XmlSchemaChoice();
			xmlSchemaChoice.MaxOccurs = (repeats ? decimal.MaxValue : 1m);
			xmlSchemaChoice.MinOccurs = (repeats ? 0 : 1);
			for (int i = 0; i < accessors.Length; i++)
			{
				this.ExportElementAccessor(xmlSchemaChoice, accessors[i], false, valueTypeOptional, ns);
			}
			if (xmlSchemaChoice.Items.Count > 0)
			{
				group.Items.Add(xmlSchemaChoice);
			}
		}

		private void ExportElementAccessor(XmlSchemaGroupBase group, ElementAccessor accessor, bool repeats, bool valueTypeOptional, string ns)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.MinOccurs = ((repeats || valueTypeOptional) ? 0 : 1);
			xmlSchemaElement.MaxOccurs = (repeats ? decimal.MaxValue : 1m);
			xmlSchemaElement.Name = accessor.Name;
			xmlSchemaElement.IsNillable = accessor.IsNullable || accessor.Mapping is NullableMapping;
			xmlSchemaElement.Form = XmlSchemaForm.Unqualified;
			xmlSchemaElement.SchemaTypeName = this.ExportTypeMapping(accessor.Mapping, accessor.Namespace);
			group.Items.Add(xmlSchemaElement);
		}

		private XmlQualifiedName ExportRootMapping(StructMapping mapping)
		{
			if (!this.exportedRoot)
			{
				this.exportedRoot = true;
				this.ExportDerivedMappings(mapping);
			}
			return new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
		}

		private XmlQualifiedName ExportStructMapping(StructMapping mapping, string ns)
		{
			if (mapping.TypeDesc.IsRoot)
			{
				return this.ExportRootMapping(mapping);
			}
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)this.types[mapping];
			if (xmlSchemaComplexType == null)
			{
				if (!mapping.IncludeInSchema)
				{
					throw new InvalidOperationException(Res.GetString("The type {0} may not be exported to a schema because the IncludeInSchema property of the SoapType attribute is 'false'.", new object[] { mapping.TypeDesc.Name }));
				}
				this.CheckForDuplicateType(mapping.TypeName, mapping.Namespace);
				xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = mapping.TypeName;
				this.types.Add(mapping, xmlSchemaComplexType);
				this.AddSchemaItem(xmlSchemaComplexType, mapping.Namespace, ns);
				xmlSchemaComplexType.IsAbstract = mapping.TypeDesc.IsAbstract;
				if (mapping.BaseMapping != null && mapping.BaseMapping.IncludeInSchema)
				{
					XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = new XmlSchemaComplexContentExtension();
					xmlSchemaComplexContentExtension.BaseTypeName = this.ExportStructMapping(mapping.BaseMapping, mapping.Namespace);
					xmlSchemaComplexType.ContentModel = new XmlSchemaComplexContent
					{
						Content = xmlSchemaComplexContentExtension
					};
				}
				this.ExportTypeMembers(xmlSchemaComplexType, mapping.Members, mapping.Namespace);
				this.ExportDerivedMappings(mapping);
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			return new XmlQualifiedName(xmlSchemaComplexType.Name, mapping.Namespace);
		}

		private XmlQualifiedName ExportMembersMapping(MembersMapping mapping, string ns)
		{
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)this.types[mapping];
			if (xmlSchemaComplexType == null)
			{
				this.CheckForDuplicateType(mapping.TypeName, mapping.Namespace);
				xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = mapping.TypeName;
				this.types.Add(mapping, xmlSchemaComplexType);
				this.AddSchemaItem(xmlSchemaComplexType, mapping.Namespace, ns);
				this.ExportTypeMembers(xmlSchemaComplexType, mapping.Members, mapping.Namespace);
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			return new XmlQualifiedName(xmlSchemaComplexType.Name, mapping.Namespace);
		}

		private void ExportTypeMembers(XmlSchemaComplexType type, MemberMapping[] members, string ns)
		{
			XmlSchemaGroupBase xmlSchemaGroupBase = new XmlSchemaSequence();
			foreach (MemberMapping memberMapping in members)
			{
				if (memberMapping.Elements.Length != 0)
				{
					bool flag = memberMapping.CheckSpecified != SpecifiedAccessor.None || memberMapping.CheckShouldPersist || !memberMapping.TypeDesc.IsValueType;
					this.ExportElementAccessors(xmlSchemaGroupBase, memberMapping.Elements, false, flag, ns);
				}
			}
			if (xmlSchemaGroupBase.Items.Count > 0)
			{
				if (type.ContentModel != null)
				{
					if (type.ContentModel.Content is XmlSchemaComplexContentExtension)
					{
						((XmlSchemaComplexContentExtension)type.ContentModel.Content).Particle = xmlSchemaGroupBase;
						return;
					}
					if (type.ContentModel.Content is XmlSchemaComplexContentRestriction)
					{
						((XmlSchemaComplexContentRestriction)type.ContentModel.Content).Particle = xmlSchemaGroupBase;
						return;
					}
					throw new InvalidOperationException(Res.GetString("Invalid content {0}.", new object[] { type.ContentModel.Content.GetType().Name }));
				}
				else
				{
					type.Particle = xmlSchemaGroupBase;
				}
			}
		}

		private void ExportDerivedMappings(StructMapping mapping)
		{
			for (StructMapping structMapping = mapping.DerivedMappings; structMapping != null; structMapping = structMapping.NextDerivedMapping)
			{
				if (structMapping.IncludeInSchema)
				{
					this.ExportStructMapping(structMapping, mapping.TypeDesc.IsRoot ? null : mapping.Namespace);
				}
			}
		}

		private XmlQualifiedName ExportEnumMapping(EnumMapping mapping, string ns)
		{
			if ((XmlSchemaSimpleType)this.types[mapping] == null)
			{
				this.CheckForDuplicateType(mapping.TypeName, mapping.Namespace);
				XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
				xmlSchemaSimpleType.Name = mapping.TypeName;
				this.types.Add(mapping, xmlSchemaSimpleType);
				this.AddSchemaItem(xmlSchemaSimpleType, mapping.Namespace, ns);
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
				xmlSchemaSimpleTypeRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
				for (int i = 0; i < mapping.Constants.Length; i++)
				{
					ConstantMapping constantMapping = mapping.Constants[i];
					XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = new XmlSchemaEnumerationFacet();
					xmlSchemaEnumerationFacet.Value = constantMapping.XmlName;
					xmlSchemaSimpleTypeRestriction.Facets.Add(xmlSchemaEnumerationFacet);
				}
				if (!mapping.IsFlags)
				{
					xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeRestriction;
				}
				else
				{
					xmlSchemaSimpleType.Content = new XmlSchemaSimpleTypeList
					{
						ItemType = new XmlSchemaSimpleType
						{
							Content = xmlSchemaSimpleTypeRestriction
						}
					};
				}
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			return new XmlQualifiedName(mapping.TypeName, mapping.Namespace);
		}

		internal const XmlSchemaForm elementFormDefault = XmlSchemaForm.Qualified;

		private XmlSchemas schemas;

		private Hashtable types = new Hashtable();

		private bool exportedRoot;

		private TypeScope scope;

		private XmlDocument document;

		private static XmlQualifiedName ArrayQName = new XmlQualifiedName("Array", "http://schemas.xmlsoap.org/soap/encoding/");

		private static XmlQualifiedName ArrayTypeQName = new XmlQualifiedName("arrayType", "http://schemas.xmlsoap.org/soap/encoding/");
	}
}
