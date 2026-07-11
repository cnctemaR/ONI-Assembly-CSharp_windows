using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public sealed class ApplicationTrustEnumerator : IEnumerator
	{
		internal ApplicationTrustEnumerator(ApplicationTrustCollection collection)
		{
			this.e = collection.GetEnumerator();
		}

		public ApplicationTrust Current
		{
			get
			{
				return (ApplicationTrust)this.e.Current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.e.Current;
			}
		}

		[SecuritySafeCritical]
		public bool MoveNext()
		{
			return this.e.MoveNext();
		}

		public void Reset()
		{
			this.e.Reset();
		}

		internal ApplicationTrustEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator e;
	}
}
