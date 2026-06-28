using System;
using System.Collections;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ChainElementCollection : ICollection, IEnumerable
	{
		internal X509ChainElementCollection()
		{
			this._list = new ArrayList();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new X509ChainElementEnumerator(this._list);
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

		public X509ChainElement this[int index]
		{
			get
			{
				return (X509ChainElement)this._list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		public void CopyTo(X509ChainElement[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public X509ChainElementEnumerator GetEnumerator()
		{
			return new X509ChainElementEnumerator(this._list);
		}

		internal void Add(X509Certificate2 certificate)
		{
			this._list.Add(new X509ChainElement(certificate));
		}

		internal void Clear()
		{
			this._list.Clear();
		}

		internal bool Contains(X509Certificate2 certificate)
		{
			for (int i = 0; i < this._list.Count; i++)
			{
				if (certificate.Equals((this._list[i] as X509ChainElement).Certificate))
				{
					return true;
				}
			}
			return false;
		}

		private ArrayList _list;
	}
}
