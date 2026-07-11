using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class AttributeQuery : BaseAxisQuery
	{
		public AttributeQuery(Query qyParent, string Name, string Prefix, XPathNodeType Type)
			: base(qyParent, Name, Prefix, Type)
		{
		}

		private AttributeQuery(AttributeQuery other)
			: base(other)
		{
			this.onAttribute = other.onAttribute;
		}

		public override void Reset()
		{
			this.onAttribute = false;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (!this.onAttribute)
				{
					this.currentNode = this.qyInput.Advance();
					if (this.currentNode == null)
					{
						break;
					}
					this.position = 0;
					this.currentNode = this.currentNode.Clone();
					this.onAttribute = this.currentNode.MoveToFirstAttribute();
				}
				else
				{
					this.onAttribute = this.currentNode.MoveToNextAttribute();
				}
				if (this.onAttribute && this.matches(this.currentNode))
				{
					goto Block_3;
				}
			}
			return null;
			Block_3:
			this.position++;
			return this.currentNode;
		}

		public override XPathNavigator MatchNode(XPathNavigator context)
		{
			if (context != null && context.NodeType == XPathNodeType.Attribute && this.matches(context))
			{
				XPathNavigator xpathNavigator = context.Clone();
				if (xpathNavigator.MoveToParent())
				{
					return this.qyInput.MatchNode(xpathNavigator);
				}
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new AttributeQuery(this);
		}

		private bool onAttribute;
	}
}
