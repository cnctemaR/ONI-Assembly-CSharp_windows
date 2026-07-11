using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal sealed class NavigatorConstructor
	{
		public XPathNavigator GetNavigator(XmlEventCache events, XmlNameTable nameTable)
		{
			if (this.cache == null)
			{
				XPathDocument xpathDocument = new XPathDocument(nameTable);
				XmlRawWriter xmlRawWriter = xpathDocument.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames | (events.HasRootNode ? XPathDocument.LoadFlags.None : XPathDocument.LoadFlags.Fragment), events.BaseUri);
				events.EventsToWriter(xmlRawWriter);
				xmlRawWriter.Close();
				this.cache = xpathDocument;
			}
			return ((XPathDocument)this.cache).CreateNavigator();
		}

		public XPathNavigator GetNavigator(string text, string baseUri, XmlNameTable nameTable)
		{
			if (this.cache == null)
			{
				XPathDocument xpathDocument = new XPathDocument(nameTable);
				XmlRawWriter xmlRawWriter = xpathDocument.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames, baseUri);
				xmlRawWriter.WriteString(text);
				xmlRawWriter.Close();
				this.cache = xpathDocument;
			}
			return ((XPathDocument)this.cache).CreateNavigator();
		}

		private object cache;
	}
}
