using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlEmptySortKey : XmlSortKey
	{
		public XmlEmptySortKey(XmlCollation collation)
		{
			this.isEmptyGreatest = collation.EmptyGreatest != collation.DescendingOrder;
		}

		public bool IsEmptyGreatest
		{
			get
			{
				return this.isEmptyGreatest;
			}
		}

		public override int CompareTo(object obj)
		{
			XmlEmptySortKey xmlEmptySortKey = obj as XmlEmptySortKey;
			if (xmlEmptySortKey == null)
			{
				return -(obj as XmlSortKey).CompareTo(this);
			}
			return base.BreakSortingTie(xmlEmptySortKey);
		}

		private bool isEmptyGreatest;
	}
}
