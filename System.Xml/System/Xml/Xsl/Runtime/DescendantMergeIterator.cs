using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct DescendantMergeIterator
	{
		public void Create(XmlNavigatorFilter filter, bool orSelf)
		{
			this.filter = filter;
			this.state = DescendantMergeIterator.IteratorState.NoPrevious;
			this.orSelf = orSelf;
		}

		public IteratorResult MoveNext(XPathNavigator input)
		{
			if (this.state != DescendantMergeIterator.IteratorState.NeedDescendant)
			{
				if (input == null)
				{
					return IteratorResult.NoMoreNodes;
				}
				if (this.state != DescendantMergeIterator.IteratorState.NoPrevious && this.navRoot.IsDescendant(input))
				{
					return IteratorResult.NeedInputNode;
				}
				this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, input);
				this.navRoot = XmlQueryRuntime.SyncToNavigator(this.navRoot, input);
				this.navEnd = XmlQueryRuntime.SyncToNavigator(this.navEnd, input);
				this.navEnd.MoveToNonDescendant();
				this.state = DescendantMergeIterator.IteratorState.NeedDescendant;
				if (this.orSelf && !this.filter.IsFiltered(input))
				{
					return IteratorResult.HaveCurrentNode;
				}
			}
			if (this.filter.MoveToFollowing(this.navCurrent, this.navEnd))
			{
				return IteratorResult.HaveCurrentNode;
			}
			this.state = DescendantMergeIterator.IteratorState.NeedCurrent;
			return IteratorResult.NeedInputNode;
		}

		public XPathNavigator Current
		{
			get
			{
				return this.navCurrent;
			}
		}

		private XmlNavigatorFilter filter;

		private XPathNavigator navCurrent;

		private XPathNavigator navRoot;

		private XPathNavigator navEnd;

		private DescendantMergeIterator.IteratorState state;

		private bool orSelf;

		private enum IteratorState
		{
			NoPrevious,
			NeedCurrent,
			NeedDescendant
		}
	}
}
