using System;
using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class AsnEncodedDataCollection : ICollection, IEnumerable
	{
		public AsnEncodedDataCollection()
		{
			this._list = new ArrayList();
		}

		public AsnEncodedDataCollection(AsnEncodedData asnEncodedData)
		{
			this._list = new ArrayList();
			this._list.Add(asnEncodedData);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new AsnEncodedDataEnumerator(this);
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

		public AsnEncodedData this[int index]
		{
			get
			{
				return (AsnEncodedData)this._list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		public int Add(AsnEncodedData asnEncodedData)
		{
			return this._list.Add(asnEncodedData);
		}

		public void CopyTo(AsnEncodedData[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public AsnEncodedDataEnumerator GetEnumerator()
		{
			return new AsnEncodedDataEnumerator(this);
		}

		public void Remove(AsnEncodedData asnEncodedData)
		{
			this._list.Remove(asnEncodedData);
		}

		private ArrayList _list;
	}
}
