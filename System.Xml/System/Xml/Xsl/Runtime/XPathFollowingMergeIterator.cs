using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct XPathFollowingMergeIterator
	{
		public void Create(XmlNavigatorFilter filter)
		{
			this.filter = filter;
			this.state = XPathFollowingMergeIterator.IteratorState.NeedCandidateCurrent;
		}

		public IteratorResult MoveNext(XPathNavigator input)
		{
			switch (this.state)
			{
			case XPathFollowingMergeIterator.IteratorState.NeedCandidateCurrent:
				break;
			case XPathFollowingMergeIterator.IteratorState.HaveCandidateCurrent:
				if (input == null)
				{
					this.state = XPathFollowingMergeIterator.IteratorState.HaveCurrentNoNext;
					return this.MoveFirst();
				}
				if (!this.navCurrent.IsDescendant(input))
				{
					this.state = XPathFollowingMergeIterator.IteratorState.HaveCurrentNeedNext;
					goto IL_0064;
				}
				break;
			case XPathFollowingMergeIterator.IteratorState.HaveCurrentNeedNext:
				goto IL_0064;
			default:
				if (!this.filter.MoveToFollowing(this.navCurrent, null))
				{
					return this.MoveFailed();
				}
				return IteratorResult.HaveCurrentNode;
			}
			if (input == null)
			{
				return IteratorResult.NoMoreNodes;
			}
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
			this.state = XPathFollowingMergeIterator.IteratorState.HaveCandidateCurrent;
			return IteratorResult.NeedInputNode;
			IL_0064:
			if (input == null)
			{
				this.state = XPathFollowingMergeIterator.IteratorState.HaveCurrentNoNext;
				return this.MoveFirst();
			}
			if (this.navCurrent.ComparePosition(input) != XmlNodeOrder.Unknown)
			{
				return IteratorResult.NeedInputNode;
			}
			this.navNext = XmlQueryRuntime.SyncToNavigator(this.navNext, input);
			this.state = XPathFollowingMergeIterator.IteratorState.HaveCurrentHaveNext;
			return this.MoveFirst();
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private IteratorResult MoveFailed()
		{
			if (this.state == XPathFollowingMergeIterator.IteratorState.HaveCurrentNoNext)
			{
				this.state = XPathFollowingMergeIterator.IteratorState.NeedCandidateCurrent;
				return IteratorResult.NoMoreNodes;
			}
			this.state = XPathFollowingMergeIterator.IteratorState.HaveCandidateCurrent;
			XPathNavigator xpathNavigator = this.navCurrent;
			this.navCurrent = this.navNext;
			this.navNext = xpathNavigator;
			return IteratorResult.NeedInputNode;
		}

		private IteratorResult MoveFirst()
		{
			if (!XPathFollowingIterator.MoveFirst(this.filter, this.navCurrent))
			{
				return this.MoveFailed();
			}
			return IteratorResult.HaveCurrentNode;
		}

		private XmlNavigatorFilter filter;

		private XPathFollowingMergeIterator.IteratorState state;

		private XPathNavigator navCurrent;

		private XPathNavigator navNext;

		private enum IteratorState
		{
			NeedCandidateCurrent,
			HaveCandidateCurrent,
			HaveCurrentNeedNext,
			HaveCurrentHaveNext,
			HaveCurrentNoNext
		}
	}
}
