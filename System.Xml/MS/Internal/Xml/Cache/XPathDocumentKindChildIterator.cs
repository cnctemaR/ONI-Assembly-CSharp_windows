using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal class XPathDocumentKindChildIterator : XPathDocumentBaseIterator
	{
		public XPathDocumentKindChildIterator(XPathDocumentNavigator parent, XPathNodeType typ)
			: base(parent)
		{
			this.typ = typ;
		}

		public XPathDocumentKindChildIterator(XPathDocumentKindChildIterator iter)
			: base(iter)
		{
			this.typ = iter.typ;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathDocumentKindChildIterator(this);
		}

		public override bool MoveNext()
		{
			if (this.pos == 0)
			{
				if (!this.ctxt.MoveToChild(this.typ))
				{
					return false;
				}
			}
			else if (!this.ctxt.MoveToNext(this.typ))
			{
				return false;
			}
			this.pos++;
			return true;
		}

		private XPathNodeType typ;
	}
}
