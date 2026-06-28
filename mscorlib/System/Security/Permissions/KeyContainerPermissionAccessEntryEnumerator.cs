using System;
using System.Collections;
using System.Runtime.InteropServices;

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

		object IEnumerator.Current
		{
			get
			{
				return this.e.Current;
			}
		}

		public KeyContainerPermissionAccessEntry Current
		{
			get
			{
				return (KeyContainerPermissionAccessEntry)this.e.Current;
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
