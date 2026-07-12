using System;
using System.Collections.Generic;

namespace System.Threading.Tasks
{
	internal sealed class SynchronizationContextTaskScheduler : TaskScheduler
	{
		internal SynchronizationContextTaskScheduler()
		{
			SynchronizationContext synchronizationContext = SynchronizationContext.Current;
			if (synchronizationContext == null)
			{
				throw new InvalidOperationException("The current SynchronizationContext may not be used as a TaskScheduler.");
			}
			this.m_synchronizationContext = synchronizationContext;
		}

		protected internal override void QueueTask(Task task)
		{
			this.m_synchronizationContext.Post(SynchronizationContextTaskScheduler.s_postCallback, task);
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			return SynchronizationContext.Current == this.m_synchronizationContext && base.TryExecuteTask(task);
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return null;
		}

		public override int MaximumConcurrencyLevel
		{
			get
			{
				return 1;
			}
		}

		private SynchronizationContext m_synchronizationContext;

		private static readonly SendOrPostCallback s_postCallback = delegate(object s)
		{
			((Task)s).ExecuteEntry(true);
		};
	}
}
