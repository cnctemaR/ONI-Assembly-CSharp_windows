using System;

namespace System.ComponentModel
{
	public class HandledEventArgs : EventArgs
	{
		public HandledEventArgs()
		{
			this.handled = false;
		}

		public HandledEventArgs(bool defaultHandledValue)
		{
			this.handled = defaultHandledValue;
		}

		public bool Handled
		{
			get
			{
				return this.handled;
			}
			set
			{
				this.handled = value;
			}
		}

		private bool handled;
	}
}
