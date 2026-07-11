using System;
using System.Collections;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfoEnumerator : IEnumerator
	{
		internal SignerInfoEnumerator(IEnumerable enumerable)
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

		public SignerInfo Current
		{
			get
			{
				return (SignerInfo)this.enumerator.Current;
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
