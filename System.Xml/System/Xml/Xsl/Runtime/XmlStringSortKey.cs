using System;
using System.Globalization;

namespace System.Xml.Xsl.Runtime
{
	internal class XmlStringSortKey : XmlSortKey
	{
		public XmlStringSortKey(SortKey sortKey, bool descendingOrder)
		{
			this.sortKey = sortKey;
			this.descendingOrder = descendingOrder;
		}

		public XmlStringSortKey(byte[] sortKey, bool descendingOrder)
		{
			this.sortKeyBytes = sortKey;
			this.descendingOrder = descendingOrder;
		}

		public override int CompareTo(object obj)
		{
			XmlStringSortKey xmlStringSortKey = obj as XmlStringSortKey;
			if (xmlStringSortKey == null)
			{
				return base.CompareToEmpty(obj);
			}
			int num;
			if (this.sortKey != null)
			{
				num = SortKey.Compare(this.sortKey, xmlStringSortKey.sortKey);
			}
			else
			{
				int num2 = ((this.sortKeyBytes.Length < xmlStringSortKey.sortKeyBytes.Length) ? this.sortKeyBytes.Length : xmlStringSortKey.sortKeyBytes.Length);
				for (int i = 0; i < num2; i++)
				{
					if (this.sortKeyBytes[i] < xmlStringSortKey.sortKeyBytes[i])
					{
						num = -1;
						goto IL_00BC;
					}
					if (this.sortKeyBytes[i] > xmlStringSortKey.sortKeyBytes[i])
					{
						num = 1;
						goto IL_00BC;
					}
				}
				if (this.sortKeyBytes.Length < xmlStringSortKey.sortKeyBytes.Length)
				{
					num = -1;
				}
				else if (this.sortKeyBytes.Length > xmlStringSortKey.sortKeyBytes.Length)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			IL_00BC:
			if (num == 0)
			{
				return base.BreakSortingTie(xmlStringSortKey);
			}
			if (!this.descendingOrder)
			{
				return num;
			}
			return -num;
		}

		private SortKey sortKey;

		private byte[] sortKeyBytes;

		private bool descendingOrder;
	}
}
