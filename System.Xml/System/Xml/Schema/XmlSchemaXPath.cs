using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaXPath : XmlSchemaAnnotated
	{
		[XmlAttribute("xpath")]
		[DefaultValue("")]
		public string XPath
		{
			get
			{
				return this.xpath;
			}
			set
			{
				this.xpath = value;
			}
		}

		private string xpath;
	}
}
