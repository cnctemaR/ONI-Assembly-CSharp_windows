using System;
using System.Security.Permissions;
using System.Threading;
using Unity;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public sealed class AsyncOperation
	{
		private AsyncOperation(object userSuppliedState, SynchronizationContext syncContext)
		{
			this.userSuppliedState = userSuppliedState;
			this.syncContext = syncContext;
			this.alreadyCompleted = false;
			this.syncContext.OperationStarted();
		}

		~AsyncOperation()
		{
			if (!this.alreadyCompleted && this.syncContext != null)
			{
				this.syncContext.OperationCompleted();
			}
		}

		public object UserSuppliedState
		{
			get
			{
				return this.userSuppliedState;
			}
		}

		public SynchronizationContext SynchronizationContext
		{
			get
			{
				return this.syncContext;
			}
		}

		public void Post(SendOrPostCallback d, object arg)
		{
			this.VerifyNotCompleted();
			this.VerifyDelegateNotNull(d);
			this.syncContext.Post(d, arg);
		}

		public void PostOperationCompleted(SendOrPostCallback d, object arg)
		{
			this.Post(d, arg);
			this.OperationCompletedCore();
		}

		public void OperationCompleted()
		{
			this.VerifyNotCompleted();
			this.OperationCompletedCore();
		}

		private void OperationCompletedCore()
		{
			try
			{
				this.syncContext.OperationCompleted();
			}
			finally
			{
				this.alreadyCompleted = true;
				GC.SuppressFinalize(this);
			}
		}

		private void VerifyNotCompleted()
		{
			if (this.alreadyCompleted)
			{
				throw new InvalidOperationException(global::SR.GetString("This operation has already had OperationCompleted called on it and further calls are illegal."));
			}
		}

		private void VerifyDelegateNotNull(SendOrPostCallback d)
		{
			if (d == null)
			{
				throw new ArgumentNullException(global::SR.GetString("A non-null SendOrPostCallback must be supplied."), "d");
			}
		}

		internal static AsyncOperation CreateOperation(object userSuppliedState, SynchronizationContext syncContext)
		{
			return new AsyncOperation(userSuppliedState, syncContext);
		}

		internal AsyncOperation()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private SynchronizationContext syncContext;

		private object userSuppliedState;

		private bool alreadyCompleted;
	}
}
