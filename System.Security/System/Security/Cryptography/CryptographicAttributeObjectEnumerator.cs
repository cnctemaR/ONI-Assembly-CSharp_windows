using System;
using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class CryptographicAttributeObjectEnumerator : IEnumerator
	{
		internal CryptographicAttributeObjectEnumerator(IEnumerable enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
		}

		public CryptographicAttributeObject Current
		{
			get
			{
				return (CryptographicAttributeObject)this.enumerator.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.enumerator.Current;
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
