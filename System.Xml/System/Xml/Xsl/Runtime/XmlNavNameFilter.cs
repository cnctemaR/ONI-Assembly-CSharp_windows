using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlNavNameFilter : XmlNavigatorFilter
	{
		public static XmlNavigatorFilter Create(string localName, string namespaceUri)
		{
			return new XmlNavNameFilter(localName, namespaceUri);
		}

		private XmlNavNameFilter(string localName, string namespaceUri)
		{
			this.localName = localName;
			this.namespaceUri = namespaceUri;
		}

		public override bool MoveToContent(XPathNavigator navigator)
		{
			return navigator.MoveToChild(this.localName, this.namespaceUri);
		}

		public override bool MoveToNextContent(XPathNavigator navigator)
		{
			return navigator.MoveToNext(this.localName, this.namespaceUri);
		}

		public override bool MoveToFollowingSibling(XPathNavigator navigator)
		{
			return navigator.MoveToNext(this.localName, this.namespaceUri);
		}

		public override bool MoveToPreviousSibling(XPathNavigator navigator)
		{
			return navigator.MoveToPrevious(this.localName, this.namespaceUri);
		}

		public override bool MoveToFollowing(XPathNavigator navigator, XPathNavigator navEnd)
		{
			return navigator.MoveToFollowing(this.localName, this.namespaceUri, navEnd);
		}

		public override bool IsFiltered(XPathNavigator navigator)
		{
			return navigator.LocalName != this.localName || navigator.NamespaceURI != this.namespaceUri;
		}

		private string localName;

		private string namespaceUri;
	}
}
