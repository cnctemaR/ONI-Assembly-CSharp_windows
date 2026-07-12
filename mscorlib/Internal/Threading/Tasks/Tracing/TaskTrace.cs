using System;
using Internal.Runtime.Augments;

namespace Internal.Threading.Tasks.Tracing
{
	internal static class TaskTrace
	{
		public static bool Enabled
		{
			get
			{
				TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
				return taskTraceCallbacks != null && taskTraceCallbacks.Enabled;
			}
		}

		public static void Initialize(TaskTraceCallbacks callbacks)
		{
			TaskTrace.s_callbacks = callbacks;
		}

		public static void TaskWaitBegin_Asynchronous(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID)
		{
			TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
			if (taskTraceCallbacks == null)
			{
				return;
			}
			taskTraceCallbacks.TaskWaitBegin_Asynchronous(OriginatingTaskSchedulerID, OriginatingTaskID, TaskID);
		}

		public static void TaskWaitBegin_Synchronous(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID)
		{
			TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
			if (taskTraceCallbacks == null)
			{
				return;
			}
			taskTraceCallbacks.TaskWaitBegin_Synchronous(OriginatingTaskSchedulerID, OriginatingTaskID, TaskID);
		}

		public static void TaskWaitEnd(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID)
		{
			TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
			if (taskTraceCallbacks == null)
			{
				return;
			}
			taskTraceCallbacks.TaskWaitEnd(OriginatingTaskSchedulerID, OriginatingTaskID, TaskID);
		}

		public static void TaskScheduled(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID, int CreatingTaskID, int TaskCreationOptions)
		{
			TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
			if (taskTraceCallbacks == null)
			{
				return;
			}
			taskTraceCallbacks.TaskScheduled(OriginatingTaskSchedulerID, OriginatingTaskID, TaskID, CreatingTaskID, TaskCreationOptions);
		}

		public static void TaskStarted(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID)
		{
			TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
			if (taskTraceCallbacks == null)
			{
				return;
			}
			taskTraceCallbacks.TaskStarted(OriginatingTaskSchedulerID, OriginatingTaskID, TaskID);
		}

		public static void TaskCompleted(int OriginatingTaskSchedulerID, int OriginatingTaskID, int TaskID, bool IsExceptional)
		{
			TaskTraceCallbacks taskTraceCallbacks = TaskTrace.s_callbacks;
			if (taskTraceCallbacks == null)
			{
				return;
			}
			taskTraceCallbacks.TaskCompleted(OriginatingTaskSchedulerID, OriginatingTaskID, TaskID, IsExceptional);
		}

		private static TaskTraceCallbacks s_callbacks;
	}
}
