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
			: base(PermissionState.None)
		{
		}

		public OdbcPermission(PermissionState state)
			: base(state)
		{
		}

		[Obsolete("use OdbcPermission(PermissionState.None)", true)]
		public OdbcPermission(PermissionState state, bool allowBlankPassword)
			: base(state)
		{
			base.AllowBlankPassword = allowBlankPassword;
		}

		internal OdbcPermission(DBDataPermission permission)
			: base(permission)
		{
		}

		internal OdbcPermission(DBDataPermissionAttribute attribute)
			: base(attribute)
		{
		}

		public override IPermission Copy()
		{
			return new OdbcPermission(this);
		}

		public override void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			base.Add(connectionString, restrictions, behavior);
		}
	}
}
