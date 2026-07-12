using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal class XPathDocumentKindDescendantIterator : XPathDocumentBaseIterator
	{
		public XPathDocumentKindDescendantIterator(XPathDocumentNavigator root, XPathNodeType typ, bool matchSelf)
			: base(root)
		{
			this._typ = typ;
			this._matchSelf = matchSelf;
			if (root.NodeType != XPathNodeType.Root)
			{
				this._end = new XPathDocumentNavigator(root);
				this._end.MoveToNonDescendant();
			}
		}

		public XPathDocumentKindDescendantIterator(XPathDocumentKindDescendantIterator iter)
			: base(iter)
		{
			this._end = iter._end;
			this._typ = iter._typ;
			this._matchSelf = iter._matchSelf;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentKindDescendantIterator(this);
		}

		public override bool MoveNext()
		{
			if (this._matchSelf)
			{
				this._matchSelf = false;
				if (this.ctxt.IsKindMatch(this._typ))
				{
					this.pos++;
					return true;
				}
			}
			if (!this.ctxt.MoveToFollowing(this._typ, this._end))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private XPathDocumentNavigator _end;

		private XPathNodeType _typ;

		private bool _matchSelf;
	}
}
