using System;
using System.Threading;

namespace Microsoft.Internal
{
	internal sealed class Lock : IDisposable
	{
		public void EnterReadLock()
		{
			this._thisLock.EnterReadLock();
		}

		public void EnterWriteLock()
		{
			this._thisLock.EnterWriteLock();
		}

		public void ExitReadLock()
		{
			this._thisLock.ExitReadLock();
		}

		public void ExitWriteLock()
		{
			this._thisLock.ExitWriteLock();
		}

		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
			{
				this._thisLock.Dispose();
			}
		}

		private ReaderWriterLockSlim _thisLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

		private int _isDisposed;
	}
}
