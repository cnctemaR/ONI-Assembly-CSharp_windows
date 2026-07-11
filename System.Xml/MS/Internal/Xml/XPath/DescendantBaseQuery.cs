using System;
using System.Xml;
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

		public override void PrintQuery(XmlWriter w)
		{
			w.WriteStartElement(base.GetType().Name);
			if (this.matchSelf)
			{
				w.WriteAttributeString("self", "yes");
			}
			if (base.NameTest)
			{
				w.WriteAttributeString("name", (base.Prefix.Length != 0) ? (base.Prefix + ":" + base.Name) : base.Name);
			}
			if (base.TypeTest != XPathNodeType.Element)
			{
				w.WriteAttributeString("nodeType", base.TypeTest.ToString());
			}
			this.qyInput.PrintQuery(w);
			w.WriteEndElement();
		}

		protected bool matchSelf;

		protected bool abbrAxis;
	}
}
