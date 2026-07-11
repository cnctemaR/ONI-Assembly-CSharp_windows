using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcPermission : DBDataPermission
	{
		[Obsolete("use OdbcPermission(PermissionState.None)", true)]
		public OdbcPermission()
			: base(null)
		{
		}

		public OdbcPermission(PermissionState state)
			: base(null)
		{
		}

		[Obsolete("use OdbcPermission(PermissionState.None)", true)]
		public OdbcPermission(PermissionState state, bool allowBlankPassword)
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
