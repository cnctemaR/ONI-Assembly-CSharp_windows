using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

namespace System.Threading.Tasks
{
	[DebuggerDisplay("Id={Id}")]
	[DebuggerTypeProxy(typeof(TaskScheduler.SystemThreadingTasks_TaskSchedulerDebugView))]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
	public abstract class TaskScheduler
	{
		[SecurityCritical]
		protected internal abstract void QueueTask(Task task);

		[SecurityCritical]
		protected abstract bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued);

		[SecurityCritical]
		protected abstract IEnumerable<Task> GetScheduledTasks();

		public virtual int MaximumConcurrencyLevel
		{
			get
			{
				return int.MaxValue;
			}
		}

		[SecuritySafeCritical]
		internal bool TryRunInline(Task task, bool taskWasPreviouslyQueued)
		{
			TaskScheduler executingTaskScheduler = task.ExecutingTaskScheduler;
			if (executingTaskScheduler != this && executingTaskScheduler != null)
			{
				return executingTaskScheduler.TryRunInline(task, taskWasPreviouslyQueued);
			}
			StackGuard currentStackGuard;
			if (executingTaskScheduler == null || task.m_action == null || task.IsDelegateInvoked || task.IsCanceled || !(currentStackGuard = Task.CurrentStackGuard).TryBeginInliningScope())
			{
				return false;
			}
			bool flag = false;
			try
			{
				task.FireTaskScheduledIfNeeded(this);
				flag = this.TryExecuteTaskInline(task, taskWasPreviouslyQueued);
			}
			finally
			{
				currentStackGuard.EndInliningScope();
			}
			if (flag && !task.IsDelegateInvoked && !task.IsCanceled)
			{
				throw new InvalidOperationException(Environment.GetResourceString("The TryExecuteTaskInline call to the underlying scheduler succeeded, but the task body was not invoked."));
			}
			return flag;
		}

		[SecurityCritical]
		protected internal virtual bool TryDequeue(Task task)
		{
			return false;
		}

		internal virtual void NotifyWorkItemProgress()
		{
		}

		internal virtual bool RequiresAtomicStartTransition
		{
			get
			{
				return true;
			}
		}

		[SecurityCritical]
		internal void InternalQueueTask(Task task)
		{
			task.FireTaskScheduledIfNeeded(this);
			this.QueueTask(task);
		}

		protected TaskScheduler()
		{
			if (Debugger.IsAttached)
			{
				this.AddToActiveTaskSchedulers();
			}
		}

		private void AddToActiveTaskSchedulers()
		{
			ConditionalWeakTable<TaskScheduler, object> conditionalWeakTable = TaskScheduler.s_activeTaskSchedulers;
			if (conditionalWeakTable == null)
			{
				Interlocked.CompareExchange<ConditionalWeakTable<TaskScheduler, object>>(ref TaskScheduler.s_activeTaskSchedulers, new ConditionalWeakTable<TaskScheduler, object>(), null);
				conditionalWeakTable = TaskScheduler.s_activeTaskSchedulers;
			}
			conditionalWeakTable.Add(this, null);
		}

		public static TaskScheduler Default
		{
			get
			{
				return TaskScheduler.s_defaultTaskScheduler;
			}
		}

		public static TaskScheduler Current
		{
			get
			{
				return TaskScheduler.InternalCurrent ?? TaskScheduler.Default;
			}
		}

		internal static bool IsDefault
		{
			get
			{
				return TaskScheduler.Current == TaskScheduler.Default;
			}
		}

		internal static TaskScheduler InternalCurrent
		{
			get
			{
				Task internalCurrent = Task.InternalCurrent;
				if (internalCurrent == null || (internalCurrent.CreationOptions & TaskCreationOptions.HideScheduler) != TaskCreationOptions.None)
				{
					return null;
				}
				return internalCurrent.ExecutingTaskScheduler;
			}
		}

		public static TaskScheduler FromCurrentSynchronizationContext()
		{
			return new SynchronizationContextTaskScheduler();
		}

		public int Id
		{
			get
			{
				if (this.m_taskSchedulerId == 0)
				{
					int num;
					do
					{
						num = Interlocked.Increment(ref TaskScheduler.s_taskSchedulerIdCounter);
					}
					while (num == 0);
					Interlocked.CompareExchange(ref this.m_taskSchedulerId, num, 0);
				}
				return this.m_taskSchedulerId;
			}
		}

		[SecurityCritical]
		protected bool TryExecuteTask(Task task)
		{
			if (task.ExecutingTaskScheduler != this)
			{
				throw new InvalidOperationException(Environment.GetResourceString("ExecuteTask may not be called for a task which was previously queued to a different TaskScheduler."));
			}
			return task.ExecuteEntry(true);
		}

		public static event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException
		{
			[SecurityCritical]
			add
			{
				if (value != null)
				{
					RuntimeHelpers.PrepareContractedDelegate(value);
					object unobservedTaskExceptionLockObject = TaskScheduler._unobservedTaskExceptionLockObject;
					lock (unobservedTaskExceptionLockObject)
					{
						TaskScheduler._unobservedTaskException = (EventHandler<UnobservedTaskExceptionEventArgs>)Delegate.Combine(TaskScheduler._unobservedTaskException, value);
					}
				}
			}
			[SecurityCritical]
			remove
			{
				object unobservedTaskExceptionLockObject = TaskScheduler._unobservedTaskExceptionLockObject;
				lock (unobservedTaskExceptionLockObject)
				{
					TaskScheduler._unobservedTaskException = (EventHandler<UnobservedTaskExceptionEventArgs>)Delegate.Remove(TaskScheduler._unobservedTaskException, value);
				}
			}
		}

		internal static void PublishUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs ueea)
		{
			object unobservedTaskExceptionLockObject = TaskScheduler._unobservedTaskExceptionLockObject;
			lock (unobservedTaskExceptionLockObject)
			{
				EventHandler<UnobservedTaskExceptionEventArgs> unobservedTaskException = TaskScheduler._unobservedTaskException;
				if (unobservedTaskException != null)
				{
					unobservedTaskException(sender, ueea);
				}
			}
		}

		[SecurityCritical]
		internal Task[] GetScheduledTasksForDebugger()
		{
			IEnumerable<Task> scheduledTasks = this.GetScheduledTasks();
			if (scheduledTasks == null)
			{
				return null;
			}
			Task[] array = scheduledTasks as Task[];
			if (array == null)
			{
				array = new List<Task>(scheduledTasks).ToArray();
			}
			Task[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int id = array2[i].Id;
			}
			return array;
		}

		[SecurityCritical]
		internal static TaskScheduler[] GetTaskSchedulersForDebugger()
		{
			if (TaskScheduler.s_activeTaskSchedulers == null)
			{
				return new TaskScheduler[] { TaskScheduler.s_defaultTaskScheduler };
			}
			ICollection<TaskScheduler> keys = TaskScheduler.s_activeTaskSchedulers.Keys;
			if (!keys.Contains(TaskScheduler.s_defaultTaskScheduler))
			{
				keys.Add(TaskScheduler.s_defaultTaskScheduler);
			}
			TaskScheduler[] array = new TaskScheduler[keys.Count];
			keys.CopyTo(array, 0);
			TaskScheduler[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				int id = array2[i].Id;
			}
			return array;
		}

		private static ConditionalWeakTable<TaskScheduler, object> s_activeTaskSchedulers;

		private static readonly TaskScheduler s_defaultTaskScheduler = new ThreadPoolTaskScheduler();

		internal static int s_taskSchedulerIdCounter;

		private volatile int m_taskSchedulerId;

		private static EventHandler<UnobservedTaskExceptionEventArgs> _unobservedTaskException;

		private static readonly object _unobservedTaskExceptionLockObject = new object();

		internal sealed class SystemThreadingTasks_TaskSchedulerDebugView
		{
			public SystemThreadingTasks_TaskSchedulerDebugView(TaskScheduler scheduler)
			{
				this.m_taskScheduler = scheduler;
			}

			public int Id
			{
				get
				{
					return this.m_taskScheduler.Id;
				}
			}

			public IEnumerable<Task> ScheduledTasks
			{
				[SecurityCritical]
				get
				{
					return this.m_taskScheduler.GetScheduledTasks();
				}
			}

			private readonly TaskScheduler m_taskScheduler;
		}
	}
}
