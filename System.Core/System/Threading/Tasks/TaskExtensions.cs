using System;

namespace System.Threading.Tasks
{
	public static class TaskExtensions
	{
		public static Task Unwrap(this Task<Task> task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			Task task2;
			if (task.IsCompletedSuccessfully)
			{
				if ((task2 = task.Result) == null)
				{
					return Task.FromCanceled(new CancellationToken(true));
				}
			}
			else
			{
				task2 = Task.CreateUnwrapPromise<VoidTaskResult>(task, false);
			}
			return task2;
		}

		public static Task<TResult> Unwrap<TResult>(this Task<Task<TResult>> task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			Task<TResult> task2;
			if (task.IsCompletedSuccessfully)
			{
				if ((task2 = task.Result) == null)
				{
					return Task.FromCanceled<TResult>(new CancellationToken(true));
				}
			}
			else
			{
				task2 = Task.CreateUnwrapPromise<TResult>(task, false);
			}
			return task2;
		}
	}
}
