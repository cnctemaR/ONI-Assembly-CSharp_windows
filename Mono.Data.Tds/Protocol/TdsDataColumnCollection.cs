using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsDataColumnCollection : IEnumerable
	{
		public TdsDataColumnCollection()
		{
			this.list = new ArrayList();
		}

		public TdsDataColumn this[int index]
		{
			get
			{
				return (TdsDataColumn)this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public int Add(TdsDataColumn schema)
		{
			int num = this.list.Add(schema);
			schema.ColumnOrdinal = new int?(num);
			return num;
		}

		public void Add(TdsDataColumnCollection columns)
		{
			foreach (object obj in columns)
			{
				TdsDataColumn tdsDataColumn = (TdsDataColumn)obj;
				this.Add(tdsDataColumn);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public void Clear()
		{
			this.list.Clear();
		}

		private ArrayList list;
	}
}
