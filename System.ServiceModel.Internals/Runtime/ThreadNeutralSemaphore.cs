using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Runtime
{
	internal class ThreadNeutralSemaphore
	{
		public ThreadNeutralSemaphore(int maxCount)
			: this(maxCount, null)
		{
		}

		public ThreadNeutralSemaphore(int maxCount, Func<Exception> abortedExceptionGenerator)
		{
			this.maxCount = maxCount;
			this.abortedExceptionGenerator = abortedExceptionGenerator;
		}

		private static Action<object, TimeoutException> EnteredAsyncCallback
		{
			get
			{
				if (ThreadNeutralSemaphore.enteredAsyncCallback == null)
				{
					ThreadNeutralSemaphore.enteredAsyncCallback = new Action<object, TimeoutException>(ThreadNeutralSemaphore.OnEnteredAsync);
				}
				return ThreadNeutralSemaphore.enteredAsyncCallback;
			}
		}

		private Queue<AsyncWaitHandle> Waiters
		{
			get
			{
				if (this.waiters == null)
				{
					this.waiters = new Queue<AsyncWaitHandle>();
				}
				return this.waiters;
			}
		}

		public bool EnterAsync(TimeSpan timeout, FastAsyncCallback callback, object state)
		{
			AsyncWaitHandle asyncWaitHandle = null;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (this.aborted)
				{
					throw Fx.Exception.AsError(this.CreateObjectAbortedException());
				}
				if (this.count < this.maxCount)
				{
					this.count++;
					return true;
				}
				asyncWaitHandle = new AsyncWaitHandle();
				this.Waiters.Enqueue(asyncWaitHandle);
			}
			return asyncWaitHandle.WaitAsync(ThreadNeutralSemaphore.EnteredAsyncCallback, new ThreadNeutralSemaphore.EnterAsyncData(this, asyncWaitHandle, callback, state), timeout);
		}

		private static void OnEnteredAsync(object state, TimeoutException exception)
		{
			ThreadNeutralSemaphore.EnterAsyncData enterAsyncData = (ThreadNeutralSemaphore.EnterAsyncData)state;
			ThreadNeutralSemaphore semaphore = enterAsyncData.Semaphore;
			Exception ex = exception;
			if (exception != null && !semaphore.RemoveWaiter(enterAsyncData.Waiter))
			{
				ex = null;
			}
			if (semaphore.aborted)
			{
				ex = semaphore.CreateObjectAbortedException();
			}
			enterAsyncData.Callback(enterAsyncData.State, ex);
		}

		public bool TryEnter()
		{
			object thisLock = this.ThisLock;
			bool flag2;
			lock (thisLock)
			{
				if (this.count < this.maxCount)
				{
					this.count++;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		public void Enter(TimeSpan timeout)
		{
			if (!this.TryEnter(timeout))
			{
				throw Fx.Exception.AsError(ThreadNeutralSemaphore.CreateEnterTimedOutException(timeout));
			}
		}

		public bool TryEnter(TimeSpan timeout)
		{
			AsyncWaitHandle asyncWaitHandle = this.EnterCore();
			if (asyncWaitHandle == null)
			{
				return true;
			}
			bool flag = !asyncWaitHandle.Wait(timeout);
			if (this.aborted)
			{
				throw Fx.Exception.AsError(this.CreateObjectAbortedException());
			}
			if (flag && !this.RemoveWaiter(asyncWaitHandle))
			{
				flag = false;
			}
			return !flag;
		}

		internal static TimeoutException CreateEnterTimedOutException(TimeSpan timeout)
		{
			return new TimeoutException(InternalSR.LockTimeoutExceptionMessage(timeout));
		}

		private Exception CreateObjectAbortedException()
		{
			if (this.abortedExceptionGenerator != null)
			{
				return this.abortedExceptionGenerator();
			}
			return new OperationCanceledException("Thread Neutral Semaphore Aborted");
		}

		private bool RemoveWaiter(AsyncWaitHandle waiter)
		{
			bool flag = false;
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				for (int i = this.Waiters.Count; i > 0; i--)
				{
					AsyncWaitHandle asyncWaitHandle = this.Waiters.Dequeue();
					if (asyncWaitHandle == waiter)
					{
						flag = true;
					}
					else
					{
						this.Waiters.Enqueue(asyncWaitHandle);
					}
				}
			}
			return flag;
		}

		private AsyncWaitHandle EnterCore()
		{
			object thisLock = this.ThisLock;
			AsyncWaitHandle asyncWaitHandle;
			lock (thisLock)
			{
				if (this.aborted)
				{
					throw Fx.Exception.AsError(this.CreateObjectAbortedException());
				}
				if (this.count < this.maxCount)
				{
					this.count++;
					return null;
				}
				asyncWaitHandle = new AsyncWaitHandle();
				this.Waiters.Enqueue(asyncWaitHandle);
			}
			return asyncWaitHandle;
		}

		public int Exit()
		{
			int num = -1;
			object thisLock = this.ThisLock;
			AsyncWaitHandle asyncWaitHandle;
			lock (thisLock)
			{
				if (this.aborted)
				{
					return num;
				}
				if (this.count == 0)
				{
					string text = "Invalid Semaphore Exit";
					throw Fx.Exception.AsError(new SynchronizationLockException(text));
				}
				if (this.waiters == null || this.waiters.Count == 0)
				{
					this.count--;
					return this.count;
				}
				asyncWaitHandle = this.waiters.Dequeue();
				num = this.count;
			}
			asyncWaitHandle.Set();
			return num;
		}

		public void Abort()
		{
			object thisLock = this.ThisLock;
			lock (thisLock)
			{
				if (!this.aborted)
				{
					this.aborted = true;
					if (this.waiters != null)
					{
						while (this.waiters.Count > 0)
						{
							this.waiters.Dequeue().Set();
						}
					}
				}
			}
		}

		private static Action<object, TimeoutException> enteredAsyncCallback;

		private bool aborted;

		private Func<Exception> abortedExceptionGenerator;

		private int count;

		private int maxCount;

		private object ThisLock = new object();

		private Queue<AsyncWaitHandle> waiters;

		private class EnterAsyncData
		{
			public EnterAsyncData(ThreadNeutralSemaphore semaphore, AsyncWaitHandle waiter, FastAsyncCallback callback, object state)
			{
				this.Waiter = waiter;
				this.Semaphore = semaphore;
				this.Callback = callback;
				this.State = state;
			}

			public ThreadNeutralSemaphore Semaphore { get; set; }

			public AsyncWaitHandle Waiter { get; set; }

			public FastAsyncCallback Callback { get; set; }

			public object State { get; set; }
		}
	}
}
