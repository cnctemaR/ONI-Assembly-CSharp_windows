using System;
using System.ComponentModel;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct XmlSortKeyAccumulator
	{
		public void Create()
		{
			if (this.keys == null)
			{
				this.keys = new XmlSortKey[64];
			}
			this.pos = 0;
			this.keys[0] = null;
		}

		public void AddStringSortKey(XmlCollation collation, string value)
		{
			this.AppendSortKey(collation.CreateSortKey(value));
		}

		public void AddDecimalSortKey(XmlCollation collation, decimal value)
		{
			this.AppendSortKey(new XmlDecimalSortKey(value, collation));
		}

		public void AddIntegerSortKey(XmlCollation collation, long value)
		{
			this.AppendSortKey(new XmlIntegerSortKey(value, collation));
		}

		public void AddIntSortKey(XmlCollation collation, int value)
		{
			this.AppendSortKey(new XmlIntSortKey(value, collation));
		}

		public void AddDoubleSortKey(XmlCollation collation, double value)
		{
			this.AppendSortKey(new XmlDoubleSortKey(value, collation));
		}

		public void AddDateTimeSortKey(XmlCollation collation, DateTime value)
		{
			this.AppendSortKey(new XmlDateTimeSortKey(value, collation));
		}

		public void AddEmptySortKey(XmlCollation collation)
		{
			this.AppendSortKey(new XmlEmptySortKey(collation));
		}

		public void FinishSortKeys()
		{
			this.pos++;
			if (this.pos >= this.keys.Length)
			{
				XmlSortKey[] array = new XmlSortKey[this.pos * 2];
				Array.Copy(this.keys, 0, array, 0, this.keys.Length);
				this.keys = array;
			}
			this.keys[this.pos] = null;
		}

		private void AppendSortKey(XmlSortKey key)
		{
			key.Priority = this.pos;
			if (this.keys[this.pos] == null)
			{
				this.keys[this.pos] = key;
				return;
			}
			this.keys[this.pos].AddSortKey(key);
		}

		public Array Keys
		{
			get
			{
				return this.keys;
			}
		}

		private XmlSortKey[] keys;

		private int pos;

		private const int DefaultSortKeyCount = 64;
	}
}
