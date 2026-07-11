using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct XPathPrecedingMergeIterator
	{
		public void Create(XmlNavigatorFilter filter)
		{
			this.filter = filter;
			this.state = XPathPrecedingMergeIterator.IteratorState.NeedCandidateCurrent;
		}

		public IteratorResult MoveNext(XPathNavigator input)
		{
			XPathPrecedingMergeIterator.IteratorState iteratorState = this.state;
			if (iteratorState != XPathPrecedingMergeIterator.IteratorState.NeedCandidateCurrent)
			{
				if (iteratorState == XPathPrecedingMergeIterator.IteratorState.HaveCandidateCurrent)
				{
					if (input == null)
					{
						this.state = XPathPrecedingMergeIterator.IteratorState.HaveCurrentNoNext;
					}
					else
					{
						if (this.navCurrent.ComparePosition(input) != XmlNodeOrder.Unknown)
						{
							this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
							return IteratorResult.NeedInputNode;
						}
						this.navNext = XmlQueryRuntime.SyncToNavigator(this.navNext, input);
						this.state = XPathPrecedingMergeIterator.IteratorState.HaveCurrentHaveNext;
					}
					this.PushAncestors();
				}
				if (!this.navStack.IsEmpty)
				{
					while (!this.filter.MoveToFollowing(this.navCurrent, this.navStack.Peek()))
					{
						this.navCurrent.MoveTo(this.navStack.Pop());
						if (this.navStack.IsEmpty)
						{
							goto IL_00CF;
						}
					}
					return IteratorResult.HaveCurrentNode;
				}
				IL_00CF:
				if (this.state == XPathPrecedingMergeIterator.IteratorState.HaveCurrentNoNext)
				{
					this.state = XPathPrecedingMergeIterator.IteratorState.NeedCandidateCurrent;
					return IteratorResult.NoMoreNodes;
				}
				this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, this.navNext);
				this.state = XPathPrecedingMergeIterator.IteratorState.HaveCandidateCurrent;
				return IteratorResult.HaveCurrentNode;
			}
			else
			{
				if (input == null)
				{
					return IteratorResult.NoMoreNodes;
				}
				this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
				this.state = XPathPrecedingMergeIterator.IteratorState.HaveCandidateCurrent;
				return IteratorResult.NeedInputNode;
			}
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private void PushAncestors()
		{
			this.navStack.Reset();
			do
			{
				this.navStack.Push(this.navCurrent.Clone());
			}
			while (this.navCurrent.MoveToParent());
			this.navStack.Pop();
		}

		private XmlNavigatorFilter filter;

		private XPathPrecedingMergeIterator.IteratorState state;

		private XPathNavigator navCurrent;

		private XPathNavigator navNext;

		private XmlNavigatorStack navStack;

		private enum IteratorState
		{
			NeedCandidateCurrent,
			HaveCandidateCurrent,
			HaveCurrentHaveNext,
			HaveCurrentNoNext
		}
	}
}
