using System;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
	public struct YieldAwaitable
	{
		public YieldAwaitable.YieldAwaiter GetAwaiter()
		{
			return default(YieldAwaitable.YieldAwaiter);
		}

		[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
		public struct YieldAwaiter : ICriticalNotifyCompletion, INotifyCompletion
		{
			public bool IsCompleted
			{
				get
				{
					return false;
				}
			}

			[SecuritySafeCritical]
			public void OnCompleted(Action continuation)
			{
				YieldAwaitable.YieldAwaiter.QueueContinuation(continuation, true);
			}

			[SecurityCritical]
			public void UnsafeOnCompleted(Action continuation)
			{
				YieldAwaitable.YieldAwaiter.QueueContinuation(continuation, false);
			}

			[SecurityCritical]
			private static void QueueContinuation(Action continuation, bool flowContext)
			{
				if (continuation == null)
				{
					throw new ArgumentNullException("continuation");
				}
				SynchronizationContext currentNoFlow = SynchronizationContext.CurrentNoFlow;
				if (currentNoFlow != null && currentNoFlow.GetType() != typeof(SynchronizationContext))
				{
					currentNoFlow.Post(YieldAwaitable.YieldAwaiter.s_sendOrPostCallbackRunAction, continuation);
					return;
				}
				TaskScheduler taskScheduler = TaskScheduler.Current;
				if (taskScheduler != TaskScheduler.Default)
				{
					Task.Factory.StartNew(continuation, default(CancellationToken), TaskCreationOptions.PreferFairness, taskScheduler);
					return;
				}
				if (flowContext)
				{
					ThreadPool.QueueUserWorkItem(YieldAwaitable.YieldAwaiter.s_waitCallbackRunAction, continuation);
					return;
				}
				ThreadPool.UnsafeQueueUserWorkItem(YieldAwaitable.YieldAwaiter.s_waitCallbackRunAction, continuation);
			}

			private static void RunAction(object state)
			{
				((Action)state)();
			}

			public void GetResult()
			{
			}

			private static readonly WaitCallback s_waitCallbackRunAction = new WaitCallback(YieldAwaitable.YieldAwaiter.RunAction);

			private static readonly SendOrPostCallback s_sendOrPostCallbackRunAction = new SendOrPostCallback(YieldAwaitable.YieldAwaiter.RunAction);
		}
	}
}
