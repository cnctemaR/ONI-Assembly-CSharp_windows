using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleTypeList : XmlSchemaSimpleTypeContent
	{
		public XmlSchemaSimpleTypeList()
		{
			this.ItemTypeName = XmlQualifiedName.Empty;
		}

		[XmlAttribute("itemType")]
		public XmlQualifiedName ItemTypeName
		{
			get
			{
				return this.itemTypeName;
			}
			set
			{
				this.itemTypeName = value;
			}
		}

		[XmlElement("simpleType", Type = typeof(XmlSchemaSimpleType))]
		public XmlSchemaSimpleType ItemType
		{
			get
			{
				return this.itemType;
			}
			set
			{
				this.itemType = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType BaseItemType
		{
			get
			{
				return this.validatedListItemSchemaType;
			}
			set
			{
			}
		}

		internal object ValidatedListItemType
		{
			get
			{
				return this.validatedListItemType;
			}
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (this.ItemType != null)
			{
				this.ItemType.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (this.CompilationId == schema.CompilationId)
			{
				return 0;
			}
			this.errorCount = 0;
			if (this.ItemType != null && !this.ItemTypeName.IsEmpty)
			{
				base.error(h, "both itemType and simpletype can't be present");
			}
			if (this.ItemType == null && this.ItemTypeName.IsEmpty)
			{
				base.error(h, "one of itemType or simpletype must be present");
			}
			if (this.ItemType != null)
			{
				this.errorCount += this.ItemType.Compile(h, schema);
			}
			if (!XmlSchemaUtil.CheckQName(this.ItemTypeName))
			{
				base.error(h, "BaseTypeName must be a XmlQualifiedName");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			this.CompilationId = schema.CompilationId;
			return this.errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (base.IsValidated(schema.ValidationId))
			{
				return this.errorCount;
			}
			XmlSchemaSimpleType xmlSchemaSimpleType = this.itemType;
			if (xmlSchemaSimpleType == null)
			{
				xmlSchemaSimpleType = schema.FindSchemaType(this.itemTypeName) as XmlSchemaSimpleType;
			}
			if (xmlSchemaSimpleType != null)
			{
				this.errorCount += xmlSchemaSimpleType.Validate(h, schema);
				this.validatedListItemType = xmlSchemaSimpleType;
			}
			else if (this.itemTypeName == XmlSchemaComplexType.AnyTypeName)
			{
				this.validatedListItemType = XmlSchemaSimpleType.AnySimpleType;
			}
			else if (XmlSchemaUtil.IsBuiltInDatatypeName(this.itemTypeName))
			{
				this.validatedListItemType = XmlSchemaDatatype.FromName(this.itemTypeName);
				if (this.validatedListItemType == null)
				{
					base.error(h, "Invalid schema type name was specified: " + this.itemTypeName);
				}
			}
			else if (!schema.IsNamespaceAbsent(this.itemTypeName.Namespace))
			{
				base.error(h, "Referenced base list item schema type " + this.itemTypeName + " was not found.");
			}
			XmlSchemaSimpleType xmlSchemaSimpleType2 = this.validatedListItemType as XmlSchemaSimpleType;
			if (xmlSchemaSimpleType2 == null && this.validatedListItemType != null)
			{
				xmlSchemaSimpleType2 = XmlSchemaType.GetBuiltInSimpleType(((XmlSchemaDatatype)this.validatedListItemType).TypeCode);
			}
			this.validatedListItemSchemaType = xmlSchemaSimpleType2;
			this.ValidationId = schema.ValidationId;
			return this.errorCount;
		}

		internal static XmlSchemaSimpleTypeList Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = new XmlSchemaSimpleTypeList();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "list")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaSimpleTypeList.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleTypeList.LineNumber = reader.LineNumber;
			xmlSchemaSimpleTypeList.LinePosition = reader.LinePosition;
			xmlSchemaSimpleTypeList.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSimpleTypeList.Id = reader.Value;
				}
				else if (reader.Name == "itemType")
				{
					Exception ex;
					xmlSchemaSimpleTypeList.ItemTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out ex);
					if (ex != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for itemType attribute", ex);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for list", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleTypeList);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleTypeList;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "list")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleTypeList.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleTypeList.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 3;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleTypeList.itemType = xmlSchemaSimpleType;
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaSimpleTypeList;
		}

		private const string xmlname = "list";

		private XmlSchemaSimpleType itemType;

		private XmlQualifiedName itemTypeName;

		private object validatedListItemType;

		private XmlSchemaSimpleType validatedListItemSchemaType;
	}
}
