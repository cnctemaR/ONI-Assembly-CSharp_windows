using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAttribute : XmlSchemaAnnotated
	{
		[DefaultValue(null)]
		[XmlAttribute("default")]
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		[XmlAttribute("fixed")]
		[DefaultValue(null)]
		public string FixedValue
		{
			get
			{
				return this.fixedValue;
			}
			set
			{
				this.fixedValue = value;
			}
		}

		[XmlAttribute("form")]
		[DefaultValue(XmlSchemaForm.None)]
		public XmlSchemaForm Form
		{
			get
			{
				return this.form;
			}
			set
			{
				this.form = value;
			}
		}

		[XmlAttribute("name")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute("ref")]
		public XmlQualifiedName RefName
		{
			get
			{
				return this.refName;
			}
			set
			{
				this.refName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		[XmlAttribute("type")]
		public XmlQualifiedName SchemaTypeName
		{
			get
			{
				return this.typeName;
			}
			set
			{
				this.typeName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		[XmlElement("simpleType")]
		public XmlSchemaSimpleType SchemaType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		[XmlAttribute("use")]
		[DefaultValue(XmlSchemaUse.None)]
		public XmlSchemaUse Use
		{
			get
			{
				return this.use;
			}
			set
			{
				this.use = value;
			}
		}

		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qualifiedName;
			}
		}

		[Obsolete("This property has been deprecated. Please use AttributeSchemaType property that returns a strongly typed attribute type. http://go.microsoft.com/fwlink/?linkid=14202")]
		[XmlIgnore]
		public object AttributeType
		{
			get
			{
				if (this.attributeType == null)
				{
					return null;
				}
				if (this.attributeType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
				{
					return this.attributeType.Datatype;
				}
				return this.attributeType;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType AttributeSchemaType
		{
			get
			{
				return this.attributeType;
			}
		}

		internal XmlReader Validate(XmlReader reader, XmlResolver resolver, XmlSchemaSet schemaSet, ValidationEventHandler valEventHandler)
		{
			if (schemaSet != null)
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.Schemas = schemaSet;
				xmlReaderSettings.ValidationEventHandler += valEventHandler;
				return new XsdValidatingReader(reader, resolver, xmlReaderSettings, this);
			}
			return null;
		}

		[XmlIgnore]
		internal XmlSchemaDatatype Datatype
		{
			get
			{
				if (this.attributeType != null)
				{
					return this.attributeType.Datatype;
				}
				return null;
			}
		}

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qualifiedName = value;
		}

		internal void SetAttributeType(XmlSchemaSimpleType value)
		{
			this.attributeType = value;
		}

		internal SchemaAttDef AttDef
		{
			get
			{
				return this.attDef;
			}
			set
			{
				this.attDef = value;
			}
		}

		internal bool HasDefault
		{
			get
			{
				return this.defaultValue != null;
			}
		}

		[XmlIgnore]
		internal override string NameAttribute
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		internal override XmlSchemaObject Clone()
		{
			XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)base.MemberwiseClone();
			xmlSchemaAttribute.refName = this.refName.Clone();
			xmlSchemaAttribute.typeName = this.typeName.Clone();
			xmlSchemaAttribute.qualifiedName = this.qualifiedName.Clone();
			return xmlSchemaAttribute;
		}

		private string defaultValue;

		private string fixedValue;

		private string name;

		private XmlSchemaForm form;

		private XmlSchemaUse use;

		private XmlQualifiedName refName = XmlQualifiedName.Empty;

		private XmlQualifiedName typeName = XmlQualifiedName.Empty;

		private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;

		private XmlSchemaSimpleType type;

		private XmlSchemaSimpleType attributeType;

		private SchemaAttDef attDef;
	}
}
