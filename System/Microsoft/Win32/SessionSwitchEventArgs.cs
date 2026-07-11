using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
	public class SessionSwitchEventArgs : EventArgs
	{
		public SessionSwitchEventArgs(SessionSwitchReason reason)
		{
			this.reason = reason;
		}

		public SessionSwitchReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		private SessionSwitchReason reason;
	}
}
