using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlDecimalSortKey : XmlSortKey
	{
		public XmlDecimalSortKey(decimal value, XmlCollation collation)
		{
			this.decVal = (collation.DescendingOrder ? (-value) : value);
		}

		public override int CompareTo(object obj)
		{
			XmlDecimalSortKey xmlDecimalSortKey = obj as XmlDecimalSortKey;
			if (xmlDecimalSortKey == null)
			{
				return base.CompareToEmpty(obj);
			}
			int num = decimal.Compare(this.decVal, xmlDecimalSortKey.decVal);
			if (num == 0)
			{
				return base.BreakSortingTie(xmlDecimalSortKey);
			}
			return num;
		}

		private decimal decVal;
	}
}
