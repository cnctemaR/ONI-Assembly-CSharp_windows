using System;
using System.Collections;
using System.Collections.Generic;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
	public sealed class OidCollection : ICollection, IEnumerable
	{
		public OidCollection()
		{
			this._list = new List<Oid>();
		}

		public int Add(Oid oid)
		{
			int count = this._list.Count;
			this._list.Add(oid);
			return count;
		}

		public Oid this[int index]
		{
			get
			{
				return this._list[index];
			}
		}

		public Oid this[string oid]
		{
			get
			{
				string text = OidLookup.ToOid(oid, OidGroup.All, false);
				if (text == null)
				{
					text = oid;
				}
				foreach (Oid oid2 in this._list)
				{
					if (oid2.Value == text)
					{
						return oid2;
					}
				}
				return null;
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public OidEnumerator GetEnumerator()
		{
			return new OidEnumerator(this);
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
			if (index + this.Count > array.Length)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index);
				index++;
			}
		}

		public void CopyTo(Oid[] array, int index)
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

		private readonly List<Oid> _list;
	}
}
