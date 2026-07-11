using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class XPathDescendantIterator : XPathAxisIterator
	{
		public XPathDescendantIterator(XPathNavigator nav, XPathNodeType type, bool matchSelf)
			: base(nav, type, matchSelf)
		{
		}

		public XPathDescendantIterator(XPathNavigator nav, string name, string namespaceURI, bool matchSelf)
			: base(nav, name, namespaceURI, matchSelf)
		{
		}

		public XPathDescendantIterator(XPathDescendantIterator it)
			: base(it)
		{
			this.level = it.level;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDescendantIterator(this);
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
			for (;;)
			{
				if (!this.nav.MoveToFirstChild())
				{
					while (this.level != 0)
					{
						if (this.nav.MoveToNext())
						{
							goto IL_0078;
						}
						this.nav.MoveToParent();
						this.level--;
					}
					break;
				}
				this.level++;
				IL_0078:
				if (this.Matches)
				{
					goto Block_7;
				}
			}
			return false;
			Block_7:
			this.position++;
			return true;
		}

		private int level;
	}
}
