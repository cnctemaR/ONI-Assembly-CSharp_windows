using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal class XPathDocumentKindChildIterator : XPathDocumentBaseIterator
	{
		public XPathDocumentKindChildIterator(XPathDocumentNavigator parent, XPathNodeType typ)
			: base(parent)
		{
			this._typ = typ;
		}

		public XPathDocumentKindChildIterator(XPathDocumentKindChildIterator iter)
			: base(iter)
		{
			this._typ = iter._typ;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentKindChildIterator(this);
		}

		public override bool MoveNext()
		{
			if (this.pos == 0)
			{
				if (!this.ctxt.MoveToChild(this._typ))
				{
					return false;
				}
			}
			else if (!this.ctxt.MoveToNext(this._typ))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private XPathNodeType _typ;
	}
}
