using System;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	[Serializable]
	public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		[Obsolete("use DBDataPermission (PermissionState.None)", true)]
		protected DBDataPermission()
		{
		}

		protected DBDataPermission(DBDataPermission permission)
		{
		}

		protected DBDataPermission(DBDataPermissionAttribute permissionAttribute)
		{
		}

		protected DBDataPermission(PermissionState state)
		{
		}

		[Obsolete("use DBDataPermission (PermissionState.None)", true)]
		protected DBDataPermission(PermissionState state, bool allowBlankPassword)
		{
		}

		public bool AllowBlankPassword
		{
			get
			{
				throw null;
			}
			set
			{
			}
		}

		public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
		}

		protected void Clear()
		{
		}

		public override IPermission Copy()
		{
			throw null;
		}

		protected virtual DBDataPermission CreateInstance()
		{
			throw null;
		}

		public override void FromXml(SecurityElement securityElement)
		{
		}

		public override IPermission Intersect(IPermission target)
		{
			throw null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			throw null;
		}

		public bool IsUnrestricted()
		{
			throw null;
		}

		public override SecurityElement ToXml()
		{
			throw null;
		}

		public override IPermission Union(IPermission target)
		{
			throw null;
		}
	}
}
