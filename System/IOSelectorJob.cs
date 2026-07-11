using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[StructLayout(LayoutKind.Sequential)]
	internal class IOSelectorJob : IThreadPoolWorkItem
	{
		public IOSelectorJob(IOOperation operation, IOAsyncCallback callback, IOAsyncResult state)
		{
			this.operation = operation;
			this.callback = callback;
			this.state = state;
		}

		void IThreadPoolWorkItem.ExecuteWorkItem()
		{
			this.callback(this.state);
		}

		void IThreadPoolWorkItem.MarkAborted(ThreadAbortException tae)
		{
		}

		public void MarkDisposed()
		{
			this.state.CompleteDisposed();
		}

		private IOOperation operation;

		private IOAsyncCallback callback;

		private IOAsyncResult state;
	}
}
