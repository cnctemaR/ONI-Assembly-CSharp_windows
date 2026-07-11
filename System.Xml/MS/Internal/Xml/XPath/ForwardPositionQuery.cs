using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class ForwardPositionQuery : CacheOutputQuery
	{
		public ForwardPositionQuery(Query input)
			: base(input)
		{
		}

		protected ForwardPositionQuery(ForwardPositionQuery other)
			: base(other)
		{
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			base.Evaluate(context);
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this.input.Advance()) != null)
			{
				this.outputBuffer.Add(xpathNavigator.Clone());
			}
			return this;
		}

		public override XPathNavigator MatchNode(XPathNavigator context)
		{
			return this.input.MatchNode(context);
		}

		public override XPathNodeIterator Clone()
		{
			return new ForwardPositionQuery(this);
		}
	}
}
