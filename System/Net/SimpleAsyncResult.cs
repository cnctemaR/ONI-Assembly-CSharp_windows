using System;
using System.Threading;

namespace System.Net
{
	internal class SimpleAsyncResult : IAsyncResult
	{
		private SimpleAsyncResult(SimpleAsyncCallback cb)
		{
			this.cb = cb;
		}

		protected SimpleAsyncResult(AsyncCallback cb, object state)
		{
			SimpleAsyncResult <>4__this = this;
			this.state = state;
			this.cb = delegate(SimpleAsyncResult result)
			{
				if (cb != null)
				{
					cb(<>4__this);
				}
			};
		}

		public static void Run(Func<SimpleAsyncResult, bool> func, SimpleAsyncCallback callback)
		{
			SimpleAsyncResult simpleAsyncResult = new SimpleAsyncResult(callback);
			try
			{
				if (!func(simpleAsyncResult))
				{
					simpleAsyncResult.SetCompleted(true);
				}
			}
			catch (Exception ex)
			{
				simpleAsyncResult.SetCompleted(true, ex);
			}
		}

		public static void RunWithLock(object locker, Func<SimpleAsyncResult, bool> func, SimpleAsyncCallback callback)
		{
			SimpleAsyncResult.Run(delegate(SimpleAsyncResult inner)
			{
				bool flag = func(inner);
				if (flag)
				{
					Monitor.Exit(locker);
				}
				return flag;
			}, delegate(SimpleAsyncResult inner)
			{
				if (inner.GotException)
				{
					if (inner.synch)
					{
						Monitor.Exit(locker);
					}
					callback(inner);
					return;
				}
				try
				{
					if (!inner.synch)
					{
						Monitor.Enter(locker);
					}
					callback(inner);
				}
				finally
				{
					Monitor.Exit(locker);
				}
			});
		}

		protected void Reset_internal()
		{
			this.callbackDone = false;
			this.exc = null;
			object obj = this.locker;
			lock (obj)
			{
				this.isCompleted = false;
				if (this.handle != null)
				{
					this.handle.Reset();
				}
			}
		}

		internal void SetCompleted(bool synch, Exception e)
		{
			this.SetCompleted_internal(synch, e);
			this.DoCallback_private();
		}

		internal void SetCompleted(bool synch)
		{
			this.SetCompleted_internal(synch);
			this.DoCallback_private();
		}

		private void SetCompleted_internal(bool synch, Exception e)
		{
			this.synch = synch;
			this.exc = e;
			object obj = this.locker;
			lock (obj)
			{
				this.isCompleted = true;
				if (this.handle != null)
				{
					this.handle.Set();
				}
			}
		}

		protected void SetCompleted_internal(bool synch)
		{
			this.SetCompleted_internal(synch, null);
		}

		private void DoCallback_private()
		{
			if (this.callbackDone)
			{
				return;
			}
			this.callbackDone = true;
			if (this.cb == null)
			{
				return;
			}
			this.cb(this);
		}

		protected void DoCallback_internal()
		{
			if (!this.callbackDone && this.cb != null)
			{
				this.callbackDone = true;
				this.cb(this);
			}
		}

		internal void WaitUntilComplete()
		{
			if (this.IsCompleted)
			{
				return;
			}
			this.AsyncWaitHandle.WaitOne();
		}

		internal bool WaitUntilComplete(int timeout, bool exitContext)
		{
			return this.IsCompleted || this.AsyncWaitHandle.WaitOne(timeout, exitContext);
		}

		public object AsyncState
		{
			get
			{
				return this.state;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				object obj = this.locker;
				lock (obj)
				{
					if (this.handle == null)
					{
						this.handle = new ManualResetEvent(this.isCompleted);
					}
				}
				return this.handle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				if (this.user_read_synch != null)
				{
					return this.user_read_synch.Value;
				}
				this.user_read_synch = new bool?(this.synch);
				return this.user_read_synch.Value;
			}
		}

		internal bool CompletedSynchronouslyPeek
		{
			get
			{
				return this.synch;
			}
		}

		public bool IsCompleted
		{
			get
			{
				object obj = this.locker;
				bool flag2;
				lock (obj)
				{
					flag2 = this.isCompleted;
				}
				return flag2;
			}
		}

		internal bool GotException
		{
			get
			{
				return this.exc != null;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.exc;
			}
		}

		private ManualResetEvent handle;

		private bool synch;

		private bool isCompleted;

		private readonly SimpleAsyncCallback cb;

		private object state;

		private bool callbackDone;

		private Exception exc;

		private object locker = new object();

		private bool? user_read_synch;
	}
}
