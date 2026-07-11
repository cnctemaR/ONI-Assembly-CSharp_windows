using System;
using System.Collections.Generic;
using System.Security;

namespace System.Threading.Tasks
{
	internal sealed class ThreadPoolTaskScheduler : TaskScheduler
	{
		internal ThreadPoolTaskScheduler()
		{
			int id = base.Id;
		}

		private static void LongRunningThreadWork(object obj)
		{
			(obj as Task).ExecuteEntry(false);
		}

		[SecurityCritical]
		protected internal override void QueueTask(Task task)
		{
			if ((task.Options & TaskCreationOptions.LongRunning) != TaskCreationOptions.None)
			{
				new Thread(ThreadPoolTaskScheduler.s_longRunningThreadWork)
				{
					IsBackground = true
				}.Start(task);
				return;
			}
			bool flag = (task.Options & TaskCreationOptions.PreferFairness) > TaskCreationOptions.None;
			ThreadPool.UnsafeQueueCustomWorkItem(task, flag);
		}

		[SecurityCritical]
		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			if (taskWasPreviouslyQueued && !ThreadPool.TryPopCustomWorkItem(task))
			{
				return false;
			}
			bool flag = false;
			try
			{
				flag = task.ExecuteEntry(false);
			}
			finally
			{
				if (taskWasPreviouslyQueued)
				{
					this.NotifyWorkItemProgress();
				}
			}
			return flag;
		}

		[SecurityCritical]
		protected internal override bool TryDequeue(Task task)
		{
			return ThreadPool.TryPopCustomWorkItem(task);
		}

		[SecurityCritical]
		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return this.FilterTasksFromWorkItems(ThreadPool.GetQueuedWorkItems());
		}

		private IEnumerable<Task> FilterTasksFromWorkItems(IEnumerable<IThreadPoolWorkItem> tpwItems)
		{
			foreach (IThreadPoolWorkItem threadPoolWorkItem in tpwItems)
			{
				if (threadPoolWorkItem is Task)
				{
					yield return (Task)threadPoolWorkItem;
				}
			}
			IEnumerator<IThreadPoolWorkItem> enumerator = null;
			yield break;
			yield break;
		}

		internal override void NotifyWorkItemProgress()
		{
			ThreadPool.NotifyWorkItemProgress();
		}

		internal override bool RequiresAtomicStartTransition
		{
			get
			{
				return false;
			}
		}

		private static readonly ParameterizedThreadStart s_longRunningThreadWork = new ParameterizedThreadStart(ThreadPoolTaskScheduler.LongRunningThreadWork);
	}
}
