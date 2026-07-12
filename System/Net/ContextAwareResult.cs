using System;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;

namespace System.Net
{
	internal class ContextAwareResult : LazyAsyncResult
	{
		private void SafeCaptureIdentity()
		{
			this._windowsIdentity = WindowsIdentity.GetCurrent();
		}

		internal WindowsIdentity Identity
		{
			get
			{
				if (base.InternalPeekCompleted)
				{
					if ((this._flags & ContextAwareResult.StateFlags.ThreadSafeContextCopy) == ContextAwareResult.StateFlags.None)
					{
						NetEventSource.Fail(this, "Called on completed result.", "Identity");
					}
					throw new InvalidOperationException("This operation cannot be performed on a completed asynchronous result object.");
				}
				if (this._windowsIdentity != null)
				{
					return this._windowsIdentity;
				}
				if ((this._flags & ContextAwareResult.StateFlags.CaptureIdentity) == ContextAwareResult.StateFlags.None)
				{
					NetEventSource.Fail(this, "No identity captured - specify captureIdentity.", "Identity");
				}
				if ((this._flags & ContextAwareResult.StateFlags.PostBlockFinished) == ContextAwareResult.StateFlags.None)
				{
					if (this._lock == null)
					{
						NetEventSource.Fail(this, "Must lock (StartPostingAsyncOp()) { ... FinishPostingAsyncOp(); } when calling Identity (unless it's only called after FinishPostingAsyncOp).", "Identity");
					}
					object @lock = this._lock;
					lock (@lock)
					{
					}
				}
				if (base.InternalPeekCompleted)
				{
					if ((this._flags & ContextAwareResult.StateFlags.ThreadSafeContextCopy) == ContextAwareResult.StateFlags.None)
					{
						NetEventSource.Fail(this, "Result became completed during call.", "Identity");
					}
					throw new InvalidOperationException("This operation cannot be performed on a completed asynchronous result object.");
				}
				return this._windowsIdentity;
			}
		}

		private void CleanupInternal()
		{
			if (this._windowsIdentity != null)
			{
				this._windowsIdentity.Dispose();
				this._windowsIdentity = null;
			}
		}

		internal ContextAwareResult(object myObject, object myState, AsyncCallback myCallBack)
			: this(false, false, myObject, myState, myCallBack)
		{
		}

		internal ContextAwareResult(bool captureIdentity, bool forceCaptureContext, object myObject, object myState, AsyncCallback myCallBack)
			: this(captureIdentity, forceCaptureContext, false, myObject, myState, myCallBack)
		{
		}

		internal ContextAwareResult(bool captureIdentity, bool forceCaptureContext, bool threadSafeContextCopy, object myObject, object myState, AsyncCallback myCallBack)
			: base(myObject, myState, myCallBack)
		{
			if (forceCaptureContext)
			{
				this._flags = ContextAwareResult.StateFlags.CaptureContext;
			}
			if (captureIdentity)
			{
				this._flags |= ContextAwareResult.StateFlags.CaptureIdentity;
			}
			if (threadSafeContextCopy)
			{
				this._flags |= ContextAwareResult.StateFlags.ThreadSafeContextCopy;
			}
		}

		internal ExecutionContext ContextCopy
		{
			get
			{
				if (base.InternalPeekCompleted)
				{
					if ((this._flags & ContextAwareResult.StateFlags.ThreadSafeContextCopy) == ContextAwareResult.StateFlags.None)
					{
						NetEventSource.Fail(this, "Called on completed result.", "ContextCopy");
					}
					throw new InvalidOperationException("This operation cannot be performed on a completed asynchronous result object.");
				}
				ExecutionContext context = this._context;
				if (context != null)
				{
					return context;
				}
				if (base.AsyncCallback == null && (this._flags & ContextAwareResult.StateFlags.CaptureContext) == ContextAwareResult.StateFlags.None)
				{
					NetEventSource.Fail(this, "No context captured - specify a callback or forceCaptureContext.", "ContextCopy");
				}
				if ((this._flags & ContextAwareResult.StateFlags.PostBlockFinished) == ContextAwareResult.StateFlags.None)
				{
					if (this._lock == null)
					{
						NetEventSource.Fail(this, "Must lock (StartPostingAsyncOp()) { ... FinishPostingAsyncOp(); } when calling ContextCopy (unless it's only called after FinishPostingAsyncOp).", "ContextCopy");
					}
					object @lock = this._lock;
					lock (@lock)
					{
					}
				}
				if (base.InternalPeekCompleted)
				{
					if ((this._flags & ContextAwareResult.StateFlags.ThreadSafeContextCopy) == ContextAwareResult.StateFlags.None)
					{
						NetEventSource.Fail(this, "Result became completed during call.", "ContextCopy");
					}
					throw new InvalidOperationException("This operation cannot be performed on a completed asynchronous result object.");
				}
				return this._context;
			}
		}

		internal object StartPostingAsyncOp()
		{
			return this.StartPostingAsyncOp(true);
		}

		internal object StartPostingAsyncOp(bool lockCapture)
		{
			if (base.InternalPeekCompleted)
			{
				NetEventSource.Fail(this, "Called on completed result.", "StartPostingAsyncOp");
			}
			this._lock = (lockCapture ? new object() : null);
			this._flags |= ContextAwareResult.StateFlags.PostBlockStarted;
			return this._lock;
		}

		internal bool FinishPostingAsyncOp()
		{
			if ((this._flags & (ContextAwareResult.StateFlags.PostBlockStarted | ContextAwareResult.StateFlags.PostBlockFinished)) != ContextAwareResult.StateFlags.PostBlockStarted)
			{
				return false;
			}
			this._flags |= ContextAwareResult.StateFlags.PostBlockFinished;
			ExecutionContext executionContext = null;
			return this.CaptureOrComplete(ref executionContext, false);
		}

		internal bool FinishPostingAsyncOp(ref CallbackClosure closure)
		{
			if ((this._flags & (ContextAwareResult.StateFlags.PostBlockStarted | ContextAwareResult.StateFlags.PostBlockFinished)) != ContextAwareResult.StateFlags.PostBlockStarted)
			{
				return false;
			}
			this._flags |= ContextAwareResult.StateFlags.PostBlockFinished;
			CallbackClosure callbackClosure = closure;
			ExecutionContext executionContext;
			if (callbackClosure == null)
			{
				executionContext = null;
			}
			else if (!callbackClosure.IsCompatible(base.AsyncCallback))
			{
				closure = null;
				executionContext = null;
			}
			else
			{
				base.AsyncCallback = callbackClosure.AsyncCallback;
				executionContext = callbackClosure.Context;
			}
			bool flag = this.CaptureOrComplete(ref executionContext, true);
			if (closure == null && base.AsyncCallback != null && executionContext != null)
			{
				closure = new CallbackClosure(executionContext, base.AsyncCallback);
			}
			return flag;
		}

		protected override void Cleanup()
		{
			base.Cleanup();
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, null, "Cleanup");
			}
			this.CleanupInternal();
		}

		private bool CaptureOrComplete(ref ExecutionContext cachedContext, bool returnContext)
		{
			if ((this._flags & ContextAwareResult.StateFlags.PostBlockStarted) == ContextAwareResult.StateFlags.None)
			{
				NetEventSource.Fail(this, "Called without calling StartPostingAsyncOp.", "CaptureOrComplete");
			}
			bool flag = base.AsyncCallback != null || (this._flags & ContextAwareResult.StateFlags.CaptureContext) > ContextAwareResult.StateFlags.None;
			if ((this._flags & ContextAwareResult.StateFlags.CaptureIdentity) != ContextAwareResult.StateFlags.None && !base.InternalPeekCompleted && !flag)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "starting identity capture", "CaptureOrComplete");
				}
				this.SafeCaptureIdentity();
			}
			if (flag && !base.InternalPeekCompleted)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "starting capture", "CaptureOrComplete");
				}
				if (cachedContext == null)
				{
					cachedContext = ExecutionContext.Capture();
				}
				if (cachedContext != null)
				{
					if (!returnContext)
					{
						this._context = cachedContext;
						cachedContext = null;
					}
					else
					{
						this._context = cachedContext;
					}
				}
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, FormattableStringFactory.Create("_context:{0}", new object[] { this._context }), "CaptureOrComplete");
				}
			}
			else
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "Skipping capture", "CaptureOrComplete");
				}
				cachedContext = null;
				if (base.AsyncCallback != null && !base.CompletedSynchronously)
				{
					NetEventSource.Fail(this, "Didn't capture context, but didn't complete synchronously!", "CaptureOrComplete");
				}
			}
			if (base.CompletedSynchronously)
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "Completing synchronously", "CaptureOrComplete");
				}
				base.Complete(IntPtr.Zero);
				return true;
			}
			return false;
		}

		protected override void Complete(IntPtr userToken)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("_context(set):{0} userToken:{1}", new object[]
				{
					this._context != null,
					userToken
				}), "Complete");
			}
			if ((this._flags & ContextAwareResult.StateFlags.PostBlockStarted) == ContextAwareResult.StateFlags.None)
			{
				base.Complete(userToken);
				return;
			}
			if (base.CompletedSynchronously)
			{
				return;
			}
			ExecutionContext context = this._context;
			if (userToken != IntPtr.Zero || context == null)
			{
				base.Complete(userToken);
				return;
			}
			ExecutionContext.Run(context, delegate(object s)
			{
				((ContextAwareResult)s).CompleteCallback();
			}, this);
		}

		private void CompleteCallback()
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, "Context set, calling callback.", "CompleteCallback");
			}
			base.Complete(IntPtr.Zero);
		}

		internal virtual EndPoint RemoteEndPoint
		{
			get
			{
				return null;
			}
		}

		private WindowsIdentity _windowsIdentity;

		private volatile ExecutionContext _context;

		private object _lock;

		private ContextAwareResult.StateFlags _flags;

		[Flags]
		private enum StateFlags : byte
		{
			None = 0,
			CaptureIdentity = 1,
			CaptureContext = 2,
			ThreadSafeContextCopy = 4,
			PostBlockStarted = 8,
			PostBlockFinished = 16
		}
	}
}
