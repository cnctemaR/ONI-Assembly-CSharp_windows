using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsRecipientCollection : ICollection, IEnumerable
	{
		public CmsRecipientCollection()
		{
			this._recipients = new List<CmsRecipient>();
		}

		public CmsRecipientCollection(CmsRecipient recipient)
		{
			this._recipients = new List<CmsRecipient>(1);
			this._recipients.Add(recipient);
		}

		public CmsRecipientCollection(SubjectIdentifierType recipientIdentifierType, X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new NullReferenceException();
			}
			this._recipients = new List<CmsRecipient>(certificates.Count);
			for (int i = 0; i < certificates.Count; i++)
			{
				this._recipients.Add(new CmsRecipient(recipientIdentifierType, certificates[i]));
			}
		}

		public CmsRecipient this[int index]
		{
			get
			{
				if (index < 0 || index >= this._recipients.Count)
				{
					throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
				}
				return this._recipients[index];
			}
		}

		public int Count
		{
			get
			{
				return this._recipients.Count;
			}
		}

		public int Add(CmsRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			int count = this._recipients.Count;
			this._recipients.Add(recipient);
			return count;
		}

		public void Remove(CmsRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			this._recipients.Remove(recipient);
		}

		public CmsRecipientEnumerator GetEnumerator()
		{
			return new CmsRecipientEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new CmsRecipientEnumerator(this);
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

		public void CopyTo(CmsRecipient[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
			}
			if (index > array.Length - this.Count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			this._recipients.CopyTo(array, index);
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

		private readonly List<CmsRecipient> _recipients;
	}
}
