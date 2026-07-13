using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	[NullableContext(2)]
	[Nullable(0)]
	internal readonly struct DataScope : IDisposable
	{
		public object Data
		{
			get
			{
				return this.data;
			}
		}

		[NullableContext(1)]
		public DataScope(ScopeHandlerBase handler, [Nullable(2)] object data)
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

		private readonly ScopeHandlerBase handler;

		private readonly object data;
	}
}
