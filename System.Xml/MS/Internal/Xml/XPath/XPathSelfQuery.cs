using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathSelfQuery : BaseAxisQuery
	{
		public XPathSelfQuery(Query qyInput, string Name, string Prefix, XPathNodeType Type)
			: base(qyInput, Name, Prefix, Type)
		{
		}

		private XPathSelfQuery(XPathSelfQuery other)
			: base(other)
		{
		}

		public override XPathNavigator Advance()
		{
			while ((this.currentNode = this.qyInput.Advance()) != null)
			{
				if (this.matches(this.currentNode))
				{
					this.position = 1;
					return this.currentNode;
				}
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathSelfQuery(this);
		}
	}
}
