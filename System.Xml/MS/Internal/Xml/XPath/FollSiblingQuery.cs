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
			this.elementStk = new ClonableStack<XPathNavigator>();
			this.parentStk = new List<XPathNavigator>();
		}

		private FollSiblingQuery(FollSiblingQuery other)
			: base(other)
		{
			this.elementStk = other.elementStk.Clone();
			this.parentStk = new List<XPathNavigator>(other.parentStk);
			this.nextInput = Query.Clone(other.nextInput);
		}

		public override void Reset()
		{
			this.elementStk.Clear();
			this.parentStk.Clear();
			this.nextInput = null;
			base.Reset();
		}

		private bool Visited(XPathNavigator nav)
		{
			XPathNavigator xpathNavigator = nav.Clone();
			xpathNavigator.MoveToParent();
			for (int i = 0; i < this.parentStk.Count; i++)
			{
				if (xpathNavigator.IsSamePosition(this.parentStk[i]))
				{
					return true;
				}
			}
			this.parentStk.Add(xpathNavigator);
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
					if (this.nextInput == null)
					{
						this.nextInput = this.FetchInput();
					}
					if (this.elementStk.Count == 0)
					{
						if (this.nextInput == null)
						{
							break;
						}
						this.currentNode = this.nextInput;
						this.nextInput = this.FetchInput();
					}
					else
					{
						this.currentNode = this.elementStk.Pop();
					}
				}
				while (this.currentNode.IsDescendant(this.nextInput))
				{
					this.elementStk.Push(this.currentNode);
					this.currentNode = this.nextInput;
					this.nextInput = this.qyInput.Advance();
					if (this.nextInput != null)
					{
						this.nextInput = this.nextInput.Clone();
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

		private ClonableStack<XPathNavigator> elementStk;

		private List<XPathNavigator> parentStk;

		private XPathNavigator nextInput;
	}
}
