using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
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
