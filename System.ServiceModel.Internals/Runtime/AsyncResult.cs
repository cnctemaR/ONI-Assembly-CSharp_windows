using System;
using System.Threading;

namespace System.Runtime
{
	internal abstract class AsyncResult : IAsyncResult
	{
		protected AsyncResult(AsyncCallback callback, object state)
		{
			this.callback = callback;
			this.state = state;
			this.thisLock = new object();
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
				if (this.manualResetEvent != null)
				{
					return this.manualResetEvent;
				}
				object obj = this.ThisLock;
				lock (obj)
				{
					if (this.manualResetEvent == null)
					{
						this.manualResetEvent = new ManualResetEvent(this.isCompleted);
					}
				}
				return this.manualResetEvent;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this.completedSynchronously;
			}
		}

		public bool HasCallback
		{
			get
			{
				return this.callback != null;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this.isCompleted;
			}
		}

		protected Action<AsyncResult, Exception> OnCompleting { get; set; }

		private object ThisLock
		{
			get
			{
				return this.thisLock;
			}
		}

		protected Action<AsyncCallback, IAsyncResult> VirtualCallback { get; set; }

		protected void Complete(bool completedSynchronously)
		{
			if (this.isCompleted)
			{
				throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.AsyncResultCompletedTwice(base.GetType())));
			}
			this.completedSynchronously = completedSynchronously;
			if (this.OnCompleting != null)
			{
				try
				{
					this.OnCompleting(this, this.exception);
				}
				catch (Exception ex)
				{
					if (Fx.IsFatal(ex))
					{
						throw;
					}
					this.exception = ex;
				}
			}
			if (completedSynchronously)
			{
				this.isCompleted = true;
			}
			else
			{
				object obj = this.ThisLock;
				lock (obj)
				{
					this.isCompleted = true;
					if (this.manualResetEvent != null)
					{
						this.manualResetEvent.Set();
					}
				}
			}
			if (this.callback != null)
			{
				try
				{
					if (this.VirtualCallback != null)
					{
						this.VirtualCallback(this.callback, this);
					}
					else
					{
						this.callback(this);
					}
				}
				catch (Exception ex2)
				{
					if (Fx.IsFatal(ex2))
					{
						throw;
					}
					throw Fx.Exception.AsError(new CallbackException("Async Callback Threw Exception", ex2));
				}
			}
		}

		protected void Complete(bool completedSynchronously, Exception exception)
		{
			this.exception = exception;
			this.Complete(completedSynchronously);
		}

		private static void AsyncCompletionWrapperCallback(IAsyncResult result)
		{
			if (result == null)
			{
				throw Fx.Exception.AsError(new InvalidOperationException("Invalid Null Async Result"));
			}
			if (result.CompletedSynchronously)
			{
				return;
			}
			AsyncResult asyncResult = (AsyncResult)result.AsyncState;
			if (!asyncResult.OnContinueAsyncCompletion(result))
			{
				return;
			}
			AsyncResult.AsyncCompletion nextCompletion = asyncResult.GetNextCompletion();
			if (nextCompletion == null)
			{
				AsyncResult.ThrowInvalidAsyncResult(result);
			}
			bool flag = false;
			Exception ex = null;
			try
			{
				flag = nextCompletion(result);
			}
			catch (Exception ex2)
			{
				if (Fx.IsFatal(ex2))
				{
					throw;
				}
				flag = true;
				ex = ex2;
			}
			if (flag)
			{
				asyncResult.Complete(false, ex);
			}
		}

		protected virtual bool OnContinueAsyncCompletion(IAsyncResult result)
		{
			return true;
		}

		protected void SetBeforePrepareAsyncCompletionAction(Action beforePrepareAsyncCompletionAction)
		{
			this.beforePrepareAsyncCompletionAction = beforePrepareAsyncCompletionAction;
		}

		protected void SetCheckSyncValidationFunc(Func<IAsyncResult, bool> checkSyncValidationFunc)
		{
			this.checkSyncValidationFunc = checkSyncValidationFunc;
		}

		protected AsyncCallback PrepareAsyncCompletion(AsyncResult.AsyncCompletion callback)
		{
			if (this.beforePrepareAsyncCompletionAction != null)
			{
				this.beforePrepareAsyncCompletionAction();
			}
			this.nextAsyncCompletion = callback;
			if (AsyncResult.asyncCompletionWrapperCallback == null)
			{
				AsyncResult.asyncCompletionWrapperCallback = Fx.ThunkCallback(new AsyncCallback(AsyncResult.AsyncCompletionWrapperCallback));
			}
			return AsyncResult.asyncCompletionWrapperCallback;
		}

		protected bool CheckSyncContinue(IAsyncResult result)
		{
			AsyncResult.AsyncCompletion asyncCompletion;
			return this.TryContinueHelper(result, out asyncCompletion);
		}

		protected bool SyncContinue(IAsyncResult result)
		{
			AsyncResult.AsyncCompletion asyncCompletion;
			return this.TryContinueHelper(result, out asyncCompletion) && asyncCompletion(result);
		}

		private bool TryContinueHelper(IAsyncResult result, out AsyncResult.AsyncCompletion callback)
		{
			if (result == null)
			{
				throw Fx.Exception.AsError(new InvalidOperationException("Invalid Null Async Result"));
			}
			callback = null;
			if (this.checkSyncValidationFunc != null)
			{
				if (!this.checkSyncValidationFunc(result))
				{
					return false;
				}
			}
			else if (!result.CompletedSynchronously)
			{
				return false;
			}
			callback = this.GetNextCompletion();
			if (callback == null)
			{
				AsyncResult.ThrowInvalidAsyncResult("Only call Check/SyncContinue once per async operation (once per PrepareAsyncCompletion).");
			}
			return true;
		}

		private AsyncResult.AsyncCompletion GetNextCompletion()
		{
			AsyncResult.AsyncCompletion asyncCompletion = this.nextAsyncCompletion;
			this.nextAsyncCompletion = null;
			return asyncCompletion;
		}

		protected static void ThrowInvalidAsyncResult(IAsyncResult result)
		{
			throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.InvalidAsyncResultImplementation(result.GetType())));
		}

		protected static void ThrowInvalidAsyncResult(string debugText)
		{
			string text = "Invalid Async Result Implementation Generic";
			throw Fx.Exception.AsError(new InvalidOperationException(text));
		}

		protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResult
		{
			if (result == null)
			{
				throw Fx.Exception.ArgumentNull("result");
			}
			TAsyncResult tasyncResult = result as TAsyncResult;
			if (tasyncResult == null)
			{
				throw Fx.Exception.Argument("result", "Invalid Async Result");
			}
			if (tasyncResult.endCalled)
			{
				throw Fx.Exception.AsError(new InvalidOperationException("Async Result Already Ended"));
			}
			tasyncResult.endCalled = true;
			if (!tasyncResult.isCompleted)
			{
				tasyncResult.AsyncWaitHandle.WaitOne();
			}
			if (tasyncResult.manualResetEvent != null)
			{
				tasyncResult.manualResetEvent.Close();
			}
			if (tasyncResult.exception != null)
			{
				throw Fx.Exception.AsError(tasyncResult.exception);
			}
			return tasyncResult;
		}

		private static AsyncCallback asyncCompletionWrapperCallback;

		private AsyncCallback callback;

		private bool completedSynchronously;

		private bool endCalled;

		private Exception exception;

		private bool isCompleted;

		private AsyncResult.AsyncCompletion nextAsyncCompletion;

		private object state;

		private Action beforePrepareAsyncCompletionAction;

		private Func<IAsyncResult, bool> checkSyncValidationFunc;

		private ManualResetEvent manualResetEvent;

		private object thisLock;

		protected delegate bool AsyncCompletion(IAsyncResult result);
	}
}
