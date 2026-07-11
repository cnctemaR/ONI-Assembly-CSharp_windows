using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal class XPathDocumentElementChildIterator : XPathDocumentBaseIterator
	{
		public XPathDocumentElementChildIterator(XPathDocumentNavigator parent, string name, string namespaceURI)
			: base(parent)
		{
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			this.localName = parent.NameTable.Get(name);
			this.namespaceUri = namespaceURI;
		}

		public XPathDocumentElementChildIterator(XPathDocumentElementChildIterator iter)
			: base(iter)
		{
			this.localName = iter.localName;
			this.namespaceUri = iter.namespaceUri;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentElementChildIterator(this);
		}

		public override bool MoveNext()
		{
			if (this.pos == 0)
			{
				if (!this.ctxt.MoveToChild(this.localName, this.namespaceUri))
				{
					return false;
				}
			}
			else if (!this.ctxt.MoveToNext(this.localName, this.namespaceUri))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private string localName;

		private string namespaceUri;
	}
}
