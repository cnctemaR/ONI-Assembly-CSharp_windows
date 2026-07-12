using System;
using Internal.Runtime.Augments;

namespace System.Threading
{
	internal struct ThreadPoolCallbackWrapper
	{
		public static ThreadPoolCallbackWrapper Enter()
		{
			return new ThreadPoolCallbackWrapper
			{
				_currentThread = RuntimeThread.InitializeThreadPoolThread()
			};
		}

		public void Exit(bool resetThread = true)
		{
			if (resetThread)
			{
				this._currentThread.ResetThreadPoolThread();
			}
		}

		private RuntimeThread _currentThread;
	}
}
