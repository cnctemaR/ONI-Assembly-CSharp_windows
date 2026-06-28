using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class CmsRecipientEnumerator : IEnumerator
	{
		internal CmsRecipientEnumerator(IEnumerable enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
		}

		object IEnumerator.Current
		{
			get
			{
				return this.enumerator.Current;
			}
		}

		public CmsRecipient Current
		{
			get
			{
				return (CmsRecipient)this.enumerator.Current;
			}
		}

		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		public void Reset()
		{
			this.enumerator.Reset();
		}

		private IEnumerator enumerator;
	}
}
