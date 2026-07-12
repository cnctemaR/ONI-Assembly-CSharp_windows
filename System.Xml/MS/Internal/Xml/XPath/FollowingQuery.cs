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
			this._input = Query.Clone(other._input);
			this._iterator = Query.Clone(other._iterator);
		}

		public override void Reset()
		{
			this._iterator = null;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			if (this._iterator == null)
			{
				this._input = this.qyInput.Advance();
				if (this._input == null)
				{
					return null;
				}
				XPathNavigator xpathNavigator;
				do
				{
					xpathNavigator = this._input.Clone();
					this._input = this.qyInput.Advance();
				}
				while (xpathNavigator.IsDescendant(this._input));
				this._input = xpathNavigator;
				this._iterator = XPathEmptyIterator.Instance;
			}
			while (!this._iterator.MoveNext())
			{
				bool flag;
				if (this._input.NodeType == XPathNodeType.Attribute || this._input.NodeType == XPathNodeType.Namespace)
				{
					this._input.MoveToParent();
					flag = false;
				}
				else
				{
					while (!this._input.MoveToNext())
					{
						if (!this._input.MoveToParent())
						{
							return null;
						}
					}
					flag = true;
				}
				if (base.NameTest)
				{
					this._iterator = this._input.SelectDescendants(base.Name, base.Namespace, flag);
				}
				else
				{
					this._iterator = this._input.SelectDescendants(base.TypeTest, flag);
				}
			}
			this.position++;
			this.currentNode = this._iterator.Current;
			return this.currentNode;
		}

		public override XPathNodeIterator Clone()
		{
			return new FollowingQuery(this);
		}

		private XPathNavigator _input;

		private XPathNodeIterator _iterator;
	}
}
