using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class FollSiblingQuery : BaseAxisQuery
	{
		public FollSiblingQuery(Query qyInput, string name, string prefix, XPathNodeType type)
			: base(qyInput, name, prefix, type)
		{
			this._elementStk = new ClonableStack<XPathNavigator>();
			this._parentStk = new List<XPathNavigator>();
		}

		private FollSiblingQuery(FollSiblingQuery other)
			: base(other)
		{
			this._elementStk = other._elementStk.Clone();
			this._parentStk = new List<XPathNavigator>(other._parentStk);
			this._nextInput = Query.Clone(other._nextInput);
		}

		public override void Reset()
		{
			this._elementStk.Clear();
			this._parentStk.Clear();
			this._nextInput = null;
			base.Reset();
		}

		private bool Visited(XPathNavigator nav)
		{
			XPathNavigator xpathNavigator = nav.Clone();
			xpathNavigator.MoveToParent();
			for (int i = 0; i < this._parentStk.Count; i++)
			{
				if (xpathNavigator.IsSamePosition(this._parentStk[i]))
				{
					return true;
				}
			}
			this._parentStk.Add(xpathNavigator);
			return false;
		}

		private XPathNavigator FetchInput()
		{
			XPathNavigator xpathNavigator;
			for (;;)
			{
				xpathNavigator = this.qyInput.Advance();
				if (xpathNavigator == null)
				{
					break;
				}
				if (!this.Visited(xpathNavigator))
				{
					goto Block_1;
				}
			}
			return null;
			Block_1:
			return xpathNavigator.Clone();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (this.currentNode == null)
				{
					if (this._nextInput == null)
					{
						this._nextInput = this.FetchInput();
					}
					if (this._elementStk.Count == 0)
					{
						if (this._nextInput == null)
						{
							break;
						}
						this.currentNode = this._nextInput;
						this._nextInput = this.FetchInput();
					}
					else
					{
						this.currentNode = this._elementStk.Pop();
					}
				}
				while (this.currentNode.IsDescendant(this._nextInput))
				{
					this._elementStk.Push(this.currentNode);
					this.currentNode = this._nextInput;
					this._nextInput = this.qyInput.Advance();
					if (this._nextInput != null)
					{
						this._nextInput = this._nextInput.Clone();
					}
				}
				while (this.currentNode.MoveToNext())
				{
					if (this.matches(this.currentNode))
					{
						goto Block_6;
					}
				}
				this.currentNode = null;
			}
			return null;
			Block_6:
			this.position++;
			return this.currentNode;
		}

		public override XPathNodeIterator Clone()
		{
			return new FollSiblingQuery(this);
		}

		private ClonableStack<XPathNavigator> _elementStk;

		private List<XPathNavigator> _parentStk;

		private XPathNavigator _nextInput;
	}
}
