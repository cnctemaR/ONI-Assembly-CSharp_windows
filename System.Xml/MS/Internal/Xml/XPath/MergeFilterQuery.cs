using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class MergeFilterQuery : CacheOutputQuery
	{
		public MergeFilterQuery(Query input, Query child)
			: base(input)
		{
			this._child = child;
		}

		private MergeFilterQuery(MergeFilterQuery other)
			: base(other)
		{
			this._child = Query.Clone(other._child);
		}

		public override void SetXsltContext(XsltContext xsltContext)
		{
			base.SetXsltContext(xsltContext);
			this._child.SetXsltContext(xsltContext);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			base.Evaluate(nodeIterator);
			while (this.input.Advance() != null)
			{
				this._child.Evaluate(this.input);
				XPathNavigator xpathNavigator;
				while ((xpathNavigator = this._child.Advance()) != null)
				{
					Query.Insert(this.outputBuffer, xpathNavigator);
				}
			}
			return this;
		}

		public override XPathNavigator MatchNode(XPathNavigator current)
		{
			XPathNavigator xpathNavigator = this._child.MatchNode(current);
			if (xpathNavigator == null)
			{
				return null;
			}
			xpathNavigator = this.input.MatchNode(xpathNavigator);
			if (xpathNavigator == null)
			{
				return null;
			}
			this.Evaluate(new XPathSingletonIterator(xpathNavigator.Clone(), true));
			for (XPathNavigator xpathNavigator2 = this.Advance(); xpathNavigator2 != null; xpathNavigator2 = this.Advance())
			{
				if (xpathNavigator2.IsSamePosition(current))
				{
					return xpathNavigator;
				}
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new MergeFilterQuery(this);
		}

		private Query _child;
	}
}
