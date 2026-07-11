using System;
using System.Collections;
using Unity;

namespace System.Security.AccessControl
{
	public sealed class AceEnumerator : IEnumerator
	{
		internal AceEnumerator(GenericAcl owner)
		{
			this.current = -1;
			base..ctor();
			this.owner = owner;
		}

		public GenericAce Current
		{
			get
			{
				if (this.current >= 0)
				{
					return this.owner[this.current];
				}
				return null;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public bool MoveNext()
		{
			if (this.current + 1 == this.owner.Count)
			{
				return false;
			}
			this.current++;
			return true;
		}

		public void Reset()
		{
			this.current = -1;
		}

		internal AceEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private GenericAcl owner;

		private int current;
	}
}
