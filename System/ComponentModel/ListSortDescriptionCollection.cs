using System;
using System.Collections;

namespace System.ComponentModel
{
	public class ListSortDescriptionCollection : IList, ICollection, IEnumerable
	{
		public ListSortDescriptionCollection()
		{
			this.list = new ArrayList();
		}

		public ListSortDescriptionCollection(ListSortDescription[] sorts)
		{
			this.list = new ArrayList();
			foreach (ListSortDescription listSortDescription in sorts)
			{
				this.list.Add(listSortDescription);
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new InvalidOperationException("ListSortDescriptorCollection is read only.");
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		int IList.Add(object value)
		{
			return this.list.Add(value);
		}

		void IList.Clear()
		{
			this.list.Clear();
		}

		void IList.Insert(int index, object value)
		{
			this.list.Insert(index, value);
		}

		void IList.Remove(object value)
		{
			this.list.Remove(value);
		}

		void IList.RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public ListSortDescription this[int index]
		{
			get
			{
				return this.list[index] as ListSortDescription;
			}
			set
			{
				throw new InvalidOperationException("ListSortDescriptorCollection is read only.");
			}
		}

		public bool Contains(object value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf(value);
		}

		private ArrayList list;
	}
}
