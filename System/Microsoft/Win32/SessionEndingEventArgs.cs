using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
	public class SessionEndingEventArgs : EventArgs
	{
		public SessionEndingEventArgs(SessionEndReasons reason)
		{
			this.myreason = reason;
		}

		public SessionEndReasons Reason
		{
			get
			{
				return this.myreason;
			}
		}

		public bool Cancel
		{
			get
			{
				return this.mycancel;
			}
			set
			{
				this.mycancel = value;
			}
		}

		private SessionEndReasons myreason;

		private bool mycancel;
	}
}
