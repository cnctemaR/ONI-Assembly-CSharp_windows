using System;

namespace System.ComponentModel
{
	public class CancelEventArgs : EventArgs
	{
		public CancelEventArgs()
		{
			this.cancel = false;
		}

		public CancelEventArgs(bool cancel)
		{
			this.cancel = cancel;
		}

		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			set
			{
				this.cancel = value;
			}
		}

		private bool cancel;
	}
}
