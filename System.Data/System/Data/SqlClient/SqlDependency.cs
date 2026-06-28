using System;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	public sealed class SqlDependency
	{
		[MonoTODO]
		public SqlDependency()
		{
		}

		[MonoTODO]
		public SqlDependency(SqlCommand command)
		{
		}

		[MonoTODO]
		public SqlDependency(SqlCommand command, string options, int timeout)
		{
		}

		public event OnChangeEventHandler OnChange;

		public string Id
		{
			get
			{
				return this.uniqueId;
			}
		}

		[MonoTODO]
		public bool HasChanges
		{
			get
			{
				return true;
			}
		}

		[MonoTODO]
		public void AddCommandDependency(SqlCommand command)
		{
		}

		[MonoTODO]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
		public static bool Start(string connectionString)
		{
			return true;
		}

		[MonoTODO]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
		public static bool Start(string connectionString, string queue)
		{
			return true;
		}

		[MonoTODO]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
		public static bool Stop(string connectionString)
		{
			return true;
		}

		[MonoTODO]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
		public static bool Stop(string connectionString, string queue)
		{
			return true;
		}

		private string uniqueId = Guid.NewGuid().ToString();
	}
}
