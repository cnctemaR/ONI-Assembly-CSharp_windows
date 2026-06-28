using System;
using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsDataRow : IEnumerable, ICollection, IList
	{
		public TdsDataRow()
		{
			this.list = new ArrayList();
			this.bigDecimalIndex = -1;
		}

		public int BigDecimalIndex
		{
			get
			{
				return this.bigDecimalIndex;
			}
			set
			{
				this.bigDecimalIndex = value;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public object this[int index]
		{
			get
			{
				if (index >= this.list.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public int Add(object value)
		{
			return this.list.Add(value);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(object value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			this.list.CopyTo(index, array, arrayIndex, count);
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			this.list.Insert(index, value);
		}

		public void Remove(object value)
		{
			this.list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		private ArrayList list;

		private int bigDecimalIndex;
	}
}
