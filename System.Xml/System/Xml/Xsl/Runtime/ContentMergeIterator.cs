using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct ContentMergeIterator
	{
		public void Create(XmlNavigatorFilter filter)
		{
			this.filter = filter;
			this.navStack.Reset();
			this.state = ContentMergeIterator.IteratorState.NeedCurrent;
		}

		public IteratorResult MoveNext(XPathNavigator input)
		{
			return this.MoveNext(input, true);
		}

		internal IteratorResult MoveNext(XPathNavigator input, bool isContent)
		{
			switch (this.state)
			{
			case ContentMergeIterator.IteratorState.NeedCurrent:
				if (input == null)
				{
					return IteratorResult.NoMoreNodes;
				}
				this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
				if (isContent ? this.filter.MoveToContent(this.navCurrent) : this.filter.MoveToFollowingSibling(this.navCurrent))
				{
					this.state = ContentMergeIterator.IteratorState.HaveCurrentNeedNext;
				}
				return IteratorResult.NeedInputNode;
			case ContentMergeIterator.IteratorState.HaveCurrentNeedNext:
				if (input == null)
				{
					this.state = ContentMergeIterator.IteratorState.HaveCurrentNoNext;
					return IteratorResult.HaveCurrentNode;
				}
				this.navNext = XmlQueryRuntime.SyncToNavigator(this.navNext, input);
				if (isContent ? this.filter.MoveToContent(this.navNext) : this.filter.MoveToFollowingSibling(this.navNext))
				{
					this.state = ContentMergeIterator.IteratorState.HaveCurrentHaveNext;
					return this.DocOrderMerge();
				}
				return IteratorResult.NeedInputNode;
			case ContentMergeIterator.IteratorState.HaveCurrentNoNext:
			case ContentMergeIterator.IteratorState.HaveCurrentHaveNext:
				if (isContent ? (!this.filter.MoveToNextContent(this.navCurrent)) : (!this.filter.MoveToFollowingSibling(this.navCurrent)))
				{
					if (this.navStack.IsEmpty)
					{
						if (this.state == ContentMergeIterator.IteratorState.HaveCurrentNoNext)
						{
							return IteratorResult.NoMoreNodes;
						}
						this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, this.navNext);
						this.state = ContentMergeIterator.IteratorState.HaveCurrentNeedNext;
						return IteratorResult.NeedInputNode;
					}
					else
					{
						this.navCurrent = this.navStack.Pop();
					}
				}
				if (this.state == ContentMergeIterator.IteratorState.HaveCurrentNoNext)
				{
					return IteratorResult.HaveCurrentNode;
				}
				return this.DocOrderMerge();
			default:
				return IteratorResult.NoMoreNodes;
			}
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private IteratorResult DocOrderMerge()
		{
			XmlNodeOrder xmlNodeOrder = this.navCurrent.ComparePosition(this.navNext);
			if (xmlNodeOrder == XmlNodeOrder.Before || xmlNodeOrder == XmlNodeOrder.Unknown)
			{
				return IteratorResult.HaveCurrentNode;
			}
			if (xmlNodeOrder == XmlNodeOrder.After)
			{
				this.navStack.Push(this.navCurrent);
				this.navCurrent = this.navNext;
				this.navNext = null;
			}
			this.state = ContentMergeIterator.IteratorState.HaveCurrentNeedNext;
			return IteratorResult.NeedInputNode;
		}

		private XmlNavigatorFilter filter;

		private XPathNavigator navCurrent;

		private XPathNavigator navNext;

		private XmlNavigatorStack navStack;

		private ContentMergeIterator.IteratorState state;

		private enum IteratorState
		{
			NeedCurrent,
			HaveCurrentNeedNext,
			HaveCurrentNoNext,
			HaveCurrentHaveNext
		}
	}
}
