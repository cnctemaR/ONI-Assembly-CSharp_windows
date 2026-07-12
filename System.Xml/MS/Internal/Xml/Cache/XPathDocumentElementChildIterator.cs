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
			this._localName = parent.NameTable.Get(name);
			this._namespaceUri = namespaceURI;
		}

		public XPathDocumentElementChildIterator(XPathDocumentElementChildIterator iter)
			: base(iter)
		{
			this._localName = iter._localName;
			this._namespaceUri = iter._namespaceUri;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentElementChildIterator(this);
		}

		public override bool MoveNext()
		{
			if (this.pos == 0)
			{
				if (!this.ctxt.MoveToChild(this._localName, this._namespaceUri))
				{
					return false;
				}
			}
			else if (!this.ctxt.MoveToNext(this._localName, this._namespaceUri))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private string _localName;

		private string _namespaceUri;
	}
}
