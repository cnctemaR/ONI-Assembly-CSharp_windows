using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Threading
{
	[ComVisible(false)]
	[DebuggerDisplay("Initial Count={InitialCount}, Current Count={CurrentCount}")]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class CountdownEvent : IDisposable
	{
		public CountdownEvent(int initialCount)
		{
			if (initialCount < 0)
			{
				throw new ArgumentOutOfRangeException("initialCount");
			}
			this.m_initialCount = initialCount;
			this.m_currentCount = initialCount;
			this.m_event = new ManualResetEventSlim();
			if (initialCount == 0)
			{
				this.m_event.Set();
			}
		}

		public int CurrentCount
		{
			get
			{
				int currentCount = this.m_currentCount;
				if (currentCount >= 0)
				{
					return currentCount;
				}
				return 0;
			}
		}

		public int InitialCount
		{
			get
			{
				return this.m_initialCount;
			}
		}

		public bool IsSet
		{
			get
			{
				return this.m_currentCount <= 0;
			}
		}

		public WaitHandle WaitHandle
		{
			get
			{
				this.ThrowIfDisposed();
				return this.m_event.WaitHandle;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_event.Dispose();
				this.m_disposed = true;
			}
		}

		public bool Signal()
		{
			this.ThrowIfDisposed();
			if (this.m_currentCount <= 0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Invalid attempt made to decrement the event's count below zero."));
			}
			int num = Interlocked.Decrement(ref this.m_currentCount);
			if (num == 0)
			{
				this.m_event.Set();
				return true;
			}
			if (num < 0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Invalid attempt made to decrement the event's count below zero."));
			}
			return false;
		}

		public bool Signal(int signalCount)
		{
			if (signalCount <= 0)
			{
				throw new ArgumentOutOfRangeException("signalCount");
			}
			this.ThrowIfDisposed();
			SpinWait spinWait = default(SpinWait);
			int currentCount;
			for (;;)
			{
				currentCount = this.m_currentCount;
				if (currentCount < signalCount)
				{
					break;
				}
				if (Interlocked.CompareExchange(ref this.m_currentCount, currentCount - signalCount, currentCount) == currentCount)
				{
					goto IL_0055;
				}
				spinWait.SpinOnce();
			}
			throw new InvalidOperationException(Environment.GetResourceString("Invalid attempt made to decrement the event's count below zero."));
			IL_0055:
			if (currentCount == signalCount)
			{
				this.m_event.Set();
				return true;
			}
			return false;
		}

		public void AddCount()
		{
			this.AddCount(1);
		}

		public bool TryAddCount()
		{
			return this.TryAddCount(1);
		}

		public void AddCount(int signalCount)
		{
			if (!this.TryAddCount(signalCount))
			{
				throw new InvalidOperationException(Environment.GetResourceString("The event is already signaled and cannot be incremented."));
			}
		}

		public bool TryAddCount(int signalCount)
		{
			if (signalCount <= 0)
			{
				throw new ArgumentOutOfRangeException("signalCount");
			}
			this.ThrowIfDisposed();
			SpinWait spinWait = default(SpinWait);
			for (;;)
			{
				int currentCount = this.m_currentCount;
				if (currentCount <= 0)
				{
					break;
				}
				if (currentCount > 2147483647 - signalCount)
				{
					goto Block_3;
				}
				if (Interlocked.CompareExchange(ref this.m_currentCount, currentCount + signalCount, currentCount) == currentCount)
				{
					return true;
				}
				spinWait.SpinOnce();
			}
			return false;
			Block_3:
			throw new InvalidOperationException(Environment.GetResourceString("The increment operation would cause the CurrentCount to overflow."));
		}

		public void Reset()
		{
			this.Reset(this.m_initialCount);
		}

		public void Reset(int count)
		{
			this.ThrowIfDisposed();
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.m_currentCount = count;
			this.m_initialCount = count;
			if (count == 0)
			{
				this.m_event.Set();
				return;
			}
			this.m_event.Reset();
		}

		public void Wait()
		{
			this.Wait(-1, default(CancellationToken));
		}

		public void Wait(CancellationToken cancellationToken)
		{
			this.Wait(-1, cancellationToken);
		}

		public bool Wait(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.Wait((int)num, default(CancellationToken));
		}

		public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.Wait((int)num, cancellationToken);
		}

		public bool Wait(int millisecondsTimeout)
		{
			return this.Wait(millisecondsTimeout, default(CancellationToken));
		}

		public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			this.ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();
			bool flag = this.IsSet;
			if (!flag)
			{
				flag = this.m_event.Wait(millisecondsTimeout, cancellationToken);
			}
			return flag;
		}

		private void ThrowIfDisposed()
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("CountdownEvent");
			}
		}

		private int m_initialCount;

		private volatile int m_currentCount;

		private ManualResetEventSlim m_event;

		private volatile bool m_disposed;
	}
}
