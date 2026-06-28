using System;

namespace System.Threading
{
	public class ThreadExceptionEventArgs : EventArgs
	{
		public ThreadExceptionEventArgs(Exception t)
		{
			this.exception = t;
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		private Exception exception;
	}
}
