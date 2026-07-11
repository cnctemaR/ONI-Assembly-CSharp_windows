using System;
using System.Runtime.Serialization;

namespace System.Threading.Tasks
{
	[Serializable]
	public class TaskCanceledException : OperationCanceledException
	{
		public TaskCanceledException()
			: base(Environment.GetResourceString("A task was canceled."))
		{
		}

		public TaskCanceledException(string message)
			: base(message)
		{
		}

		public TaskCanceledException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public TaskCanceledException(Task task)
			: base(Environment.GetResourceString("A task was canceled."), (task != null) ? task.CancellationToken : default(CancellationToken))
		{
			this.m_canceledTask = task;
		}

		protected TaskCanceledException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public Task Task
		{
			get
			{
				return this.m_canceledTask;
			}
		}

		[NonSerialized]
		private Task m_canceledTask;
	}
}
