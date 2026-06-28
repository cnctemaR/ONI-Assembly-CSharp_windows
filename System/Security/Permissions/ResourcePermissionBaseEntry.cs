using System;

namespace System.Security.Permissions
{
	[Serializable]
	public class ResourcePermissionBaseEntry
	{
		public ResourcePermissionBaseEntry()
		{
			this.permissionAccessPath = new string[0];
		}

		public ResourcePermissionBaseEntry(int permissionAccess, string[] permissionAccessPath)
		{
			if (permissionAccessPath == null)
			{
				throw new ArgumentNullException("permissionAccessPath");
			}
			this.permissionAccess = permissionAccess;
			this.permissionAccessPath = permissionAccessPath;
		}

		public int PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
		}

		public string[] PermissionAccessPath
		{
			get
			{
				return this.permissionAccessPath;
			}
		}

		private int permissionAccess;

		private string[] permissionAccessPath;
	}
}
