using System;
using System.Collections.Generic;

namespace MonoMod.Utils
{
	internal sealed class WeakReferenceComparer : EqualityComparer<WeakReference>
	{
		public override bool Equals(WeakReference x, WeakReference y)
		{
			return x.SafeGetTarget() == y.SafeGetTarget() && x.SafeGetIsAlive() == y.SafeGetIsAlive();
		}

		public override int GetHashCode(WeakReference obj)
		{
			object obj2 = obj.SafeGetTarget();
			if (obj2 == null)
			{
				return 0;
			}
			return obj2.GetHashCode();
		}
	}
}
