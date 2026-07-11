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
			this.localName = root.NameTable.Get(name);
			this.namespaceUri = namespaceURI;
			this.matchSelf = matchSelf;
			if (root.NodeType != XPathNodeType.Root)
			{
				this.end = new XPathDocumentNavigator(root);
				this.end.MoveToNonDescendant();
			}
		}

		public XPathDocumentElementDescendantIterator(XPathDocumentElementDescendantIterator iter)
			: base(iter)
		{
			this.end = iter.end;
			this.localName = iter.localName;
			this.namespaceUri = iter.namespaceUri;
			this.matchSelf = iter.matchSelf;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentElementDescendantIterator(this);
		}

		public override bool MoveNext()
		{
			if (this.matchSelf)
			{
				this.matchSelf = false;
				if (this.ctxt.IsElementMatch(this.localName, this.namespaceUri))
				{
					this.pos++;
					return true;
				}
			}
			if (!this.ctxt.MoveToFollowing(this.localName, this.namespaceUri, this.end))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private XPathDocumentNavigator end;

		private string localName;

		private string namespaceUri;

		private bool matchSelf;
	}
}
