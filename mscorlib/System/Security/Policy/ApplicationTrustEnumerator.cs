using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public sealed class ApplicationTrustEnumerator : IEnumerator
	{
		internal ApplicationTrustEnumerator(ApplicationTrustCollection atc)
		{
			this.trusts = atc;
			this.current = -1;
		}

		public ApplicationTrust Current
		{
			get
			{
				return this.trusts[this.current];
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.trusts[this.current];
			}
		}

		public void Reset()
		{
			this.current = -1;
		}

		[SecuritySafeCritical]
		public bool MoveNext()
		{
			if (this.current == this.trusts.Count - 1)
			{
				return false;
			}
			this.current++;
			return true;
		}

		internal ApplicationTrustEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ApplicationTrustCollection trusts;

		private int current;
	}
}
