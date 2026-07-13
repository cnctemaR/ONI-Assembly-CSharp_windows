using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(2)]
	[Nullable(0)]
	internal sealed class LazyDisposable : IDisposable
	{
		public event Action OnDispose;

		public LazyDisposable()
		{
		}

		[NullableContext(1)]
		public LazyDisposable(Action a)
			: this()
		{
			this.OnDispose += a;
		}

		public void Dispose()
		{
			Action onDispose = this.OnDispose;
			if (onDispose == null)
			{
				return;
			}
			onDispose();
		}
	}
}
