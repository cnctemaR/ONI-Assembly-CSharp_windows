using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal abstract class DescendantBaseQuery : BaseAxisQuery
	{
		public DescendantBaseQuery(Query qyParent, string Name, string Prefix, XPathNodeType Type, bool matchSelf, bool abbrAxis)
			: base(qyParent, Name, Prefix, Type)
		{
			this.matchSelf = matchSelf;
			this.abbrAxis = abbrAxis;
		}

		public DescendantBaseQuery(DescendantBaseQuery other)
			: base(other)
		{
			this.matchSelf = other.matchSelf;
			this.abbrAxis = other.abbrAxis;
		}

		public override XPathNavigator MatchNode(XPathNavigator context)
		{
			if (context != null)
			{
				if (!this.abbrAxis)
				{
					throw XPathException.Create("'{0}' is an invalid XSLT pattern.");
				}
				if (this.matches(context))
				{
					XPathNavigator xpathNavigator;
					if (this.matchSelf && (xpathNavigator = this.qyInput.MatchNode(context)) != null)
					{
						return xpathNavigator;
					}
					XPathNavigator xpathNavigator2 = context.Clone();
					while (xpathNavigator2.MoveToParent())
					{
						if ((xpathNavigator = this.qyInput.MatchNode(xpathNavigator2)) != null)
						{
							return xpathNavigator;
						}
					}
				}
			}
			return null;
		}

		protected bool matchSelf;

		protected bool abbrAxis;
	}
}
