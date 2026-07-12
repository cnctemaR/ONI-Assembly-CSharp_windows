using System;

namespace MS.Internal.Xml.Cache
{
	internal sealed class XPathNodePageInfo
	{
		public XPathNodePageInfo(XPathNode[] pagePrev, int pageNum)
		{
			this._pagePrev = pagePrev;
			this._pageNum = pageNum;
			this._nodeCount = 1;
		}

		public int PageNumber
		{
			get
			{
				return this._pageNum;
			}
		}

		public int NodeCount
		{
			get
			{
				return this._nodeCount;
			}
			set
			{
				this._nodeCount = value;
			}
		}

		public XPathNode[] PreviousPage
		{
			get
			{
				return this._pagePrev;
			}
		}

		public XPathNode[] NextPage
		{
			get
			{
				return this._pageNext;
			}
			set
			{
				this._pageNext = value;
			}
		}

		private int _pageNum;

		private int _nodeCount;

		private XPathNode[] _pagePrev;

		private XPathNode[] _pageNext;
	}
}
