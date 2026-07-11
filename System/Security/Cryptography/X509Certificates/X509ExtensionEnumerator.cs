using System;
using System.Collections;
using Unity;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ExtensionEnumerator : IEnumerator
	{
		internal X509ExtensionEnumerator(ArrayList list)
		{
			this.enumerator = list.GetEnumerator();
		}

		public X509Extension Current
		{
			get
			{
				return (X509Extension)this.enumerator.Current;
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

		internal X509ExtensionEnumerator()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator enumerator;
	}
}
