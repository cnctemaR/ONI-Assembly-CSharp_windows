using System;

namespace System.Threading.Tasks
{
	internal sealed class DecoupledTask : IDecoupledTask
	{
		public DecoupledTask(Task task)
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

		public Task Task { get; private set; }
	}
}
