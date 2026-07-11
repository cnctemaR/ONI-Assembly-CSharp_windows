using System;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class PrecedingQuery : BaseAxisQuery
	{
		public PrecedingQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
			: base(qyInput, name, prefix, typeTest)
		{
			this.ancestorStk = new ClonableStack<XPathNavigator>();
		}

		private PrecedingQuery(PrecedingQuery other)
			: base(other)
		{
			this.workIterator = Query.Clone(other.workIterator);
			this.ancestorStk = other.ancestorStk.Clone();
		}

		public override void Reset()
		{
			this.workIterator = null;
			this.ancestorStk.Clear();
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			if (this.workIterator == null)
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
					this.ancestorStk.Push(xpathNavigator2.Clone());
				}
				while (xpathNavigator2.MoveToParent());
				this.workIterator = xpathNavigator2.SelectDescendants(XPathNodeType.All, true);
			}
			while (this.workIterator.MoveNext())
			{
				this.currentNode = this.workIterator.Current;
				if (this.currentNode.IsSamePosition(this.ancestorStk.Peek()))
				{
					this.ancestorStk.Pop();
					if (this.ancestorStk.Count == 0)
					{
						this.currentNode = null;
						this.workIterator = null;
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

		private XPathNodeIterator workIterator;

		private ClonableStack<XPathNavigator> ancestorStk;
	}
}
