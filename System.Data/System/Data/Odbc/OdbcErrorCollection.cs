using System;
using System.Collections;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcErrorCollection : ICollection, IEnumerable
	{
		internal OdbcErrorCollection()
		{
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
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
			this._items.CopyTo(array, i);
		}

		public void CopyTo(OdbcError[] array, int i)
		{
			this._items.CopyTo(array, i);
		}

		public IEnumerator GetEnumerator()
		{
			return this._items.GetEnumerator();
		}

		internal void SetSource(string Source)
		{
			foreach (object obj in this._items)
			{
				((OdbcError)obj).SetSource(Source);
			}
		}

		private ArrayList _items = new ArrayList();
	}
}
