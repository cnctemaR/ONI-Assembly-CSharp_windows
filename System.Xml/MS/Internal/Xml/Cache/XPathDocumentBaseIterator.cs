using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.Cache
{
	internal abstract class XPathDocumentBaseIterator : XPathNodeIterator
	{
		protected XPathDocumentBaseIterator(XPathDocumentNavigator ctxt)
		{
			this.ctxt = new XPathDocumentNavigator(ctxt);
		}

		protected XPathDocumentBaseIterator(XPathDocumentBaseIterator iter)
		{
			this.ctxt = new XPathDocumentNavigator(iter.ctxt);
			this.pos = iter.pos;
		}

		public override XPathNavigator Current
		{
			get
			{
				return this.ctxt;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return this.pos;
			}
		}

		protected XPathDocumentNavigator ctxt;

		protected int pos;
	}
}
