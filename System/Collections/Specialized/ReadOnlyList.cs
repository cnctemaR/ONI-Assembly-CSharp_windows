using System;

namespace System.Collections.Specialized
{
	internal sealed class ReadOnlyList : IList, ICollection, IEnumerable
	{
		internal ReadOnlyList(IList list)
		{
			this._list = list;
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._list.IsSynchronized;
			}
		}

		public object this[int index]
		{
			get
			{
				return this._list[index];
			}
			set
			{
				throw new NotSupportedException("Collection is read-only.");
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		public int Add(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		public void Clear()
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		public bool Contains(object value)
		{
			return this._list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return this._list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		public void Remove(object value)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException("Collection is read-only.");
		}

		private readonly IList _list;
	}
}
