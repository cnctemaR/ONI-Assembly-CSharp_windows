using System;
using System.Collections;
using System.Globalization;
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
				this._cinfo = CultureInfo.CurrentCulture;
			}
			else
			{
				try
				{
					this._cinfo = new CultureInfo(lang);
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
			this._order = order;
			this._caseOrder = caseOrder;
			this._dataType = dataType;
		}

		public int Compare(object x, object y)
		{
			XmlDataType dataType = this._dataType;
			if (dataType != XmlDataType.Text)
			{
				if (dataType != XmlDataType.Number)
				{
					throw new InvalidOperationException("Operation is not valid due to the current state of the object.");
				}
				double num = XmlConvert.ToXPathDouble(x);
				double num2 = XmlConvert.ToXPathDouble(y);
				int num3 = num.CompareTo(num2);
				if (this._order != XmlSortOrder.Ascending)
				{
					return -num3;
				}
				return num3;
			}
			else
			{
				string text = Convert.ToString(x, this._cinfo);
				string text2 = Convert.ToString(y, this._cinfo);
				int num3 = this._cinfo.CompareInfo.Compare(text, text2, (this._caseOrder != XmlCaseOrder.None) ? CompareOptions.IgnoreCase : CompareOptions.None);
				if (num3 != 0 || this._caseOrder == XmlCaseOrder.None)
				{
					if (this._order != XmlSortOrder.Ascending)
					{
						return -num3;
					}
					return num3;
				}
				else
				{
					num3 = this._cinfo.CompareInfo.Compare(text, text2);
					if (this._caseOrder != XmlCaseOrder.LowerFirst)
					{
						return -num3;
					}
					return num3;
				}
			}
		}

		private XmlSortOrder _order;

		private XmlCaseOrder _caseOrder;

		private CultureInfo _cinfo;

		private XmlDataType _dataType;
	}
}
