using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsRecipientEnumerator : IEnumerator
	{
		internal CmsRecipientEnumerator(CmsRecipientCollection recipients)
		{
			this._recipients = recipients;
			this._current = -1;
		}

		public CmsRecipient Current
		{
			get
			{
				return this._recipients[this._current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._recipients[this._current];
			}
		}

		public bool MoveNext()
		{
			if (this._current >= this._recipients.Count - 1)
			{
				return false;
			}
			this._current++;
			return true;
		}

		public void Reset()
		{
			this._current = -1;
		}

		internal CmsRecipientEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly CmsRecipientCollection _recipients;

		private int _current;
	}
}
