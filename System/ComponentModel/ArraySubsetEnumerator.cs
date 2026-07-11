using System;
using System.Collections;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	internal class ArraySubsetEnumerator : IEnumerator
	{
		public ArraySubsetEnumerator(Array array, int count)
		{
			this.array = array;
			this.total = count;
			this.current = -1;
		}

		public bool MoveNext()
		{
			if (this.current < this.total - 1)
			{
				this.current++;
				return true;
			}
			return false;
		}

		public void Reset()
		{
			this.current = -1;
		}

		public object Current
		{
			get
			{
				if (this.current == -1)
				{
					throw new InvalidOperationException();
				}
				return this.array.GetValue(this.current);
			}
		}

		private Array array;

		private int total;

		private int current;
	}
}
