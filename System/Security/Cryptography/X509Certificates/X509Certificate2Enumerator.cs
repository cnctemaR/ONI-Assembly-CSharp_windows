using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509Certificate2Enumerator : IEnumerator
	{
		internal X509Certificate2Enumerator(X509Certificate2Collection collection)
		{
			this.enumerator = ((IEnumerable)collection).GetEnumerator();
		}

		public X509Certificate2 Current
		{
			get
			{
				return (X509Certificate2)this.enumerator.Current;
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

		object IEnumerator.Current
		{
			get
			{
				return this.enumerator.Current;
			}
		}

		bool IEnumerator.MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			this.enumerator.Reset();
		}

		internal X509Certificate2Enumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator enumerator;
	}
}
