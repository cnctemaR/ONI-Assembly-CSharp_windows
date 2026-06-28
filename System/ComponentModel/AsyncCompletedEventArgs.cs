using System;
using System.Reflection;

namespace System.ComponentModel
{
	public class AsyncCompletedEventArgs : EventArgs
	{
		public AsyncCompletedEventArgs(Exception error, bool cancelled, object userState)
		{
			this._error = error;
			this._cancelled = cancelled;
			this._userState = userState;
		}

		protected void RaiseExceptionIfNecessary()
		{
			if (this._error != null)
			{
				throw new TargetInvocationException(this._error);
			}
			if (this._cancelled)
			{
				throw new InvalidOperationException("The operation was cancelled");
			}
		}

		public bool Cancelled
		{
			get
			{
				return this._cancelled;
			}
		}

		public Exception Error
		{
			get
			{
				return this._error;
			}
		}

		public object UserState
		{
			get
			{
				return this._userState;
			}
		}

		private Exception _error;

		private bool _cancelled;

		private object _userState;
	}
}
