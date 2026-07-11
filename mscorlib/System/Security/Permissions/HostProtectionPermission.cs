using System;

namespace System.Security.Permissions
{
	[Serializable]
	internal sealed class HostProtectionPermission : CodeAccessPermission, IUnrestrictedPermission, IBuiltInPermission
	{
		public HostProtectionPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this._resources = HostProtectionResource.All;
				return;
			}
			this._resources = HostProtectionResource.None;
		}

		public HostProtectionPermission(HostProtectionResource resources)
		{
			this.Resources = this._resources;
		}

		public HostProtectionResource Resources
		{
			get
			{
				return this._resources;
			}
			set
			{
				if (!Enum.IsDefined(typeof(HostProtectionResource), value))
				{
					throw new ArgumentException(string.Format(Locale.GetText("Invalid enum {0}"), value), "HostProtectionResource");
				}
				this._resources = value;
			}
		}

		public override IPermission Copy()
		{
			return new HostProtectionPermission(this._resources);
		}

		public override IPermission Intersect(IPermission target)
		{
			HostProtectionPermission hostProtectionPermission = this.Cast(target);
			if (hostProtectionPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted() && hostProtectionPermission.IsUnrestricted())
			{
				return new HostProtectionPermission(PermissionState.Unrestricted);
			}
			if (this.IsUnrestricted())
			{
				return hostProtectionPermission.Copy();
			}
			if (hostProtectionPermission.IsUnrestricted())
			{
				return this.Copy();
			}
			return new HostProtectionPermission(this._resources & hostProtectionPermission._resources);
		}

		public override IPermission Union(IPermission target)
		{
			HostProtectionPermission hostProtectionPermission = this.Cast(target);
			if (hostProtectionPermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || hostProtectionPermission.IsUnrestricted())
			{
				return new HostProtectionPermission(PermissionState.Unrestricted);
			}
			return new HostProtectionPermission(this._resources | hostProtectionPermission._resources);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			HostProtectionPermission hostProtectionPermission = this.Cast(target);
			if (hostProtectionPermission == null)
			{
				return this._resources == HostProtectionResource.None;
			}
			return hostProtectionPermission.IsUnrestricted() || (!this.IsUnrestricted() && (this._resources & ~hostProtectionPermission._resources) == HostProtectionResource.None);
		}

		public override void FromXml(SecurityElement e)
		{
			CodeAccessPermission.CheckSecurityElement(e, "e", 1, 1);
			this._resources = (HostProtectionResource)Enum.Parse(typeof(HostProtectionResource), e.Attribute("Resources"));
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.Element(1);
			securityElement.AddAttribute("Resources", this._resources.ToString());
			return securityElement;
		}

		public bool IsUnrestricted()
		{
			return this._resources == HostProtectionResource.All;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 9;
		}

		private HostProtectionPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			HostProtectionPermission hostProtectionPermission = target as HostProtectionPermission;
			if (hostProtectionPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(HostProtectionPermission));
			}
			return hostProtectionPermission;
		}

		private const int version = 1;

		private HostProtectionResource _resources;
	}
}
