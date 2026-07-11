using System;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct NodeRangeIterator
	{
		public void Create(XPathNavigator start, XmlNavigatorFilter filter, XPathNavigator end)
		{
			this.navCurrent = XmlQueryRuntime.SyncToNavigator(this.navCurrent, start);
			this.navEnd = XmlQueryRuntime.SyncToNavigator(this.navEnd, end);
			this.filter = filter;
			if (start.IsSamePosition(end))
			{
				this.state = ((!filter.IsFiltered(start)) ? NodeRangeIterator.IteratorState.HaveCurrentNoNext : NodeRangeIterator.IteratorState.NoNext);
				return;
			}
			this.state = ((!filter.IsFiltered(start)) ? NodeRangeIterator.IteratorState.HaveCurrent : NodeRangeIterator.IteratorState.NeedCurrent);
		}

		public bool MoveNext()
		{
			switch (this.state)
			{
			case NodeRangeIterator.IteratorState.HaveCurrent:
				this.state = NodeRangeIterator.IteratorState.NeedCurrent;
				return true;
			case NodeRangeIterator.IteratorState.NeedCurrent:
				if (!this.filter.MoveToFollowing(this.navCurrent, this.navEnd))
				{
					if (this.filter.IsFiltered(this.navEnd))
					{
						this.state = NodeRangeIterator.IteratorState.NoNext;
						return false;
					}
					this.navCurrent.MoveTo(this.navEnd);
					this.state = NodeRangeIterator.IteratorState.NoNext;
				}
				return true;
			case NodeRangeIterator.IteratorState.HaveCurrentNoNext:
				this.state = NodeRangeIterator.IteratorState.NoNext;
				return true;
			default:
				return false;
			}
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

		private XPathNavigator navEnd;

		private NodeRangeIterator.IteratorState state;

		private enum IteratorState
		{
			HaveCurrent,
			NeedCurrent,
			HaveCurrentNoNext,
			NoNext
		}
	}
}
