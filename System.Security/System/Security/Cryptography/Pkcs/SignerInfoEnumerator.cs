using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class SignerInfoEnumerator : IEnumerator
	{
		internal SignerInfoEnumerator(IEnumerable enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
		}

		public SignerInfo Current
		{
			get
			{
				return (SignerInfo)this.enumerator.Current;
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

		internal SignerInfoEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator enumerator;
	}
}
