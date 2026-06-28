using System;

namespace System.ComponentModel
{
	public class DoWorkEventArgs : CancelEventArgs
	{
		public DoWorkEventArgs(object argument)
		{
			this.arg = argument;
		}

		public object Argument
		{
			get
			{
				return this.arg;
			}
		}

		public object Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		private object arg;

		private object result;
	}
}
