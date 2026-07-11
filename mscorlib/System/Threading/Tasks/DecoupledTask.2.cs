using System;

namespace System.Threading.Tasks
{
	internal sealed class DecoupledTask<T> : IDecoupledTask
	{
		public DecoupledTask(Task<T> task)
		{
			this.Task = task;
		}

		public bool IsCompleted
		{
			get
			{
				return this.Task.IsCompleted;
			}
		}

		public Task<T> Task { get; private set; }
	}
}
