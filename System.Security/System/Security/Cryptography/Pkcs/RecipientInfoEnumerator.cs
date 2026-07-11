using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class RecipientInfoEnumerator : IEnumerator
	{
		internal RecipientInfoEnumerator(IEnumerable enumerable)
		{
			this.enumerator = enumerable.GetEnumerator();
		}

		public RecipientInfo Current
		{
			get
			{
				return (RecipientInfo)this.enumerator.Current;
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

		internal RecipientInfoEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator enumerator;
	}
}
