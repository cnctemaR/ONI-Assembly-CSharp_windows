using System;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathAncestorQuery : CacheAxisQuery
	{
		public XPathAncestorQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest, bool matchSelf)
			: base(qyInput, name, prefix, typeTest)
		{
			this.matchSelf = matchSelf;
		}

		private XPathAncestorQuery(XPathAncestorQuery other)
			: base(other)
		{
			this.matchSelf = other.matchSelf;
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			base.Evaluate(context);
			XPathNavigator xpathNavigator = null;
			XPathNavigator xpathNavigator2;
			while ((xpathNavigator2 = this.qyInput.Advance()) != null)
			{
				if (!this.matchSelf || !this.matches(xpathNavigator2) || base.Insert(this.outputBuffer, xpathNavigator2))
				{
					if (xpathNavigator == null || !xpathNavigator.MoveTo(xpathNavigator2))
					{
						xpathNavigator = xpathNavigator2.Clone();
					}
					while (xpathNavigator.MoveToParent() && (!this.matches(xpathNavigator) || base.Insert(this.outputBuffer, xpathNavigator)))
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

		private bool matchSelf;
	}
}
