using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class ParentQuery : CacheAxisQuery
	{
		public ParentQuery(Query qyInput, string Name, string Prefix, XPathNodeType Type)
			: base(qyInput, Name, Prefix, Type)
		{
		}

		private ParentQuery(ParentQuery other)
			: base(other)
		{
		}

		public override object Evaluate(XPathNodeIterator context)
		{
			base.Evaluate(context);
			XPathNavigator xpathNavigator;
			while ((xpathNavigator = this.qyInput.Advance()) != null)
			{
				xpathNavigator = xpathNavigator.Clone();
				if (xpathNavigator.MoveToParent() && this.matches(xpathNavigator))
				{
					base.Insert(this.outputBuffer, xpathNavigator);
				}
			}
			return this;
		}

		public override XPathNodeIterator Clone()
		{
			return new ParentQuery(this);
		}
	}
}
