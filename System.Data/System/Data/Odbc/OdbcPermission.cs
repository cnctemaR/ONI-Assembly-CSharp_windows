using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcPermission : DBDataPermission
	{
		[Obsolete("OdbcPermission() has been deprecated.  Use the OdbcPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
		public OdbcPermission()
			: this(PermissionState.None)
		{
		}

		public OdbcPermission(PermissionState state)
			: base(state)
		{
		}

		[Obsolete("OdbcPermission(PermissionState state, Boolean allowBlankPassword) has been deprecated.  Use the OdbcPermission(PermissionState.None) constructor.  http://go.microsoft.com/fwlink/?linkid=14202", true)]
		public OdbcPermission(PermissionState state, bool allowBlankPassword)
			: this(state)
		{
			base.AllowBlankPassword = allowBlankPassword;
		}

		private OdbcPermission(OdbcPermission permission)
			: base(permission)
		{
		}

		internal OdbcPermission(OdbcPermissionAttribute permissionAttribute)
			: base(permissionAttribute)
		{
		}

		internal OdbcPermission(OdbcConnectionString constr)
			: base(constr)
		{
			if (constr == null || constr.IsEmpty)
			{
				base.Add(ADP.StrEmpty, ADP.StrEmpty, KeyRestrictionBehavior.AllowOnly);
			}
		}

		public override void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			DBConnectionString dbconnectionString = new DBConnectionString(connectionString, restrictions, behavior, null, true);
			base.AddPermissionEntry(dbconnectionString);
		}

		public override IPermission Copy()
		{
			return new OdbcPermission(this);
		}
	}
}
