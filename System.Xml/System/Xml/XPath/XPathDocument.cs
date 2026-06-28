using System;
using System.IO;
using Mono.Xml.XPath;

namespace System.Xml.XPath
{
	public class XPathDocument : IXPathNavigable
	{
		public XPathDocument(Stream stream)
		{
			this.Initialize(new XmlValidatingReader(new XmlTextReader(stream))
			{
				ValidationType = ValidationType.None
			}, XmlSpace.None);
		}

		public XPathDocument(string uri)
			: this(uri, XmlSpace.None)
		{
		}

		public XPathDocument(TextReader reader)
		{
			this.Initialize(new XmlValidatingReader(new XmlTextReader(reader))
			{
				ValidationType = ValidationType.None
			}, XmlSpace.None);
		}

		public XPathDocument(XmlReader reader)
			: this(reader, XmlSpace.None)
		{
		}

		public XPathDocument(string uri, XmlSpace space)
		{
			XmlValidatingReader xmlValidatingReader = null;
			try
			{
				xmlValidatingReader = new XmlValidatingReader(new XmlTextReader(uri));
				xmlValidatingReader.ValidationType = ValidationType.None;
				this.Initialize(xmlValidatingReader, space);
			}
			finally
			{
				if (xmlValidatingReader != null)
				{
					xmlValidatingReader.Close();
				}
			}
		}

		public XPathDocument(XmlReader reader, XmlSpace space)
		{
			this.Initialize(reader, space);
		}

		private void Initialize(XmlReader reader, XmlSpace space)
		{
			this.document = new DTMXPathDocumentBuilder2(reader, space).CreateDocument();
		}

		public XPathNavigator CreateNavigator()
		{
			return this.document.CreateNavigator();
		}

		private IXPathNavigable document;
	}
}
