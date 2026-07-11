using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class RecipientInfoEnumerator : IEnumerator
	{
		internal RecipientInfoEnumerator(IEnumerable enumerable)
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

		public RecipientInfo Current
		{
			get
			{
				return (RecipientInfo)this.enumerator.Current;
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
