using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Serializable]
	public class EventLogPermissionEntry
	{
		public EventLogPermissionEntry(EventLogPermissionAccess permissionAccess, string machineName)
		{
			global::System.Security.Permissions.ResourcePermissionBase.ValidateMachineName(machineName);
			this.permissionAccess = permissionAccess;
			this.machineName = machineName;
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public EventLogPermissionAccess PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
		}

		internal global::System.Security.Permissions.ResourcePermissionBaseEntry CreateResourcePermissionBaseEntry()
		{
			return new global::System.Security.Permissions.ResourcePermissionBaseEntry((int)this.permissionAccess, new string[] { this.machineName });
		}

		private EventLogPermissionAccess permissionAccess;

		private string machineName;
	}
}
