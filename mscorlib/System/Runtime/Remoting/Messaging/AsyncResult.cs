using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class AsyncResult : IAsyncResult, IMessageSink
	{
		internal AsyncResult()
		{
		}

		public virtual object AsyncState
		{
			get
			{
				return this.async_state;
			}
		}

		public virtual WaitHandle AsyncWaitHandle
		{
			get
			{
				WaitHandle waitHandle;
				lock (this)
				{
					if (this.handle == null)
					{
						this.handle = new ManualResetEvent(this.completed);
					}
					waitHandle = this.handle;
				}
				return waitHandle;
			}
		}

		public virtual bool CompletedSynchronously
		{
			get
			{
				return this.sync_completed;
			}
		}

		public virtual bool IsCompleted
		{
			get
			{
				return this.completed;
			}
		}

		public bool EndInvokeCalled
		{
			get
			{
				return this.endinvoke_called;
			}
			set
			{
				this.endinvoke_called = value;
			}
		}

		public virtual object AsyncDelegate
		{
			get
			{
				return this.async_delegate;
			}
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		public virtual IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
		{
			throw new NotSupportedException();
		}

		public virtual IMessage GetReplyMessage()
		{
			return this.reply_message;
		}

		public virtual void SetMessageCtrl(IMessageCtrl mc)
		{
			this.message_ctrl = mc;
		}

		internal void SetCompletedSynchronously(bool completed)
		{
			this.sync_completed = completed;
		}

		internal IMessage EndInvoke()
		{
			lock (this)
			{
				if (this.completed)
				{
					return this.reply_message;
				}
			}
			this.AsyncWaitHandle.WaitOne();
			return this.reply_message;
		}

		public virtual IMessage SyncProcessMessage(IMessage msg)
		{
			this.reply_message = msg;
			lock (this)
			{
				this.completed = true;
				if (this.handle != null)
				{
					((ManualResetEvent)this.AsyncWaitHandle).Set();
				}
			}
			if (this.async_callback != null)
			{
				AsyncCallback asyncCallback = (AsyncCallback)this.async_callback;
				asyncCallback(this);
			}
			return null;
		}

		internal MonoMethodMessage CallMessage
		{
			get
			{
				return this.call_message;
			}
			set
			{
				this.call_message = value;
			}
		}

		private object async_state;

		private WaitHandle handle;

		private object async_delegate;

		private IntPtr data;

		private object object_data;

		private bool sync_completed;

		private bool completed;

		private bool endinvoke_called;

		private object async_callback;

		private ExecutionContext current;

		private ExecutionContext original;

		private int gchandle;

		private MonoMethodMessage call_message;

		private IMessageCtrl message_ctrl;

		private IMessage reply_message;
	}
}
