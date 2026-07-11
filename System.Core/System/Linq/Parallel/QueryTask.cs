using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Parallel
{
	internal abstract class QueryTask
	{
		protected QueryTask(int taskIndex, QueryTaskGroupState groupState)
		{
			this._taskIndex = taskIndex;
			this._groupState = groupState;
		}

		private static void RunTaskSynchronously(object o)
		{
			((QueryTask)o).BaseWork(null);
		}

		internal Task RunSynchronously(TaskScheduler taskScheduler)
		{
			Task task = new Task(QueryTask.s_runTaskSynchronouslyDelegate, this, TaskCreationOptions.AttachedToParent);
			task.RunSynchronously(taskScheduler);
			return task;
		}

		internal Task RunAsynchronously(TaskScheduler taskScheduler)
		{
			return Task.Factory.StartNew(QueryTask.s_baseWorkDelegate, this, default(CancellationToken), TaskCreationOptions.PreferFairness | TaskCreationOptions.AttachedToParent, taskScheduler);
		}

		private void BaseWork(object unused)
		{
			PlinqEtwProvider.Log.ParallelQueryFork(this._groupState.QueryId);
			try
			{
				this.Work();
			}
			finally
			{
				PlinqEtwProvider.Log.ParallelQueryJoin(this._groupState.QueryId);
			}
		}

		protected abstract void Work();

		protected int _taskIndex;

		protected QueryTaskGroupState _groupState;

		private static Action<object> s_runTaskSynchronouslyDelegate = new Action<object>(QueryTask.RunTaskSynchronously);

		private static Action<object> s_baseWorkDelegate = delegate(object o)
		{
			((QueryTask)o).BaseWork(null);
		};
	}
}
