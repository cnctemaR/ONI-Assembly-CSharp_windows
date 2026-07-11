using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	[Serializable]
	public class PerformanceCounterPermissionEntry
	{
		public PerformanceCounterPermissionEntry(PerformanceCounterPermissionAccess permissionAccess, string machineName, string categoryName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			if ((permissionAccess | PerformanceCounterPermissionAccess.Administer) != PerformanceCounterPermissionAccess.Administer)
			{
				throw new ArgumentException("permissionAccess");
			}
			ResourcePermissionBase.ValidateMachineName(machineName);
			if (categoryName == null)
			{
				throw new ArgumentNullException("categoryName");
			}
			this.permissionAccess = permissionAccess;
			this.machineName = machineName;
			this.categoryName = categoryName;
		}

		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public PerformanceCounterPermissionAccess PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
		}

		internal ResourcePermissionBaseEntry CreateResourcePermissionBaseEntry()
		{
			return new ResourcePermissionBaseEntry((int)this.permissionAccess, new string[] { this.machineName, this.categoryName });
		}

		private const PerformanceCounterPermissionAccess All = PerformanceCounterPermissionAccess.Administer;

		private PerformanceCounterPermissionAccess permissionAccess;

		private string machineName;

		private string categoryName;
	}
}
