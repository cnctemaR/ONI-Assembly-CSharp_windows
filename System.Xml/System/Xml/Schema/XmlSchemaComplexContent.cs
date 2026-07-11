using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaComplexContent : XmlSchemaContentModel
	{
		[XmlAttribute("mixed")]
		public bool IsMixed
		{
			get
			{
				return this.isMixed;
			}
			set
			{
				this.isMixed = value;
				this.hasMixedAttribute = true;
			}
		}

		[XmlElement("extension", typeof(XmlSchemaComplexContentExtension))]
		[XmlElement("restriction", typeof(XmlSchemaComplexContentRestriction))]
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

		[XmlIgnore]
		internal bool HasMixedAttribute
		{
			get
			{
				return this.hasMixedAttribute;
			}
		}

		private XmlSchemaContent content;

		private bool isMixed;

		private bool hasMixedAttribute;
	}
}
