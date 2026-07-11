using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleContent : XmlSchemaContentModel
	{
		[XmlElement("extension", typeof(XmlSchemaSimpleContentExtension))]
		[XmlElement("restriction", typeof(XmlSchemaSimpleContentRestriction))]
		public override XmlSchemaContent Content
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

		private XmlSchemaContent content;
	}
}
