using System;

namespace MS.Internal.Xml.Cache
{
	internal struct XPathNodeRef
	{
		public XPathNodeRef(XPathNode[] page, int idx)
		{
			this._page = page;
			this._idx = idx;
		}

		public XPathNode[] Page
		{
			get
			{
				return this._page;
			}
		}

		public int Index
		{
			get
			{
				return this._idx;
			}
		}

		public override int GetHashCode()
		{
			return XPathNodeHelper.GetLocation(this._page, this._idx);
		}

		private XPathNode[] _page;

		private int _idx;
	}
}
