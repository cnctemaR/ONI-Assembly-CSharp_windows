using System;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
	internal readonly struct ForceAsyncAwaiter : ICriticalNotifyCompletion, INotifyCompletion
	{
		internal ForceAsyncAwaiter(Task task)
		{
			this._task = task;
		}

		public ForceAsyncAwaiter GetAwaiter()
		{
			return this;
		}

		public bool IsCompleted
		{
			get
			{
				return false;
			}
		}

		public void GetResult()
		{
			this._task.GetAwaiter().GetResult();
		}

		public void OnCompleted(Action action)
		{
			this._task.ConfigureAwait(false).GetAwaiter().OnCompleted(action);
		}

		public void UnsafeOnCompleted(Action action)
		{
			this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(action);
		}

		private readonly Task _task;
	}
}
