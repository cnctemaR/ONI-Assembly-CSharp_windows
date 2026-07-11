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

		[MonoTODO]
		public bool HasChanges
		{
			get
			{
				throw null;
			}
		}

		public string Id
		{
			get
			{
				throw null;
			}
		}

		public event OnChangeEventHandler OnChange
		{
			add
			{
			}
			remove
			{
			}
		}

		[MonoTODO]
		public void AddCommandDependency(SqlCommand command)
		{
		}

		[MonoTODO]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public static bool Start(string connectionString)
		{
			throw null;
		}

		[MonoTODO]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public static bool Start(string connectionString, string queue)
		{
			throw null;
		}

		[MonoTODO]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public static bool Stop(string connectionString)
		{
			throw null;
		}

		[MonoTODO]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public static bool Stop(string connectionString, string queue)
		{
			throw null;
		}
	}
}
