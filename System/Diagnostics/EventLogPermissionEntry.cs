using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Serializable]
	public class EventLogPermissionEntry
	{
		public EventLogPermissionEntry(EventLogPermissionAccess permissionAccess, string machineName)
		{
			ResourcePermissionBase.ValidateMachineName(machineName);
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

		internal ResourcePermissionBaseEntry CreateResourcePermissionBaseEntry()
		{
			return new ResourcePermissionBaseEntry((int)this.permissionAccess, new string[] { this.machineName });
		}

		private EventLogPermissionAccess permissionAccess;

		private string machineName;
	}
}
