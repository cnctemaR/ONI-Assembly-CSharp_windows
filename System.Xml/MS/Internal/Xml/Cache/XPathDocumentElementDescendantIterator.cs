using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal class XPathDocumentElementDescendantIterator : XPathDocumentBaseIterator
	{
		public XPathDocumentElementDescendantIterator(XPathDocumentNavigator root, string name, string namespaceURI, bool matchSelf)
			: base(root)
		{
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			this._localName = root.NameTable.Get(name);
			this._namespaceUri = namespaceURI;
			this._matchSelf = matchSelf;
			if (root.NodeType != XPathNodeType.Root)
			{
				this._end = new XPathDocumentNavigator(root);
				this._end.MoveToNonDescendant();
			}
		}

		public XPathDocumentElementDescendantIterator(XPathDocumentElementDescendantIterator iter)
			: base(iter)
		{
			this._end = iter._end;
			this._localName = iter._localName;
			this._namespaceUri = iter._namespaceUri;
			this._matchSelf = iter._matchSelf;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentElementDescendantIterator(this);
		}

		public override bool MoveNext()
		{
			if (this._matchSelf)
			{
				this._matchSelf = false;
				if (this.ctxt.IsElementMatch(this._localName, this._namespaceUri))
				{
					this.pos++;
					return true;
				}
			}
			if (!this.ctxt.MoveToFollowing(this._localName, this._namespaceUri, this._end))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private XPathDocumentNavigator _end;

		private string _localName;

		private string _namespaceUri;

		private bool _matchSelf;
	}
}
