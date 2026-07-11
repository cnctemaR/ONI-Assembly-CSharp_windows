using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaKeyref : XmlSchemaIdentityConstraint
	{
		[XmlAttribute("refer")]
		public XmlQualifiedName Refer
		{
			get
			{
				return this.refer;
			}
			set
			{
				this.refer = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		private XmlQualifiedName refer = XmlQualifiedName.Empty;
	}
}
