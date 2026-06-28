using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfoCollection : IEnumerable, ICollection
	{
		internal SignerInfoCollection()
		{
			this._list = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SignerInfoEnumerator(this._list);
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
				return false;
			}
		}

		public SignerInfo this[int index]
		{
			get
			{
				return (SignerInfo)this._list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		internal void Add(SignerInfo signer)
		{
			this._list.Add(signer);
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._list.CopyTo(array, index);
		}

		public void CopyTo(SignerInfo[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this._list.CopyTo(array, index);
		}

		public SignerInfoEnumerator GetEnumerator()
		{
			return new SignerInfoEnumerator(this._list);
		}

		private ArrayList _list;
	}
}
