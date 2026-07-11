using System;
using System.Threading;

namespace System.Net
{
	internal class DnsAsyncResult : IAsyncResult
	{
		public DnsAsyncResult(AsyncCallback cb, object state)
		{
			this.callback = cb;
			this.state = state;
		}

		public void SetCompleted(bool synch, IPHostEntry entry, Exception e)
		{
			this.synch = synch;
			this.entry = entry;
			this.exc = e;
			lock (this)
			{
				if (this.is_completed)
				{
					return;
				}
				this.is_completed = true;
				if (this.handle != null)
				{
					this.handle.Set();
				}
			}
			if (this.callback != null)
			{
				ThreadPool.QueueUserWorkItem(DnsAsyncResult.internal_cb, this);
			}
		}

		public void SetCompleted(bool synch, Exception e)
		{
			this.SetCompleted(synch, null, e);
		}

		public void SetCompleted(bool synch, IPHostEntry entry)
		{
			this.SetCompleted(synch, entry, null);
		}

		private static void CB(object _this)
		{
			DnsAsyncResult dnsAsyncResult = (DnsAsyncResult)_this;
			dnsAsyncResult.callback(dnsAsyncResult);
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
				lock (this)
				{
					if (this.handle == null)
					{
						this.handle = new ManualResetEvent(this.is_completed);
					}
				}
				return this.handle;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exc;
			}
		}

		public IPHostEntry HostEntry
		{
			get
			{
				return this.entry;
			}
		}

		public bool CompletedSynchronously
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
				bool flag2;
				lock (this)
				{
					flag2 = this.is_completed;
				}
				return flag2;
			}
		}

		private static WaitCallback internal_cb = new WaitCallback(DnsAsyncResult.CB);

		private ManualResetEvent handle;

		private bool synch;

		private bool is_completed;

		private AsyncCallback callback;

		private object state;

		private IPHostEntry entry;

		private Exception exc;
	}
}
