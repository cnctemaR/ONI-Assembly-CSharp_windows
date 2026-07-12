using System;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class CacheChildrenQuery : ChildrenQuery
	{
		public CacheChildrenQuery(Query qyInput, string name, string prefix, XPathNodeType type)
			: base(qyInput, name, prefix, type)
		{
			this._elementStk = new ClonableStack<XPathNavigator>();
			this._positionStk = new ClonableStack<int>();
			this._needInput = true;
		}

		private CacheChildrenQuery(CacheChildrenQuery other)
			: base(other)
		{
			this._nextInput = Query.Clone(other._nextInput);
			this._elementStk = other._elementStk.Clone();
			this._positionStk = other._positionStk.Clone();
			this._needInput = other._needInput;
		}

		public override void Reset()
		{
			this._nextInput = null;
			this._elementStk.Clear();
			this._positionStk.Clear();
			this._needInput = true;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (this._needInput)
				{
					if (this._elementStk.Count == 0)
					{
						this.currentNode = this.GetNextInput();
						if (this.currentNode == null)
						{
							break;
						}
						if (!this.currentNode.MoveToFirstChild())
						{
							continue;
						}
						this.position = 0;
					}
					else
					{
						this.currentNode = this._elementStk.Pop();
						this.position = this._positionStk.Pop();
						if (!this.DecideNextNode())
						{
							continue;
						}
					}
					this._needInput = false;
				}
				else if (!this.currentNode.MoveToNext() || !this.DecideNextNode())
				{
					this._needInput = true;
					continue;
				}
				if (this.matches(this.currentNode))
				{
					goto Block_5;
				}
			}
			return null;
			Block_5:
			this.position++;
			return this.currentNode;
		}

		private bool DecideNextNode()
		{
			this._nextInput = this.GetNextInput();
			if (this._nextInput != null && Query.CompareNodes(this.currentNode, this._nextInput) == XmlNodeOrder.After)
			{
				this._elementStk.Push(this.currentNode);
				this._positionStk.Push(this.position);
				this.currentNode = this._nextInput;
				this._nextInput = null;
				if (!this.currentNode.MoveToFirstChild())
				{
					return false;
				}
				this.position = 0;
			}
			return true;
		}

		private XPathNavigator GetNextInput()
		{
			XPathNavigator xpathNavigator;
			if (this._nextInput != null)
			{
				xpathNavigator = this._nextInput;
				this._nextInput = null;
			}
			else
			{
				xpathNavigator = this.qyInput.Advance();
				if (xpathNavigator != null)
				{
					xpathNavigator = xpathNavigator.Clone();
				}
			}
			return xpathNavigator;
		}

		public override XPathNodeIterator Clone()
		{
			return new CacheChildrenQuery(this);
		}

		private XPathNavigator _nextInput;

		private ClonableStack<XPathNavigator> _elementStk;

		private ClonableStack<int> _positionStk;

		private bool _needInput;
	}
}
