using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct UnionIterator
	{
		public void Create(XmlQueryRuntime runtime)
		{
			this.runtime = runtime;
			this.state = UnionIterator.IteratorState.InitLeft;
		}

		public SetIteratorResult MoveNext(XPathNavigator nestedNavigator)
		{
			switch (this.state)
			{
			case UnionIterator.IteratorState.InitLeft:
				this.navOther = nestedNavigator;
				this.state = UnionIterator.IteratorState.NeedRight;
				return SetIteratorResult.InitRightIterator;
			case UnionIterator.IteratorState.NeedLeft:
				this.navCurr = nestedNavigator;
				this.state = UnionIterator.IteratorState.LeftIsCurrent;
				break;
			case UnionIterator.IteratorState.NeedRight:
				this.navCurr = nestedNavigator;
				this.state = UnionIterator.IteratorState.RightIsCurrent;
				break;
			case UnionIterator.IteratorState.LeftIsCurrent:
				this.state = UnionIterator.IteratorState.NeedLeft;
				return SetIteratorResult.NeedLeftNode;
			case UnionIterator.IteratorState.RightIsCurrent:
				this.state = UnionIterator.IteratorState.NeedRight;
				return SetIteratorResult.NeedRightNode;
			}
			if (this.navCurr == null)
			{
				if (this.navOther == null)
				{
					return SetIteratorResult.NoMoreNodes;
				}
				this.Swap();
			}
			else if (this.navOther != null)
			{
				int num = this.runtime.ComparePosition(this.navOther, this.navCurr);
				if (num == 0)
				{
					if (this.state == UnionIterator.IteratorState.LeftIsCurrent)
					{
						this.state = UnionIterator.IteratorState.NeedLeft;
						return SetIteratorResult.NeedLeftNode;
					}
					this.state = UnionIterator.IteratorState.NeedRight;
					return SetIteratorResult.NeedRightNode;
				}
				else if (num < 0)
				{
					this.Swap();
				}
			}
			return SetIteratorResult.HaveCurrentNode;
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurr;
			}
		}

		private void Swap()
		{
			XPathNavigator xpathNavigator = this.navCurr;
			this.navCurr = this.navOther;
			this.navOther = xpathNavigator;
			if (this.state == UnionIterator.IteratorState.LeftIsCurrent)
			{
				this.state = UnionIterator.IteratorState.RightIsCurrent;
				return;
			}
			this.state = UnionIterator.IteratorState.LeftIsCurrent;
		}

		private XmlQueryRuntime runtime;

		private XPathNavigator navCurr;

		private XPathNavigator navOther;

		private UnionIterator.IteratorState state;

		private enum IteratorState
		{
			InitLeft,
			NeedLeft,
			NeedRight,
			LeftIsCurrent,
			RightIsCurrent
		}
	}
}
