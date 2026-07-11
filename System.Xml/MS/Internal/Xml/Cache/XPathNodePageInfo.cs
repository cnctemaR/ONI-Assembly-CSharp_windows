using System;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathNodePageInfo
	{
		public XPathNodePageInfo(XPathNode[] pagePrev, int pageNum)
		{
			this.pagePrev = pagePrev;
			this.pageNum = pageNum;
			this.nodeCount = 1;
		}

		public int PageNumber
		{
			get
			{
				return this.pageNum;
			}
		}

		public int NodeCount
		{
			get
			{
				return this.nodeCount;
			}
			set
			{
				this.nodeCount = value;
			}
		}

		public XPathNode[] PreviousPage
		{
			get
			{
				return this.pagePrev;
			}
		}

		public XPathNode[] NextPage
		{
			get
			{
				return this.pageNext;
			}
			set
			{
				this.pageNext = value;
			}
		}

		private int pageNum;

		private int nodeCount;

		private XPathNode[] pagePrev;

		private XPathNode[] pageNext;
	}
}
