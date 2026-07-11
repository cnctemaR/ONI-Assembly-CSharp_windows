using System;

namespace System.ComponentModel
{
	public class RunWorkerCompletedEventArgs : AsyncCompletedEventArgs
	{
		public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
			: base(error, cancelled, null)
		{
			this.result = result;
		}

		public object Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this.result;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new object UserState
		{
			get
			{
				return null;
			}
		}

		private object result;
	}
}
