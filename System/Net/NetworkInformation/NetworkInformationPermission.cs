using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net.NetworkInformation
{
	[Serializable]
	public sealed class NetworkInformationPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		[global::System.MonoTODO]
		public NetworkInformationPermission(PermissionState state)
		{
		}

		[global::System.MonoTODO]
		public NetworkInformationPermission(NetworkInformationAccess access)
		{
		}

		[global::System.MonoTODO]
		public void AddPermission(NetworkInformationAccess access)
		{
		}

		[global::System.MonoTODO]
		public override IPermission Copy()
		{
			return null;
		}

		[global::System.MonoTODO]
		public override void FromXml(SecurityElement securityElement)
		{
		}

		[global::System.MonoTODO]
		public override IPermission Intersect(IPermission target)
		{
			return null;
		}

		[global::System.MonoTODO]
		public override bool IsSubsetOf(IPermission target)
		{
			return false;
		}

		[global::System.MonoTODO]
		public bool IsUnrestricted()
		{
			return false;
		}

		[global::System.MonoTODO]
		public override SecurityElement ToXml()
		{
			return global::System.Security.Permissions.PermissionHelper.Element(typeof(NetworkInformationPermission), 1);
		}

		[global::System.MonoTODO]
		public override IPermission Union(IPermission target)
		{
			return null;
		}

		[global::System.MonoTODO]
		public NetworkInformationAccess Access
		{
			get
			{
				return NetworkInformationAccess.None;
			}
		}

		private const int version = 1;
	}
}
