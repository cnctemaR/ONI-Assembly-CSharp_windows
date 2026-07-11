using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal class XPathDocumentKindDescendantIterator : XPathDocumentBaseIterator
	{
		public XPathDocumentKindDescendantIterator(XPathDocumentNavigator root, XPathNodeType typ, bool matchSelf)
			: base(root)
		{
			this.typ = typ;
			this.matchSelf = matchSelf;
			if (root.NodeType != XPathNodeType.Root)
			{
				this.end = new XPathDocumentNavigator(root);
				this.end.MoveToNonDescendant();
			}
		}

		public XPathDocumentKindDescendantIterator(XPathDocumentKindDescendantIterator iter)
			: base(iter)
		{
			this.end = iter.end;
			this.typ = iter.typ;
			this.matchSelf = iter.matchSelf;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentKindDescendantIterator(this);
		}

		public override bool MoveNext()
		{
			if (this.matchSelf)
			{
				this.matchSelf = false;
				if (this.ctxt.IsKindMatch(this.typ))
				{
					this.pos++;
					return true;
				}
			}
			if (!this.ctxt.MoveToFollowing(this.typ, this.end))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private XPathDocumentNavigator end;

		private XPathNodeType typ;

		private bool matchSelf;
	}
}
