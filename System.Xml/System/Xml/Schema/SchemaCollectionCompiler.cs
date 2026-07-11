using System;
using System.Collections;
using System.Globalization;

namespace System.Xml.Schema
{
	internal sealed class SchemaCollectionCompiler : BaseProcessor
	{
		public SchemaCollectionCompiler(XmlNameTable nameTable, ValidationEventHandler eventHandler)
			: base(nameTable, null, eventHandler)
		{
		}

		public bool Execute(XmlSchema schema, SchemaInfo schemaInfo, bool compileContentModel)
		{
			this.compileContentModel = compileContentModel;
			this.schema = schema;
			this.Prepare();
			this.Cleanup();
			this.Compile();
			if (!base.HasErrors)
			{
				this.Output(schemaInfo);
			}
			return !base.HasErrors;
		}

		private void Prepare()
		{
			foreach (object obj in this.schema.Elements.Values)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj;
				if (!xmlSchemaElement.SubstitutionGroup.IsEmpty)
				{
					XmlSchemaSubstitutionGroup xmlSchemaSubstitutionGroup = (XmlSchemaSubstitutionGroup)this.examplars[xmlSchemaElement.SubstitutionGroup];
					if (xmlSchemaSubstitutionGroup == null)
					{
						xmlSchemaSubstitutionGroup = new XmlSchemaSubstitutionGroupV1Compat();
						xmlSchemaSubstitutionGroup.Examplar = xmlSchemaElement.SubstitutionGroup;
						this.examplars.Add(xmlSchemaElement.SubstitutionGroup, xmlSchemaSubstitutionGroup);
					}
					xmlSchemaSubstitutionGroup.Members.Add(xmlSchemaElement);
				}
			}
		}

		private void Cleanup()
		{
			foreach (object obj in this.schema.Groups.Values)
			{
				SchemaCollectionCompiler.CleanupGroup((XmlSchemaGroup)obj);
			}
			foreach (object obj2 in this.schema.AttributeGroups.Values)
			{
				SchemaCollectionCompiler.CleanupAttributeGroup((XmlSchemaAttributeGroup)obj2);
			}
			foreach (object obj3 in this.schema.SchemaTypes.Values)
			{
				XmlSchemaType xmlSchemaType = (XmlSchemaType)obj3;
				if (xmlSchemaType is XmlSchemaComplexType)
				{
					SchemaCollectionCompiler.CleanupComplexType((XmlSchemaComplexType)xmlSchemaType);
				}
				else
				{
					SchemaCollectionCompiler.CleanupSimpleType((XmlSchemaSimpleType)xmlSchemaType);
				}
			}
			foreach (object obj4 in this.schema.Elements.Values)
			{
				SchemaCollectionCompiler.CleanupElement((XmlSchemaElement)obj4);
			}
			foreach (object obj5 in this.schema.Attributes.Values)
			{
				SchemaCollectionCompiler.CleanupAttribute((XmlSchemaAttribute)obj5);
			}
		}

		internal static void Cleanup(XmlSchema schema)
		{
			for (int i = 0; i < schema.Includes.Count; i++)
			{
				XmlSchemaExternal xmlSchemaExternal = (XmlSchemaExternal)schema.Includes[i];
				if (xmlSchemaExternal.Schema != null)
				{
					SchemaCollectionCompiler.Cleanup(xmlSchemaExternal.Schema);
				}
				XmlSchemaRedefine xmlSchemaRedefine = xmlSchemaExternal as XmlSchemaRedefine;
				if (xmlSchemaRedefine != null)
				{
					xmlSchemaRedefine.AttributeGroups.Clear();
					xmlSchemaRedefine.Groups.Clear();
					xmlSchemaRedefine.SchemaTypes.Clear();
					for (int j = 0; j < xmlSchemaRedefine.Items.Count; j++)
					{
						object obj = xmlSchemaRedefine.Items[j];
						XmlSchemaAttribute xmlSchemaAttribute;
						XmlSchemaAttributeGroup xmlSchemaAttributeGroup;
						XmlSchemaComplexType xmlSchemaComplexType;
						XmlSchemaSimpleType xmlSchemaSimpleType;
						XmlSchemaElement xmlSchemaElement;
						XmlSchemaGroup xmlSchemaGroup;
						if ((xmlSchemaAttribute = obj as XmlSchemaAttribute) != null)
						{
							SchemaCollectionCompiler.CleanupAttribute(xmlSchemaAttribute);
						}
						else if ((xmlSchemaAttributeGroup = obj as XmlSchemaAttributeGroup) != null)
						{
							SchemaCollectionCompiler.CleanupAttributeGroup(xmlSchemaAttributeGroup);
						}
						else if ((xmlSchemaComplexType = obj as XmlSchemaComplexType) != null)
						{
							SchemaCollectionCompiler.CleanupComplexType(xmlSchemaComplexType);
						}
						else if ((xmlSchemaSimpleType = obj as XmlSchemaSimpleType) != null)
						{
							SchemaCollectionCompiler.CleanupSimpleType(xmlSchemaSimpleType);
						}
						else if ((xmlSchemaElement = obj as XmlSchemaElement) != null)
						{
							SchemaCollectionCompiler.CleanupElement(xmlSchemaElement);
						}
						else if ((xmlSchemaGroup = obj as XmlSchemaGroup) != null)
						{
							SchemaCollectionCompiler.CleanupGroup(xmlSchemaGroup);
						}
					}
				}
			}
			for (int k = 0; k < schema.Items.Count; k++)
			{
				XmlSchemaAttribute xmlSchemaAttribute2;
				XmlSchemaAttributeGroup xmlSchemaAttributeGroup2;
				XmlSchemaComplexType xmlSchemaComplexType2;
				XmlSchemaSimpleType xmlSchemaSimpleType2;
				XmlSchemaElement xmlSchemaElement2;
				XmlSchemaGroup xmlSchemaGroup2;
				if ((xmlSchemaAttribute2 = schema.Items[k] as XmlSchemaAttribute) != null)
				{
					SchemaCollectionCompiler.CleanupAttribute(xmlSchemaAttribute2);
				}
				else if ((xmlSchemaAttributeGroup2 = schema.Items[k] as XmlSchemaAttributeGroup) != null)
				{
					SchemaCollectionCompiler.CleanupAttributeGroup(xmlSchemaAttributeGroup2);
				}
				else if ((xmlSchemaComplexType2 = schema.Items[k] as XmlSchemaComplexType) != null)
				{
					SchemaCollectionCompiler.CleanupComplexType(xmlSchemaComplexType2);
				}
				else if ((xmlSchemaSimpleType2 = schema.Items[k] as XmlSchemaSimpleType) != null)
				{
					SchemaCollectionCompiler.CleanupSimpleType(xmlSchemaSimpleType2);
				}
				else if ((xmlSchemaElement2 = schema.Items[k] as XmlSchemaElement) != null)
				{
					SchemaCollectionCompiler.CleanupElement(xmlSchemaElement2);
				}
				else if ((xmlSchemaGroup2 = schema.Items[k] as XmlSchemaGroup) != null)
				{
					SchemaCollectionCompiler.CleanupGroup(xmlSchemaGroup2);
				}
			}
			schema.Attributes.Clear();
			schema.AttributeGroups.Clear();
			schema.SchemaTypes.Clear();
			schema.Elements.Clear();
			schema.Groups.Clear();
			schema.Notations.Clear();
			schema.Ids.Clear();
			schema.IdentityConstraints.Clear();
		}

		private void Compile()
		{
			this.schema.SchemaTypes.Insert(DatatypeImplementation.QnAnyType, XmlSchemaComplexType.AnyType);
			foreach (object obj in this.examplars.Values)
			{
				XmlSchemaSubstitutionGroupV1Compat xmlSchemaSubstitutionGroupV1Compat = (XmlSchemaSubstitutionGroupV1Compat)obj;
				this.CompileSubstitutionGroup(xmlSchemaSubstitutionGroupV1Compat);
			}
			foreach (object obj2 in this.schema.Groups.Values)
			{
				XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup)obj2;
				this.CompileGroup(xmlSchemaGroup);
			}
			foreach (object obj3 in this.schema.AttributeGroups.Values)
			{
				XmlSchemaAttributeGroup xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)obj3;
				this.CompileAttributeGroup(xmlSchemaAttributeGroup);
			}
			foreach (object obj4 in this.schema.SchemaTypes.Values)
			{
				XmlSchemaType xmlSchemaType = (XmlSchemaType)obj4;
				if (xmlSchemaType is XmlSchemaComplexType)
				{
					this.CompileComplexType((XmlSchemaComplexType)xmlSchemaType);
				}
				else
				{
					this.CompileSimpleType((XmlSchemaSimpleType)xmlSchemaType);
				}
			}
			foreach (object obj5 in this.schema.Elements.Values)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj5;
				if (xmlSchemaElement.ElementDecl == null)
				{
					this.CompileElement(xmlSchemaElement);
				}
			}
			foreach (object obj6 in this.schema.Attributes.Values)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)obj6;
				if (xmlSchemaAttribute.AttDef == null)
				{
					this.CompileAttribute(xmlSchemaAttribute);
				}
			}
			using (IEnumerator enumerator = this.schema.IdentityConstraints.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj7 = enumerator.Current;
					XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = (XmlSchemaIdentityConstraint)obj7;
					if (xmlSchemaIdentityConstraint.CompiledConstraint == null)
					{
						this.CompileIdentityConstraint(xmlSchemaIdentityConstraint);
					}
				}
				goto IL_025B;
			}
			IL_0241:
			XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)this.complexTypeStack.Pop();
			this.CompileCompexTypeElements(xmlSchemaComplexType);
			IL_025B:
			if (this.complexTypeStack.Count <= 0)
			{
				foreach (object obj8 in this.schema.SchemaTypes.Values)
				{
					XmlSchemaType xmlSchemaType2 = (XmlSchemaType)obj8;
					if (xmlSchemaType2 is XmlSchemaComplexType)
					{
						this.CheckParticleDerivation((XmlSchemaComplexType)xmlSchemaType2);
					}
				}
				foreach (object obj9 in this.schema.Elements.Values)
				{
					XmlSchemaElement xmlSchemaElement2 = (XmlSchemaElement)obj9;
					if (xmlSchemaElement2.ElementSchemaType is XmlSchemaComplexType && xmlSchemaElement2.SchemaTypeName == XmlQualifiedName.Empty)
					{
						this.CheckParticleDerivation((XmlSchemaComplexType)xmlSchemaElement2.ElementSchemaType);
					}
				}
				foreach (object obj10 in this.examplars.Values)
				{
					XmlSchemaSubstitutionGroup xmlSchemaSubstitutionGroup = (XmlSchemaSubstitutionGroup)obj10;
					this.CheckSubstitutionGroup(xmlSchemaSubstitutionGroup);
				}
				this.schema.SchemaTypes.Remove(DatatypeImplementation.QnAnyType);
				return;
			}
			goto IL_0241;
		}

		private void Output(SchemaInfo schemaInfo)
		{
			foreach (object obj in this.schema.Elements.Values)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj;
				schemaInfo.TargetNamespaces[xmlSchemaElement.QualifiedName.Namespace] = true;
				schemaInfo.ElementDecls.Add(xmlSchemaElement.QualifiedName, xmlSchemaElement.ElementDecl);
			}
			foreach (object obj2 in this.schema.Attributes.Values)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)obj2;
				schemaInfo.TargetNamespaces[xmlSchemaAttribute.QualifiedName.Namespace] = true;
				schemaInfo.AttributeDecls.Add(xmlSchemaAttribute.QualifiedName, xmlSchemaAttribute.AttDef);
			}
			foreach (object obj3 in this.schema.SchemaTypes.Values)
			{
				XmlSchemaType xmlSchemaType = (XmlSchemaType)obj3;
				schemaInfo.TargetNamespaces[xmlSchemaType.QualifiedName.Namespace] = true;
				XmlSchemaComplexType xmlSchemaComplexType = xmlSchemaType as XmlSchemaComplexType;
				if (xmlSchemaComplexType == null || (!xmlSchemaComplexType.IsAbstract && xmlSchemaType != XmlSchemaComplexType.AnyType))
				{
					schemaInfo.ElementDeclsByType.Add(xmlSchemaType.QualifiedName, xmlSchemaType.ElementDecl);
				}
			}
			foreach (object obj4 in this.schema.Notations.Values)
			{
				XmlSchemaNotation xmlSchemaNotation = (XmlSchemaNotation)obj4;
				schemaInfo.TargetNamespaces[xmlSchemaNotation.QualifiedName.Namespace] = true;
				SchemaNotation schemaNotation = new SchemaNotation(xmlSchemaNotation.QualifiedName);
				schemaNotation.SystemLiteral = xmlSchemaNotation.System;
				schemaNotation.Pubid = xmlSchemaNotation.Public;
				if (!schemaInfo.Notations.ContainsKey(schemaNotation.Name.Name))
				{
					schemaInfo.Notations.Add(schemaNotation.Name.Name, schemaNotation);
				}
			}
		}

		private static void CleanupAttribute(XmlSchemaAttribute attribute)
		{
			if (attribute.SchemaType != null)
			{
				SchemaCollectionCompiler.CleanupSimpleType(attribute.SchemaType);
			}
			attribute.AttDef = null;
		}

		private static void CleanupAttributeGroup(XmlSchemaAttributeGroup attributeGroup)
		{
			SchemaCollectionCompiler.CleanupAttributes(attributeGroup.Attributes);
			attributeGroup.AttributeUses.Clear();
			attributeGroup.AttributeWildcard = null;
		}

		private static void CleanupComplexType(XmlSchemaComplexType complexType)
		{
			if (complexType.ContentModel != null)
			{
				if (complexType.ContentModel is XmlSchemaSimpleContent)
				{
					XmlSchemaSimpleContent xmlSchemaSimpleContent = (XmlSchemaSimpleContent)complexType.ContentModel;
					if (xmlSchemaSimpleContent.Content is XmlSchemaSimpleContentExtension)
					{
						SchemaCollectionCompiler.CleanupAttributes(((XmlSchemaSimpleContentExtension)xmlSchemaSimpleContent.Content).Attributes);
					}
					else
					{
						SchemaCollectionCompiler.CleanupAttributes(((XmlSchemaSimpleContentRestriction)xmlSchemaSimpleContent.Content).Attributes);
					}
				}
				else
				{
					XmlSchemaComplexContent xmlSchemaComplexContent = (XmlSchemaComplexContent)complexType.ContentModel;
					if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
					{
						XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = (XmlSchemaComplexContentExtension)xmlSchemaComplexContent.Content;
						SchemaCollectionCompiler.CleanupParticle(xmlSchemaComplexContentExtension.Particle);
						SchemaCollectionCompiler.CleanupAttributes(xmlSchemaComplexContentExtension.Attributes);
					}
					else
					{
						XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = (XmlSchemaComplexContentRestriction)xmlSchemaComplexContent.Content;
						SchemaCollectionCompiler.CleanupParticle(xmlSchemaComplexContentRestriction.Particle);
						SchemaCollectionCompiler.CleanupAttributes(xmlSchemaComplexContentRestriction.Attributes);
					}
				}
			}
			else
			{
				SchemaCollectionCompiler.CleanupParticle(complexType.Particle);
				SchemaCollectionCompiler.CleanupAttributes(complexType.Attributes);
			}
			complexType.LocalElements.Clear();
			complexType.AttributeUses.Clear();
			complexType.SetAttributeWildcard(null);
			complexType.SetContentTypeParticle(XmlSchemaParticle.Empty);
			complexType.ElementDecl = null;
		}

		private static void CleanupSimpleType(XmlSchemaSimpleType simpleType)
		{
			simpleType.ElementDecl = null;
		}

		private static void CleanupElement(XmlSchemaElement element)
		{
			if (element.SchemaType != null)
			{
				XmlSchemaComplexType xmlSchemaComplexType = element.SchemaType as XmlSchemaComplexType;
				if (xmlSchemaComplexType != null)
				{
					SchemaCollectionCompiler.CleanupComplexType(xmlSchemaComplexType);
				}
				else
				{
					SchemaCollectionCompiler.CleanupSimpleType((XmlSchemaSimpleType)element.SchemaType);
				}
			}
			for (int i = 0; i < element.Constraints.Count; i++)
			{
				((XmlSchemaIdentityConstraint)element.Constraints[i]).CompiledConstraint = null;
			}
			element.ElementDecl = null;
		}

		private static void CleanupAttributes(XmlSchemaObjectCollection attributes)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				XmlSchemaAttribute xmlSchemaAttribute = attributes[i] as XmlSchemaAttribute;
				if (xmlSchemaAttribute != null)
				{
					SchemaCollectionCompiler.CleanupAttribute(xmlSchemaAttribute);
				}
			}
		}

		private static void CleanupGroup(XmlSchemaGroup group)
		{
			SchemaCollectionCompiler.CleanupParticle(group.Particle);
			group.CanonicalParticle = null;
		}

		private static void CleanupParticle(XmlSchemaParticle particle)
		{
			if (particle is XmlSchemaElement)
			{
				SchemaCollectionCompiler.CleanupElement((XmlSchemaElement)particle);
				return;
			}
			if (particle is XmlSchemaGroupBase)
			{
				XmlSchemaObjectCollection items = ((XmlSchemaGroupBase)particle).Items;
				for (int i = 0; i < items.Count; i++)
				{
					SchemaCollectionCompiler.CleanupParticle((XmlSchemaParticle)items[i]);
				}
			}
		}

		private void CompileSubstitutionGroup(XmlSchemaSubstitutionGroupV1Compat substitutionGroup)
		{
			if (substitutionGroup.IsProcessing && substitutionGroup.Members.Count > 0)
			{
				base.SendValidationEvent("Circular substitution group affiliation.", (XmlSchemaElement)substitutionGroup.Members[0]);
				return;
			}
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)this.schema.Elements[substitutionGroup.Examplar];
			if (substitutionGroup.Members.Contains(xmlSchemaElement))
			{
				return;
			}
			substitutionGroup.IsProcessing = true;
			if (xmlSchemaElement != null)
			{
				if (xmlSchemaElement.FinalResolved == XmlSchemaDerivationMethod.All)
				{
					base.SendValidationEvent("Cannot be nominated as the {substitution group affiliation} of any other declaration.", xmlSchemaElement);
				}
				for (int i = 0; i < substitutionGroup.Members.Count; i++)
				{
					XmlSchemaElement xmlSchemaElement2 = (XmlSchemaElement)substitutionGroup.Members[i];
					XmlSchemaSubstitutionGroupV1Compat xmlSchemaSubstitutionGroupV1Compat = (XmlSchemaSubstitutionGroupV1Compat)this.examplars[xmlSchemaElement2.QualifiedName];
					if (xmlSchemaSubstitutionGroupV1Compat != null)
					{
						this.CompileSubstitutionGroup(xmlSchemaSubstitutionGroupV1Compat);
						for (int j = 0; j < xmlSchemaSubstitutionGroupV1Compat.Choice.Items.Count; j++)
						{
							substitutionGroup.Choice.Items.Add(xmlSchemaSubstitutionGroupV1Compat.Choice.Items[j]);
						}
					}
					else
					{
						substitutionGroup.Choice.Items.Add(xmlSchemaElement2);
					}
				}
				substitutionGroup.Choice.Items.Add(xmlSchemaElement);
				substitutionGroup.Members.Add(xmlSchemaElement);
			}
			else if (substitutionGroup.Members.Count > 0)
			{
				base.SendValidationEvent("Reference to undeclared substitution group affiliation.", (XmlSchemaElement)substitutionGroup.Members[0]);
			}
			substitutionGroup.IsProcessing = false;
		}

		private void CheckSubstitutionGroup(XmlSchemaSubstitutionGroup substitutionGroup)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)this.schema.Elements[substitutionGroup.Examplar];
			if (xmlSchemaElement != null)
			{
				for (int i = 0; i < substitutionGroup.Members.Count; i++)
				{
					XmlSchemaElement xmlSchemaElement2 = (XmlSchemaElement)substitutionGroup.Members[i];
					if (xmlSchemaElement2 != xmlSchemaElement && !XmlSchemaType.IsDerivedFrom(xmlSchemaElement2.ElementSchemaType, xmlSchemaElement.ElementSchemaType, xmlSchemaElement.FinalResolved))
					{
						base.SendValidationEvent("'{0}' cannot be a member of substitution group with head element '{1}'.", xmlSchemaElement2.QualifiedName.ToString(), xmlSchemaElement.QualifiedName.ToString(), xmlSchemaElement2);
					}
				}
			}
		}

		private void CompileGroup(XmlSchemaGroup group)
		{
			if (group.IsProcessing)
			{
				base.SendValidationEvent("Circular group reference.", group);
				group.CanonicalParticle = XmlSchemaParticle.Empty;
				return;
			}
			group.IsProcessing = true;
			if (group.CanonicalParticle == null)
			{
				group.CanonicalParticle = this.CannonicalizeParticle(group.Particle, true, true);
			}
			group.IsProcessing = false;
		}

		private void CompileSimpleType(XmlSchemaSimpleType simpleType)
		{
			if (simpleType.IsProcessing)
			{
				throw new XmlSchemaException("Circular type reference.", simpleType);
			}
			if (simpleType.ElementDecl != null)
			{
				return;
			}
			simpleType.IsProcessing = true;
			try
			{
				if (simpleType.Content is XmlSchemaSimpleTypeList)
				{
					XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = (XmlSchemaSimpleTypeList)simpleType.Content;
					simpleType.SetBaseSchemaType(DatatypeImplementation.AnySimpleType);
					XmlSchemaDatatype xmlSchemaDatatype;
					if (xmlSchemaSimpleTypeList.ItemTypeName.IsEmpty)
					{
						this.CompileSimpleType(xmlSchemaSimpleTypeList.ItemType);
						xmlSchemaSimpleTypeList.BaseItemType = xmlSchemaSimpleTypeList.ItemType;
						xmlSchemaDatatype = xmlSchemaSimpleTypeList.ItemType.Datatype;
					}
					else
					{
						XmlSchemaSimpleType simpleType2 = this.GetSimpleType(xmlSchemaSimpleTypeList.ItemTypeName);
						if (simpleType2 == null)
						{
							throw new XmlSchemaException("Type '{0}' is not declared, or is not a simple type.", xmlSchemaSimpleTypeList.ItemTypeName.ToString(), simpleType);
						}
						if ((simpleType2.FinalResolved & XmlSchemaDerivationMethod.List) != XmlSchemaDerivationMethod.Empty)
						{
							base.SendValidationEvent("The base type is the final list.", simpleType);
						}
						xmlSchemaSimpleTypeList.BaseItemType = simpleType2;
						xmlSchemaDatatype = simpleType2.Datatype;
					}
					simpleType.SetDatatype(xmlSchemaDatatype.DeriveByList(simpleType));
					simpleType.SetDerivedBy(XmlSchemaDerivationMethod.List);
				}
				else if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
				{
					XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;
					XmlSchemaDatatype xmlSchemaDatatype2;
					if (xmlSchemaSimpleTypeRestriction.BaseTypeName.IsEmpty)
					{
						this.CompileSimpleType(xmlSchemaSimpleTypeRestriction.BaseType);
						simpleType.SetBaseSchemaType(xmlSchemaSimpleTypeRestriction.BaseType);
						xmlSchemaDatatype2 = xmlSchemaSimpleTypeRestriction.BaseType.Datatype;
					}
					else if (simpleType.Redefined != null && xmlSchemaSimpleTypeRestriction.BaseTypeName == simpleType.Redefined.QualifiedName)
					{
						this.CompileSimpleType((XmlSchemaSimpleType)simpleType.Redefined);
						simpleType.SetBaseSchemaType(simpleType.Redefined.BaseXmlSchemaType);
						xmlSchemaDatatype2 = simpleType.Redefined.Datatype;
					}
					else
					{
						if (xmlSchemaSimpleTypeRestriction.BaseTypeName.Equals(DatatypeImplementation.QnAnySimpleType))
						{
							throw new XmlSchemaException("Restriction of 'anySimpleType' is not allowed.", xmlSchemaSimpleTypeRestriction.BaseTypeName.ToString(), simpleType);
						}
						XmlSchemaSimpleType simpleType3 = this.GetSimpleType(xmlSchemaSimpleTypeRestriction.BaseTypeName);
						if (simpleType3 == null)
						{
							throw new XmlSchemaException("Type '{0}' is not declared, or is not a simple type.", xmlSchemaSimpleTypeRestriction.BaseTypeName.ToString(), simpleType);
						}
						if ((simpleType3.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
						{
							base.SendValidationEvent("The base type is final restriction.", simpleType);
						}
						simpleType.SetBaseSchemaType(simpleType3);
						xmlSchemaDatatype2 = simpleType3.Datatype;
					}
					simpleType.SetDatatype(xmlSchemaDatatype2.DeriveByRestriction(xmlSchemaSimpleTypeRestriction.Facets, base.NameTable, simpleType));
					simpleType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
				}
				else
				{
					XmlSchemaSimpleType[] array = this.CompileBaseMemberTypes(simpleType);
					simpleType.SetBaseSchemaType(DatatypeImplementation.AnySimpleType);
					simpleType.SetDatatype(XmlSchemaDatatype.DeriveByUnion(array, simpleType));
					simpleType.SetDerivedBy(XmlSchemaDerivationMethod.Union);
				}
			}
			catch (XmlSchemaException ex)
			{
				if (ex.SourceSchemaObject == null)
				{
					ex.SetSource(simpleType);
				}
				base.SendValidationEvent(ex);
				simpleType.SetDatatype(DatatypeImplementation.AnySimpleType.Datatype);
			}
			finally
			{
				simpleType.ElementDecl = new SchemaElementDecl
				{
					ContentValidator = ContentValidator.TextOnly,
					SchemaType = simpleType,
					Datatype = simpleType.Datatype
				};
				simpleType.IsProcessing = false;
			}
		}

		private XmlSchemaSimpleType[] CompileBaseMemberTypes(XmlSchemaSimpleType simpleType)
		{
			ArrayList arrayList = new ArrayList();
			XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = (XmlSchemaSimpleTypeUnion)simpleType.Content;
			XmlQualifiedName[] memberTypes = xmlSchemaSimpleTypeUnion.MemberTypes;
			if (memberTypes != null)
			{
				for (int i = 0; i < memberTypes.Length; i++)
				{
					XmlSchemaSimpleType simpleType2 = this.GetSimpleType(memberTypes[i]);
					if (simpleType2 == null)
					{
						throw new XmlSchemaException("Type '{0}' is not declared, or is not a simple type.", memberTypes[i].ToString(), simpleType);
					}
					if (simpleType2.Datatype.Variety == XmlSchemaDatatypeVariety.Union)
					{
						this.CheckUnionType(simpleType2, arrayList, simpleType);
					}
					else
					{
						arrayList.Add(simpleType2);
					}
					if ((simpleType2.FinalResolved & XmlSchemaDerivationMethod.Union) != XmlSchemaDerivationMethod.Empty)
					{
						base.SendValidationEvent("The base type is the final union.", simpleType);
					}
				}
			}
			XmlSchemaObjectCollection baseTypes = xmlSchemaSimpleTypeUnion.BaseTypes;
			if (baseTypes != null)
			{
				for (int j = 0; j < baseTypes.Count; j++)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)baseTypes[j];
					this.CompileSimpleType(xmlSchemaSimpleType);
					if (xmlSchemaSimpleType.Datatype.Variety == XmlSchemaDatatypeVariety.Union)
					{
						this.CheckUnionType(xmlSchemaSimpleType, arrayList, simpleType);
					}
					else
					{
						arrayList.Add(xmlSchemaSimpleType);
					}
				}
			}
			xmlSchemaSimpleTypeUnion.SetBaseMemberTypes(arrayList.ToArray(typeof(XmlSchemaSimpleType)) as XmlSchemaSimpleType[]);
			return xmlSchemaSimpleTypeUnion.BaseMemberTypes;
		}

		private void CheckUnionType(XmlSchemaSimpleType unionMember, ArrayList memberTypeDefinitions, XmlSchemaSimpleType parentType)
		{
			XmlSchemaDatatype datatype = unionMember.Datatype;
			if (unionMember.DerivedBy == XmlSchemaDerivationMethod.Restriction && (datatype.HasLexicalFacets || datatype.HasValueFacets))
			{
				base.SendValidationEvent("It is an error if a union type has a member with variety union and this member cannot be substituted with its own members. This may be due to the fact that the union member is a restriction of a union with facets.", parentType);
				return;
			}
			Datatype_union datatype_union = unionMember.Datatype as Datatype_union;
			memberTypeDefinitions.AddRange(datatype_union.BaseMemberTypes);
		}

		private void CompileComplexType(XmlSchemaComplexType complexType)
		{
			if (complexType.ElementDecl != null)
			{
				return;
			}
			if (complexType.IsProcessing)
			{
				base.SendValidationEvent("Circular type reference.", complexType);
				return;
			}
			complexType.IsProcessing = true;
			if (complexType.ContentModel != null)
			{
				if (complexType.ContentModel is XmlSchemaSimpleContent)
				{
					XmlSchemaSimpleContent xmlSchemaSimpleContent = (XmlSchemaSimpleContent)complexType.ContentModel;
					complexType.SetContentType(XmlSchemaContentType.TextOnly);
					if (xmlSchemaSimpleContent.Content is XmlSchemaSimpleContentExtension)
					{
						this.CompileSimpleContentExtension(complexType, (XmlSchemaSimpleContentExtension)xmlSchemaSimpleContent.Content);
					}
					else
					{
						this.CompileSimpleContentRestriction(complexType, (XmlSchemaSimpleContentRestriction)xmlSchemaSimpleContent.Content);
					}
				}
				else
				{
					XmlSchemaComplexContent xmlSchemaComplexContent = (XmlSchemaComplexContent)complexType.ContentModel;
					if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
					{
						this.CompileComplexContentExtension(complexType, xmlSchemaComplexContent, (XmlSchemaComplexContentExtension)xmlSchemaComplexContent.Content);
					}
					else
					{
						this.CompileComplexContentRestriction(complexType, xmlSchemaComplexContent, (XmlSchemaComplexContentRestriction)xmlSchemaComplexContent.Content);
					}
				}
			}
			else
			{
				complexType.SetBaseSchemaType(XmlSchemaComplexType.AnyType);
				this.CompileLocalAttributes(XmlSchemaComplexType.AnyType, complexType, complexType.Attributes, complexType.AnyAttribute, XmlSchemaDerivationMethod.Restriction);
				complexType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
				complexType.SetContentTypeParticle(this.CompileContentTypeParticle(complexType.Particle, true));
				complexType.SetContentType(this.GetSchemaContentType(complexType, null, complexType.ContentTypeParticle));
			}
			bool flag = false;
			foreach (object obj in complexType.AttributeUses.Values)
			{
				XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)obj;
				if (xmlSchemaAttribute.Use != XmlSchemaUse.Prohibited)
				{
					XmlSchemaDatatype datatype = xmlSchemaAttribute.Datatype;
					if (datatype != null && datatype.TokenizedType == XmlTokenizedType.ID)
					{
						if (flag)
						{
							base.SendValidationEvent("Two distinct members of the attribute uses must not have type definitions which are both xs:ID or are derived from xs:ID.", complexType);
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			SchemaElementDecl schemaElementDecl = new SchemaElementDecl();
			schemaElementDecl.ContentValidator = this.CompileComplexContent(complexType);
			schemaElementDecl.SchemaType = complexType;
			schemaElementDecl.IsAbstract = complexType.IsAbstract;
			schemaElementDecl.Datatype = complexType.Datatype;
			schemaElementDecl.Block = complexType.BlockResolved;
			schemaElementDecl.AnyAttribute = complexType.AttributeWildcard;
			foreach (object obj2 in complexType.AttributeUses.Values)
			{
				XmlSchemaAttribute xmlSchemaAttribute2 = (XmlSchemaAttribute)obj2;
				if (xmlSchemaAttribute2.Use == XmlSchemaUse.Prohibited)
				{
					if (!schemaElementDecl.ProhibitedAttributes.ContainsKey(xmlSchemaAttribute2.QualifiedName))
					{
						schemaElementDecl.ProhibitedAttributes.Add(xmlSchemaAttribute2.QualifiedName, xmlSchemaAttribute2.QualifiedName);
					}
				}
				else if (!schemaElementDecl.AttDefs.ContainsKey(xmlSchemaAttribute2.QualifiedName) && xmlSchemaAttribute2.AttDef != null && xmlSchemaAttribute2.AttDef.Name != XmlQualifiedName.Empty && xmlSchemaAttribute2.AttDef != SchemaAttDef.Empty)
				{
					schemaElementDecl.AddAttDef(xmlSchemaAttribute2.AttDef);
				}
			}
			complexType.ElementDecl = schemaElementDecl;
			complexType.IsProcessing = false;
		}

		private void CompileSimpleContentExtension(XmlSchemaComplexType complexType, XmlSchemaSimpleContentExtension simpleExtension)
		{
			XmlSchemaComplexType xmlSchemaComplexType;
			if (complexType.Redefined != null && simpleExtension.BaseTypeName == complexType.Redefined.QualifiedName)
			{
				xmlSchemaComplexType = (XmlSchemaComplexType)complexType.Redefined;
				this.CompileComplexType(xmlSchemaComplexType);
				complexType.SetBaseSchemaType(xmlSchemaComplexType);
				complexType.SetDatatype(xmlSchemaComplexType.Datatype);
			}
			else
			{
				XmlSchemaType anySchemaType = this.GetAnySchemaType(simpleExtension.BaseTypeName);
				if (anySchemaType == null)
				{
					base.SendValidationEvent("Type '{0}' is not declared.", simpleExtension.BaseTypeName.ToString(), complexType);
				}
				else
				{
					complexType.SetBaseSchemaType(anySchemaType);
					complexType.SetDatatype(anySchemaType.Datatype);
				}
				xmlSchemaComplexType = anySchemaType as XmlSchemaComplexType;
			}
			if (xmlSchemaComplexType != null)
			{
				if ((xmlSchemaComplexType.FinalResolved & XmlSchemaDerivationMethod.Extension) != XmlSchemaDerivationMethod.Empty)
				{
					base.SendValidationEvent("The base type is the final extension.", complexType);
				}
				if (xmlSchemaComplexType.ContentType != XmlSchemaContentType.TextOnly)
				{
					base.SendValidationEvent("The content type of the base type must be a simple type definition or it must be mixed, and simpleType child must be present.", complexType);
				}
			}
			complexType.SetDerivedBy(XmlSchemaDerivationMethod.Extension);
			this.CompileLocalAttributes(xmlSchemaComplexType, complexType, simpleExtension.Attributes, simpleExtension.AnyAttribute, XmlSchemaDerivationMethod.Extension);
		}

		private void CompileSimpleContentRestriction(XmlSchemaComplexType complexType, XmlSchemaSimpleContentRestriction simpleRestriction)
		{
			XmlSchemaComplexType xmlSchemaComplexType = null;
			XmlSchemaDatatype xmlSchemaDatatype = null;
			if (complexType.Redefined != null && simpleRestriction.BaseTypeName == complexType.Redefined.QualifiedName)
			{
				xmlSchemaComplexType = (XmlSchemaComplexType)complexType.Redefined;
				this.CompileComplexType(xmlSchemaComplexType);
				xmlSchemaDatatype = xmlSchemaComplexType.Datatype;
			}
			else
			{
				xmlSchemaComplexType = this.GetComplexType(simpleRestriction.BaseTypeName);
				if (xmlSchemaComplexType == null)
				{
					base.SendValidationEvent("Undefined complexType '{0}' is used as a base for complex type restriction.", simpleRestriction.BaseTypeName.ToString(), simpleRestriction);
					return;
				}
				if (xmlSchemaComplexType.ContentType == XmlSchemaContentType.TextOnly)
				{
					if (simpleRestriction.BaseType == null)
					{
						xmlSchemaDatatype = xmlSchemaComplexType.Datatype;
					}
					else
					{
						this.CompileSimpleType(simpleRestriction.BaseType);
						if (!XmlSchemaType.IsDerivedFromDatatype(simpleRestriction.BaseType.Datatype, xmlSchemaComplexType.Datatype, XmlSchemaDerivationMethod.None))
						{
							base.SendValidationEvent("The data type of the simple content is not a valid restriction of the base complex type.", simpleRestriction);
						}
						xmlSchemaDatatype = simpleRestriction.BaseType.Datatype;
					}
				}
				else if (xmlSchemaComplexType.ContentType == XmlSchemaContentType.Mixed && xmlSchemaComplexType.ElementDecl.ContentValidator.IsEmptiable)
				{
					if (simpleRestriction.BaseType != null)
					{
						this.CompileSimpleType(simpleRestriction.BaseType);
						complexType.SetBaseSchemaType(simpleRestriction.BaseType);
						xmlSchemaDatatype = simpleRestriction.BaseType.Datatype;
					}
					else
					{
						base.SendValidationEvent("Simple content restriction must have a simple type child if the content type of the base type is not a simple type definition.", simpleRestriction);
					}
				}
				else
				{
					base.SendValidationEvent("The content type of the base type must be a simple type definition or it must be mixed, and simpleType child must be present.", complexType);
				}
			}
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.ElementDecl != null && (xmlSchemaComplexType.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
			{
				base.SendValidationEvent("The base type is final restriction.", complexType);
			}
			if (xmlSchemaComplexType != null)
			{
				complexType.SetBaseSchemaType(xmlSchemaComplexType);
			}
			if (xmlSchemaDatatype != null)
			{
				try
				{
					complexType.SetDatatype(xmlSchemaDatatype.DeriveByRestriction(simpleRestriction.Facets, base.NameTable, complexType));
				}
				catch (XmlSchemaException ex)
				{
					if (ex.SourceSchemaObject == null)
					{
						ex.SetSource(complexType);
					}
					base.SendValidationEvent(ex);
					complexType.SetDatatype(DatatypeImplementation.AnySimpleType.Datatype);
				}
			}
			complexType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
			this.CompileLocalAttributes(xmlSchemaComplexType, complexType, simpleRestriction.Attributes, simpleRestriction.AnyAttribute, XmlSchemaDerivationMethod.Restriction);
		}

		private void CompileComplexContentExtension(XmlSchemaComplexType complexType, XmlSchemaComplexContent complexContent, XmlSchemaComplexContentExtension complexExtension)
		{
			XmlSchemaComplexType xmlSchemaComplexType;
			if (complexType.Redefined != null && complexExtension.BaseTypeName == complexType.Redefined.QualifiedName)
			{
				xmlSchemaComplexType = (XmlSchemaComplexType)complexType.Redefined;
				this.CompileComplexType(xmlSchemaComplexType);
			}
			else
			{
				xmlSchemaComplexType = this.GetComplexType(complexExtension.BaseTypeName);
				if (xmlSchemaComplexType == null)
				{
					base.SendValidationEvent("Undefined complexType '{0}' is used as a base for complex type extension.", complexExtension.BaseTypeName.ToString(), complexExtension);
					return;
				}
			}
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.ElementDecl != null && xmlSchemaComplexType.ContentType == XmlSchemaContentType.TextOnly)
			{
				base.SendValidationEvent("The content type of the base type must not be a simple type definition.", complexType);
				return;
			}
			complexType.SetBaseSchemaType(xmlSchemaComplexType);
			if ((xmlSchemaComplexType.FinalResolved & XmlSchemaDerivationMethod.Extension) != XmlSchemaDerivationMethod.Empty)
			{
				base.SendValidationEvent("The base type is the final extension.", complexType);
			}
			this.CompileLocalAttributes(xmlSchemaComplexType, complexType, complexExtension.Attributes, complexExtension.AnyAttribute, XmlSchemaDerivationMethod.Extension);
			XmlSchemaParticle contentTypeParticle = xmlSchemaComplexType.ContentTypeParticle;
			XmlSchemaParticle xmlSchemaParticle = this.CannonicalizeParticle(complexExtension.Particle, true, true);
			if (contentTypeParticle != XmlSchemaParticle.Empty)
			{
				if (xmlSchemaParticle != XmlSchemaParticle.Empty)
				{
					complexType.SetContentTypeParticle(this.CompileContentTypeParticle(new XmlSchemaSequence
					{
						Items = { contentTypeParticle, xmlSchemaParticle }
					}, false));
				}
				else
				{
					complexType.SetContentTypeParticle(contentTypeParticle);
				}
				XmlSchemaContentType xmlSchemaContentType = this.GetSchemaContentType(complexType, complexContent, xmlSchemaParticle);
				if (xmlSchemaContentType == XmlSchemaContentType.Empty)
				{
					xmlSchemaContentType = xmlSchemaComplexType.ContentType;
				}
				complexType.SetContentType(xmlSchemaContentType);
				if (complexType.ContentType != xmlSchemaComplexType.ContentType)
				{
					base.SendValidationEvent("The derived type and the base type must have the same content type.", complexType);
				}
			}
			else
			{
				complexType.SetContentTypeParticle(xmlSchemaParticle);
				complexType.SetContentType(this.GetSchemaContentType(complexType, complexContent, complexType.ContentTypeParticle));
			}
			complexType.SetDerivedBy(XmlSchemaDerivationMethod.Extension);
		}

		private void CompileComplexContentRestriction(XmlSchemaComplexType complexType, XmlSchemaComplexContent complexContent, XmlSchemaComplexContentRestriction complexRestriction)
		{
			XmlSchemaComplexType xmlSchemaComplexType;
			if (complexType.Redefined != null && complexRestriction.BaseTypeName == complexType.Redefined.QualifiedName)
			{
				xmlSchemaComplexType = (XmlSchemaComplexType)complexType.Redefined;
				this.CompileComplexType(xmlSchemaComplexType);
			}
			else
			{
				xmlSchemaComplexType = this.GetComplexType(complexRestriction.BaseTypeName);
				if (xmlSchemaComplexType == null)
				{
					base.SendValidationEvent("Undefined complexType '{0}' is used as a base for complex type restriction.", complexRestriction.BaseTypeName.ToString(), complexRestriction);
					return;
				}
			}
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.ElementDecl != null && xmlSchemaComplexType.ContentType == XmlSchemaContentType.TextOnly)
			{
				base.SendValidationEvent("The content type of the base type must not be a simple type definition.", complexType);
				return;
			}
			complexType.SetBaseSchemaType(xmlSchemaComplexType);
			if ((xmlSchemaComplexType.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
			{
				base.SendValidationEvent("The base type is final restriction.", complexType);
			}
			this.CompileLocalAttributes(xmlSchemaComplexType, complexType, complexRestriction.Attributes, complexRestriction.AnyAttribute, XmlSchemaDerivationMethod.Restriction);
			complexType.SetContentTypeParticle(this.CompileContentTypeParticle(complexRestriction.Particle, true));
			complexType.SetContentType(this.GetSchemaContentType(complexType, complexContent, complexType.ContentTypeParticle));
			if (complexType.ContentType == XmlSchemaContentType.Empty)
			{
				SchemaElementDecl elementDecl = xmlSchemaComplexType.ElementDecl;
				if (xmlSchemaComplexType.ElementDecl != null && !xmlSchemaComplexType.ElementDecl.ContentValidator.IsEmptiable)
				{
					base.SendValidationEvent("Invalid content type derivation by restriction.", complexType);
				}
			}
			complexType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
		}

		private void CheckParticleDerivation(XmlSchemaComplexType complexType)
		{
			XmlSchemaComplexType xmlSchemaComplexType = complexType.BaseXmlSchemaType as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null && xmlSchemaComplexType != XmlSchemaComplexType.AnyType && complexType.DerivedBy == XmlSchemaDerivationMethod.Restriction && !this.IsValidRestriction(complexType.ContentTypeParticle, xmlSchemaComplexType.ContentTypeParticle))
			{
				base.SendValidationEvent("Invalid particle derivation by restriction.", complexType);
			}
		}

		private XmlSchemaParticle CompileContentTypeParticle(XmlSchemaParticle particle, bool substitution)
		{
			XmlSchemaParticle xmlSchemaParticle = this.CannonicalizeParticle(particle, true, substitution);
			XmlSchemaChoice xmlSchemaChoice = xmlSchemaParticle as XmlSchemaChoice;
			if (xmlSchemaChoice != null && xmlSchemaChoice.Items.Count == 0)
			{
				if (xmlSchemaChoice.MinOccurs != 0m)
				{
					base.SendValidationEvent("Empty choice cannot be satisfied if 'minOccurs' is not equal to 0.", xmlSchemaChoice, XmlSeverityType.Warning);
				}
				return XmlSchemaParticle.Empty;
			}
			return xmlSchemaParticle;
		}

		private XmlSchemaParticle CannonicalizeParticle(XmlSchemaParticle particle, bool root, bool substitution)
		{
			if (particle == null || particle.IsEmpty)
			{
				return XmlSchemaParticle.Empty;
			}
			if (particle is XmlSchemaElement)
			{
				return this.CannonicalizeElement((XmlSchemaElement)particle, substitution);
			}
			if (particle is XmlSchemaGroupRef)
			{
				return this.CannonicalizeGroupRef((XmlSchemaGroupRef)particle, root, substitution);
			}
			if (particle is XmlSchemaAll)
			{
				return this.CannonicalizeAll((XmlSchemaAll)particle, root, substitution);
			}
			if (particle is XmlSchemaChoice)
			{
				return this.CannonicalizeChoice((XmlSchemaChoice)particle, root, substitution);
			}
			if (particle is XmlSchemaSequence)
			{
				return this.CannonicalizeSequence((XmlSchemaSequence)particle, root, substitution);
			}
			return particle;
		}

		private XmlSchemaParticle CannonicalizeElement(XmlSchemaElement element, bool substitution)
		{
			if (element.RefName.IsEmpty || !substitution || (element.BlockResolved & XmlSchemaDerivationMethod.Substitution) != XmlSchemaDerivationMethod.Empty)
			{
				return element;
			}
			XmlSchemaSubstitutionGroupV1Compat xmlSchemaSubstitutionGroupV1Compat = (XmlSchemaSubstitutionGroupV1Compat)this.examplars[element.QualifiedName];
			if (xmlSchemaSubstitutionGroupV1Compat == null)
			{
				return element;
			}
			XmlSchemaChoice xmlSchemaChoice = (XmlSchemaChoice)xmlSchemaSubstitutionGroupV1Compat.Choice.Clone();
			xmlSchemaChoice.MinOccurs = element.MinOccurs;
			xmlSchemaChoice.MaxOccurs = element.MaxOccurs;
			return xmlSchemaChoice;
		}

		private XmlSchemaParticle CannonicalizeGroupRef(XmlSchemaGroupRef groupRef, bool root, bool substitution)
		{
			XmlSchemaGroup xmlSchemaGroup;
			if (groupRef.Redefined != null)
			{
				xmlSchemaGroup = groupRef.Redefined;
			}
			else
			{
				xmlSchemaGroup = (XmlSchemaGroup)this.schema.Groups[groupRef.RefName];
			}
			if (xmlSchemaGroup == null)
			{
				base.SendValidationEvent("Reference to undeclared model group '{0}'.", groupRef.RefName.ToString(), groupRef);
				return XmlSchemaParticle.Empty;
			}
			if (xmlSchemaGroup.CanonicalParticle == null)
			{
				this.CompileGroup(xmlSchemaGroup);
			}
			if (xmlSchemaGroup.CanonicalParticle == XmlSchemaParticle.Empty)
			{
				return XmlSchemaParticle.Empty;
			}
			XmlSchemaGroupBase xmlSchemaGroupBase = (XmlSchemaGroupBase)xmlSchemaGroup.CanonicalParticle;
			if (xmlSchemaGroupBase is XmlSchemaAll)
			{
				if (!root)
				{
					base.SendValidationEvent("The group ref to 'all' is not the root particle, or it is being used as an extension.", "", groupRef);
					return XmlSchemaParticle.Empty;
				}
				if (groupRef.MinOccurs != 1m || groupRef.MaxOccurs != 1m)
				{
					base.SendValidationEvent("The group ref to 'all' must have {min occurs}= 0 or 1 and {max occurs}=1.", groupRef);
					return XmlSchemaParticle.Empty;
				}
			}
			else if (xmlSchemaGroupBase is XmlSchemaChoice && xmlSchemaGroupBase.Items.Count == 0)
			{
				if (groupRef.MinOccurs != 0m)
				{
					base.SendValidationEvent("Empty choice cannot be satisfied if 'minOccurs' is not equal to 0.", groupRef, XmlSeverityType.Warning);
				}
				return XmlSchemaParticle.Empty;
			}
			XmlSchemaGroupBase xmlSchemaGroupBase2 = ((xmlSchemaGroupBase is XmlSchemaSequence) ? new XmlSchemaSequence() : ((xmlSchemaGroupBase is XmlSchemaChoice) ? new XmlSchemaChoice() : new XmlSchemaAll()));
			xmlSchemaGroupBase2.MinOccurs = groupRef.MinOccurs;
			xmlSchemaGroupBase2.MaxOccurs = groupRef.MaxOccurs;
			for (int i = 0; i < xmlSchemaGroupBase.Items.Count; i++)
			{
				xmlSchemaGroupBase2.Items.Add((XmlSchemaParticle)xmlSchemaGroupBase.Items[i]);
			}
			groupRef.SetParticle(xmlSchemaGroupBase2);
			return xmlSchemaGroupBase2;
		}

		private XmlSchemaParticle CannonicalizeAll(XmlSchemaAll all, bool root, bool substitution)
		{
			if (all.Items.Count > 0)
			{
				XmlSchemaAll xmlSchemaAll = new XmlSchemaAll();
				xmlSchemaAll.MinOccurs = all.MinOccurs;
				xmlSchemaAll.MaxOccurs = all.MaxOccurs;
				xmlSchemaAll.SourceUri = all.SourceUri;
				xmlSchemaAll.LineNumber = all.LineNumber;
				xmlSchemaAll.LinePosition = all.LinePosition;
				for (int i = 0; i < all.Items.Count; i++)
				{
					XmlSchemaParticle xmlSchemaParticle = this.CannonicalizeParticle((XmlSchemaElement)all.Items[i], false, substitution);
					if (xmlSchemaParticle != XmlSchemaParticle.Empty)
					{
						xmlSchemaAll.Items.Add(xmlSchemaParticle);
					}
				}
				all = xmlSchemaAll;
			}
			if (all.Items.Count == 0)
			{
				return XmlSchemaParticle.Empty;
			}
			if (root && all.Items.Count == 1)
			{
				return new XmlSchemaSequence
				{
					MinOccurs = all.MinOccurs,
					MaxOccurs = all.MaxOccurs,
					Items = { (XmlSchemaParticle)all.Items[0] }
				};
			}
			if (!root && all.Items.Count == 1 && all.MinOccurs == 1m && all.MaxOccurs == 1m)
			{
				return (XmlSchemaParticle)all.Items[0];
			}
			if (!root)
			{
				base.SendValidationEvent("'all' is not the only particle in a group, or is being used as an extension.", all);
				return XmlSchemaParticle.Empty;
			}
			return all;
		}

		private XmlSchemaParticle CannonicalizeChoice(XmlSchemaChoice choice, bool root, bool substitution)
		{
			XmlSchemaChoice xmlSchemaChoice = choice;
			if (choice.Items.Count > 0)
			{
				XmlSchemaChoice xmlSchemaChoice2 = new XmlSchemaChoice();
				xmlSchemaChoice2.MinOccurs = choice.MinOccurs;
				xmlSchemaChoice2.MaxOccurs = choice.MaxOccurs;
				for (int i = 0; i < choice.Items.Count; i++)
				{
					XmlSchemaParticle xmlSchemaParticle = this.CannonicalizeParticle((XmlSchemaParticle)choice.Items[i], false, substitution);
					if (xmlSchemaParticle != XmlSchemaParticle.Empty)
					{
						if (xmlSchemaParticle.MinOccurs == 1m && xmlSchemaParticle.MaxOccurs == 1m && xmlSchemaParticle is XmlSchemaChoice)
						{
							XmlSchemaChoice xmlSchemaChoice3 = (XmlSchemaChoice)xmlSchemaParticle;
							for (int j = 0; j < xmlSchemaChoice3.Items.Count; j++)
							{
								xmlSchemaChoice2.Items.Add(xmlSchemaChoice3.Items[j]);
							}
						}
						else
						{
							xmlSchemaChoice2.Items.Add(xmlSchemaParticle);
						}
					}
				}
				choice = xmlSchemaChoice2;
			}
			if (!root && choice.Items.Count == 0)
			{
				if (choice.MinOccurs != 0m)
				{
					base.SendValidationEvent("Empty choice cannot be satisfied if 'minOccurs' is not equal to 0.", xmlSchemaChoice, XmlSeverityType.Warning);
				}
				return XmlSchemaParticle.Empty;
			}
			if (!root && choice.Items.Count == 1 && choice.MinOccurs == 1m && choice.MaxOccurs == 1m)
			{
				return (XmlSchemaParticle)choice.Items[0];
			}
			return choice;
		}

		private XmlSchemaParticle CannonicalizeSequence(XmlSchemaSequence sequence, bool root, bool substitution)
		{
			if (sequence.Items.Count > 0)
			{
				XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
				xmlSchemaSequence.MinOccurs = sequence.MinOccurs;
				xmlSchemaSequence.MaxOccurs = sequence.MaxOccurs;
				for (int i = 0; i < sequence.Items.Count; i++)
				{
					XmlSchemaParticle xmlSchemaParticle = this.CannonicalizeParticle((XmlSchemaParticle)sequence.Items[i], false, substitution);
					if (xmlSchemaParticle != XmlSchemaParticle.Empty)
					{
						if (xmlSchemaParticle.MinOccurs == 1m && xmlSchemaParticle.MaxOccurs == 1m && xmlSchemaParticle is XmlSchemaSequence)
						{
							XmlSchemaSequence xmlSchemaSequence2 = (XmlSchemaSequence)xmlSchemaParticle;
							for (int j = 0; j < xmlSchemaSequence2.Items.Count; j++)
							{
								xmlSchemaSequence.Items.Add(xmlSchemaSequence2.Items[j]);
							}
						}
						else
						{
							xmlSchemaSequence.Items.Add(xmlSchemaParticle);
						}
					}
				}
				sequence = xmlSchemaSequence;
			}
			if (sequence.Items.Count == 0)
			{
				return XmlSchemaParticle.Empty;
			}
			if (!root && sequence.Items.Count == 1 && sequence.MinOccurs == 1m && sequence.MaxOccurs == 1m)
			{
				return (XmlSchemaParticle)sequence.Items[0];
			}
			return sequence;
		}

		private bool IsValidRestriction(XmlSchemaParticle derivedParticle, XmlSchemaParticle baseParticle)
		{
			if (derivedParticle == baseParticle)
			{
				return true;
			}
			if (derivedParticle == null || derivedParticle == XmlSchemaParticle.Empty)
			{
				return this.IsParticleEmptiable(baseParticle);
			}
			if (baseParticle == null || baseParticle == XmlSchemaParticle.Empty)
			{
				return false;
			}
			if (baseParticle is XmlSchemaElement)
			{
				return derivedParticle is XmlSchemaElement && this.IsElementFromElement((XmlSchemaElement)derivedParticle, (XmlSchemaElement)baseParticle);
			}
			if (!(baseParticle is XmlSchemaAny))
			{
				if (baseParticle is XmlSchemaAll)
				{
					if (derivedParticle is XmlSchemaElement)
					{
						return this.IsElementFromGroupBase((XmlSchemaElement)derivedParticle, (XmlSchemaGroupBase)baseParticle, true);
					}
					if (derivedParticle is XmlSchemaAll)
					{
						return this.IsGroupBaseFromGroupBase((XmlSchemaGroupBase)derivedParticle, (XmlSchemaGroupBase)baseParticle, true);
					}
					if (derivedParticle is XmlSchemaSequence)
					{
						return this.IsSequenceFromAll((XmlSchemaSequence)derivedParticle, (XmlSchemaAll)baseParticle);
					}
				}
				else if (baseParticle is XmlSchemaChoice)
				{
					if (derivedParticle is XmlSchemaElement)
					{
						return this.IsElementFromGroupBase((XmlSchemaElement)derivedParticle, (XmlSchemaGroupBase)baseParticle, false);
					}
					if (derivedParticle is XmlSchemaChoice)
					{
						return this.IsGroupBaseFromGroupBase((XmlSchemaGroupBase)derivedParticle, (XmlSchemaGroupBase)baseParticle, false);
					}
					if (derivedParticle is XmlSchemaSequence)
					{
						return this.IsSequenceFromChoice((XmlSchemaSequence)derivedParticle, (XmlSchemaChoice)baseParticle);
					}
				}
				else if (baseParticle is XmlSchemaSequence)
				{
					if (derivedParticle is XmlSchemaElement)
					{
						return this.IsElementFromGroupBase((XmlSchemaElement)derivedParticle, (XmlSchemaGroupBase)baseParticle, true);
					}
					if (derivedParticle is XmlSchemaSequence)
					{
						return this.IsGroupBaseFromGroupBase((XmlSchemaGroupBase)derivedParticle, (XmlSchemaGroupBase)baseParticle, true);
					}
				}
				return false;
			}
			if (derivedParticle is XmlSchemaElement)
			{
				return this.IsElementFromAny((XmlSchemaElement)derivedParticle, (XmlSchemaAny)baseParticle);
			}
			if (derivedParticle is XmlSchemaAny)
			{
				return this.IsAnyFromAny((XmlSchemaAny)derivedParticle, (XmlSchemaAny)baseParticle);
			}
			return this.IsGroupBaseFromAny((XmlSchemaGroupBase)derivedParticle, (XmlSchemaAny)baseParticle);
		}

		private bool IsElementFromElement(XmlSchemaElement derivedElement, XmlSchemaElement baseElement)
		{
			return derivedElement.QualifiedName == baseElement.QualifiedName && derivedElement.IsNillable == baseElement.IsNillable && this.IsValidOccurrenceRangeRestriction(derivedElement, baseElement) && (baseElement.FixedValue == null || baseElement.FixedValue == derivedElement.FixedValue) && (derivedElement.BlockResolved | baseElement.BlockResolved) == derivedElement.BlockResolved && derivedElement.ElementSchemaType != null && baseElement.ElementSchemaType != null && XmlSchemaType.IsDerivedFrom(derivedElement.ElementSchemaType, baseElement.ElementSchemaType, ~XmlSchemaDerivationMethod.Restriction);
		}

		private bool IsElementFromAny(XmlSchemaElement derivedElement, XmlSchemaAny baseAny)
		{
			return baseAny.Allows(derivedElement.QualifiedName) && this.IsValidOccurrenceRangeRestriction(derivedElement, baseAny);
		}

		private bool IsAnyFromAny(XmlSchemaAny derivedAny, XmlSchemaAny baseAny)
		{
			return this.IsValidOccurrenceRangeRestriction(derivedAny, baseAny) && NamespaceList.IsSubset(derivedAny.NamespaceList, baseAny.NamespaceList);
		}

		private bool IsGroupBaseFromAny(XmlSchemaGroupBase derivedGroupBase, XmlSchemaAny baseAny)
		{
			decimal num;
			decimal num2;
			this.CalculateEffectiveTotalRange(derivedGroupBase, out num, out num2);
			if (!this.IsValidOccurrenceRangeRestriction(num, num2, baseAny.MinOccurs, baseAny.MaxOccurs))
			{
				return false;
			}
			string minOccursString = baseAny.MinOccursString;
			baseAny.MinOccurs = 0m;
			for (int i = 0; i < derivedGroupBase.Items.Count; i++)
			{
				if (!this.IsValidRestriction((XmlSchemaParticle)derivedGroupBase.Items[i], baseAny))
				{
					baseAny.MinOccursString = minOccursString;
					return false;
				}
			}
			baseAny.MinOccursString = minOccursString;
			return true;
		}

		private bool IsElementFromGroupBase(XmlSchemaElement derivedElement, XmlSchemaGroupBase baseGroupBase, bool skipEmptableOnly)
		{
			bool flag = false;
			for (int i = 0; i < baseGroupBase.Items.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)baseGroupBase.Items[i];
				if (!flag)
				{
					string minOccursString = xmlSchemaParticle.MinOccursString;
					string maxOccursString = xmlSchemaParticle.MaxOccursString;
					xmlSchemaParticle.MinOccurs *= baseGroupBase.MinOccurs;
					if (xmlSchemaParticle.MaxOccurs != 79228162514264337593543950335m)
					{
						if (baseGroupBase.MaxOccurs == 79228162514264337593543950335m)
						{
							xmlSchemaParticle.MaxOccurs = decimal.MaxValue;
						}
						else
						{
							xmlSchemaParticle.MaxOccurs *= baseGroupBase.MaxOccurs;
						}
					}
					flag = this.IsValidRestriction(derivedElement, xmlSchemaParticle);
					xmlSchemaParticle.MinOccursString = minOccursString;
					xmlSchemaParticle.MaxOccursString = maxOccursString;
				}
				else if (skipEmptableOnly && !this.IsParticleEmptiable(xmlSchemaParticle))
				{
					return false;
				}
			}
			return flag;
		}

		private bool IsGroupBaseFromGroupBase(XmlSchemaGroupBase derivedGroupBase, XmlSchemaGroupBase baseGroupBase, bool skipEmptableOnly)
		{
			if (!this.IsValidOccurrenceRangeRestriction(derivedGroupBase, baseGroupBase) || derivedGroupBase.Items.Count > baseGroupBase.Items.Count)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < baseGroupBase.Items.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)baseGroupBase.Items[i];
				if (num < derivedGroupBase.Items.Count && this.IsValidRestriction((XmlSchemaParticle)derivedGroupBase.Items[num], xmlSchemaParticle))
				{
					num++;
				}
				else if (skipEmptableOnly && !this.IsParticleEmptiable(xmlSchemaParticle))
				{
					return false;
				}
			}
			return num >= derivedGroupBase.Items.Count;
		}

		private bool IsSequenceFromAll(XmlSchemaSequence derivedSequence, XmlSchemaAll baseAll)
		{
			if (!this.IsValidOccurrenceRangeRestriction(derivedSequence, baseAll) || derivedSequence.Items.Count > baseAll.Items.Count)
			{
				return false;
			}
			BitSet bitSet = new BitSet(baseAll.Items.Count);
			for (int i = 0; i < derivedSequence.Items.Count; i++)
			{
				int mappingParticle = this.GetMappingParticle((XmlSchemaParticle)derivedSequence.Items[i], baseAll.Items);
				if (mappingParticle < 0)
				{
					return false;
				}
				if (bitSet[mappingParticle])
				{
					return false;
				}
				bitSet.Set(mappingParticle);
			}
			for (int j = 0; j < baseAll.Items.Count; j++)
			{
				if (!bitSet[j] && !this.IsParticleEmptiable((XmlSchemaParticle)baseAll.Items[j]))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsSequenceFromChoice(XmlSchemaSequence derivedSequence, XmlSchemaChoice baseChoice)
		{
			decimal num;
			decimal num2;
			this.CalculateSequenceRange(derivedSequence, out num, out num2);
			if (!this.IsValidOccurrenceRangeRestriction(num, num2, baseChoice.MinOccurs, baseChoice.MaxOccurs) || derivedSequence.Items.Count > baseChoice.Items.Count)
			{
				return false;
			}
			for (int i = 0; i < derivedSequence.Items.Count; i++)
			{
				if (this.GetMappingParticle((XmlSchemaParticle)derivedSequence.Items[i], baseChoice.Items) < 0)
				{
					return false;
				}
			}
			return true;
		}

		private void CalculateSequenceRange(XmlSchemaSequence sequence, out decimal minOccurs, out decimal maxOccurs)
		{
			minOccurs = 0m;
			maxOccurs = 0m;
			for (int i = 0; i < sequence.Items.Count; i++)
			{
				XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)sequence.Items[i];
				minOccurs += xmlSchemaParticle.MinOccurs;
				if (xmlSchemaParticle.MaxOccurs == 79228162514264337593543950335m)
				{
					maxOccurs = decimal.MaxValue;
				}
				else if (maxOccurs != 79228162514264337593543950335m)
				{
					maxOccurs += xmlSchemaParticle.MaxOccurs;
				}
			}
			minOccurs *= sequence.MinOccurs;
			if (sequence.MaxOccurs == 79228162514264337593543950335m)
			{
				maxOccurs = decimal.MaxValue;
				return;
			}
			if (maxOccurs != 79228162514264337593543950335m)
			{
				maxOccurs *= sequence.MaxOccurs;
			}
		}

		private bool IsValidOccurrenceRangeRestriction(XmlSchemaParticle derivedParticle, XmlSchemaParticle baseParticle)
		{
			return this.IsValidOccurrenceRangeRestriction(derivedParticle.MinOccurs, derivedParticle.MaxOccurs, baseParticle.MinOccurs, baseParticle.MaxOccurs);
		}

		private bool IsValidOccurrenceRangeRestriction(decimal minOccurs, decimal maxOccurs, decimal baseMinOccurs, decimal baseMaxOccurs)
		{
			return baseMinOccurs <= minOccurs && maxOccurs <= baseMaxOccurs;
		}

		private int GetMappingParticle(XmlSchemaParticle particle, XmlSchemaObjectCollection collection)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (this.IsValidRestriction(particle, (XmlSchemaParticle)collection[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private bool IsParticleEmptiable(XmlSchemaParticle particle)
		{
			decimal num;
			decimal num2;
			this.CalculateEffectiveTotalRange(particle, out num, out num2);
			return num == 0m;
		}

		private void CalculateEffectiveTotalRange(XmlSchemaParticle particle, out decimal minOccurs, out decimal maxOccurs)
		{
			if (particle is XmlSchemaElement || particle is XmlSchemaAny)
			{
				minOccurs = particle.MinOccurs;
				maxOccurs = particle.MaxOccurs;
				return;
			}
			if (particle is XmlSchemaChoice)
			{
				if (((XmlSchemaChoice)particle).Items.Count == 0)
				{
					minOccurs = (maxOccurs = 0m);
					return;
				}
				minOccurs = decimal.MaxValue;
				maxOccurs = 0m;
				XmlSchemaChoice xmlSchemaChoice = (XmlSchemaChoice)particle;
				for (int i = 0; i < xmlSchemaChoice.Items.Count; i++)
				{
					decimal num;
					decimal num2;
					this.CalculateEffectiveTotalRange((XmlSchemaParticle)xmlSchemaChoice.Items[i], out num, out num2);
					if (num < minOccurs)
					{
						minOccurs = num;
					}
					if (num2 > maxOccurs)
					{
						maxOccurs = num2;
					}
				}
				minOccurs *= particle.MinOccurs;
				if (maxOccurs != 79228162514264337593543950335m)
				{
					if (particle.MaxOccurs == 79228162514264337593543950335m)
					{
						maxOccurs = decimal.MaxValue;
						return;
					}
					maxOccurs *= particle.MaxOccurs;
					return;
				}
			}
			else
			{
				XmlSchemaObjectCollection items = ((XmlSchemaGroupBase)particle).Items;
				if (items.Count == 0)
				{
					minOccurs = (maxOccurs = 0m);
					return;
				}
				minOccurs = 0m;
				maxOccurs = 0m;
				for (int j = 0; j < items.Count; j++)
				{
					decimal num3;
					decimal num4;
					this.CalculateEffectiveTotalRange((XmlSchemaParticle)items[j], out num3, out num4);
					minOccurs += num3;
					if (maxOccurs != 79228162514264337593543950335m)
					{
						if (num4 == 79228162514264337593543950335m)
						{
							maxOccurs = decimal.MaxValue;
						}
						else
						{
							maxOccurs += num4;
						}
					}
				}
				minOccurs *= particle.MinOccurs;
				if (maxOccurs != 79228162514264337593543950335m)
				{
					if (particle.MaxOccurs == 79228162514264337593543950335m)
					{
						maxOccurs = decimal.MaxValue;
						return;
					}
					maxOccurs *= particle.MaxOccurs;
				}
			}
		}

		private void PushComplexType(XmlSchemaComplexType complexType)
		{
			this.complexTypeStack.Push(complexType);
		}

		private XmlSchemaContentType GetSchemaContentType(XmlSchemaComplexType complexType, XmlSchemaComplexContent complexContent, XmlSchemaParticle particle)
		{
			if ((complexContent != null && complexContent.IsMixed) || (complexContent == null && complexType.IsMixed))
			{
				return XmlSchemaContentType.Mixed;
			}
			if (particle != null && !particle.IsEmpty)
			{
				return XmlSchemaContentType.ElementOnly;
			}
			return XmlSchemaContentType.Empty;
		}

		private void CompileAttributeGroup(XmlSchemaAttributeGroup attributeGroup)
		{
			if (attributeGroup.IsProcessing)
			{
				base.SendValidationEvent("Circular attribute group reference.", attributeGroup);
				return;
			}
			if (attributeGroup.AttributeUses.Count > 0)
			{
				return;
			}
			attributeGroup.IsProcessing = true;
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute = attributeGroup.AnyAttribute;
			for (int i = 0; i < attributeGroup.Attributes.Count; i++)
			{
				XmlSchemaAttribute xmlSchemaAttribute = attributeGroup.Attributes[i] as XmlSchemaAttribute;
				if (xmlSchemaAttribute != null)
				{
					if (xmlSchemaAttribute.Use != XmlSchemaUse.Prohibited)
					{
						this.CompileAttribute(xmlSchemaAttribute);
					}
					if (attributeGroup.AttributeUses[xmlSchemaAttribute.QualifiedName] == null)
					{
						attributeGroup.AttributeUses.Add(xmlSchemaAttribute.QualifiedName, xmlSchemaAttribute);
					}
					else
					{
						base.SendValidationEvent("The attribute '{0}' already exists.", xmlSchemaAttribute.QualifiedName.ToString(), xmlSchemaAttribute);
					}
				}
				else
				{
					XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = (XmlSchemaAttributeGroupRef)attributeGroup.Attributes[i];
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup;
					if (attributeGroup.Redefined != null && xmlSchemaAttributeGroupRef.RefName == attributeGroup.Redefined.QualifiedName)
					{
						xmlSchemaAttributeGroup = attributeGroup.Redefined;
					}
					else
					{
						xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)this.schema.AttributeGroups[xmlSchemaAttributeGroupRef.RefName];
					}
					if (xmlSchemaAttributeGroup != null)
					{
						this.CompileAttributeGroup(xmlSchemaAttributeGroup);
						foreach (object obj in xmlSchemaAttributeGroup.AttributeUses.Values)
						{
							XmlSchemaAttribute xmlSchemaAttribute2 = (XmlSchemaAttribute)obj;
							if (attributeGroup.AttributeUses[xmlSchemaAttribute2.QualifiedName] == null)
							{
								attributeGroup.AttributeUses.Add(xmlSchemaAttribute2.QualifiedName, xmlSchemaAttribute2);
							}
							else
							{
								base.SendValidationEvent("The attribute '{0}' already exists.", xmlSchemaAttribute2.QualifiedName.ToString(), xmlSchemaAttribute2);
							}
						}
						xmlSchemaAnyAttribute = this.CompileAnyAttributeIntersection(xmlSchemaAnyAttribute, xmlSchemaAttributeGroup.AttributeWildcard);
					}
					else
					{
						base.SendValidationEvent("Reference to undeclared attribute group '{0}'.", xmlSchemaAttributeGroupRef.RefName.ToString(), xmlSchemaAttributeGroupRef);
					}
				}
			}
			attributeGroup.AttributeWildcard = xmlSchemaAnyAttribute;
			attributeGroup.IsProcessing = false;
		}

		private void CompileLocalAttributes(XmlSchemaComplexType baseType, XmlSchemaComplexType derivedType, XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, XmlSchemaDerivationMethod derivedBy)
		{
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute = ((baseType != null) ? baseType.AttributeWildcard : null);
			for (int i = 0; i < attributes.Count; i++)
			{
				XmlSchemaAttribute xmlSchemaAttribute = attributes[i] as XmlSchemaAttribute;
				if (xmlSchemaAttribute != null)
				{
					if (xmlSchemaAttribute.Use != XmlSchemaUse.Prohibited)
					{
						this.CompileAttribute(xmlSchemaAttribute);
					}
					if (xmlSchemaAttribute.Use != XmlSchemaUse.Prohibited || (xmlSchemaAttribute.Use == XmlSchemaUse.Prohibited && derivedBy == XmlSchemaDerivationMethod.Restriction && baseType != XmlSchemaComplexType.AnyType))
					{
						if (derivedType.AttributeUses[xmlSchemaAttribute.QualifiedName] == null)
						{
							derivedType.AttributeUses.Add(xmlSchemaAttribute.QualifiedName, xmlSchemaAttribute);
						}
						else
						{
							base.SendValidationEvent("The attribute '{0}' already exists.", xmlSchemaAttribute.QualifiedName.ToString(), xmlSchemaAttribute);
						}
					}
					else
					{
						base.SendValidationEvent("The '{0}' attribute is ignored, because the value of 'prohibited' for attribute use only prevents inheritance of an identically named attribute from the base type definition.", xmlSchemaAttribute.QualifiedName.ToString(), xmlSchemaAttribute, XmlSeverityType.Warning);
					}
				}
				else
				{
					XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef = (XmlSchemaAttributeGroupRef)attributes[i];
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)this.schema.AttributeGroups[xmlSchemaAttributeGroupRef.RefName];
					if (xmlSchemaAttributeGroup != null)
					{
						this.CompileAttributeGroup(xmlSchemaAttributeGroup);
						foreach (object obj in xmlSchemaAttributeGroup.AttributeUses.Values)
						{
							XmlSchemaAttribute xmlSchemaAttribute2 = (XmlSchemaAttribute)obj;
							if (xmlSchemaAttribute2.Use != XmlSchemaUse.Prohibited || (xmlSchemaAttribute2.Use == XmlSchemaUse.Prohibited && derivedBy == XmlSchemaDerivationMethod.Restriction && baseType != XmlSchemaComplexType.AnyType))
							{
								if (derivedType.AttributeUses[xmlSchemaAttribute2.QualifiedName] == null)
								{
									derivedType.AttributeUses.Add(xmlSchemaAttribute2.QualifiedName, xmlSchemaAttribute2);
								}
								else
								{
									base.SendValidationEvent("The attribute '{0}' already exists.", xmlSchemaAttribute2.QualifiedName.ToString(), xmlSchemaAttributeGroupRef);
								}
							}
							else
							{
								base.SendValidationEvent("The '{0}' attribute is ignored, because the value of 'prohibited' for attribute use only prevents inheritance of an identically named attribute from the base type definition.", xmlSchemaAttribute2.QualifiedName.ToString(), xmlSchemaAttribute2, XmlSeverityType.Warning);
							}
						}
						anyAttribute = this.CompileAnyAttributeIntersection(anyAttribute, xmlSchemaAttributeGroup.AttributeWildcard);
					}
					else
					{
						base.SendValidationEvent("Reference to undeclared attribute group '{0}'.", xmlSchemaAttributeGroupRef.RefName.ToString(), xmlSchemaAttributeGroupRef);
					}
				}
			}
			if (baseType != null)
			{
				if (derivedBy == XmlSchemaDerivationMethod.Extension)
				{
					derivedType.SetAttributeWildcard(this.CompileAnyAttributeUnion(anyAttribute, xmlSchemaAnyAttribute));
					using (IEnumerator enumerator = baseType.AttributeUses.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							object obj2 = enumerator.Current;
							XmlSchemaAttribute xmlSchemaAttribute3 = (XmlSchemaAttribute)obj2;
							XmlSchemaAttribute xmlSchemaAttribute4 = (XmlSchemaAttribute)derivedType.AttributeUses[xmlSchemaAttribute3.QualifiedName];
							if (xmlSchemaAttribute4 != null)
							{
								if (xmlSchemaAttribute4.AttributeSchemaType != xmlSchemaAttribute3.AttributeSchemaType || xmlSchemaAttribute3.Use == XmlSchemaUse.Prohibited)
								{
									base.SendValidationEvent("Invalid attribute extension.", xmlSchemaAttribute4);
								}
							}
							else
							{
								derivedType.AttributeUses.Add(xmlSchemaAttribute3.QualifiedName, xmlSchemaAttribute3);
							}
						}
						return;
					}
				}
				if (anyAttribute != null && (xmlSchemaAnyAttribute == null || !XmlSchemaAnyAttribute.IsSubset(anyAttribute, xmlSchemaAnyAttribute)))
				{
					base.SendValidationEvent("The base any attribute must be a superset of the derived 'anyAttribute'.", derivedType);
				}
				else
				{
					derivedType.SetAttributeWildcard(anyAttribute);
				}
				foreach (object obj3 in baseType.AttributeUses.Values)
				{
					XmlSchemaAttribute xmlSchemaAttribute5 = (XmlSchemaAttribute)obj3;
					XmlSchemaAttribute xmlSchemaAttribute6 = (XmlSchemaAttribute)derivedType.AttributeUses[xmlSchemaAttribute5.QualifiedName];
					if (xmlSchemaAttribute6 == null)
					{
						derivedType.AttributeUses.Add(xmlSchemaAttribute5.QualifiedName, xmlSchemaAttribute5);
					}
					else if (xmlSchemaAttribute5.Use == XmlSchemaUse.Prohibited && xmlSchemaAttribute6.Use != XmlSchemaUse.Prohibited)
					{
						base.SendValidationEvent("Invalid attribute restriction. Attribute restriction is prohibited in base type.", xmlSchemaAttribute6);
					}
					else if (xmlSchemaAttribute6.Use != XmlSchemaUse.Prohibited && (xmlSchemaAttribute5.AttributeSchemaType == null || xmlSchemaAttribute6.AttributeSchemaType == null || !XmlSchemaType.IsDerivedFrom(xmlSchemaAttribute6.AttributeSchemaType, xmlSchemaAttribute5.AttributeSchemaType, XmlSchemaDerivationMethod.Empty)))
					{
						base.SendValidationEvent("Invalid attribute restriction. Derived attribute's type is not a valid restriction of the base attribute's type.", xmlSchemaAttribute6);
					}
				}
				using (IEnumerator enumerator = derivedType.AttributeUses.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj4 = enumerator.Current;
						XmlSchemaAttribute xmlSchemaAttribute7 = (XmlSchemaAttribute)obj4;
						if ((XmlSchemaAttribute)baseType.AttributeUses[xmlSchemaAttribute7.QualifiedName] == null && (xmlSchemaAnyAttribute == null || !xmlSchemaAnyAttribute.Allows(xmlSchemaAttribute7.QualifiedName)))
						{
							base.SendValidationEvent("The {base type definition} must have an {attribute wildcard} and the {target namespace} of the R's {attribute declaration} must be valid with respect to that wildcard.", xmlSchemaAttribute7);
						}
					}
					return;
				}
			}
			derivedType.SetAttributeWildcard(anyAttribute);
		}

		private XmlSchemaAnyAttribute CompileAnyAttributeUnion(XmlSchemaAnyAttribute a, XmlSchemaAnyAttribute b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute = XmlSchemaAnyAttribute.Union(a, b, true);
			if (xmlSchemaAnyAttribute == null)
			{
				base.SendValidationEvent("The 'anyAttribute' is not expressible.", a);
			}
			return xmlSchemaAnyAttribute;
		}

		private XmlSchemaAnyAttribute CompileAnyAttributeIntersection(XmlSchemaAnyAttribute a, XmlSchemaAnyAttribute b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			XmlSchemaAnyAttribute xmlSchemaAnyAttribute = XmlSchemaAnyAttribute.Intersection(a, b, true);
			if (xmlSchemaAnyAttribute == null)
			{
				base.SendValidationEvent("The 'anyAttribute' is not expressible.", a);
			}
			return xmlSchemaAnyAttribute;
		}

		private void CompileAttribute(XmlSchemaAttribute xa)
		{
			if (xa.IsProcessing)
			{
				base.SendValidationEvent("Circular attribute reference.", xa);
				return;
			}
			if (xa.AttDef != null)
			{
				return;
			}
			xa.IsProcessing = true;
			try
			{
				SchemaAttDef schemaAttDef;
				if (!xa.RefName.IsEmpty)
				{
					XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)this.schema.Attributes[xa.RefName];
					if (xmlSchemaAttribute == null)
					{
						throw new XmlSchemaException("The '{0}' attribute is not declared.", xa.RefName.ToString(), xa);
					}
					this.CompileAttribute(xmlSchemaAttribute);
					if (xmlSchemaAttribute.AttDef == null)
					{
						throw new XmlSchemaException("Reference to invalid attribute '{0}'.", xa.RefName.ToString(), xa);
					}
					schemaAttDef = xmlSchemaAttribute.AttDef.Clone();
					if (schemaAttDef.Datatype != null)
					{
						if (xmlSchemaAttribute.FixedValue != null)
						{
							if (xa.DefaultValue != null)
							{
								throw new XmlSchemaException("The default value constraint cannot be present on the '{0}' attribute reference if the fixed value constraint is present on the declaration.", xa.RefName.ToString(), xa);
							}
							if (xa.FixedValue != null)
							{
								if (xa.FixedValue != xmlSchemaAttribute.FixedValue)
								{
									throw new XmlSchemaException("The fixed value constraint on the '{0}' attribute reference must match the fixed value constraint on the declaration.", xa.RefName.ToString(), xa);
								}
							}
							else
							{
								schemaAttDef.Presence = SchemaDeclBase.Use.Fixed;
								schemaAttDef.DefaultValueRaw = (schemaAttDef.DefaultValueExpanded = xmlSchemaAttribute.FixedValue);
								schemaAttDef.DefaultValueTyped = schemaAttDef.Datatype.ParseValue(schemaAttDef.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xa), true);
							}
						}
						else if (xmlSchemaAttribute.DefaultValue != null && xa.DefaultValue == null && xa.FixedValue == null)
						{
							schemaAttDef.Presence = SchemaDeclBase.Use.Default;
							schemaAttDef.DefaultValueRaw = (schemaAttDef.DefaultValueExpanded = xmlSchemaAttribute.DefaultValue);
							schemaAttDef.DefaultValueTyped = schemaAttDef.Datatype.ParseValue(schemaAttDef.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xa), true);
						}
					}
					xa.SetAttributeType(xmlSchemaAttribute.AttributeSchemaType);
				}
				else
				{
					schemaAttDef = new SchemaAttDef(xa.QualifiedName);
					if (xa.SchemaType != null)
					{
						this.CompileSimpleType(xa.SchemaType);
						xa.SetAttributeType(xa.SchemaType);
						schemaAttDef.SchemaType = xa.SchemaType;
						schemaAttDef.Datatype = xa.SchemaType.Datatype;
					}
					else if (!xa.SchemaTypeName.IsEmpty)
					{
						XmlSchemaSimpleType simpleType = this.GetSimpleType(xa.SchemaTypeName);
						if (simpleType == null)
						{
							throw new XmlSchemaException("Type '{0}' is not declared, or is not a simple type.", xa.SchemaTypeName.ToString(), xa);
						}
						xa.SetAttributeType(simpleType);
						schemaAttDef.Datatype = simpleType.Datatype;
						schemaAttDef.SchemaType = simpleType;
					}
					else
					{
						schemaAttDef.SchemaType = DatatypeImplementation.AnySimpleType;
						schemaAttDef.Datatype = DatatypeImplementation.AnySimpleType.Datatype;
						xa.SetAttributeType(DatatypeImplementation.AnySimpleType);
					}
				}
				if (schemaAttDef.Datatype != null)
				{
					schemaAttDef.Datatype.VerifySchemaValid(this.schema.Notations, xa);
				}
				if (xa.DefaultValue != null || xa.FixedValue != null)
				{
					if (xa.DefaultValue != null)
					{
						schemaAttDef.Presence = SchemaDeclBase.Use.Default;
						schemaAttDef.DefaultValueRaw = (schemaAttDef.DefaultValueExpanded = xa.DefaultValue);
					}
					else
					{
						schemaAttDef.Presence = SchemaDeclBase.Use.Fixed;
						schemaAttDef.DefaultValueRaw = (schemaAttDef.DefaultValueExpanded = xa.FixedValue);
					}
					if (schemaAttDef.Datatype != null)
					{
						schemaAttDef.DefaultValueTyped = schemaAttDef.Datatype.ParseValue(schemaAttDef.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xa), true);
					}
				}
				else
				{
					switch (xa.Use)
					{
					case XmlSchemaUse.None:
					case XmlSchemaUse.Optional:
						schemaAttDef.Presence = SchemaDeclBase.Use.Implied;
						break;
					case XmlSchemaUse.Required:
						schemaAttDef.Presence = SchemaDeclBase.Use.Required;
						break;
					}
				}
				schemaAttDef.SchemaAttribute = xa;
				xa.AttDef = schemaAttDef;
			}
			catch (XmlSchemaException ex)
			{
				if (ex.SourceSchemaObject == null)
				{
					ex.SetSource(xa);
				}
				base.SendValidationEvent(ex);
				xa.AttDef = SchemaAttDef.Empty;
			}
			finally
			{
				xa.IsProcessing = false;
			}
		}

		private void CompileIdentityConstraint(XmlSchemaIdentityConstraint xi)
		{
			if (xi.IsProcessing)
			{
				xi.CompiledConstraint = CompiledIdentityConstraint.Empty;
				base.SendValidationEvent("Circular identity constraint reference.", xi);
				return;
			}
			if (xi.CompiledConstraint != null)
			{
				return;
			}
			xi.IsProcessing = true;
			try
			{
				SchemaNamespaceManager schemaNamespaceManager = new SchemaNamespaceManager(xi);
				CompiledIdentityConstraint compiledIdentityConstraint = new CompiledIdentityConstraint(xi, schemaNamespaceManager);
				if (xi is XmlSchemaKeyref)
				{
					XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = (XmlSchemaIdentityConstraint)this.schema.IdentityConstraints[((XmlSchemaKeyref)xi).Refer];
					if (xmlSchemaIdentityConstraint == null)
					{
						throw new XmlSchemaException("The '{0}' identity constraint is not declared.", ((XmlSchemaKeyref)xi).Refer.ToString(), xi);
					}
					this.CompileIdentityConstraint(xmlSchemaIdentityConstraint);
					if (xmlSchemaIdentityConstraint.CompiledConstraint == null)
					{
						throw new XmlSchemaException("Reference to an invalid identity constraint, '{0}'.", ((XmlSchemaKeyref)xi).Refer.ToString(), xi);
					}
					if (xmlSchemaIdentityConstraint.Fields.Count != xi.Fields.Count)
					{
						throw new XmlSchemaException("Keyref '{0}' has different cardinality as the referred key or unique element.", xi.QualifiedName.ToString(), xi);
					}
					if (xmlSchemaIdentityConstraint.CompiledConstraint.Role == CompiledIdentityConstraint.ConstraintRole.Keyref)
					{
						throw new XmlSchemaException("The '{0}' Keyref can refer to key or unique only.", xi.QualifiedName.ToString(), xi);
					}
				}
				xi.CompiledConstraint = compiledIdentityConstraint;
			}
			catch (XmlSchemaException ex)
			{
				if (ex.SourceSchemaObject == null)
				{
					ex.SetSource(xi);
				}
				base.SendValidationEvent(ex);
				xi.CompiledConstraint = CompiledIdentityConstraint.Empty;
			}
			finally
			{
				xi.IsProcessing = false;
			}
		}

		private void CompileElement(XmlSchemaElement xe)
		{
			if (xe.IsProcessing)
			{
				base.SendValidationEvent("Circular element reference.", xe);
				return;
			}
			if (xe.ElementDecl != null)
			{
				return;
			}
			xe.IsProcessing = true;
			SchemaElementDecl schemaElementDecl = null;
			try
			{
				if (!xe.RefName.IsEmpty)
				{
					XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)this.schema.Elements[xe.RefName];
					if (xmlSchemaElement == null)
					{
						throw new XmlSchemaException("The '{0}' element is not declared.", xe.RefName.ToString(), xe);
					}
					this.CompileElement(xmlSchemaElement);
					if (xmlSchemaElement.ElementDecl == null)
					{
						throw new XmlSchemaException("Reference to invalid element '{0}'.", xe.RefName.ToString(), xe);
					}
					xe.SetElementType(xmlSchemaElement.ElementSchemaType);
					schemaElementDecl = xmlSchemaElement.ElementDecl.Clone();
				}
				else
				{
					if (xe.SchemaType != null)
					{
						xe.SetElementType(xe.SchemaType);
					}
					else if (!xe.SchemaTypeName.IsEmpty)
					{
						xe.SetElementType(this.GetAnySchemaType(xe.SchemaTypeName));
						if (xe.ElementSchemaType == null)
						{
							throw new XmlSchemaException("Type '{0}' is not declared.", xe.SchemaTypeName.ToString(), xe);
						}
					}
					else if (!xe.SubstitutionGroup.IsEmpty)
					{
						XmlSchemaElement xmlSchemaElement2 = (XmlSchemaElement)this.schema.Elements[xe.SubstitutionGroup];
						if (xmlSchemaElement2 == null)
						{
							throw new XmlSchemaException("Substitution group refers to '{0}', an undeclared element.", xe.SubstitutionGroup.Name.ToString(CultureInfo.InvariantCulture), xe);
						}
						if (xmlSchemaElement2.IsProcessing)
						{
							return;
						}
						this.CompileElement(xmlSchemaElement2);
						if (xmlSchemaElement2.ElementDecl == null)
						{
							xe.SetElementType(XmlSchemaComplexType.AnyType);
							schemaElementDecl = XmlSchemaComplexType.AnyType.ElementDecl.Clone();
						}
						else
						{
							xe.SetElementType(xmlSchemaElement2.ElementSchemaType);
							schemaElementDecl = xmlSchemaElement2.ElementDecl.Clone();
						}
					}
					else
					{
						xe.SetElementType(XmlSchemaComplexType.AnyType);
						schemaElementDecl = XmlSchemaComplexType.AnyType.ElementDecl.Clone();
					}
					if (schemaElementDecl == null)
					{
						if (xe.ElementSchemaType is XmlSchemaComplexType)
						{
							XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)xe.ElementSchemaType;
							this.CompileComplexType(xmlSchemaComplexType);
							if (xmlSchemaComplexType.ElementDecl != null)
							{
								schemaElementDecl = xmlSchemaComplexType.ElementDecl.Clone();
							}
						}
						else if (xe.ElementSchemaType is XmlSchemaSimpleType)
						{
							XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)xe.ElementSchemaType;
							this.CompileSimpleType(xmlSchemaSimpleType);
							if (xmlSchemaSimpleType.ElementDecl != null)
							{
								schemaElementDecl = xmlSchemaSimpleType.ElementDecl.Clone();
							}
						}
					}
					schemaElementDecl.Name = xe.QualifiedName;
					schemaElementDecl.IsAbstract = xe.IsAbstract;
					XmlSchemaComplexType xmlSchemaComplexType2 = xe.ElementSchemaType as XmlSchemaComplexType;
					if (xmlSchemaComplexType2 != null)
					{
						schemaElementDecl.IsAbstract |= xmlSchemaComplexType2.IsAbstract;
					}
					schemaElementDecl.IsNillable = xe.IsNillable;
					schemaElementDecl.Block |= xe.BlockResolved;
				}
				if (schemaElementDecl.Datatype != null)
				{
					schemaElementDecl.Datatype.VerifySchemaValid(this.schema.Notations, xe);
				}
				if ((xe.DefaultValue != null || xe.FixedValue != null) && schemaElementDecl.ContentValidator != null)
				{
					if (schemaElementDecl.ContentValidator.ContentType == XmlSchemaContentType.TextOnly)
					{
						if (xe.DefaultValue != null)
						{
							schemaElementDecl.Presence = SchemaDeclBase.Use.Default;
							schemaElementDecl.DefaultValueRaw = xe.DefaultValue;
						}
						else
						{
							schemaElementDecl.Presence = SchemaDeclBase.Use.Fixed;
							schemaElementDecl.DefaultValueRaw = xe.FixedValue;
						}
						if (schemaElementDecl.Datatype != null)
						{
							schemaElementDecl.DefaultValueTyped = schemaElementDecl.Datatype.ParseValue(schemaElementDecl.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xe), true);
						}
					}
					else if (schemaElementDecl.ContentValidator.ContentType != XmlSchemaContentType.Mixed || !schemaElementDecl.ContentValidator.IsEmptiable)
					{
						throw new XmlSchemaException("Element's type does not allow fixed or default value constraint.", xe);
					}
				}
				if (xe.HasConstraints)
				{
					XmlSchemaObjectCollection constraints = xe.Constraints;
					CompiledIdentityConstraint[] array = new CompiledIdentityConstraint[constraints.Count];
					int num = 0;
					for (int i = 0; i < constraints.Count; i++)
					{
						XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint = (XmlSchemaIdentityConstraint)constraints[i];
						this.CompileIdentityConstraint(xmlSchemaIdentityConstraint);
						array[num++] = xmlSchemaIdentityConstraint.CompiledConstraint;
					}
					schemaElementDecl.Constraints = array;
				}
				schemaElementDecl.SchemaElement = xe;
				xe.ElementDecl = schemaElementDecl;
			}
			catch (XmlSchemaException ex)
			{
				if (ex.SourceSchemaObject == null)
				{
					ex.SetSource(xe);
				}
				base.SendValidationEvent(ex);
				xe.ElementDecl = SchemaElementDecl.Empty;
			}
			finally
			{
				xe.IsProcessing = false;
			}
		}

		private ContentValidator CompileComplexContent(XmlSchemaComplexType complexType)
		{
			if (complexType.ContentType == XmlSchemaContentType.Empty)
			{
				return ContentValidator.Empty;
			}
			if (complexType.ContentType == XmlSchemaContentType.TextOnly)
			{
				return ContentValidator.TextOnly;
			}
			XmlSchemaParticle contentTypeParticle = complexType.ContentTypeParticle;
			if (contentTypeParticle == null || contentTypeParticle == XmlSchemaParticle.Empty)
			{
				if (complexType.ContentType == XmlSchemaContentType.ElementOnly)
				{
					return ContentValidator.Empty;
				}
				return ContentValidator.Mixed;
			}
			else
			{
				this.PushComplexType(complexType);
				if (contentTypeParticle is XmlSchemaAll)
				{
					XmlSchemaAll xmlSchemaAll = (XmlSchemaAll)contentTypeParticle;
					AllElementsContentValidator allElementsContentValidator = new AllElementsContentValidator(complexType.ContentType, xmlSchemaAll.Items.Count, xmlSchemaAll.MinOccurs == 0m);
					for (int i = 0; i < xmlSchemaAll.Items.Count; i++)
					{
						XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)xmlSchemaAll.Items[i];
						if (!allElementsContentValidator.AddElement(xmlSchemaElement.QualifiedName, xmlSchemaElement, xmlSchemaElement.MinOccurs == 0m))
						{
							base.SendValidationEvent("The '{0}' element already exists in the content model.", xmlSchemaElement.QualifiedName.ToString(), xmlSchemaElement);
						}
					}
					return allElementsContentValidator;
				}
				ParticleContentValidator particleContentValidator = new ParticleContentValidator(complexType.ContentType);
				ContentValidator contentValidator;
				try
				{
					particleContentValidator.Start();
					this.BuildParticleContentModel(particleContentValidator, contentTypeParticle);
					contentValidator = particleContentValidator.Finish(this.compileContentModel);
				}
				catch (UpaException ex)
				{
					if (ex.Particle1 is XmlSchemaElement)
					{
						if (ex.Particle2 is XmlSchemaElement)
						{
							base.SendValidationEvent("Multiple definition of element '{0}' causes the content model to become ambiguous. A content model must be formed such that during validation of an element information item sequence, the particle contained directly, indirectly or implicitly therein with which to attempt to validate each item in the sequence in turn can be uniquely determined without examining the content or attributes of that item, and without any information about the items in the remainder of the sequence.", ((XmlSchemaElement)ex.Particle1).QualifiedName.ToString(), (XmlSchemaElement)ex.Particle2);
						}
						else
						{
							base.SendValidationEvent("Wildcard '{0}' allows element '{1}', and causes the content model to become ambiguous. A content model must be formed such that during validation of an element information item sequence, the particle contained directly, indirectly or implicitly therein with which to attempt to validate each item in the sequence in turn can be uniquely determined without examining the content or attributes of that item, and without any information about the items in the remainder of the sequence.", ((XmlSchemaAny)ex.Particle2).NamespaceList.ToString(), ((XmlSchemaElement)ex.Particle1).QualifiedName.ToString(), (XmlSchemaAny)ex.Particle2);
						}
					}
					else if (ex.Particle2 is XmlSchemaElement)
					{
						base.SendValidationEvent("Wildcard '{0}' allows element '{1}', and causes the content model to become ambiguous. A content model must be formed such that during validation of an element information item sequence, the particle contained directly, indirectly or implicitly therein with which to attempt to validate each item in the sequence in turn can be uniquely determined without examining the content or attributes of that item, and without any information about the items in the remainder of the sequence.", ((XmlSchemaAny)ex.Particle1).NamespaceList.ToString(), ((XmlSchemaElement)ex.Particle2).QualifiedName.ToString(), (XmlSchemaAny)ex.Particle1);
					}
					else
					{
						base.SendValidationEvent("Wildcards '{0}' and '{1}' have not empty intersection, and causes the content model to become ambiguous. A content model must be formed such that during validation of an element information item sequence, the particle contained directly, indirectly or implicitly therein with which to attempt to validate each item in the sequence in turn can be uniquely determined without examining the content or attributes of that item, and without any information about the items in the remainder of the sequence.", ((XmlSchemaAny)ex.Particle1).NamespaceList.ToString(), ((XmlSchemaAny)ex.Particle2).NamespaceList.ToString(), (XmlSchemaAny)ex.Particle1);
					}
					contentValidator = XmlSchemaComplexType.AnyTypeContentValidator;
				}
				catch (NotSupportedException)
				{
					base.SendValidationEvent("Content model validation resulted in a large number of states, possibly due to large occurrence ranges. Therefore, content model may not be validated accurately.", complexType, XmlSeverityType.Warning);
					contentValidator = XmlSchemaComplexType.AnyTypeContentValidator;
				}
				return contentValidator;
			}
		}

		private void BuildParticleContentModel(ParticleContentValidator contentValidator, XmlSchemaParticle particle)
		{
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)particle;
				contentValidator.AddName(xmlSchemaElement.QualifiedName, xmlSchemaElement);
			}
			else if (particle is XmlSchemaAny)
			{
				XmlSchemaAny xmlSchemaAny = (XmlSchemaAny)particle;
				contentValidator.AddNamespaceList(xmlSchemaAny.NamespaceList, xmlSchemaAny);
			}
			else if (particle is XmlSchemaGroupBase)
			{
				XmlSchemaObjectCollection items = ((XmlSchemaGroupBase)particle).Items;
				bool flag = particle is XmlSchemaChoice;
				contentValidator.OpenGroup();
				bool flag2 = true;
				for (int i = 0; i < items.Count; i++)
				{
					XmlSchemaParticle xmlSchemaParticle = (XmlSchemaParticle)items[i];
					if (flag2)
					{
						flag2 = false;
					}
					else if (flag)
					{
						contentValidator.AddChoice();
					}
					else
					{
						contentValidator.AddSequence();
					}
					this.BuildParticleContentModel(contentValidator, xmlSchemaParticle);
				}
				contentValidator.CloseGroup();
			}
			if (!(particle.MinOccurs == 1m) || !(particle.MaxOccurs == 1m))
			{
				if (particle.MinOccurs == 0m && particle.MaxOccurs == 1m)
				{
					contentValidator.AddQMark();
					return;
				}
				if (particle.MinOccurs == 0m && particle.MaxOccurs == 79228162514264337593543950335m)
				{
					contentValidator.AddStar();
					return;
				}
				if (particle.MinOccurs == 1m && particle.MaxOccurs == 79228162514264337593543950335m)
				{
					contentValidator.AddPlus();
					return;
				}
				contentValidator.AddLeafRange(particle.MinOccurs, particle.MaxOccurs);
			}
		}

		private void CompileParticleElements(XmlSchemaComplexType complexType, XmlSchemaParticle particle)
		{
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)particle;
				this.CompileElement(xmlSchemaElement);
				if (complexType.LocalElements[xmlSchemaElement.QualifiedName] == null)
				{
					complexType.LocalElements.Add(xmlSchemaElement.QualifiedName, xmlSchemaElement);
					return;
				}
				if (((XmlSchemaElement)complexType.LocalElements[xmlSchemaElement.QualifiedName]).ElementSchemaType != xmlSchemaElement.ElementSchemaType)
				{
					base.SendValidationEvent("Elements with the same name and in the same scope must have the same type.", particle);
					return;
				}
			}
			else if (particle is XmlSchemaGroupBase)
			{
				XmlSchemaObjectCollection items = ((XmlSchemaGroupBase)particle).Items;
				for (int i = 0; i < items.Count; i++)
				{
					this.CompileParticleElements(complexType, (XmlSchemaParticle)items[i]);
				}
			}
		}

		private void CompileCompexTypeElements(XmlSchemaComplexType complexType)
		{
			if (complexType.IsProcessing)
			{
				base.SendValidationEvent("Circular type reference.", complexType);
				return;
			}
			complexType.IsProcessing = true;
			if (complexType.ContentTypeParticle != XmlSchemaParticle.Empty)
			{
				this.CompileParticleElements(complexType, complexType.ContentTypeParticle);
			}
			complexType.IsProcessing = false;
		}

		private XmlSchemaSimpleType GetSimpleType(XmlQualifiedName name)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = this.schema.SchemaTypes[name] as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType != null)
			{
				this.CompileSimpleType(xmlSchemaSimpleType);
			}
			else
			{
				xmlSchemaSimpleType = DatatypeImplementation.GetSimpleTypeFromXsdType(name);
				if (xmlSchemaSimpleType != null)
				{
					if (xmlSchemaSimpleType.TypeCode == XmlTypeCode.NormalizedString)
					{
						xmlSchemaSimpleType = DatatypeImplementation.GetNormalizedStringTypeV1Compat();
					}
					else if (xmlSchemaSimpleType.TypeCode == XmlTypeCode.Token)
					{
						xmlSchemaSimpleType = DatatypeImplementation.GetTokenTypeV1Compat();
					}
				}
			}
			return xmlSchemaSimpleType;
		}

		private XmlSchemaComplexType GetComplexType(XmlQualifiedName name)
		{
			XmlSchemaComplexType xmlSchemaComplexType = this.schema.SchemaTypes[name] as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null)
			{
				this.CompileComplexType(xmlSchemaComplexType);
			}
			return xmlSchemaComplexType;
		}

		private XmlSchemaType GetAnySchemaType(XmlQualifiedName name)
		{
			XmlSchemaType xmlSchemaType = (XmlSchemaType)this.schema.SchemaTypes[name];
			if (xmlSchemaType != null)
			{
				if (xmlSchemaType is XmlSchemaComplexType)
				{
					this.CompileComplexType((XmlSchemaComplexType)xmlSchemaType);
				}
				else
				{
					this.CompileSimpleType((XmlSchemaSimpleType)xmlSchemaType);
				}
				return xmlSchemaType;
			}
			return DatatypeImplementation.GetSimpleTypeFromXsdType(name);
		}

		private bool compileContentModel;

		private XmlSchemaObjectTable examplars = new XmlSchemaObjectTable();

		private Stack complexTypeStack = new Stack();

		private XmlSchema schema;
	}
}
