using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Collections.ObjectModel
{
	[ComVisible(false)]
	[Serializable]
	public class ReadOnlyCollection<T> : IEnumerable, ICollection, IList, ICollection<T>, IList<T>, IEnumerable<T>
	{
		public ReadOnlyCollection(IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.list = list;
		}

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		T IList<T>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return true;
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
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
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
			throw new NotSupportedException();
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
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
				throw new NotSupportedException();
			}
		}

		public bool Contains(T value)
		{
			return this.list.Contains(value);
		}

		public void CopyTo(T[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int IndexOf(T value)
		{
			return this.list.IndexOf(value);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		protected IList<T> Items
		{
			get
			{
				return this.list;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		private IList<T> list;
	}
}
