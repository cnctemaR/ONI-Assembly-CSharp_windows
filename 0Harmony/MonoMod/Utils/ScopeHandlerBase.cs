using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	internal abstract class ScopeHandlerBase
	{
		[NullableContext(2)]
		public abstract void EndScope(object data);
	}
}
