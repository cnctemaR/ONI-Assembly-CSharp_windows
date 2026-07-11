using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security.Permissions;

namespace System.Threading.Tasks
{
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public class TaskCompletionSource<TResult>
	{
		public TaskCompletionSource()
		{
			this.m_task = new Task<TResult>();
		}

		public TaskCompletionSource(TaskCreationOptions creationOptions)
			: this(null, creationOptions)
		{
		}

		public TaskCompletionSource(object state)
			: this(state, TaskCreationOptions.None)
		{
		}

		public TaskCompletionSource(object state, TaskCreationOptions creationOptions)
		{
			this.m_task = new Task<TResult>(state, creationOptions);
		}

		public Task<TResult> Task
		{
			get
			{
				return this.m_task;
			}
		}

		private void SpinUntilCompleted()
		{
			SpinWait spinWait = default(SpinWait);
			while (!this.m_task.IsCompleted)
			{
				spinWait.SpinOnce();
			}
		}

		public bool TrySetException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			bool flag = this.m_task.TrySetException(exception);
			if (!flag && !this.m_task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		public bool TrySetException(IEnumerable<Exception> exceptions)
		{
			if (exceptions == null)
			{
				throw new ArgumentNullException("exceptions");
			}
			List<Exception> list = new List<Exception>();
			foreach (Exception ex in exceptions)
			{
				if (ex == null)
				{
					throw new ArgumentException(Environment.GetResourceString("The exceptions collection included at least one null element."), "exceptions");
				}
				list.Add(ex);
			}
			if (list.Count == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("The exceptions collection was empty."), "exceptions");
			}
			bool flag = this.m_task.TrySetException(list);
			if (!flag && !this.m_task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		internal bool TrySetException(IEnumerable<ExceptionDispatchInfo> exceptions)
		{
			bool flag = this.m_task.TrySetException(exceptions);
			if (!flag && !this.m_task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		public void SetException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (!this.TrySetException(exception))
			{
				throw new InvalidOperationException(Environment.GetResourceString("An attempt was made to transition a task to a final state when it had already completed."));
			}
		}

		public void SetException(IEnumerable<Exception> exceptions)
		{
			if (!this.TrySetException(exceptions))
			{
				throw new InvalidOperationException(Environment.GetResourceString("An attempt was made to transition a task to a final state when it had already completed."));
			}
		}

		public bool TrySetResult(TResult result)
		{
			bool flag = this.m_task.TrySetResult(result);
			if (!flag && !this.m_task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		public void SetResult(TResult result)
		{
			if (!this.TrySetResult(result))
			{
				throw new InvalidOperationException(Environment.GetResourceString("An attempt was made to transition a task to a final state when it had already completed."));
			}
		}

		public bool TrySetCanceled()
		{
			return this.TrySetCanceled(default(CancellationToken));
		}

		public bool TrySetCanceled(CancellationToken cancellationToken)
		{
			bool flag = this.m_task.TrySetCanceled(cancellationToken);
			if (!flag && !this.m_task.IsCompleted)
			{
				this.SpinUntilCompleted();
			}
			return flag;
		}

		public void SetCanceled()
		{
			if (!this.TrySetCanceled())
			{
				throw new InvalidOperationException(Environment.GetResourceString("An attempt was made to transition a task to a final state when it had already completed."));
			}
		}

		private readonly Task<TResult> m_task;
	}
}
