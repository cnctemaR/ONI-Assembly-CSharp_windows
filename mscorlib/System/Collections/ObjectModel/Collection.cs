using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
	[ComVisible(false)]
	[Serializable]
	public class Collection<T> : IEnumerable, ICollection, IList, ICollection<T>, IList<T>, IEnumerable<T>
	{
		public Collection()
		{
			List<T> list = new List<T>();
			IList list2 = list;
			this.syncRoot = list2.SyncRoot;
			this.list = list;
		}

		public Collection(IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.list = list;
			ICollection collection = list as ICollection;
			this.syncRoot = ((collection == null) ? new object() : collection.SyncRoot);
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.list).CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		int IList.Add(object value)
		{
			int count = this.list.Count;
			this.InsertItem(count, Collection<T>.ConvertItem(value));
			return count;
		}

		bool IList.Contains(object value)
		{
			return Collection<T>.IsValidItem(value) && this.list.Contains((T)((object)value));
		}

		int IList.IndexOf(object value)
		{
			if (Collection<T>.IsValidItem(value))
			{
				return this.list.IndexOf((T)((object)value));
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			this.InsertItem(index, Collection<T>.ConvertItem(value));
		}

		void IList.Remove(object value)
		{
			Collection<T>.CheckWritable(this.list);
			int num = this.IndexOf(Collection<T>.ConvertItem(value));
			this.RemoveItem(num);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return Collection<T>.IsSynchronized(this.list);
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return Collection<T>.IsFixedSize(this.list);
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.list.IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.SetItem(index, Collection<T>.ConvertItem(value));
			}
		}

		public void Add(T item)
		{
			int count = this.list.Count;
			this.InsertItem(count, item);
		}

		public void Clear()
		{
			this.ClearItems();
		}

		protected virtual void ClearItems()
		{
			this.list.Clear();
		}

		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		public void CopyTo(T[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return this.list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.InsertItem(index, item);
		}

		protected virtual void InsertItem(int index, T item)
		{
			this.list.Insert(index, item);
		}

		protected IList<T> Items
		{
			get
			{
				return this.list;
			}
		}

		public bool Remove(T item)
		{
			int num = this.IndexOf(item);
			if (num == -1)
			{
				return false;
			}
			this.RemoveItem(num);
			return true;
		}

		public void RemoveAt(int index)
		{
			this.RemoveItem(index);
		}

		protected virtual void RemoveItem(int index)
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

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.SetItem(index, value);
			}
		}

		protected virtual void SetItem(int index, T item)
		{
			this.list[index] = item;
		}

		internal static bool IsValidItem(object item)
		{
			return item is T || (item == null && !typeof(T).IsValueType);
		}

		internal static T ConvertItem(object item)
		{
			if (Collection<T>.IsValidItem(item))
			{
				return (T)((object)item);
			}
			throw new ArgumentException("item");
		}

		internal static void CheckWritable(IList<T> list)
		{
			if (list.IsReadOnly)
			{
				throw new NotSupportedException();
			}
		}

		internal static bool IsSynchronized(IList<T> list)
		{
			ICollection collection = list as ICollection;
			return collection != null && collection.IsSynchronized;
		}

		internal static bool IsFixedSize(IList<T> list)
		{
			IList list2 = list as IList;
			return list2 != null && list2.IsFixedSize;
		}

		private IList<T> list;

		private object syncRoot;
	}
}
