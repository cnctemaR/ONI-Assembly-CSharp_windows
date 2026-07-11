using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleType : XmlSchemaType
	{
		[XmlElement("union", typeof(XmlSchemaSimpleTypeUnion))]
		[XmlElement("restriction", typeof(XmlSchemaSimpleTypeRestriction))]
		[XmlElement("list", typeof(XmlSchemaSimpleTypeList))]
		public XmlSchemaSimpleTypeContent Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		internal override XmlQualifiedName DerivedFrom
		{
			get
			{
				if (this.content == null)
				{
					return XmlQualifiedName.Empty;
				}
				if (this.content is XmlSchemaSimpleTypeRestriction)
				{
					return ((XmlSchemaSimpleTypeRestriction)this.content).BaseTypeName;
				}
				return XmlQualifiedName.Empty;
			}
		}

		internal override XmlSchemaObject Clone()
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)base.MemberwiseClone();
			if (this.content != null)
			{
				xmlSchemaSimpleType.Content = (XmlSchemaSimpleTypeContent)this.content.Clone();
			}
			return xmlSchemaSimpleType;
		}

		private XmlSchemaSimpleTypeContent content;
	}
}
