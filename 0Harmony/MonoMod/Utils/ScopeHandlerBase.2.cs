using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(2)]
	[Nullable(0)]
	internal abstract class ScopeHandlerBase<T> : ScopeHandlerBase
	{
		public sealed override void EndScope(object data)
		{
			this.EndScope((T)((object)data));
		}

		[NullableContext(1)]
		public abstract void EndScope(T data);
	}
}
