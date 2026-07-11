using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlDateTimeSortKey : XmlIntegerSortKey
	{
		public XmlDateTimeSortKey(DateTime value, XmlCollation collation)
			: base(value.Ticks, collation)
		{
		}
	}
}
