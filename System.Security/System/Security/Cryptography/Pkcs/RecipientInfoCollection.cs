using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class RecipientInfoCollection : IEnumerable, ICollection
	{
		internal RecipientInfoCollection()
		{
			this._list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new RecipientInfoEnumerator(this._list);
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._list.IsSynchronized;
			}
		}

		public RecipientInfo this[int index]
		{
			get
			{
				return (RecipientInfo)this._list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		internal int Add(RecipientInfo ri)
		{
			return this._list.Add(ri);
		}

		public void CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public void CopyTo(RecipientInfo[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public RecipientInfoEnumerator GetEnumerator()
		{
			return new RecipientInfoEnumerator(this._list);
		}

		private ArrayList _list;
	}
}
