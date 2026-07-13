using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct DataScope<[Nullable(2)] T> : IDisposable
	{
		public T Data
		{
			get
			{
				return this.data;
			}
		}

		public DataScope(ScopeHandlerBase<T> handler, T data)
		{
			this.handler = handler;
			this.data = data;
		}

		public void Dispose()
		{
			if (this.handler != null)
			{
				this.handler.EndScope(this.data);
			}
		}

		[Nullable(new byte[] { 2, 1 })]
		private readonly ScopeHandlerBase<T> handler;

		private readonly T data;
	}
}
