using System;
using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class OidCollection : ICollection, IEnumerable
	{
		public OidCollection()
		{
			this._list = new ArrayList();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new OidEnumerator(this);
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

		public Oid this[int index]
		{
			get
			{
				return (Oid)this._list[index];
			}
		}

		public Oid this[string oid]
		{
			get
			{
				foreach (object obj in this._list)
				{
					Oid oid2 = (Oid)obj;
					if (oid2.Value == oid)
					{
						return oid2;
					}
				}
				return null;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		public int Add(Oid oid)
		{
			return (!this._readOnly) ? this._list.Add(oid) : 0;
		}

		public void CopyTo(Oid[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public OidEnumerator GetEnumerator()
		{
			return new OidEnumerator(this);
		}

		internal bool ReadOnly
		{
			get
			{
				return this._readOnly;
			}
			set
			{
				this._readOnly = value;
			}
		}

		internal OidCollection ReadOnlyCopy()
		{
			OidCollection oidCollection = new OidCollection();
			foreach (object obj in this._list)
			{
				Oid oid = (Oid)obj;
				oidCollection.Add(oid);
			}
			oidCollection._readOnly = true;
			return oidCollection;
		}

		private ArrayList _list;

		private bool _readOnly;
	}
}
