using System;

namespace System
{
	[Serializable]
	public class UnhandledExceptionEventArgs : EventArgs
	{
		public UnhandledExceptionEventArgs(object exception, bool isTerminating)
		{
			this._exception = exception;
			this._isTerminating = isTerminating;
		}

		public object ExceptionObject
		{
			get
			{
				return this._exception;
			}
		}

		public bool IsTerminating
		{
			get
			{
				return this._isTerminating;
			}
		}

		private object _exception;

		private bool _isTerminating;
	}
}
