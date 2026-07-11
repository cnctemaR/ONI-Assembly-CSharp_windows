using System;

namespace System.Xml.Xsl.Runtime
{
	internal abstract class XmlSortKey : IComparable
	{
		public int Priority
		{
			set
			{
				for (XmlSortKey xmlSortKey = this; xmlSortKey != null; xmlSortKey = xmlSortKey.nextKey)
				{
					xmlSortKey.priority = value;
				}
			}
		}

		public XmlSortKey AddSortKey(XmlSortKey sortKey)
		{
			if (this.nextKey != null)
			{
				this.nextKey.AddSortKey(sortKey);
			}
			else
			{
				this.nextKey = sortKey;
			}
			return this;
		}

		protected int BreakSortingTie(XmlSortKey that)
		{
			if (this.nextKey != null)
			{
				return this.nextKey.CompareTo(that.nextKey);
			}
			if (this.priority >= that.priority)
			{
				return 1;
			}
			return -1;
		}

		protected int CompareToEmpty(object obj)
		{
			if (!(obj as XmlEmptySortKey).IsEmptyGreatest)
			{
				return 1;
			}
			return -1;
		}

		public abstract int CompareTo(object that);

		private int priority;

		private XmlSortKey nextKey;
	}
}
