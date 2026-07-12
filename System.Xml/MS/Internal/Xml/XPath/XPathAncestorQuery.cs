using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathAncestorQuery : CacheAxisQuery
	{
		public XPathAncestorQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest, bool matchSelf)
			: base(qyInput, name, prefix, typeTest)
		{
			this._matchSelf = matchSelf;
		}

		private XPathAncestorQuery(XPathAncestorQuery other)
			: base(other)
		{
			this._matchSelf = other._matchSelf;
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			base.Evaluate(context);
			XPathNavigator xpathNavigator = null;
			XPathNavigator xpathNavigator2;
			while ((xpathNavigator2 = this.qyInput.Advance()) != null)
			{
				if (!this._matchSelf || !this.matches(xpathNavigator2) || Query.Insert(this.outputBuffer, xpathNavigator2))
				{
					if (xpathNavigator == null || !xpathNavigator.MoveTo(xpathNavigator2))
					{
						xpathNavigator = xpathNavigator2.Clone();
					}
					while (xpathNavigator.MoveToParent() && (!this.matches(xpathNavigator) || Query.Insert(this.outputBuffer, xpathNavigator)))
					{
					}
				}
			}
			return this;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathAncestorQuery(this);
		}

		public override int CurrentPosition
		{
			get
			{
				return this.outputBuffer.Count - this.count + 1;
			}
		}

		public override QueryProps Properties
		{
			get
			{
				return base.Properties | QueryProps.Reverse;
			}
		}

		private bool _matchSelf;
	}
}
