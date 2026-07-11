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
			this.elementStk = new ClonableStack<XPathNavigator>();
			this.positionStk = new ClonableStack<int>();
			this.needInput = true;
		}

		private CacheChildrenQuery(CacheChildrenQuery other)
			: base(other)
		{
			this.nextInput = Query.Clone(other.nextInput);
			this.elementStk = other.elementStk.Clone();
			this.positionStk = other.positionStk.Clone();
			this.needInput = other.needInput;
		}

		public override void Reset()
		{
			this.nextInput = null;
			this.elementStk.Clear();
			this.positionStk.Clear();
			this.needInput = true;
			base.Reset();
		}

		public override XPathNavigator Advance()
		{
			for (;;)
			{
				if (this.needInput)
				{
					if (this.elementStk.Count == 0)
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
						this.currentNode = this.elementStk.Pop();
						this.position = this.positionStk.Pop();
						if (!this.DecideNextNode())
						{
							continue;
						}
					}
					this.needInput = false;
				}
				else if (!this.currentNode.MoveToNext() || !this.DecideNextNode())
				{
					this.needInput = true;
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
			this.nextInput = this.GetNextInput();
			if (this.nextInput != null && Query.CompareNodes(this.currentNode, this.nextInput) == XmlNodeOrder.After)
			{
				this.elementStk.Push(this.currentNode);
				this.positionStk.Push(this.position);
				this.currentNode = this.nextInput;
				this.nextInput = null;
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
			if (this.nextInput != null)
			{
				xpathNavigator = this.nextInput;
				this.nextInput = null;
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

		private XPathNavigator nextInput;

		private ClonableStack<XPathNavigator> elementStk;

		private ClonableStack<int> positionStk;

		private bool needInput;
	}
}
