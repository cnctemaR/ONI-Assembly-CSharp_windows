using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.XPath
{
	internal class XPathEditableDocument : IXPathNavigable
	{
		public XPathEditableDocument(XmlNode node)
		{
			this.node = node;
		}

		public XmlNode Node
		{
			get
			{
				return this.node;
			}
		}

		public XPathNavigator CreateNavigator()
		{
			return new XmlDocumentEditableNavigator(this);
		}

		private XmlNode node;
	}
}
