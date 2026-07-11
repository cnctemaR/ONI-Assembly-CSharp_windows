using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
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
