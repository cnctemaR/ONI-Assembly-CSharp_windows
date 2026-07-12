using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Security.Cryptography
{
	public sealed class AsnEncodedDataCollection : ICollection, IEnumerable
	{
		public AsnEncodedDataCollection()
		{
			this._list = new List<AsnEncodedData>();
		}

		public AsnEncodedDataCollection(AsnEncodedData asnEncodedData)
			: this()
		{
			this._list.Add(asnEncodedData);
		}

		public int Add(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			int count = this._list.Count;
			this._list.Add(asnEncodedData);
			return count;
		}

		public void Remove(AsnEncodedData asnEncodedData)
		{
			if (asnEncodedData == null)
			{
				throw new ArgumentNullException("asnEncodedData");
			}
			this._list.Remove(asnEncodedData);
		}

		public AsnEncodedData this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public AsnEncodedDataEnumerator GetEnumerator()
		{
			return new AsnEncodedDataEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (this.Count > array.Length - index)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index);
				index++;
			}
		}

		public void CopyTo(AsnEncodedData[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			this._list.CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private readonly List<AsnEncodedData> _list;
	}
}
