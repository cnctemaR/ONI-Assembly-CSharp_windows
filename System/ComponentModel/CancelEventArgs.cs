using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class CancelEventArgs : EventArgs
	{
		public CancelEventArgs()
			: this(false)
		{
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
