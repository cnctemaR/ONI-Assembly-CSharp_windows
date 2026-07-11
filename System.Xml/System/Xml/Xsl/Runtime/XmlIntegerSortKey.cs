using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlIntegerSortKey : XmlSortKey
	{
		public XmlIntegerSortKey(long value, XmlCollation collation)
		{
			this.longVal = (collation.DescendingOrder ? (~value) : value);
		}

		public override int CompareTo(object obj)
		{
			XmlIntegerSortKey xmlIntegerSortKey = obj as XmlIntegerSortKey;
			if (xmlIntegerSortKey == null)
			{
				return base.CompareToEmpty(obj);
			}
			if (this.longVal == xmlIntegerSortKey.longVal)
			{
				return base.BreakSortingTie(xmlIntegerSortKey);
			}
			if (this.longVal >= xmlIntegerSortKey.longVal)
			{
				return 1;
			}
			return -1;
		}

		private long longVal;
	}
}
