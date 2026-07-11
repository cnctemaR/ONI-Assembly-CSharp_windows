using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleContentExtension : XmlSchemaContent
	{
		[XmlAttribute("base")]
		public XmlQualifiedName BaseTypeName
		{
			get
			{
				return this.baseTypeName;
			}
			set
			{
				this.baseTypeName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
		public XmlSchemaObjectCollection Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		[XmlElement("anyAttribute")]
		public XmlSchemaAnyAttribute AnyAttribute
		{
			get
			{
				return this.anyAttribute;
			}
			set
			{
				this.anyAttribute = value;
			}
		}

		internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
		{
			this.attributes = newAttributes;
		}

		private XmlSchemaObjectCollection attributes = new XmlSchemaObjectCollection();

		private XmlSchemaAnyAttribute anyAttribute;

		private XmlQualifiedName baseTypeName = XmlQualifiedName.Empty;
	}
}
