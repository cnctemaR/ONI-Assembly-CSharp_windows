using System;
using System.Threading;

namespace System.Text.RegularExpressions
{
	internal sealed class SharedReference
	{
		internal object Get()
		{
			if (Interlocked.Exchange(ref this._locked, 1) == 0)
			{
				object target = this._ref.Target;
				this._locked = 0;
				return target;
			}
			return null;
		}

		internal void Cache(object obj)
		{
			if (Interlocked.Exchange(ref this._locked, 1) == 0)
			{
				this._ref.Target = obj;
				this._locked = 0;
			}
		}

		private WeakReference _ref = new WeakReference(null);

		private int _locked;
	}
}
