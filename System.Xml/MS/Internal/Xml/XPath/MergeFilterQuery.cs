using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace MS.Internal.Xml.XPath
{
	internal sealed class MergeFilterQuery : CacheOutputQuery
	{
		public MergeFilterQuery(Query input, Query child)
			: base(input)
		{
			this.child = child;
		}

		private MergeFilterQuery(MergeFilterQuery other)
			: base(other)
		{
			this.child = Query.Clone(other.child);
		}

		public override void SetXsltContext(XsltContext xsltContext)
		{
			base.SetXsltContext(xsltContext);
			this.child.SetXsltContext(xsltContext);
		}

		public override object Evaluate(XPathNodeIterator nodeIterator)
		{
			base.Evaluate(nodeIterator);
			while (this.input.Advance() != null)
			{
				this.child.Evaluate(this.input);
				XPathNavigator xpathNavigator;
				while ((xpathNavigator = this.child.Advance()) != null)
				{
					base.Insert(this.outputBuffer, xpathNavigator);
				}
			}
			return this;
		}

		public override XPathNavigator MatchNode(XPathNavigator current)
		{
			XPathNavigator xpathNavigator = this.child.MatchNode(current);
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

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			this.input.PrintQuery(w);
			this.child.PrintQuery(w);
			w.WriteEndElement();
		}

		private Query child;
	}
}
