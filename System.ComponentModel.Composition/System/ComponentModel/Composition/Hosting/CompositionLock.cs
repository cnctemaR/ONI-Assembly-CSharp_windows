using System;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
	internal sealed class CompositionLock : IDisposable
	{
		public CompositionLock(bool isThreadSafe)
		{
			this._isThreadSafe = isThreadSafe;
			if (isThreadSafe)
			{
				this._stateLock = new Microsoft.Internal.Lock();
			}
		}

		public void Dispose()
		{
			if (this._isThreadSafe && Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
			{
				this._stateLock.Dispose();
			}
		}

		public bool IsThreadSafe
		{
			get
			{
				return this._isThreadSafe;
			}
		}

		private void EnterCompositionLock()
		{
			if (this._isThreadSafe)
			{
				Monitor.Enter(CompositionLock._compositionLock);
			}
		}

		private void ExitCompositionLock()
		{
			if (this._isThreadSafe)
			{
				Monitor.Exit(CompositionLock._compositionLock);
			}
		}

		public IDisposable LockComposition()
		{
			if (this._isThreadSafe)
			{
				return new CompositionLock.CompositionLockHolder(this);
			}
			return CompositionLock._EmptyLockHolder;
		}

		public IDisposable LockStateForRead()
		{
			if (this._isThreadSafe)
			{
				return new ReadLock(this._stateLock);
			}
			return CompositionLock._EmptyLockHolder;
		}

		public IDisposable LockStateForWrite()
		{
			if (this._isThreadSafe)
			{
				return new WriteLock(this._stateLock);
			}
			return CompositionLock._EmptyLockHolder;
		}

		private readonly Microsoft.Internal.Lock _stateLock;

		private static object _compositionLock = new object();

		private int _isDisposed;

		private bool _isThreadSafe;

		private static readonly CompositionLock.EmptyLockHolder _EmptyLockHolder = new CompositionLock.EmptyLockHolder();

		public sealed class CompositionLockHolder : IDisposable
		{
			public CompositionLockHolder(CompositionLock @lock)
			{
				this._lock = @lock;
				this._isDisposed = 0;
				this._lock.EnterCompositionLock();
			}

			public void Dispose()
			{
				if (Interlocked.CompareExchange(ref this._isDisposed, 1, 0) == 0)
				{
					this._lock.ExitCompositionLock();
				}
			}

			private CompositionLock _lock;

			private int _isDisposed;
		}

		private sealed class EmptyLockHolder : IDisposable
		{
			public void Dispose()
			{
			}
		}
	}
}
