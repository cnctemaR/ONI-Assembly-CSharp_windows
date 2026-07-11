using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;

namespace System.Runtime
{
	internal class AsyncWaitHandle
	{
		public AsyncWaitHandle()
			: this(EventResetMode.AutoReset)
		{
		}

		public AsyncWaitHandle(EventResetMode resetMode)
		{
			this.resetMode = resetMode;
			this.syncObject = new object();
		}

		public bool WaitAsync(Action<object, TimeoutException> callback, object state, TimeSpan timeout)
		{
			if (!this.isSignaled || (this.isSignaled && this.resetMode == EventResetMode.AutoReset))
			{
				object obj = this.syncObject;
				lock (obj)
				{
					if (this.isSignaled && this.resetMode == EventResetMode.AutoReset)
					{
						this.isSignaled = false;
					}
					else if (!this.isSignaled)
					{
						AsyncWaitHandle.AsyncWaiter asyncWaiter = new AsyncWaitHandle.AsyncWaiter(this, callback, state);
						if (this.asyncWaiters == null)
						{
							this.asyncWaiters = new List<AsyncWaitHandle.AsyncWaiter>();
						}
						this.asyncWaiters.Add(asyncWaiter);
						if (timeout != TimeSpan.MaxValue)
						{
							if (AsyncWaitHandle.timerCompleteCallback == null)
							{
								AsyncWaitHandle.timerCompleteCallback = new Action<object>(AsyncWaitHandle.OnTimerComplete);
							}
							asyncWaiter.SetTimer(AsyncWaitHandle.timerCompleteCallback, asyncWaiter, timeout);
						}
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private static void OnTimerComplete(object state)
		{
			AsyncWaitHandle.AsyncWaiter asyncWaiter = (AsyncWaitHandle.AsyncWaiter)state;
			AsyncWaitHandle parent = asyncWaiter.Parent;
			bool flag = false;
			object obj = parent.syncObject;
			lock (obj)
			{
				if (parent.asyncWaiters != null && parent.asyncWaiters.Remove(asyncWaiter))
				{
					asyncWaiter.TimedOut = true;
					flag = true;
				}
			}
			asyncWaiter.CancelTimer();
			if (flag)
			{
				asyncWaiter.Call();
			}
		}

		public bool Wait(TimeSpan timeout)
		{
			if (!this.isSignaled || (this.isSignaled && this.resetMode == EventResetMode.AutoReset))
			{
				object obj = this.syncObject;
				lock (obj)
				{
					if (this.isSignaled && this.resetMode == EventResetMode.AutoReset)
					{
						this.isSignaled = false;
					}
					else if (!this.isSignaled)
					{
						bool flag2 = false;
						try
						{
							try
							{
							}
							finally
							{
								this.syncWaiterCount++;
								flag2 = true;
							}
							if (timeout == TimeSpan.MaxValue)
							{
								if (!Monitor.Wait(this.syncObject, -1))
								{
									return false;
								}
							}
							else if (!Monitor.Wait(this.syncObject, timeout))
							{
								return false;
							}
						}
						finally
						{
							if (flag2)
							{
								this.syncWaiterCount--;
							}
						}
					}
				}
				return true;
			}
			return true;
		}

		public void Set()
		{
			List<AsyncWaitHandle.AsyncWaiter> list = null;
			AsyncWaitHandle.AsyncWaiter asyncWaiter = null;
			if (!this.isSignaled)
			{
				object obj = this.syncObject;
				lock (obj)
				{
					if (!this.isSignaled)
					{
						if (this.resetMode == EventResetMode.ManualReset)
						{
							this.isSignaled = true;
							Monitor.PulseAll(this.syncObject);
							list = this.asyncWaiters;
							this.asyncWaiters = null;
						}
						else if (this.syncWaiterCount > 0)
						{
							Monitor.Pulse(this.syncObject);
						}
						else if (this.asyncWaiters != null && this.asyncWaiters.Count > 0)
						{
							asyncWaiter = this.asyncWaiters[0];
							this.asyncWaiters.RemoveAt(0);
						}
						else
						{
							this.isSignaled = true;
						}
					}
				}
			}
			if (list != null)
			{
				foreach (AsyncWaitHandle.AsyncWaiter asyncWaiter2 in list)
				{
					asyncWaiter2.CancelTimer();
					asyncWaiter2.Call();
				}
			}
			if (asyncWaiter != null)
			{
				asyncWaiter.CancelTimer();
				asyncWaiter.Call();
			}
		}

		public void Reset()
		{
			this.isSignaled = false;
		}

		private static Action<object> timerCompleteCallback;

		private List<AsyncWaitHandle.AsyncWaiter> asyncWaiters;

		private bool isSignaled;

		private EventResetMode resetMode;

		private object syncObject;

		private int syncWaiterCount;

		private class AsyncWaiter : ActionItem
		{
			[SecuritySafeCritical]
			public AsyncWaiter(AsyncWaitHandle parent, Action<object, TimeoutException> callback, object state)
			{
				this.Parent = parent;
				this.callback = callback;
				this.state = state;
			}

			public AsyncWaitHandle Parent { get; private set; }

			public bool TimedOut { get; set; }

			[SecuritySafeCritical]
			public void Call()
			{
				base.Schedule();
			}

			[SecurityCritical]
			protected override void Invoke()
			{
				this.callback(this.state, this.TimedOut ? new TimeoutException(InternalSR.TimeoutOnOperation(this.originalTimeout)) : null);
			}

			public void SetTimer(Action<object> callback, object state, TimeSpan timeout)
			{
				if (this.timer != null)
				{
					throw Fx.Exception.AsError(new InvalidOperationException("Must Cancel Old Timer"));
				}
				this.originalTimeout = timeout;
				this.timer = new IOThreadTimer(callback, state, false);
				this.timer.Set(timeout);
			}

			public void CancelTimer()
			{
				if (this.timer != null)
				{
					this.timer.Cancel();
					this.timer = null;
				}
			}

			[SecurityCritical]
			private Action<object, TimeoutException> callback;

			[SecurityCritical]
			private object state;

			private IOThreadTimer timer;

			private TimeSpan originalTimeout;
		}
	}
}
