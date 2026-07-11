using System;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlDoubleSortKey : XmlSortKey
	{
		public XmlDoubleSortKey(double value, XmlCollation collation)
		{
			if (double.IsNaN(value))
			{
				this.isNaN = true;
				this.dblVal = ((collation.EmptyGreatest != collation.DescendingOrder) ? double.PositiveInfinity : double.NegativeInfinity);
				return;
			}
			this.dblVal = (collation.DescendingOrder ? (-value) : value);
		}

		public override int CompareTo(object obj)
		{
			XmlDoubleSortKey xmlDoubleSortKey = obj as XmlDoubleSortKey;
			if (xmlDoubleSortKey == null)
			{
				if (this.isNaN)
				{
					return base.BreakSortingTie(obj as XmlSortKey);
				}
				return base.CompareToEmpty(obj);
			}
			else if (this.dblVal == xmlDoubleSortKey.dblVal)
			{
				if (this.isNaN)
				{
					if (xmlDoubleSortKey.isNaN)
					{
						return base.BreakSortingTie(xmlDoubleSortKey);
					}
					if (this.dblVal != double.NegativeInfinity)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					if (!xmlDoubleSortKey.isNaN)
					{
						return base.BreakSortingTie(xmlDoubleSortKey);
					}
					if (xmlDoubleSortKey.dblVal != double.NegativeInfinity)
					{
						return -1;
					}
					return 1;
				}
			}
			else
			{
				if (this.dblVal >= xmlDoubleSortKey.dblVal)
				{
					return 1;
				}
				return -1;
			}
		}

		private double dblVal;

		private bool isNaN;
	}
}
