using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class FollowingQuery : BaseAxisQuery
	{
		public FollowingQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
			: base(qyInput, name, prefix, typeTest)
		{
		}

		private FollowingQuery(FollowingQuery other)
			: base(other)
		{
			this.input = Query.Clone(other.input);
			this.iterator = Query.Clone(other.iterator);
		}

		public override void Reset()
		{
			this.iterator = null;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			if (this.iterator == null)
			{
				this.input = this.qyInput.Advance();
				if (this.input == null)
				{
					return null;
				}
				XPathNavigator xpathNavigator;
				do
				{
					xpathNavigator = this.input.Clone();
					this.input = this.qyInput.Advance();
				}
				while (xpathNavigator.IsDescendant(this.input));
				this.input = xpathNavigator;
				this.iterator = XPathEmptyIterator.Instance;
			}
			while (!this.iterator.MoveNext())
			{
				bool flag;
				if (this.input.NodeType == XPathNodeType.Attribute || this.input.NodeType == XPathNodeType.Namespace)
				{
					this.input.MoveToParent();
					flag = false;
				}
				else
				{
					while (!this.input.MoveToNext())
					{
						if (!this.input.MoveToParent())
						{
							return null;
						}
					}
					flag = true;
				}
				if (base.NameTest)
				{
					this.iterator = this.input.SelectDescendants(base.Name, base.Namespace, flag);
				}
				else
				{
					this.iterator = this.input.SelectDescendants(base.TypeTest, flag);
				}
			}
			this.position++;
			this.currentNode = this.iterator.Current;
			return this.currentNode;
		}

		public override XPathNodeIterator Clone()
		{
			return new FollowingQuery(this);
		}

		private XPathNavigator input;

		private XPathNodeIterator iterator;
	}
}
