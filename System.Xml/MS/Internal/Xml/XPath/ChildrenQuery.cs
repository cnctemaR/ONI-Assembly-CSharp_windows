using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal class ChildrenQuery : BaseAxisQuery
	{
		public ChildrenQuery(Query qyInput, string name, string prefix, XPathNodeType type)
			: base(qyInput, name, prefix, type)
		{
		}

		protected ChildrenQuery(ChildrenQuery other)
			: base(other)
		{
			this.iterator = Query.Clone(other.iterator);
		}

		public override void Reset()
		{
			this.iterator = XPathEmptyIterator.Instance;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			while (!this.iterator.MoveNext())
			{
				XPathNavigator xpathNavigator = this.qyInput.Advance();
				if (xpathNavigator == null)
				{
					return null;
				}
				if (base.NameTest)
				{
					if (base.TypeTest == XPathNodeType.ProcessingInstruction)
					{
						this.iterator = new IteratorFilter(xpathNavigator.SelectChildren(base.TypeTest), base.Name);
					}
					else
					{
						this.iterator = xpathNavigator.SelectChildren(base.Name, base.Namespace);
					}
				}
				else
				{
					this.iterator = xpathNavigator.SelectChildren(base.TypeTest);
				}
				this.position = 0;
			}
			this.position++;
			this.currentNode = this.iterator.Current;
			return this.currentNode;
		}

		public sealed override XPathNavigator MatchNode(XPathNavigator context)
		{
			if (context == null || !this.matches(context))
			{
				return null;
			}
			XPathNavigator xpathNavigator = context.Clone();
			if (xpathNavigator.NodeType != XPathNodeType.Attribute && xpathNavigator.MoveToParent())
			{
				return this.qyInput.MatchNode(xpathNavigator);
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new ChildrenQuery(this);
		}

		private XPathNodeIterator iterator = XPathEmptyIterator.Instance;
	}
}
