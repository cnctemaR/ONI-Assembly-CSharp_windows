using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class Sort
	{
		public Sort(int sortkey, string xmllang, XmlDataType datatype, XmlSortOrder xmlorder, XmlCaseOrder xmlcaseorder)
		{
			this.select = sortkey;
			this.lang = xmllang;
			this.dataType = datatype;
			this.order = xmlorder;
			this.caseOrder = xmlcaseorder;
		}

		internal int select;

		internal string lang;

		internal XmlDataType dataType;

		internal XmlSortOrder order;

		internal XmlCaseOrder caseOrder;
	}
}
