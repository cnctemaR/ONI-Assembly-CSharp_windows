using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class PowerModeChangedEventArgs : EventArgs
	{
		public PowerModeChangedEventArgs(PowerModes mode)
		{
			this.mymode = mode;
		}

		public PowerModes Mode
		{
			get
			{
				return this.mymode;
			}
		}

		private PowerModes mymode;
	}
}
