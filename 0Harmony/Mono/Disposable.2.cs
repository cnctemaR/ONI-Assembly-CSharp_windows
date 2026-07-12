using System;

namespace Mono
{
	internal struct Disposable<T> : IDisposable where T : class, IDisposable
	{
		public Disposable(T value, bool owned)
		{
			this.value = value;
			this.owned = owned;
		}

		public void Dispose()
		{
			if (this.value != null && this.owned)
			{
				this.value.Dispose();
			}
		}

		internal readonly T value;

		private readonly bool owned;
	}
}
