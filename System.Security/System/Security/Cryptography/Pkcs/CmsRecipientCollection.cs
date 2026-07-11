using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsRecipientCollection : IEnumerable, ICollection
	{
		public CmsRecipientCollection()
		{
			this._list = new ArrayList();
		}

		public CmsRecipientCollection(CmsRecipient recipient)
		{
			this._list.Add(recipient);
		}

		public CmsRecipientCollection(SubjectIdentifierType recipientIdentifierType, X509Certificate2Collection certificates)
		{
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				CmsRecipient cmsRecipient = new CmsRecipient(recipientIdentifierType, x509Certificate);
				this._list.Add(cmsRecipient);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new CmsRecipientEnumerator(this._list);
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

		public CmsRecipient this[int index]
		{
			get
			{
				return (CmsRecipient)this._list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return this._list.SyncRoot;
			}
		}

		public int Add(CmsRecipient recipient)
		{
			return this._list.Add(recipient);
		}

		public void CopyTo(Array array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public void CopyTo(CmsRecipient[] array, int index)
		{
			this._list.CopyTo(array, index);
		}

		public CmsRecipientEnumerator GetEnumerator()
		{
			return new CmsRecipientEnumerator(this._list);
		}

		public void Remove(CmsRecipient recipient)
		{
			this._list.Remove(recipient);
		}

		private ArrayList _list;
	}
}
