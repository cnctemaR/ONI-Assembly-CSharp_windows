using System;

namespace System.ComponentModel
{
	public class CancelEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public CancelEventArgs()
		{
		}

		public CancelEventArgs(bool cancel)
		{
			this.Cancel = cancel;
		}
	}
}
