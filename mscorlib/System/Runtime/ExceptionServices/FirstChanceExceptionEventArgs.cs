using System;

namespace System.Runtime.ExceptionServices
{
	public class FirstChanceExceptionEventArgs : EventArgs
	{
		public FirstChanceExceptionEventArgs(Exception exception)
		{
			this.Exception = exception;
		}

		public Exception Exception { get; }
	}
}
