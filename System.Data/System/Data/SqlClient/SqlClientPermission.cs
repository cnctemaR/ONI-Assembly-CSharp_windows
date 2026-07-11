using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlClientPermission : DBDataPermission
	{
		[Obsolete("Use SqlClientPermission(PermissionState.None)", true)]
		public SqlClientPermission()
			: base(null)
		{
		}

		public SqlClientPermission(PermissionState state)
			: base(null)
		{
		}

		[Obsolete("Use SqlClientPermission(PermissionState.None)", true)]
		public SqlClientPermission(PermissionState state, bool allowBlankPassword)
			: base(null)
		{
		}

		public override void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
		}

		public override IPermission Copy()
		{
			throw null;
		}
	}
}
