using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	internal sealed class LazyDisposable<[Nullable(2)] T> : IDisposable
	{
		[Nullable(new byte[] { 2, 1 })]
		[field: Nullable(new byte[] { 2, 1 })]
		public event Action<T> OnDispose;

		[NullableContext(1)]
		public LazyDisposable(T instance)
		{
			this.Instance = instance;
		}

		[NullableContext(1)]
		public LazyDisposable(T instance, Action<T> a)
			: this(instance)
		{
			this.OnDispose += a;
		}

		public void Dispose()
		{
			Action<T> onDispose = this.OnDispose;
			if (onDispose != null)
			{
				onDispose(this.Instance);
			}
			this.Instance = default(T);
		}

		[Nullable(2)]
		private T Instance;
	}
}
