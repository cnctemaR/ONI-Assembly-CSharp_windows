using System;

namespace System.ComponentModel
{
	public class HandledEventArgs : EventArgs
	{
		public HandledEventArgs()
			: this(false)
		{
		}

		public HandledEventArgs(bool defaultHandledValue)
		{
			this.Handled = defaultHandledValue;
		}

		public bool Handled { get; set; }
	}
}
