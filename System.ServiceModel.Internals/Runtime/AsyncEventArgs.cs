using System;

namespace System.Runtime
{
	internal abstract class AsyncEventArgs : IAsyncEventArgs
	{
		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public object AsyncState
		{
			get
			{
				return this.asyncState;
			}
		}

		private AsyncEventArgs.OperationState State
		{
			set
			{
				if (value != AsyncEventArgs.OperationState.PendingCompletion)
				{
					if (value - AsyncEventArgs.OperationState.CompletedSynchronously <= 1)
					{
						if (this.state != AsyncEventArgs.OperationState.PendingCompletion)
						{
							throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.AsyncEventArgsCompletedTwice(base.GetType())));
						}
					}
				}
				else if (this.state == AsyncEventArgs.OperationState.PendingCompletion)
				{
					throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.AsyncEventArgsCompletionPending(base.GetType())));
				}
				this.state = value;
			}
		}

		public void Complete(bool completedSynchronously)
		{
			this.Complete(completedSynchronously, null);
		}

		public virtual void Complete(bool completedSynchronously, Exception exception)
		{
			this.exception = exception;
			if (completedSynchronously)
			{
				this.State = AsyncEventArgs.OperationState.CompletedSynchronously;
				return;
			}
			this.State = AsyncEventArgs.OperationState.CompletedAsynchronously;
			this.callback(this);
		}

		protected void SetAsyncState(AsyncEventArgsCallback callback, object state)
		{
			if (callback == null)
			{
				throw Fx.Exception.ArgumentNull("callback");
			}
			this.State = AsyncEventArgs.OperationState.PendingCompletion;
			this.asyncState = state;
			this.callback = callback;
		}

		private AsyncEventArgs.OperationState state;

		private object asyncState;

		private AsyncEventArgsCallback callback;

		private Exception exception;

		private enum OperationState
		{
			Created,
			PendingCompletion,
			CompletedSynchronously,
			CompletedAsynchronously
		}
	}
}
