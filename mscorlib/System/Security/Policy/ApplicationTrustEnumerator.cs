using System;
using System.Collections;
using System.Runtime.InteropServices;

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

		public bool MoveNext()
		{
			return this.e.MoveNext();
		}

		public void Reset()
		{
			this.e.Reset();
		}

		private IEnumerator e;
	}
}
