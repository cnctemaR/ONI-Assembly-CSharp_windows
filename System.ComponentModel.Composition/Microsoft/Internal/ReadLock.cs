using System;
using System.Threading;

namespace Microsoft.Internal
{
	internal struct ReadLock : IDisposable
	{
		public ReadLock(Lock @lock)
		{
			this._isDisposed = 0;
			this._lock = @lock;
			this._lock.EnterReadLock();
		}

		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
			{
				this._lock.ExitReadLock();
			}
		}

		private readonly Lock _lock;

		private int _isDisposed;
	}
}
