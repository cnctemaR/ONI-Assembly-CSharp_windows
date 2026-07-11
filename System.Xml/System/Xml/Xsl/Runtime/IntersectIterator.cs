using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct IntersectIterator
	{
		public void Create(XmlQueryRuntime runtime)
		{
			this.runtime = runtime;
			this.state = IntersectIterator.IteratorState.InitLeft;
		}

		public SetIteratorResult MoveNext(XPathNavigator nestedNavigator)
		{
			switch (this.state)
			{
			case IntersectIterator.IteratorState.InitLeft:
				this.navLeft = nestedNavigator;
				this.state = IntersectIterator.IteratorState.NeedRight;
				return SetIteratorResult.InitRightIterator;
			case IntersectIterator.IteratorState.NeedLeft:
				this.navLeft = nestedNavigator;
				break;
			case IntersectIterator.IteratorState.NeedRight:
				this.navRight = nestedNavigator;
				break;
			case IntersectIterator.IteratorState.NeedLeftAndRight:
				this.navLeft = nestedNavigator;
				this.state = IntersectIterator.IteratorState.NeedRight;
				return SetIteratorResult.NeedRightNode;
			case IntersectIterator.IteratorState.HaveCurrent:
				this.state = IntersectIterator.IteratorState.NeedLeftAndRight;
				return SetIteratorResult.NeedLeftNode;
			}
			if (this.navLeft == null || this.navRight == null)
			{
				return SetIteratorResult.NoMoreNodes;
			}
			int num = this.runtime.ComparePosition(this.navLeft, this.navRight);
			if (num < 0)
			{
				this.state = IntersectIterator.IteratorState.NeedLeft;
				return SetIteratorResult.NeedLeftNode;
			}
			if (num > 0)
			{
				this.state = IntersectIterator.IteratorState.NeedRight;
				return SetIteratorResult.NeedRightNode;
			}
			this.state = IntersectIterator.IteratorState.HaveCurrent;
			return SetIteratorResult.HaveCurrentNode;
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navLeft;
			}
		}

		private XmlQueryRuntime runtime;

		private XPathNavigator navLeft;

		private XPathNavigator navRight;

		private IntersectIterator.IteratorState state;

		private enum IteratorState
		{
			InitLeft,
			NeedLeft,
			NeedRight,
			NeedLeftAndRight,
			HaveCurrent
		}
	}
}
