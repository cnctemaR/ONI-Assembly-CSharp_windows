using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class RecipientInfoCollection : ICollection, IEnumerable
	{
		internal RecipientInfoCollection()
		{
			this._recipientInfos = Array.Empty<RecipientInfo>();
		}

		internal RecipientInfoCollection(RecipientInfo recipientInfo)
		{
			this._recipientInfos = new RecipientInfo[] { recipientInfo };
		}

		internal RecipientInfoCollection(ICollection<RecipientInfo> recipientInfos)
		{
			this._recipientInfos = new RecipientInfo[recipientInfos.Count];
			recipientInfos.CopyTo(this._recipientInfos, 0);
		}

		public RecipientInfo this[int index]
		{
			get
			{
				if (index < 0 || index >= this._recipientInfos.Length)
				{
					throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				return this._recipientInfos[index];
			}
		}

		public int Count
		{
			get
			{
				return this._recipientInfos.Length;
			}
		}

		public RecipientInfoEnumerator GetEnumerator()
		{
			return new RecipientInfoEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
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
			if (index > array.Length - this.Count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			for (int i = 0; i < this.Count; i++)
			{
				array.SetValue(this[i], index);
				index++;
			}
		}

		public void CopyTo(RecipientInfo[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			this._recipientInfos.CopyTo(array, index);
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

		private readonly RecipientInfo[] _recipientInfos;
	}
}
