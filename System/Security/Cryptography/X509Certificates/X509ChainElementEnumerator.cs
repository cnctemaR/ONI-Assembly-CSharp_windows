using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ChainElementEnumerator : IEnumerator
	{
		internal X509ChainElementEnumerator(IEnumerable enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
		}

		public X509ChainElement Current
		{
			get
			{
				return (X509ChainElement)this.enumerator.Current;
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

		internal X509ChainElementEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator enumerator;
	}
}
