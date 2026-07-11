using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathAncestorIterator : XPathAxisIterator
	{
		public XPathAncestorIterator(XPathNavigator nav, XPathNodeType type, bool matchSelf)
			: base(nav, type, matchSelf)
		{
		}

		public XPathAncestorIterator(XPathNavigator nav, string name, string namespaceURI, bool matchSelf)
			: base(nav, name, namespaceURI, matchSelf)
		{
		}

		public XPathAncestorIterator(XPathAncestorIterator other)
			: base(other)
		{
		}

		public override bool MoveNext()
		{
			if (this.first)
			{
				this.first = false;
				if (this.matchSelf && this.Matches)
				{
					this.position = 1;
					return true;
				}
			}
			while (this.nav.MoveToParent())
			{
				if (this.Matches)
				{
					this.position++;
					return true;
				}
			}
			return false;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathAncestorIterator(this);
		}
	}
}
