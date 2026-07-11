using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.Xml;
using System.Xml.XPath;

namespace MS.Internal.Xml.XPath
{
	internal sealed class XPathComparerHelper : IComparer
	{
		public XPathComparerHelper(XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
		{
			if (lang == null)
			{
				this.cinfo = Thread.CurrentThread.CurrentCulture;
			}
			else
			{
				try
				{
					this.cinfo = new CultureInfo(lang);
				}
				catch (ArgumentException)
				{
					throw;
				}
			}
			if (order == XmlSortOrder.Descending)
			{
				if (caseOrder == XmlCaseOrder.LowerFirst)
				{
					caseOrder = XmlCaseOrder.UpperFirst;
				}
				else if (caseOrder == XmlCaseOrder.UpperFirst)
				{
					caseOrder = XmlCaseOrder.LowerFirst;
				}
			}
			this.order = order;
			this.caseOrder = caseOrder;
			this.dataType = dataType;
		}

		public int Compare(object x, object y)
		{
			XmlDataType xmlDataType = this.dataType;
			if (xmlDataType != XmlDataType.Text)
			{
				if (xmlDataType != XmlDataType.Number)
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				double num = XmlConvert.ToXPathDouble(x);
				double num2 = XmlConvert.ToXPathDouble(y);
				int num3 = num.CompareTo(num2);
				if (this.order != XmlSortOrder.Ascending)
				{
					return -num3;
				}
				return num3;
			}
			else
			{
				string text = Convert.ToString(x, this.cinfo);
				string text2 = Convert.ToString(y, this.cinfo);
				int num3 = string.Compare(text, text2, this.caseOrder > XmlCaseOrder.None, this.cinfo);
				if (num3 != 0 || this.caseOrder == XmlCaseOrder.None)
				{
					if (this.order != XmlSortOrder.Ascending)
					{
						return -num3;
					}
					return num3;
				}
				else
				{
					num3 = string.Compare(text, text2, false, this.cinfo);
					if (this.caseOrder != XmlCaseOrder.LowerFirst)
					{
						return -num3;
					}
					return num3;
				}
			}
		}

		private XmlSortOrder order;

		private XmlCaseOrder caseOrder;

		private CultureInfo cinfo;

		private XmlDataType dataType;
	}
}
