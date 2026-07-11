using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	public class XmlSchemaExporter
	{
		public XmlSchemaExporter(XmlSchemas schemas)
		{
			this.schemas = schemas;
		}

		public void ExportTypeMapping(XmlTypeMapping xmlTypeMapping)
		{
			xmlTypeMapping.CheckShallow();
			this.CheckScope(xmlTypeMapping.Scope);
			this.ExportElement(xmlTypeMapping.Accessor);
			this.ExportRootIfNecessary(xmlTypeMapping.Scope);
		}

		public XmlQualifiedName ExportTypeMapping(XmlMembersMapping xmlMembersMapping)
		{
			xmlMembersMapping.CheckShallow();
			this.CheckScope(xmlMembersMapping.Scope);
			MembersMapping membersMapping = (MembersMapping)xmlMembersMapping.Accessor.Mapping;
			if (membersMapping.Members.Length == 1 && membersMapping.Members[0].Elements[0].Mapping is SpecialMapping)
			{
				SpecialMapping specialMapping = (SpecialMapping)membersMapping.Members[0].Elements[0].Mapping;
				XmlSchemaType xmlSchemaType = this.ExportSpecialMapping(specialMapping, xmlMembersMapping.Accessor.Namespace, false, null);
				if (xmlSchemaType != null && xmlSchemaType.Name != null && xmlSchemaType.Name.Length > 0)
				{
					xmlSchemaType.Name = xmlMembersMapping.Accessor.Name;
					this.AddSchemaItem(xmlSchemaType, xmlMembersMapping.Accessor.Namespace, null);
				}
				this.ExportRootIfNecessary(xmlMembersMapping.Scope);
				return new XmlQualifiedName(xmlMembersMapping.Accessor.Name, xmlMembersMapping.Accessor.Namespace);
			}
			return null;
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping)
		{
			this.ExportMembersMapping(xmlMembersMapping, true);
		}

		public void ExportMembersMapping(XmlMembersMapping xmlMembersMapping, bool exportEnclosingType)
		{
			xmlMembersMapping.CheckShallow();
			MembersMapping membersMapping = (MembersMapping)xmlMembersMapping.Accessor.Mapping;
			this.CheckScope(xmlMembersMapping.Scope);
			if (membersMapping.HasWrapperElement && exportEnclosingType)
			{
				this.ExportElement(xmlMembersMapping.Accessor);
			}
			else
			{
				foreach (MemberMapping memberMapping in membersMapping.Members)
				{
					if (memberMapping.Attribute != null)
					{
						throw new InvalidOperationException(Res.GetString("There was an error exporting '{0}': bare members cannot be attributes.", new object[] { memberMapping.Attribute.Name }));
					}
					if (memberMapping.Text != null)
					{
						throw new InvalidOperationException(Res.GetString("There was an error exporting '{0}': bare members cannot contain text content.", new object[] { memberMapping.Text.Name }));
					}
					if (memberMapping.Elements != null && memberMapping.Elements.Length != 0)
					{
						if (memberMapping.TypeDesc.IsArrayLike && !(memberMapping.Elements[0].Mapping is ArrayMapping))
						{
							throw new InvalidOperationException(Res.GetString("An element declared at the top level of a schema cannot have maxOccurs > 1. Provide a wrapper element for '{0}' by using XmlArray or XmlArrayItem instead of XmlElementAttribute, or by using the Wrapped parameter style.", new object[] { memberMapping.Elements[0].Name }));
						}
						if (exportEnclosingType)
						{
							this.ExportElement(memberMapping.Elements[0]);
						}
						else
						{
							this.ExportMapping(memberMapping.Elements[0].Mapping, memberMapping.Elements[0].Namespace, memberMapping.Elements[0].Any);
						}
					}
				}
			}
			this.ExportRootIfNecessary(xmlMembersMapping.Scope);
		}

		private static XmlSchemaType FindSchemaType(string name, XmlSchemaObjectCollection items)
		{
			foreach (XmlSchemaObject xmlSchemaObject in items)
			{
				XmlSchemaType xmlSchemaType = xmlSchemaObject as XmlSchemaType;
				if (xmlSchemaType != null && xmlSchemaType.Name == name)
				{
					return xmlSchemaType;
				}
			}
			return null;
		}

		private static bool IsAnyType(XmlSchemaType schemaType, bool mixed, bool unbounded)
		{
			XmlSchemaComplexType xmlSchemaComplexType = schemaType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null)
			{
				if (xmlSchemaComplexType.IsMixed != mixed)
				{
					return false;
				}
				if (xmlSchemaComplexType.Particle is XmlSchemaSequence)
				{
					XmlSchemaSequence xmlSchemaSequence = (XmlSchemaSequence)xmlSchemaComplexType.Particle;
					if (xmlSchemaSequence.Items.Count == 1 && xmlSchemaSequence.Items[0] is XmlSchemaAny)
					{
						XmlSchemaAny xmlSchemaAny = (XmlSchemaAny)xmlSchemaSequence.Items[0];
						return unbounded == xmlSchemaAny.IsMultipleOccurrence;
					}
				}
			}
			return false;
		}

		public string ExportAnyType(string ns)
		{
			string text = "any";
			int num = 0;
			XmlSchema xmlSchema = this.schemas[ns];
			if (xmlSchema != null)
			{
				for (;;)
				{
					XmlSchemaType xmlSchemaType = XmlSchemaExporter.FindSchemaType(text, xmlSchema.Items);
					if (xmlSchemaType == null)
					{
						goto IL_0054;
					}
					if (XmlSchemaExporter.IsAnyType(xmlSchemaType, true, true))
					{
						break;
					}
					num++;
					text = "any" + num.ToString(CultureInfo.InvariantCulture);
				}
				return text;
			}
			IL_0054:
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			xmlSchemaComplexType.Name = text;
			xmlSchemaComplexType.IsMixed = true;
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
			xmlSchemaAny.MinOccurs = 0m;
			xmlSchemaAny.MaxOccurs = decimal.MaxValue;
			xmlSchemaSequence.Items.Add(xmlSchemaAny);
			xmlSchemaComplexType.Particle = xmlSchemaSequence;
			this.AddSchemaItem(xmlSchemaComplexType, ns, null);
			return text;
		}

		public string ExportAnyType(XmlMembersMapping members)
		{
			if (members.Count == 1 && members[0].Any && members[0].ElementName.Length == 0)
			{
				XmlMemberMapping xmlMemberMapping = members[0];
				string @namespace = xmlMemberMapping.Namespace;
				bool flag = xmlMemberMapping.Mapping.TypeDesc.IsArrayLike;
				bool flag2 = ((flag && xmlMemberMapping.Mapping.TypeDesc.ArrayElementTypeDesc != null) ? xmlMemberMapping.Mapping.TypeDesc.ArrayElementTypeDesc.IsMixed : xmlMemberMapping.Mapping.TypeDesc.IsMixed);
				if (flag2 && xmlMemberMapping.Mapping.TypeDesc.IsMixed)
				{
					flag = true;
				}
				string text = (flag2 ? "any" : (flag ? "anyElements" : "anyElement"));
				string text2 = text;
				int num = 0;
				XmlSchema xmlSchema = this.schemas[@namespace];
				if (xmlSchema != null)
				{
					for (;;)
					{
						XmlSchemaType xmlSchemaType = XmlSchemaExporter.FindSchemaType(text2, xmlSchema.Items);
						if (xmlSchemaType == null)
						{
							goto IL_011A;
						}
						if (XmlSchemaExporter.IsAnyType(xmlSchemaType, flag2, flag))
						{
							break;
						}
						num++;
						text2 = text + num.ToString(CultureInfo.InvariantCulture);
					}
					return text2;
				}
				IL_011A:
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.Name = text2;
				xmlSchemaComplexType.IsMixed = flag2;
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.MinOccurs = 0m;
				if (flag)
				{
					xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				}
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				this.AddSchemaItem(xmlSchemaComplexType, @namespace, null);
				return text2;
			}
			return null;
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

		private XmlSchemaElement ExportElement(ElementAccessor accessor)
		{
			if (!accessor.Mapping.IncludeInSchema && !accessor.Mapping.TypeDesc.IsRoot)
			{
				return null;
			}
			if (accessor.Any && accessor.Name.Length == 0)
			{
				throw new InvalidOperationException(Res.GetString("Cannot use wildcards at the top level of a schema."));
			}
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)this.elements[accessor];
			if (xmlSchemaElement != null)
			{
				return xmlSchemaElement;
			}
			xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = accessor.Name;
			xmlSchemaElement.IsNillable = accessor.IsNullable;
			this.elements.Add(accessor, xmlSchemaElement);
			xmlSchemaElement.Form = accessor.Form;
			this.AddSchemaItem(xmlSchemaElement, accessor.Namespace, null);
			this.ExportElementMapping(xmlSchemaElement, accessor.Mapping, accessor.Namespace, accessor.Any);
			return xmlSchemaElement;
		}

		private void CheckForDuplicateType(TypeMapping mapping, string newNamespace)
		{
			if (mapping.IsAnonymousType)
			{
				return;
			}
			string typeName = mapping.TypeName;
			XmlSchema xmlSchema = this.schemas[newNamespace];
			if (xmlSchema != null)
			{
				foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				{
					XmlSchemaType xmlSchemaType = xmlSchemaObject as XmlSchemaType;
					if (xmlSchemaType != null && xmlSchemaType.Name == typeName)
					{
						throw new InvalidOperationException(Res.GetString("A type with the name {0} has already been added in namespace {1}.", new object[] { typeName, newNamespace }));
					}
				}
			}
		}

		private XmlSchema AddSchema(string targetNamespace)
		{
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.TargetNamespace = (string.IsNullOrEmpty(targetNamespace) ? null : targetNamespace);
			xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
			xmlSchema.AttributeFormDefault = XmlSchemaForm.None;
			this.schemas.Add(xmlSchema);
			return xmlSchema;
		}

		private void AddSchemaItem(XmlSchemaObject item, string ns, string referencingNs)
		{
			XmlSchema xmlSchema = this.schemas[ns];
			if (xmlSchema == null)
			{
				xmlSchema = this.AddSchema(ns);
			}
			if (item is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)item;
				if (xmlSchemaElement.Form == XmlSchemaForm.Unqualified)
				{
					throw new InvalidOperationException(Res.GetString("There was an error exporting '{0}': elements declared at the top level of a schema cannot be unqualified.", new object[] { xmlSchemaElement.Name }));
				}
				xmlSchemaElement.Form = XmlSchemaForm.None;
			}
			else if (item is XmlSchemaAttribute)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)item;
				if (xmlSchemaAttribute.Form == XmlSchemaForm.Unqualified)
				{
					throw new InvalidOperationException(Res.GetString("There was an error exporting '{0}': elements declared at the top level of a schema cannot be unqualified.", new object[] { xmlSchemaAttribute.Name }));
				}
				xmlSchemaAttribute.Form = XmlSchemaForm.None;
			}
			xmlSchema.Items.Add(item);
			this.AddSchemaImport(ns, referencingNs);
		}

		private void AddSchemaImport(string ns, string referencingNs)
		{
			if (referencingNs == null)
			{
				return;
			}
			if (XmlSchemaExporter.NamespacesEqual(ns, referencingNs))
			{
				return;
			}
			XmlSchema xmlSchema = this.schemas[referencingNs];
			if (xmlSchema == null)
			{
				xmlSchema = this.AddSchema(referencingNs);
			}
			if (this.FindImport(xmlSchema, ns) == null)
			{
				XmlSchemaImport xmlSchemaImport = new XmlSchemaImport();
				if (ns != null && ns.Length > 0)
				{
					xmlSchemaImport.Namespace = ns;
				}
				xmlSchema.Includes.Add(xmlSchemaImport);
			}
		}

		private static bool NamespacesEqual(string ns1, string ns2)
		{
			if (ns1 == null || ns1.Length == 0)
			{
				return ns2 == null || ns2.Length == 0;
			}
			return ns1 == ns2;
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
					if (XmlSchemaExporter.NamespacesEqual(xmlSchemaImport.Namespace, ns))
					{
						return xmlSchemaImport;
					}
				}
			}
			return null;
		}

		private void ExportMapping(Mapping mapping, string ns, bool isAny)
		{
			if (mapping is ArrayMapping)
			{
				this.ExportArrayMapping((ArrayMapping)mapping, ns, null);
				return;
			}
			if (mapping is PrimitiveMapping)
			{
				this.ExportPrimitiveMapping((PrimitiveMapping)mapping, ns);
				return;
			}
			if (mapping is StructMapping)
			{
				this.ExportStructMapping((StructMapping)mapping, ns, null);
				return;
			}
			if (mapping is MembersMapping)
			{
				this.ExportMembersMapping((MembersMapping)mapping, ns);
				return;
			}
			if (mapping is SpecialMapping)
			{
				this.ExportSpecialMapping((SpecialMapping)mapping, ns, isAny, null);
				return;
			}
			if (mapping is NullableMapping)
			{
				this.ExportMapping(((NullableMapping)mapping).BaseMapping, ns, isAny);
				return;
			}
			throw new ArgumentException(Res.GetString("Internal error."), "mapping");
		}

		private void ExportElementMapping(XmlSchemaElement element, Mapping mapping, string ns, bool isAny)
		{
			if (mapping is ArrayMapping)
			{
				this.ExportArrayMapping((ArrayMapping)mapping, ns, element);
				return;
			}
			if (mapping is PrimitiveMapping)
			{
				PrimitiveMapping primitiveMapping = (PrimitiveMapping)mapping;
				if (primitiveMapping.IsAnonymousType)
				{
					element.SchemaType = this.ExportAnonymousPrimitiveMapping(primitiveMapping);
					return;
				}
				element.SchemaTypeName = this.ExportPrimitiveMapping(primitiveMapping, ns);
				return;
			}
			else
			{
				if (mapping is StructMapping)
				{
					this.ExportStructMapping((StructMapping)mapping, ns, element);
					return;
				}
				if (mapping is MembersMapping)
				{
					element.SchemaType = this.ExportMembersMapping((MembersMapping)mapping, ns);
					return;
				}
				if (mapping is SpecialMapping)
				{
					this.ExportSpecialMapping((SpecialMapping)mapping, ns, isAny, element);
					return;
				}
				if (mapping is NullableMapping)
				{
					this.ExportElementMapping(element, ((NullableMapping)mapping).BaseMapping, ns, isAny);
					return;
				}
				throw new ArgumentException(Res.GetString("Internal error."), "mapping");
			}
		}

		private XmlQualifiedName ExportNonXsdPrimitiveMapping(PrimitiveMapping mapping, string ns)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)mapping.TypeDesc.DataType;
			if (!this.SchemaContainsItem(xmlSchemaSimpleType, "http://microsoft.com/wsdl/types/"))
			{
				this.AddSchemaItem(xmlSchemaSimpleType, "http://microsoft.com/wsdl/types/", ns);
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			return new XmlQualifiedName(mapping.TypeDesc.DataType.Name, "http://microsoft.com/wsdl/types/");
		}

		private XmlSchemaType ExportSpecialMapping(SpecialMapping mapping, string ns, bool isAny, XmlSchemaElement element)
		{
			TypeKind kind = mapping.TypeDesc.Kind;
			if (kind == TypeKind.Node)
			{
				XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
				xmlSchemaComplexType.IsMixed = mapping.TypeDesc.IsMixed;
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				if (isAny)
				{
					xmlSchemaComplexType.AnyAttribute = new XmlSchemaAnyAttribute();
					xmlSchemaComplexType.IsMixed = true;
					xmlSchemaAny.MaxOccurs = decimal.MaxValue;
				}
				xmlSchemaSequence.Items.Add(xmlSchemaAny);
				xmlSchemaComplexType.Particle = xmlSchemaSequence;
				if (element != null)
				{
					element.SchemaType = xmlSchemaComplexType;
				}
				return xmlSchemaComplexType;
			}
			if (kind != TypeKind.Serializable)
			{
				throw new ArgumentException(Res.GetString("Internal error."), "mapping");
			}
			SerializableMapping serializableMapping = (SerializableMapping)mapping;
			if (serializableMapping.IsAny)
			{
				XmlSchemaComplexType xmlSchemaComplexType2 = new XmlSchemaComplexType();
				xmlSchemaComplexType2.IsMixed = mapping.TypeDesc.IsMixed;
				XmlSchemaSequence xmlSchemaSequence2 = new XmlSchemaSequence();
				XmlSchemaAny xmlSchemaAny2 = new XmlSchemaAny();
				if (isAny)
				{
					xmlSchemaComplexType2.AnyAttribute = new XmlSchemaAnyAttribute();
					xmlSchemaComplexType2.IsMixed = true;
					xmlSchemaAny2.MaxOccurs = decimal.MaxValue;
				}
				if (serializableMapping.NamespaceList.Length > 0)
				{
					xmlSchemaAny2.Namespace = serializableMapping.NamespaceList;
				}
				xmlSchemaAny2.ProcessContents = XmlSchemaContentProcessing.Lax;
				if (serializableMapping.Schemas != null)
				{
					foreach (object obj in serializableMapping.Schemas.Schemas())
					{
						XmlSchema xmlSchema = (XmlSchema)obj;
						if (xmlSchema.TargetNamespace != "http://www.w3.org/2001/XMLSchema")
						{
							this.schemas.Add(xmlSchema, true);
							this.AddSchemaImport(xmlSchema.TargetNamespace, ns);
						}
					}
				}
				xmlSchemaSequence2.Items.Add(xmlSchemaAny2);
				xmlSchemaComplexType2.Particle = xmlSchemaSequence2;
				if (element != null)
				{
					element.SchemaType = xmlSchemaComplexType2;
				}
				return xmlSchemaComplexType2;
			}
			if (serializableMapping.XsiType != null || serializableMapping.XsdType != null)
			{
				XmlSchemaType xmlSchemaType = serializableMapping.XsdType;
				foreach (object obj2 in serializableMapping.Schemas.Schemas())
				{
					XmlSchema xmlSchema2 = (XmlSchema)obj2;
					if (xmlSchema2.TargetNamespace != "http://www.w3.org/2001/XMLSchema")
					{
						this.schemas.Add(xmlSchema2, true);
						this.AddSchemaImport(xmlSchema2.TargetNamespace, ns);
						if (!serializableMapping.XsiType.IsEmpty && serializableMapping.XsiType.Namespace == xmlSchema2.TargetNamespace)
						{
							xmlSchemaType = (XmlSchemaType)xmlSchema2.SchemaTypes[serializableMapping.XsiType];
						}
					}
				}
				if (element != null)
				{
					element.SchemaTypeName = serializableMapping.XsiType;
					if (element.SchemaTypeName.IsEmpty)
					{
						element.SchemaType = xmlSchemaType;
					}
				}
				serializableMapping.CheckDuplicateElement(element, ns);
				return xmlSchemaType;
			}
			if (serializableMapping.Schema != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType3 = new XmlSchemaComplexType();
				XmlSchemaAny xmlSchemaAny3 = new XmlSchemaAny();
				xmlSchemaComplexType3.Particle = new XmlSchemaSequence
				{
					Items = { xmlSchemaAny3 }
				};
				string targetNamespace = serializableMapping.Schema.TargetNamespace;
				xmlSchemaAny3.Namespace = ((targetNamespace == null) ? "" : targetNamespace);
				XmlSchema xmlSchema3 = this.schemas[targetNamespace];
				if (xmlSchema3 == null)
				{
					this.schemas.Add(serializableMapping.Schema);
				}
				else if (xmlSchema3 != serializableMapping.Schema)
				{
					throw new InvalidOperationException(Res.GetString("The namespace, {0}, is a duplicate.", new object[] { targetNamespace }));
				}
				if (element != null)
				{
					element.SchemaType = xmlSchemaComplexType3;
				}
				serializableMapping.CheckDuplicateElement(element, ns);
				return xmlSchemaComplexType3;
			}
			XmlSchemaComplexType xmlSchemaComplexType4 = new XmlSchemaComplexType();
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.RefName = new XmlQualifiedName("schema", "http://www.w3.org/2001/XMLSchema");
			xmlSchemaComplexType4.Particle = new XmlSchemaSequence
			{
				Items = 
				{
					xmlSchemaElement,
					new XmlSchemaAny()
				}
			};
			this.AddSchemaImport("http://www.w3.org/2001/XMLSchema", ns);
			if (element != null)
			{
				element.SchemaType = xmlSchemaComplexType4;
			}
			return xmlSchemaComplexType4;
		}

		private XmlSchemaType ExportMembersMapping(MembersMapping mapping, string ns)
		{
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			this.ExportTypeMembers(xmlSchemaComplexType, mapping.Members, mapping.TypeName, ns, false, false);
			if (mapping.XmlnsMember != null)
			{
				this.AddXmlnsAnnotation(xmlSchemaComplexType, mapping.XmlnsMember.Name);
			}
			return xmlSchemaComplexType;
		}

		private XmlSchemaType ExportAnonymousPrimitiveMapping(PrimitiveMapping mapping)
		{
			if (mapping is EnumMapping)
			{
				return this.ExportEnumMapping((EnumMapping)mapping, null);
			}
			throw new InvalidOperationException(Res.GetString("Internal error: {0}.", new object[] { "Unsuported anonymous mapping type: " + mapping.ToString() }));
		}

		private XmlQualifiedName ExportPrimitiveMapping(PrimitiveMapping mapping, string ns)
		{
			XmlQualifiedName xmlQualifiedName;
			if (mapping is EnumMapping)
			{
				xmlQualifiedName = new XmlQualifiedName(this.ExportEnumMapping((EnumMapping)mapping, ns).Name, mapping.Namespace);
			}
			else if (mapping.TypeDesc.IsXsdType)
			{
				xmlQualifiedName = new XmlQualifiedName(mapping.TypeDesc.DataType.Name, "http://www.w3.org/2001/XMLSchema");
			}
			else
			{
				xmlQualifiedName = this.ExportNonXsdPrimitiveMapping(mapping, ns);
			}
			return xmlQualifiedName;
		}

		private void ExportArrayMapping(ArrayMapping mapping, string ns, XmlSchemaElement element)
		{
			ArrayMapping arrayMapping = mapping;
			while (arrayMapping.Next != null)
			{
				arrayMapping = arrayMapping.Next;
			}
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)this.types[arrayMapping];
			if (xmlSchemaComplexType == null)
			{
				this.CheckForDuplicateType(arrayMapping, arrayMapping.Namespace);
				xmlSchemaComplexType = new XmlSchemaComplexType();
				if (!mapping.IsAnonymousType)
				{
					xmlSchemaComplexType.Name = mapping.TypeName;
					this.AddSchemaItem(xmlSchemaComplexType, mapping.Namespace, ns);
				}
				if (!arrayMapping.IsAnonymousType)
				{
					this.types.Add(arrayMapping, xmlSchemaComplexType);
				}
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				this.ExportElementAccessors(xmlSchemaSequence, mapping.Elements, true, false, mapping.Namespace);
				if (xmlSchemaSequence.Items.Count > 0)
				{
					if (xmlSchemaSequence.Items[0] is XmlSchemaChoice)
					{
						xmlSchemaComplexType.Particle = (XmlSchemaChoice)xmlSchemaSequence.Items[0];
					}
					else
					{
						xmlSchemaComplexType.Particle = xmlSchemaSequence;
					}
				}
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			if (element != null)
			{
				if (mapping.IsAnonymousType)
				{
					element.SchemaType = xmlSchemaComplexType;
					return;
				}
				element.SchemaTypeName = new XmlQualifiedName(xmlSchemaComplexType.Name, mapping.Namespace);
			}
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

		private void ExportAttributeAccessor(XmlSchemaComplexType type, AttributeAccessor accessor, bool valueTypeOptional, string ns)
		{
			if (accessor == null)
			{
				return;
			}
			XmlSchemaObjectCollection xmlSchemaObjectCollection;
			if (type.ContentModel != null)
			{
				if (type.ContentModel.Content is XmlSchemaComplexContentRestriction)
				{
					xmlSchemaObjectCollection = ((XmlSchemaComplexContentRestriction)type.ContentModel.Content).Attributes;
				}
				else if (type.ContentModel.Content is XmlSchemaComplexContentExtension)
				{
					xmlSchemaObjectCollection = ((XmlSchemaComplexContentExtension)type.ContentModel.Content).Attributes;
				}
				else
				{
					if (!(type.ContentModel.Content is XmlSchemaSimpleContentExtension))
					{
						throw new InvalidOperationException(Res.GetString("Invalid content {0}.", new object[] { type.ContentModel.Content.GetType().Name }));
					}
					xmlSchemaObjectCollection = ((XmlSchemaSimpleContentExtension)type.ContentModel.Content).Attributes;
				}
			}
			else
			{
				xmlSchemaObjectCollection = type.Attributes;
			}
			if (accessor.IsSpecialXmlNamespace)
			{
				this.AddSchemaImport("http://www.w3.org/XML/1998/namespace", ns);
				xmlSchemaObjectCollection.Add(new XmlSchemaAttribute
				{
					Use = XmlSchemaUse.Optional,
					RefName = new XmlQualifiedName(accessor.Name, "http://www.w3.org/XML/1998/namespace")
				});
				return;
			}
			if (accessor.Any)
			{
				if (type.ContentModel == null)
				{
					type.AnyAttribute = new XmlSchemaAnyAttribute();
					return;
				}
				XmlSchemaContent content = type.ContentModel.Content;
				if (content is XmlSchemaComplexContentExtension)
				{
					((XmlSchemaComplexContentExtension)content).AnyAttribute = new XmlSchemaAnyAttribute();
					return;
				}
				if (content is XmlSchemaComplexContentRestriction)
				{
					((XmlSchemaComplexContentRestriction)content).AnyAttribute = new XmlSchemaAnyAttribute();
					return;
				}
				if (type.ContentModel.Content is XmlSchemaSimpleContentExtension)
				{
					((XmlSchemaSimpleContentExtension)content).AnyAttribute = new XmlSchemaAnyAttribute();
					return;
				}
			}
			else
			{
				XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
				xmlSchemaAttribute.Use = XmlSchemaUse.None;
				if (!accessor.HasDefault && !valueTypeOptional && accessor.Mapping.TypeDesc.IsValueType)
				{
					xmlSchemaAttribute.Use = XmlSchemaUse.Required;
				}
				xmlSchemaAttribute.Name = accessor.Name;
				if (accessor.Namespace == null || accessor.Namespace == ns)
				{
					XmlSchema xmlSchema = this.schemas[ns];
					if (xmlSchema == null)
					{
						xmlSchemaAttribute.Form = ((accessor.Form == XmlSchemaForm.Unqualified) ? XmlSchemaForm.None : accessor.Form);
					}
					else
					{
						xmlSchemaAttribute.Form = ((accessor.Form == xmlSchema.AttributeFormDefault) ? XmlSchemaForm.None : accessor.Form);
					}
					xmlSchemaObjectCollection.Add(xmlSchemaAttribute);
				}
				else
				{
					if (this.attributes[accessor] == null)
					{
						xmlSchemaAttribute.Use = XmlSchemaUse.None;
						xmlSchemaAttribute.Form = accessor.Form;
						this.AddSchemaItem(xmlSchemaAttribute, accessor.Namespace, ns);
						this.attributes.Add(accessor, accessor);
					}
					xmlSchemaObjectCollection.Add(new XmlSchemaAttribute
					{
						Use = XmlSchemaUse.None,
						RefName = new XmlQualifiedName(accessor.Name, accessor.Namespace)
					});
					this.AddSchemaImport(accessor.Namespace, ns);
				}
				if (accessor.Mapping is PrimitiveMapping)
				{
					PrimitiveMapping primitiveMapping = (PrimitiveMapping)accessor.Mapping;
					if (primitiveMapping.IsList)
					{
						XmlSchemaSimpleType xmlSchemaSimpleType = new XmlSchemaSimpleType();
						XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList();
						if (primitiveMapping.IsAnonymousType)
						{
							xmlSchemaSimpleTypeList.ItemType = (XmlSchemaSimpleType)this.ExportAnonymousPrimitiveMapping(primitiveMapping);
						}
						else
						{
							xmlSchemaSimpleTypeList.ItemTypeName = this.ExportPrimitiveMapping(primitiveMapping, (accessor.Namespace == null) ? ns : accessor.Namespace);
						}
						xmlSchemaSimpleType.Content = xmlSchemaSimpleTypeList;
						xmlSchemaAttribute.SchemaType = xmlSchemaSimpleType;
					}
					else if (primitiveMapping.IsAnonymousType)
					{
						xmlSchemaAttribute.SchemaType = (XmlSchemaSimpleType)this.ExportAnonymousPrimitiveMapping(primitiveMapping);
					}
					else
					{
						xmlSchemaAttribute.SchemaTypeName = this.ExportPrimitiveMapping(primitiveMapping, (accessor.Namespace == null) ? ns : accessor.Namespace);
					}
				}
				else if (!(accessor.Mapping is SpecialMapping))
				{
					throw new InvalidOperationException(Res.GetString("Internal error."));
				}
				if (accessor.HasDefault)
				{
					xmlSchemaAttribute.DefaultValue = XmlSchemaExporter.ExportDefaultValue(accessor.Mapping, accessor.Default);
				}
			}
		}

		private void ExportElementAccessor(XmlSchemaGroupBase group, ElementAccessor accessor, bool repeats, bool valueTypeOptional, string ns)
		{
			if (accessor.Any && accessor.Name.Length == 0)
			{
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccurs = (repeats ? decimal.MaxValue : 1m);
				if (accessor.Namespace != null && accessor.Namespace.Length > 0 && accessor.Namespace != ns)
				{
					xmlSchemaAny.Namespace = accessor.Namespace;
				}
				group.Items.Add(xmlSchemaAny);
				return;
			}
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)this.elements[accessor];
			int num = ((repeats || accessor.HasDefault || (!accessor.IsNullable && !accessor.Mapping.TypeDesc.IsValueType) || valueTypeOptional) ? 0 : 1);
			decimal num2 = ((repeats || accessor.IsUnbounded) ? decimal.MaxValue : 1m);
			if (xmlSchemaElement == null)
			{
				xmlSchemaElement = new XmlSchemaElement();
				xmlSchemaElement.IsNillable = accessor.IsNullable;
				xmlSchemaElement.Name = accessor.Name;
				if (accessor.HasDefault)
				{
					xmlSchemaElement.DefaultValue = XmlSchemaExporter.ExportDefaultValue(accessor.Mapping, accessor.Default);
				}
				if (accessor.IsTopLevelInSchema)
				{
					this.elements.Add(accessor, xmlSchemaElement);
					xmlSchemaElement.Form = accessor.Form;
					this.AddSchemaItem(xmlSchemaElement, accessor.Namespace, ns);
				}
				else
				{
					xmlSchemaElement.MinOccurs = num;
					xmlSchemaElement.MaxOccurs = num2;
					XmlSchema xmlSchema = this.schemas[ns];
					if (xmlSchema == null)
					{
						xmlSchemaElement.Form = ((accessor.Form == XmlSchemaForm.Qualified) ? XmlSchemaForm.None : accessor.Form);
					}
					else
					{
						xmlSchemaElement.Form = ((accessor.Form == xmlSchema.ElementFormDefault) ? XmlSchemaForm.None : accessor.Form);
					}
				}
				this.ExportElementMapping(xmlSchemaElement, accessor.Mapping, accessor.Namespace, accessor.Any);
			}
			if (accessor.IsTopLevelInSchema)
			{
				XmlSchemaElement xmlSchemaElement2 = new XmlSchemaElement();
				xmlSchemaElement2.RefName = new XmlQualifiedName(accessor.Name, accessor.Namespace);
				xmlSchemaElement2.MinOccurs = num;
				xmlSchemaElement2.MaxOccurs = num2;
				group.Items.Add(xmlSchemaElement2);
				this.AddSchemaImport(accessor.Namespace, ns);
				return;
			}
			group.Items.Add(xmlSchemaElement);
		}

		internal static string ExportDefaultValue(TypeMapping mapping, object value)
		{
			if (!(mapping is PrimitiveMapping))
			{
				return null;
			}
			if (value == null || value == DBNull.Value)
			{
				return null;
			}
			if (mapping is EnumMapping)
			{
				EnumMapping enumMapping = (EnumMapping)mapping;
				ConstantMapping[] constants = enumMapping.Constants;
				if (!enumMapping.IsFlags)
				{
					for (int i = 0; i < constants.Length; i++)
					{
						if (constants[i].Name == (string)value)
						{
							return constants[i].XmlName;
						}
					}
					return null;
				}
				string[] array = new string[constants.Length];
				long[] array2 = new long[constants.Length];
				Hashtable hashtable = new Hashtable();
				for (int j = 0; j < constants.Length; j++)
				{
					array[j] = constants[j].XmlName;
					array2[j] = 1L << (j & 31);
					hashtable.Add(constants[j].Name, array2[j]);
				}
				long num = XmlCustomFormatter.ToEnum((string)value, hashtable, enumMapping.TypeName, false);
				if (num == 0L)
				{
					return null;
				}
				return XmlCustomFormatter.FromEnum(num, array, array2, mapping.TypeDesc.FullName);
			}
			else
			{
				PrimitiveMapping primitiveMapping = (PrimitiveMapping)mapping;
				if (!primitiveMapping.TypeDesc.HasCustomFormatter)
				{
					if (primitiveMapping.TypeDesc.FormatterName == "String")
					{
						return (string)value;
					}
					Type typeFromHandle = typeof(XmlConvert);
					MethodInfo method = typeFromHandle.GetMethod("ToString", new Type[] { primitiveMapping.TypeDesc.Type });
					if (method != null)
					{
						return (string)method.Invoke(typeFromHandle, new object[] { value });
					}
					throw new InvalidOperationException(Res.GetString("Value '{0}' cannot be converted to {1}.", new object[]
					{
						value.ToString(),
						primitiveMapping.TypeDesc.Name
					}));
				}
				else
				{
					string text = XmlCustomFormatter.FromDefaultValue(value, primitiveMapping.TypeDesc.FormatterName);
					if (text == null)
					{
						throw new InvalidOperationException(Res.GetString("Value '{0}' cannot be converted to {1}.", new object[]
						{
							value.ToString(),
							primitiveMapping.TypeDesc.Name
						}));
					}
					return text;
				}
			}
		}

		private void ExportRootIfNecessary(TypeScope typeScope)
		{
			if (!this.needToExportRoot)
			{
				return;
			}
			foreach (object obj in typeScope.TypeMappings)
			{
				TypeMapping typeMapping = (TypeMapping)obj;
				if (typeMapping is StructMapping && typeMapping.TypeDesc.IsRoot)
				{
					this.ExportDerivedMappings((StructMapping)typeMapping);
				}
				else if (typeMapping is ArrayMapping)
				{
					this.ExportArrayMapping((ArrayMapping)typeMapping, typeMapping.Namespace, null);
				}
				else if (typeMapping is SerializableMapping)
				{
					this.ExportSpecialMapping((SerializableMapping)typeMapping, typeMapping.Namespace, false, null);
				}
			}
		}

		private XmlQualifiedName ExportStructMapping(StructMapping mapping, string ns, XmlSchemaElement element)
		{
			if (mapping.TypeDesc.IsRoot)
			{
				this.needToExportRoot = true;
				return XmlQualifiedName.Empty;
			}
			if (mapping.IsAnonymousType)
			{
				if (this.references[mapping] != null)
				{
					throw new InvalidOperationException(Res.GetString("A circular type reference was detected in anonymous type '{0}'.  Please change '{0}' to be a named type by setting {1}={2} in the type definition.", new object[]
					{
						mapping.TypeDesc.Name,
						"AnonymousType",
						"false"
					}));
				}
				this.references[mapping] = mapping;
			}
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)this.types[mapping];
			if (xmlSchemaComplexType == null)
			{
				if (!mapping.IncludeInSchema)
				{
					throw new InvalidOperationException(Res.GetString("The type {0} may not be exported to a schema because the IncludeInSchema property of the XmlType attribute is 'false'.", new object[] { mapping.TypeDesc.Name }));
				}
				this.CheckForDuplicateType(mapping, mapping.Namespace);
				xmlSchemaComplexType = new XmlSchemaComplexType();
				if (!mapping.IsAnonymousType)
				{
					xmlSchemaComplexType.Name = mapping.TypeName;
					this.AddSchemaItem(xmlSchemaComplexType, mapping.Namespace, ns);
					this.types.Add(mapping, xmlSchemaComplexType);
				}
				xmlSchemaComplexType.IsAbstract = mapping.TypeDesc.IsAbstract;
				bool flag = mapping.IsOpenModel;
				if (mapping.BaseMapping != null && mapping.BaseMapping.IncludeInSchema)
				{
					if (mapping.BaseMapping.IsAnonymousType)
					{
						throw new InvalidOperationException(Res.GetString("Illegal type derivation: Type '{0}' derives from anonymous type '{1}'. Please change '{1}' to be a named type by setting {2}={3} in the type definition.", new object[]
						{
							mapping.TypeDesc.Name,
							mapping.BaseMapping.TypeDesc.Name,
							"AnonymousType",
							"false"
						}));
					}
					if (mapping.HasSimpleContent)
					{
						xmlSchemaComplexType.ContentModel = new XmlSchemaSimpleContent
						{
							Content = new XmlSchemaSimpleContentExtension
							{
								BaseTypeName = this.ExportStructMapping(mapping.BaseMapping, mapping.Namespace, null)
							}
						};
					}
					else
					{
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = new XmlSchemaComplexContentExtension();
						xmlSchemaComplexContentExtension.BaseTypeName = this.ExportStructMapping(mapping.BaseMapping, mapping.Namespace, null);
						xmlSchemaComplexType.ContentModel = new XmlSchemaComplexContent
						{
							Content = xmlSchemaComplexContentExtension,
							IsMixed = XmlSchemaImporter.IsMixed((XmlSchemaComplexType)this.types[mapping.BaseMapping])
						};
					}
					flag = false;
				}
				this.ExportTypeMembers(xmlSchemaComplexType, mapping.Members, mapping.TypeName, mapping.Namespace, mapping.HasSimpleContent, flag);
				this.ExportDerivedMappings(mapping);
				if (mapping.XmlnsMember != null)
				{
					this.AddXmlnsAnnotation(xmlSchemaComplexType, mapping.XmlnsMember.Name);
				}
			}
			else
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			if (mapping.IsAnonymousType)
			{
				this.references[mapping] = null;
				if (element != null)
				{
					element.SchemaType = xmlSchemaComplexType;
				}
				return XmlQualifiedName.Empty;
			}
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(xmlSchemaComplexType.Name, mapping.Namespace);
			if (element != null)
			{
				element.SchemaTypeName = xmlQualifiedName;
			}
			return xmlQualifiedName;
		}

		private void ExportTypeMembers(XmlSchemaComplexType type, MemberMapping[] members, string name, string ns, bool hasSimpleContent, bool openModel)
		{
			XmlSchemaGroupBase xmlSchemaGroupBase = new XmlSchemaSequence();
			TypeMapping typeMapping = null;
			foreach (MemberMapping memberMapping in members)
			{
				if (!memberMapping.Ignore)
				{
					if (memberMapping.Text != null)
					{
						if (typeMapping != null)
						{
							throw new InvalidOperationException(Res.GetString("Cannot serialize object of type '{0}' because it has multiple XmlText attributes. Consider using an array of strings with XmlTextAttribute for serialization of a mixed complex type.", new object[] { name }));
						}
						typeMapping = memberMapping.Text.Mapping;
					}
					if (memberMapping.Elements.Length != 0)
					{
						bool flag = memberMapping.TypeDesc.IsArrayLike && (memberMapping.Elements.Length != 1 || !(memberMapping.Elements[0].Mapping is ArrayMapping));
						bool flag2 = memberMapping.CheckSpecified != SpecifiedAccessor.None || memberMapping.CheckShouldPersist;
						this.ExportElementAccessors(xmlSchemaGroupBase, memberMapping.Elements, flag, flag2, ns);
					}
				}
			}
			if (xmlSchemaGroupBase.Items.Count > 0)
			{
				if (type.ContentModel != null)
				{
					if (type.ContentModel.Content is XmlSchemaComplexContentRestriction)
					{
						((XmlSchemaComplexContentRestriction)type.ContentModel.Content).Particle = xmlSchemaGroupBase;
					}
					else
					{
						if (!(type.ContentModel.Content is XmlSchemaComplexContentExtension))
						{
							throw new InvalidOperationException(Res.GetString("Invalid content {0}.", new object[] { type.ContentModel.Content.GetType().Name }));
						}
						((XmlSchemaComplexContentExtension)type.ContentModel.Content).Particle = xmlSchemaGroupBase;
					}
				}
				else
				{
					type.Particle = xmlSchemaGroupBase;
				}
			}
			if (typeMapping != null)
			{
				if (hasSimpleContent)
				{
					if (typeMapping is PrimitiveMapping && xmlSchemaGroupBase.Items.Count == 0)
					{
						PrimitiveMapping primitiveMapping = (PrimitiveMapping)typeMapping;
						if (primitiveMapping.IsList)
						{
							type.IsMixed = true;
						}
						else
						{
							if (primitiveMapping.IsAnonymousType)
							{
								throw new InvalidOperationException(Res.GetString("Illegal type derivation: Type '{0}' derives from anonymous type '{1}'. Please change '{1}' to be a named type by setting {2}={3} in the type definition.", new object[]
								{
									typeMapping.TypeDesc.Name,
									primitiveMapping.TypeDesc.Name,
									"AnonymousType",
									"false"
								}));
							}
							XmlSchemaSimpleContent xmlSchemaSimpleContent = new XmlSchemaSimpleContent();
							XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = new XmlSchemaSimpleContentExtension();
							xmlSchemaSimpleContent.Content = xmlSchemaSimpleContentExtension;
							type.ContentModel = xmlSchemaSimpleContent;
							xmlSchemaSimpleContentExtension.BaseTypeName = this.ExportPrimitiveMapping(primitiveMapping, ns);
						}
					}
				}
				else
				{
					type.IsMixed = true;
				}
			}
			bool flag3 = false;
			for (int j = 0; j < members.Length; j++)
			{
				if (members[j].Attribute != null)
				{
					this.ExportAttributeAccessor(type, members[j].Attribute, members[j].CheckSpecified != SpecifiedAccessor.None || members[j].CheckShouldPersist, ns);
					if (members[j].Attribute.Any)
					{
						flag3 = true;
					}
				}
			}
			if (openModel && !flag3)
			{
				this.ExportAttributeAccessor(type, new AttributeAccessor
				{
					Any = true
				}, false, ns);
			}
		}

		private void ExportDerivedMappings(StructMapping mapping)
		{
			if (mapping.IsAnonymousType)
			{
				return;
			}
			for (StructMapping structMapping = mapping.DerivedMappings; structMapping != null; structMapping = structMapping.NextDerivedMapping)
			{
				if (structMapping.IncludeInSchema)
				{
					this.ExportStructMapping(structMapping, structMapping.Namespace, null);
				}
			}
		}

		private XmlSchemaType ExportEnumMapping(EnumMapping mapping, string ns)
		{
			if (!mapping.IncludeInSchema)
			{
				throw new InvalidOperationException(Res.GetString("The type {0} may not be exported to a schema because the IncludeInSchema property of the XmlType attribute is 'false'.", new object[] { mapping.TypeDesc.Name }));
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)this.types[mapping];
			if (xmlSchemaSimpleType == null)
			{
				this.CheckForDuplicateType(mapping, mapping.Namespace);
				xmlSchemaSimpleType = new XmlSchemaSimpleType();
				xmlSchemaSimpleType.Name = mapping.TypeName;
				if (!mapping.IsAnonymousType)
				{
					this.types.Add(mapping, xmlSchemaSimpleType);
					this.AddSchemaItem(xmlSchemaSimpleType, mapping.Namespace, ns);
				}
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
			if (!mapping.IsAnonymousType)
			{
				this.AddSchemaImport(mapping.Namespace, ns);
			}
			return xmlSchemaSimpleType;
		}

		private void AddXmlnsAnnotation(XmlSchemaComplexType type, string xmlnsMemberName)
		{
			XmlSchemaAnnotation xmlSchemaAnnotation = new XmlSchemaAnnotation();
			XmlSchemaAppInfo xmlSchemaAppInfo = new XmlSchemaAppInfo();
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("keepNamespaceDeclarations");
			if (xmlnsMemberName != null)
			{
				xmlElement.InsertBefore(xmlDocument.CreateTextNode(xmlnsMemberName), null);
			}
			xmlSchemaAppInfo.Markup = new XmlNode[] { xmlElement };
			xmlSchemaAnnotation.Items.Add(xmlSchemaAppInfo);
			type.Annotation = xmlSchemaAnnotation;
		}

		internal const XmlSchemaForm elementFormDefault = XmlSchemaForm.Qualified;

		internal const XmlSchemaForm attributeFormDefault = XmlSchemaForm.Unqualified;

		private XmlSchemas schemas;

		private Hashtable elements = new Hashtable();

		private Hashtable attributes = new Hashtable();

		private Hashtable types = new Hashtable();

		private Hashtable references = new Hashtable();

		private bool needToExportRoot;

		private TypeScope scope;
	}
}
