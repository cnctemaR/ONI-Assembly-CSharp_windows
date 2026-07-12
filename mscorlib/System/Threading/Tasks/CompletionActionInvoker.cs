using System;

namespace System.Threading.Tasks
{
	internal sealed class CompletionActionInvoker : IThreadPoolWorkItem
	{
		internal CompletionActionInvoker(ITaskCompletionAction action, Task completingTask)
		{
			this.m_action = action;
			this.m_completingTask = completingTask;
		}

		void IThreadPoolWorkItem.ExecuteWorkItem()
		{
			this.m_action.Invoke(this.m_completingTask);
		}

		public void MarkAborted(ThreadAbortException e)
		{
		}

		private readonly ITaskCompletionAction m_action;

		private readonly Task m_completingTask;
	}
}
