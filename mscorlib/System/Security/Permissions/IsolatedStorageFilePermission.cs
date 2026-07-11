using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class IsolatedStorageFilePermission : IsolatedStoragePermission, IBuiltInPermission
	{
		public IsolatedStorageFilePermission(PermissionState state)
			: base(state)
		{
		}

		public override IPermission Copy()
		{
			return new IsolatedStorageFilePermission(PermissionState.None)
			{
				m_userQuota = this.m_userQuota,
				m_machineQuota = this.m_machineQuota,
				m_expirationDays = this.m_expirationDays,
				m_permanentData = this.m_permanentData,
				m_allowed = this.m_allowed
			};
		}

		public override IPermission Intersect(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = this.Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return null;
			}
			if (base.IsEmpty() && isolatedStorageFilePermission.IsEmpty())
			{
				return null;
			}
			return new IsolatedStorageFilePermission(PermissionState.None)
			{
				m_userQuota = ((this.m_userQuota < isolatedStorageFilePermission.m_userQuota) ? this.m_userQuota : isolatedStorageFilePermission.m_userQuota),
				m_machineQuota = ((this.m_machineQuota < isolatedStorageFilePermission.m_machineQuota) ? this.m_machineQuota : isolatedStorageFilePermission.m_machineQuota),
				m_expirationDays = ((this.m_expirationDays < isolatedStorageFilePermission.m_expirationDays) ? this.m_expirationDays : isolatedStorageFilePermission.m_expirationDays),
				m_permanentData = (this.m_permanentData && isolatedStorageFilePermission.m_permanentData),
				UsageAllowed = ((this.m_allowed < isolatedStorageFilePermission.m_allowed) ? this.m_allowed : isolatedStorageFilePermission.m_allowed)
			};
		}

		public override bool IsSubsetOf(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = this.Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return base.IsEmpty();
			}
			return isolatedStorageFilePermission.IsUnrestricted() || (this.m_userQuota <= isolatedStorageFilePermission.m_userQuota && this.m_machineQuota <= isolatedStorageFilePermission.m_machineQuota && this.m_expirationDays <= isolatedStorageFilePermission.m_expirationDays && this.m_permanentData == isolatedStorageFilePermission.m_permanentData && this.m_allowed <= isolatedStorageFilePermission.m_allowed);
		}

		public override IPermission Union(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = this.Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return this.Copy();
			}
			return new IsolatedStorageFilePermission(PermissionState.None)
			{
				m_userQuota = ((this.m_userQuota > isolatedStorageFilePermission.m_userQuota) ? this.m_userQuota : isolatedStorageFilePermission.m_userQuota),
				m_machineQuota = ((this.m_machineQuota > isolatedStorageFilePermission.m_machineQuota) ? this.m_machineQuota : isolatedStorageFilePermission.m_machineQuota),
				m_expirationDays = ((this.m_expirationDays > isolatedStorageFilePermission.m_expirationDays) ? this.m_expirationDays : isolatedStorageFilePermission.m_expirationDays),
				m_permanentData = (this.m_permanentData || isolatedStorageFilePermission.m_permanentData),
				UsageAllowed = ((this.m_allowed > isolatedStorageFilePermission.m_allowed) ? this.m_allowed : isolatedStorageFilePermission.m_allowed)
			};
		}

		[MonoTODO("(2.0) new override - something must have been added ???")]
		[ComVisible(false)]
		public override SecurityElement ToXml()
		{
			return base.ToXml();
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 3;
		}

		private IsolatedStorageFilePermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			IsolatedStorageFilePermission isolatedStorageFilePermission = target as IsolatedStorageFilePermission;
			if (isolatedStorageFilePermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(IsolatedStorageFilePermission));
			}
			return isolatedStorageFilePermission;
		}
	}
}
