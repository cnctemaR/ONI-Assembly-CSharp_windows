using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
	public class UserPreferenceChangingEventArgs : EventArgs
	{
		public UserPreferenceChangingEventArgs(UserPreferenceCategory category)
		{
			this.mycategory = category;
		}

		public UserPreferenceCategory Category
		{
			get
			{
				return this.mycategory;
			}
		}

		private UserPreferenceCategory mycategory;
	}
}
