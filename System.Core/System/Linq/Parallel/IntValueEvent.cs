using System;
using System.Threading;

namespace System.Linq.Parallel
{
	internal class IntValueEvent : ManualResetEventSlim
	{
		internal IntValueEvent()
			: base(false)
		{
			this.Value = 0;
		}

		internal void Set(int index)
		{
			this.Value = index;
			base.Set();
		}

		internal int Value;
	}
}
