using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class RecipientInfoEnumerator : IEnumerator
	{
		internal RecipientInfoEnumerator(RecipientInfoCollection RecipientInfos)
		{
			this._recipientInfos = RecipientInfos;
			this._current = -1;
		}

		public RecipientInfo Current
		{
			get
			{
				return this._recipientInfos[this._current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._recipientInfos[this._current];
			}
		}

		public bool MoveNext()
		{
			if (this._current >= this._recipientInfos.Count - 1)
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

		internal RecipientInfoEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly RecipientInfoCollection _recipientInfos;

		private int _current;
	}
}
