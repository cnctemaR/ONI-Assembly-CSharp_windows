using System;
using System.Collections;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class SchemaGraph
	{
		internal SchemaGraph(Hashtable scope, XmlSchemas schemas)
		{
			this.scope = scope;
			schemas.Compile(null, false);
			this.schemas = schemas;
			this.items = 0;
			foreach (object obj in schemas)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				this.items += xmlSchema.Items.Count;
				foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				{
					this.Depends(xmlSchemaObject);
				}
			}
		}

		internal ArrayList GetItems()
		{
			return new ArrayList(this.scope.Keys);
		}

		internal void AddRef(ArrayList list, XmlSchemaObject o)
		{
			if (o == null)
			{
				return;
			}
			if (this.schemas.IsReference(o))
			{
				return;
			}
			if (o.Parent is XmlSchema)
			{
				if (((XmlSchema)o.Parent).TargetNamespace == "http://www.w3.org/2001/XMLSchema")
				{
					return;
				}
				if (list.Contains(o))
				{
					return;
				}
				list.Add(o);
			}
		}

		internal ArrayList Depends(XmlSchemaObject item)
		{
			if (!(item.Parent is XmlSchema))
			{
				return this.empty;
			}
			if (this.scope[item] != null)
			{
				return (ArrayList)this.scope[item];
			}
			ArrayList arrayList = new ArrayList();
			this.Depends(item, arrayList);
			this.scope.Add(item, arrayList);
			return arrayList;
		}

		internal void Depends(XmlSchemaObject item, ArrayList refs)
		{
			if (item == null || this.scope[item] != null)
			{
				return;
			}
			Type type = item.GetType();
			if (typeof(XmlSchemaType).IsAssignableFrom(type))
			{
				XmlQualifiedName xmlQualifiedName = XmlQualifiedName.Empty;
				XmlSchemaType xmlSchemaType = null;
				XmlSchemaParticle xmlSchemaParticle = null;
				XmlSchemaObjectCollection xmlSchemaObjectCollection = null;
				if (item is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)item;
					if (xmlSchemaComplexType.ContentModel != null)
					{
						XmlSchemaContent content = xmlSchemaComplexType.ContentModel.Content;
						if (content is XmlSchemaComplexContentRestriction)
						{
							xmlQualifiedName = ((XmlSchemaComplexContentRestriction)content).BaseTypeName;
							xmlSchemaObjectCollection = ((XmlSchemaComplexContentRestriction)content).Attributes;
						}
						else if (content is XmlSchemaSimpleContentRestriction)
						{
							XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = (XmlSchemaSimpleContentRestriction)content;
							if (xmlSchemaSimpleContentRestriction.BaseType != null)
							{
								xmlSchemaType = xmlSchemaSimpleContentRestriction.BaseType;
							}
							else
							{
								xmlQualifiedName = xmlSchemaSimpleContentRestriction.BaseTypeName;
							}
							xmlSchemaObjectCollection = xmlSchemaSimpleContentRestriction.Attributes;
						}
						else if (content is XmlSchemaComplexContentExtension)
						{
							XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = (XmlSchemaComplexContentExtension)content;
							xmlSchemaObjectCollection = xmlSchemaComplexContentExtension.Attributes;
							xmlSchemaParticle = xmlSchemaComplexContentExtension.Particle;
							xmlQualifiedName = xmlSchemaComplexContentExtension.BaseTypeName;
						}
						else if (content is XmlSchemaSimpleContentExtension)
						{
							XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = (XmlSchemaSimpleContentExtension)content;
							xmlSchemaObjectCollection = xmlSchemaSimpleContentExtension.Attributes;
							xmlQualifiedName = xmlSchemaSimpleContentExtension.BaseTypeName;
						}
					}
					else
					{
						xmlSchemaObjectCollection = xmlSchemaComplexType.Attributes;
						xmlSchemaParticle = xmlSchemaComplexType.Particle;
					}
					if (xmlSchemaParticle is XmlSchemaGroupRef)
					{
						XmlSchemaGroupRef xmlSchemaGroupRef = (XmlSchemaGroupRef)xmlSchemaParticle;
						xmlSchemaParticle = ((XmlSchemaGroup)this.schemas.Find(xmlSchemaGroupRef.RefName, typeof(XmlSchemaGroup), false)).Particle;
					}
					else if (xmlSchemaParticle is XmlSchemaGroupBase)
					{
						xmlSchemaParticle = (XmlSchemaGroupBase)xmlSchemaParticle;
					}
				}
				else if (item is XmlSchemaSimpleType)
				{
					XmlSchemaSimpleTypeContent content2 = ((XmlSchemaSimpleType)item).Content;
					if (content2 is XmlSchemaSimpleTypeRestriction)
					{
						xmlSchemaType = ((XmlSchemaSimpleTypeRestriction)content2).BaseType;
						xmlQualifiedName = ((XmlSchemaSimpleTypeRestriction)content2).BaseTypeName;
					}
					else if (content2 is XmlSchemaSimpleTypeList)
					{
						XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = (XmlSchemaSimpleTypeList)content2;
						if (xmlSchemaSimpleTypeList.ItemTypeName != null && !xmlSchemaSimpleTypeList.ItemTypeName.IsEmpty)
						{
							xmlQualifiedName = xmlSchemaSimpleTypeList.ItemTypeName;
						}
						if (xmlSchemaSimpleTypeList.ItemType != null)
						{
							xmlSchemaType = xmlSchemaSimpleTypeList.ItemType;
						}
					}
					else if (content2 is XmlSchemaSimpleTypeRestriction)
					{
						xmlQualifiedName = ((XmlSchemaSimpleTypeRestriction)content2).BaseTypeName;
					}
					else if (type == typeof(XmlSchemaSimpleTypeUnion))
					{
						XmlQualifiedName[] memberTypes = ((XmlSchemaSimpleTypeUnion)item).MemberTypes;
						if (memberTypes != null)
						{
							for (int i = 0; i < memberTypes.Length; i++)
							{
								XmlSchemaType xmlSchemaType2 = (XmlSchemaType)this.schemas.Find(memberTypes[i], typeof(XmlSchemaType), false);
								this.AddRef(refs, xmlSchemaType2);
							}
						}
					}
				}
				if (xmlSchemaType == null && !xmlQualifiedName.IsEmpty && xmlQualifiedName.Namespace != "http://www.w3.org/2001/XMLSchema")
				{
					xmlSchemaType = (XmlSchemaType)this.schemas.Find(xmlQualifiedName, typeof(XmlSchemaType), false);
				}
				if (xmlSchemaType != null)
				{
					this.AddRef(refs, xmlSchemaType);
				}
				if (xmlSchemaParticle != null)
				{
					this.Depends(xmlSchemaParticle, refs);
				}
				if (xmlSchemaObjectCollection != null)
				{
					for (int j = 0; j < xmlSchemaObjectCollection.Count; j++)
					{
						this.Depends(xmlSchemaObjectCollection[j], refs);
					}
				}
			}
			else if (type == typeof(XmlSchemaElement))
			{
				XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)item;
				if (!xmlSchemaElement.SubstitutionGroup.IsEmpty && xmlSchemaElement.SubstitutionGroup.Namespace != "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaElement xmlSchemaElement2 = (XmlSchemaElement)this.schemas.Find(xmlSchemaElement.SubstitutionGroup, typeof(XmlSchemaElement), false);
					this.AddRef(refs, xmlSchemaElement2);
				}
				if (!xmlSchemaElement.RefName.IsEmpty)
				{
					xmlSchemaElement = (XmlSchemaElement)this.schemas.Find(xmlSchemaElement.RefName, typeof(XmlSchemaElement), false);
					this.AddRef(refs, xmlSchemaElement);
				}
				else if (!xmlSchemaElement.SchemaTypeName.IsEmpty)
				{
					XmlSchemaType xmlSchemaType3 = (XmlSchemaType)this.schemas.Find(xmlSchemaElement.SchemaTypeName, typeof(XmlSchemaType), false);
					this.AddRef(refs, xmlSchemaType3);
				}
				else
				{
					this.Depends(xmlSchemaElement.SchemaType, refs);
				}
			}
			else if (type == typeof(XmlSchemaGroup))
			{
				this.Depends(((XmlSchemaGroup)item).Particle);
			}
			else if (type == typeof(XmlSchemaGroupRef))
			{
				XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup)this.schemas.Find(((XmlSchemaGroupRef)item).RefName, typeof(XmlSchemaGroup), false);
				this.AddRef(refs, xmlSchemaGroup);
			}
			else
			{
				if (typeof(XmlSchemaGroupBase).IsAssignableFrom(type))
				{
					using (XmlSchemaObjectEnumerator xmlSchemaObjectEnumerator = ((XmlSchemaGroupBase)item).Items.GetEnumerator())
					{
						while (xmlSchemaObjectEnumerator.MoveNext())
						{
							XmlSchemaObject xmlSchemaObject = xmlSchemaObjectEnumerator.Current;
							this.Depends(xmlSchemaObject, refs);
						}
						goto IL_0614;
					}
				}
				if (type == typeof(XmlSchemaAttributeGroupRef))
				{
					XmlSchemaAttributeGroup xmlSchemaAttributeGroup = (XmlSchemaAttributeGroup)this.schemas.Find(((XmlSchemaAttributeGroupRef)item).RefName, typeof(XmlSchemaAttributeGroup), false);
					this.AddRef(refs, xmlSchemaAttributeGroup);
				}
				else
				{
					if (type == typeof(XmlSchemaAttributeGroup))
					{
						using (XmlSchemaObjectEnumerator xmlSchemaObjectEnumerator = ((XmlSchemaAttributeGroup)item).Attributes.GetEnumerator())
						{
							while (xmlSchemaObjectEnumerator.MoveNext())
							{
								XmlSchemaObject xmlSchemaObject2 = xmlSchemaObjectEnumerator.Current;
								this.Depends(xmlSchemaObject2, refs);
							}
							goto IL_0614;
						}
					}
					if (type == typeof(XmlSchemaAttribute))
					{
						XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)item;
						if (!xmlSchemaAttribute.RefName.IsEmpty)
						{
							xmlSchemaAttribute = (XmlSchemaAttribute)this.schemas.Find(xmlSchemaAttribute.RefName, typeof(XmlSchemaAttribute), false);
							this.AddRef(refs, xmlSchemaAttribute);
						}
						else if (!xmlSchemaAttribute.SchemaTypeName.IsEmpty)
						{
							XmlSchemaType xmlSchemaType4 = (XmlSchemaType)this.schemas.Find(xmlSchemaAttribute.SchemaTypeName, typeof(XmlSchemaType), false);
							this.AddRef(refs, xmlSchemaType4);
						}
						else
						{
							this.Depends(xmlSchemaAttribute.SchemaType, refs);
						}
					}
				}
			}
			IL_0614:
			if (typeof(XmlSchemaAnnotated).IsAssignableFrom(type))
			{
				XmlAttribute[] unhandledAttributes = ((XmlSchemaAnnotated)item).UnhandledAttributes;
				if (unhandledAttributes != null)
				{
					foreach (XmlAttribute xmlAttribute in unhandledAttributes)
					{
						if (xmlAttribute.LocalName == "arrayType" && xmlAttribute.NamespaceURI == "http://schemas.xmlsoap.org/wsdl/")
						{
							string text;
							XmlQualifiedName xmlQualifiedName2 = TypeScope.ParseWsdlArrayType(xmlAttribute.Value, out text, item);
							XmlSchemaType xmlSchemaType5 = (XmlSchemaType)this.schemas.Find(xmlQualifiedName2, typeof(XmlSchemaType), false);
							this.AddRef(refs, xmlSchemaType5);
						}
					}
				}
			}
		}

		private ArrayList empty = new ArrayList();

		private XmlSchemas schemas;

		private Hashtable scope;

		private int items;
	}
}
