using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class KeyContainerPermissionAccessEntryEnumerator : IEnumerator
	{
		internal KeyContainerPermissionAccessEntryEnumerator(ArrayList list)
		{
			this.e = list.GetEnumerator();
		}

		public KeyContainerPermissionAccessEntry Current
		{
			get
			{
				return (KeyContainerPermissionAccessEntry)this.e.Current;
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

		internal KeyContainerPermissionAccessEntryEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator e;
	}
}
