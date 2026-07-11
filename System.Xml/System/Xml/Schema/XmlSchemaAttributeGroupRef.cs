using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAttributeGroupRef : XmlSchemaAnnotated
	{
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

		private XmlQualifiedName refName = XmlQualifiedName.Empty;
	}
}
