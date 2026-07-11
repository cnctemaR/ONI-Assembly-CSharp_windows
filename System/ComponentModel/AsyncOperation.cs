using System;
using System.Threading;

namespace System.ComponentModel
{
	public sealed class AsyncOperation
	{
		internal AsyncOperation(SynchronizationContext ctx, object state)
		{
			this.ctx = ctx;
			this.state = state;
			ctx.OperationStarted();
		}

		~AsyncOperation()
		{
			if (!this.done && this.ctx != null)
			{
				this.ctx.OperationCompleted();
			}
		}

		public SynchronizationContext SynchronizationContext
		{
			get
			{
				return this.ctx;
			}
		}

		public object UserSuppliedState
		{
			get
			{
				return this.state;
			}
		}

		public void OperationCompleted()
		{
			if (this.done)
			{
				throw new InvalidOperationException("This task is already completed. Multiple call to OperationCompleted is not allowed.");
			}
			this.ctx.OperationCompleted();
			this.done = true;
		}

		public void Post(SendOrPostCallback d, object arg)
		{
			if (this.done)
			{
				throw new InvalidOperationException("This task is already completed. Multiple call to Post is not allowed.");
			}
			this.ctx.Post(d, arg);
		}

		public void PostOperationCompleted(SendOrPostCallback d, object arg)
		{
			if (this.done)
			{
				throw new InvalidOperationException("This task is already completed. Multiple call to PostOperationCompleted is not allowed.");
			}
			this.Post(d, arg);
			this.OperationCompleted();
		}

		private SynchronizationContext ctx;

		private object state;

		private bool done;
	}
}
