using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
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
