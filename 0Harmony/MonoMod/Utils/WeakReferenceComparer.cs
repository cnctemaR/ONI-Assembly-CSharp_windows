using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	internal sealed class WeakReferenceComparer : EqualityComparer<WeakReference>
	{
		[NullableContext(2)]
		public override bool Equals(WeakReference x, WeakReference y)
		{
			if (((x != null) ? x.SafeGetTarget() : null) == ((y != null) ? y.SafeGetTarget() : null))
			{
				bool? flag = ((x != null) ? new bool?(x.SafeGetIsAlive()) : null);
				bool? flag2 = ((y != null) ? new bool?(y.SafeGetIsAlive()) : null);
				return (flag.GetValueOrDefault() == flag2.GetValueOrDefault()) & (flag != null == (flag2 != null));
			}
			return false;
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
