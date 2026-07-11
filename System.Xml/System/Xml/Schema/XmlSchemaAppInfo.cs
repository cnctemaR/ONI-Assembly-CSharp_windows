using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaAppInfo : XmlSchemaObject
	{
		[XmlAttribute("source", DataType = "anyURI")]
		public string Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		[XmlAnyElement]
		[XmlText]
		public XmlNode[] Markup
		{
			get
			{
				return this.markup;
			}
			set
			{
				this.markup = value;
			}
		}

		private string source;

		private XmlNode[] markup;
	}
}
