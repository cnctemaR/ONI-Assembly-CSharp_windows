using System;
using System.Collections;

namespace Mono.Data.Tds
{
	public class TdsMetaParameterCollection : IEnumerable, ICollection
	{
		public TdsMetaParameterCollection()
		{
			this.list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public TdsMetaParameter this[int index]
		{
			get
			{
				return (TdsMetaParameter)this.list[index];
			}
		}

		public TdsMetaParameter this[string name]
		{
			get
			{
				return this[this.IndexOf(name)];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		public int Add(TdsMetaParameter value)
		{
			return this.list.Add(value);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(TdsMetaParameter value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public int IndexOf(TdsMetaParameter value)
		{
			return this.list.IndexOf(value);
		}

		public int IndexOf(string name)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].ParameterName.Equals(name))
				{
					return i;
				}
			}
			return -1;
		}

		public void Insert(int index, TdsMetaParameter value)
		{
			this.list.Insert(index, value);
		}

		public void Remove(TdsMetaParameter value)
		{
			this.list.Remove(value);
		}

		public void Remove(string name)
		{
			this.RemoveAt(this.IndexOf(name));
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		private ArrayList list;
	}
}
