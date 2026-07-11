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
			return Task.CreateUnwrapPromise<TaskExtensions.VoidResult>(task, false);
		}

		public static Task<TResult> Unwrap<TResult>(this Task<Task<TResult>> task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}
			return Task.CreateUnwrapPromise<TResult>(task, false);
		}

		private struct VoidResult
		{
		}
	}
}
