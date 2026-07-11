using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlIntSortKey : XmlSortKey
	{
		public XmlIntSortKey(int value, XmlCollation collation)
		{
			this.intVal = (collation.DescendingOrder ? (~value) : value);
		}

		public override int CompareTo(object obj)
		{
			XmlIntSortKey xmlIntSortKey = obj as XmlIntSortKey;
			if (xmlIntSortKey == null)
			{
				return base.CompareToEmpty(obj);
			}
			if (this.intVal == xmlIntSortKey.intVal)
			{
				return base.BreakSortingTie(xmlIntSortKey);
			}
			if (this.intVal >= xmlIntSortKey.intVal)
			{
				return 1;
			}
			return -1;
		}

		private int intVal;
	}
}
