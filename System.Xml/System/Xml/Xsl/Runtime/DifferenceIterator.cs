using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct DifferenceIterator
	{
		public void Create(XmlQueryRuntime runtime)
		{
			this.runtime = runtime;
			this.state = DifferenceIterator.IteratorState.InitLeft;
		}

		public SetIteratorResult MoveNext(XPathNavigator nestedNavigator)
		{
			switch (this.state)
			{
			case DifferenceIterator.IteratorState.InitLeft:
				this.navLeft = nestedNavigator;
				this.state = DifferenceIterator.IteratorState.NeedRight;
				return SetIteratorResult.InitRightIterator;
			case DifferenceIterator.IteratorState.NeedLeft:
				this.navLeft = nestedNavigator;
				break;
			case DifferenceIterator.IteratorState.NeedRight:
				this.navRight = nestedNavigator;
				break;
			case DifferenceIterator.IteratorState.NeedLeftAndRight:
				this.navLeft = nestedNavigator;
				this.state = DifferenceIterator.IteratorState.NeedRight;
				return SetIteratorResult.NeedRightNode;
			case DifferenceIterator.IteratorState.HaveCurrent:
				this.state = DifferenceIterator.IteratorState.NeedLeft;
				return SetIteratorResult.NeedLeftNode;
			}
			if (this.navLeft == null)
			{
				return SetIteratorResult.NoMoreNodes;
			}
			if (this.navRight != null)
			{
				int num = this.runtime.ComparePosition(this.navLeft, this.navRight);
				if (num == 0)
				{
					this.state = DifferenceIterator.IteratorState.NeedLeftAndRight;
					return SetIteratorResult.NeedLeftNode;
				}
				if (num > 0)
				{
					this.state = DifferenceIterator.IteratorState.NeedRight;
					return SetIteratorResult.NeedRightNode;
				}
			}
			this.state = DifferenceIterator.IteratorState.HaveCurrent;
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

		private DifferenceIterator.IteratorState state;

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
