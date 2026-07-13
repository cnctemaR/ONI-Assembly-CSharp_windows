using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	internal sealed class WeakBox
	{
		[Nullable(2)]
		public object Value;
	}
}
