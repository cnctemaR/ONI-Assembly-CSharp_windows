using System;

namespace System.IO
{
	public class ErrorEventArgs : EventArgs
	{
		public ErrorEventArgs(Exception exception)
		{
			this.exception = exception;
		}

		public virtual Exception GetException()
		{
			return this.exception;
		}

		private Exception exception;
	}
}
