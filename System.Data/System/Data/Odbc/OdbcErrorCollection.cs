using System;
using System.Collections;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcErrorCollection : IEnumerable, ICollection
	{
		internal OdbcErrorCollection()
		{
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this._items.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this._items.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this._items.Count;
			}
		}

		public OdbcError this[int i]
		{
			get
			{
				return (OdbcError)this._items[i];
			}
		}

		internal void Add(OdbcError error)
		{
			this._items.Add(error);
		}

		public void CopyTo(Array array, int i)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (i < array.GetLowerBound(0) || i > array.GetUpperBound(0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.IsFixedSize || i + this.Count > array.GetUpperBound(0))
			{
				throw new ArgumentException("array");
			}
			((OdbcError[])this._items.ToArray()).CopyTo(array, i);
		}

		public IEnumerator GetEnumerator()
		{
			return this._items.GetEnumerator();
		}

		public void CopyTo(OdbcError[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < array.GetLowerBound(0) || index > array.GetUpperBound(0))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			((OdbcError[])this._items.ToArray()).CopyTo(array, index);
		}

		private readonly ArrayList _items = new ArrayList();
	}
}
