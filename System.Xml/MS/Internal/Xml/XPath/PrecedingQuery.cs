using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class PrecedingQuery : BaseAxisQuery
	{
		public PrecedingQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
			: base(qyInput, name, prefix, typeTest)
		{
			this._ancestorStk = new ClonableStack<XPathNavigator>();
		}

		private PrecedingQuery(PrecedingQuery other)
			: base(other)
		{
			this._workIterator = Query.Clone(other._workIterator);
			this._ancestorStk = other._ancestorStk.Clone();
		}

		public override void Reset()
		{
			this._workIterator = null;
			this._ancestorStk.Clear();
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			if (this._workIterator == null)
			{
				XPathNavigator xpathNavigator = this.qyInput.Advance();
				if (xpathNavigator == null)
				{
					return null;
				}
				XPathNavigator xpathNavigator2 = xpathNavigator.Clone();
				do
				{
					xpathNavigator2.MoveTo(xpathNavigator);
				}
				while ((xpathNavigator = this.qyInput.Advance()) != null);
				if (xpathNavigator2.NodeType == XPathNodeType.Attribute || xpathNavigator2.NodeType == XPathNodeType.Namespace)
				{
					xpathNavigator2.MoveToParent();
				}
				do
				{
					this._ancestorStk.Push(xpathNavigator2.Clone());
				}
				while (xpathNavigator2.MoveToParent());
				this._workIterator = xpathNavigator2.SelectDescendants(XPathNodeType.All, true);
			}
			while (this._workIterator.MoveNext())
			{
				this.currentNode = this._workIterator.Current;
				if (this.currentNode.IsSamePosition(this._ancestorStk.Peek()))
				{
					this._ancestorStk.Pop();
					if (this._ancestorStk.Count == 0)
					{
						this.currentNode = null;
						this._workIterator = null;
						return null;
					}
				}
				else if (this.matches(this.currentNode))
				{
					this.position++;
					return this.currentNode;
				}
			}
			return null;
		}

		public override XPathNodeIterator Clone()
		{
			return new PrecedingQuery(this);
		}

		public override QueryProps Properties
		{
			get
			{
				return base.Properties | QueryProps.Reverse;
			}
		}

		private XPathNodeIterator _workIterator;

		private ClonableStack<XPathNavigator> _ancestorStk;
	}
}
