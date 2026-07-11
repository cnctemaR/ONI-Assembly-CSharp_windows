using System;
using System.Security;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public struct TaskAwaiter<TResult> : ICriticalNotifyCompletion, INotifyCompletion
	{
		internal TaskAwaiter(Task<TResult> task)
		{
			this.m_task = task;
		}

		public bool IsCompleted
		{
			get
			{
				return this.m_task.IsCompleted;
			}
		}

		[SecuritySafeCritical]
		public void OnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(this.m_task, continuation, true, true);
		}

		[SecurityCritical]
		public void UnsafeOnCompleted(Action continuation)
		{
			TaskAwaiter.OnCompletedInternal(this.m_task, continuation, true, false);
		}

		public TResult GetResult()
		{
			TaskAwaiter.ValidateEnd(this.m_task);
			return this.m_task.ResultOnSuccess;
		}

		private readonly Task<TResult> m_task;
	}
}
