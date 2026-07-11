using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathChildIterator : XPathAxisIterator
	{
		public XPathChildIterator(XPathNavigator nav, XPathNodeType type)
			: base(nav, type, false)
		{
		}

		public XPathChildIterator(XPathNavigator nav, string name, string namespaceURI)
			: base(nav, name, namespaceURI, false)
		{
		}

		public XPathChildIterator(XPathChildIterator it)
			: base(it)
		{
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathChildIterator(this);
		}

		public override bool MoveNext()
		{
			while (this.first ? this.nav.MoveToFirstChild() : this.nav.MoveToNext())
			{
				this.first = false;
				if (this.Matches)
				{
					this.position++;
					return true;
				}
			}
			return false;
		}
	}
}
