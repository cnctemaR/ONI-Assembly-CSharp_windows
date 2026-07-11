using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal abstract class XPathAxisIterator : XPathNodeIterator
	{
		public XPathAxisIterator(XPathNavigator nav, bool matchSelf)
		{
			this.nav = nav;
			this.matchSelf = matchSelf;
		}

		public XPathAxisIterator(XPathNavigator nav, XPathNodeType type, bool matchSelf)
			: this(nav, matchSelf)
		{
			this.type = type;
		}

		public XPathAxisIterator(XPathNavigator nav, string name, string namespaceURI, bool matchSelf)
			: this(nav, matchSelf)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			this.name = name;
			this.uri = namespaceURI;
		}

		public XPathAxisIterator(XPathAxisIterator it)
		{
			this.nav = it.nav.Clone();
			this.type = it.type;
			this.name = it.name;
			this.uri = it.uri;
			this.position = it.position;
			this.matchSelf = it.matchSelf;
			this.first = it.first;
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.nav;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.position;
			}
		}

		protected virtual bool Matches
		{
			get
			{
				if (this.name == null)
				{
					return this.type == this.nav.NodeType || this.type == XPathNodeType.All || (this.type == XPathNodeType.Text && (this.nav.NodeType == XPathNodeType.Whitespace || this.nav.NodeType == XPathNodeType.SignificantWhitespace));
				}
				return this.nav.NodeType == XPathNodeType.Element && (this.name.Length == 0 || this.name == this.nav.LocalName) && this.uri == this.nav.NamespaceURI;
			}
		}

		internal XPathNavigator nav;

		internal XPathNodeType type;

		internal string name;

		internal string uri;

		internal int position;

		internal bool matchSelf;

		internal bool first = true;
	}
}
