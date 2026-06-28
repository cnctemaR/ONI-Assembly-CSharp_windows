using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class SessionEndedEventArgs : EventArgs
	{
		public SessionEndedEventArgs(SessionEndReasons reason)
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

		private SessionEndReasons myreason;
	}
}
