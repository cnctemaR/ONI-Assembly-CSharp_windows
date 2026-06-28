using System;
using System.Security.Permissions;

namespace Microsoft.Win32
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
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
