using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net
{
	[Serializable]
	public sealed class DnsPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public DnsPermission(PermissionState state)
		{
			this.m_noRestriction = state == PermissionState.Unrestricted;
		}

		public override IPermission Copy()
		{
			return new DnsPermission(this.m_noRestriction ? PermissionState.Unrestricted : PermissionState.None);
		}

		public override IPermission Intersect(IPermission target)
		{
			DnsPermission dnsPermission = this.Cast(target);
			if (dnsPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted() && dnsPermission.IsUnrestricted())
			{
				return new DnsPermission(PermissionState.Unrestricted);
			}
			return null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			DnsPermission dnsPermission = this.Cast(target);
			if (dnsPermission == null)
			{
				return this.IsEmpty();
			}
			return dnsPermission.IsUnrestricted() || this.m_noRestriction == dnsPermission.m_noRestriction;
		}

		public bool IsUnrestricted()
		{
			return this.m_noRestriction;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(typeof(DnsPermission), 1);
			if (this.m_noRestriction)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			return securityElement;
		}

		public override void FromXml(SecurityElement securityElement)
		{
			PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			if (securityElement.Tag != "IPermission")
			{
				throw new ArgumentException("securityElement");
			}
			this.m_noRestriction = PermissionHelper.IsUnrestricted(securityElement);
		}

		public override IPermission Union(IPermission target)
		{
			DnsPermission dnsPermission = this.Cast(target);
			if (dnsPermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || dnsPermission.IsUnrestricted())
			{
				return new DnsPermission(PermissionState.Unrestricted);
			}
			return new DnsPermission(PermissionState.None);
		}

		private bool IsEmpty()
		{
			return !this.m_noRestriction;
		}

		private DnsPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			DnsPermission dnsPermission = target as DnsPermission;
			if (dnsPermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, typeof(DnsPermission));
			}
			return dnsPermission;
		}

		private const int version = 1;

		private bool m_noRestriction;
	}
}
