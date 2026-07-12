using System;

namespace System.Security.Permissions
{
	[Serializable]
	public sealed class DataProtectionPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public DataProtectionPermission(PermissionState state)
		{
			if (PermissionHelper.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this._flags = DataProtectionPermissionFlags.AllFlags;
			}
		}

		public DataProtectionPermission(DataProtectionPermissionFlags flag)
		{
			this.Flags = flag;
		}

		public DataProtectionPermissionFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if ((value & ~(DataProtectionPermissionFlags.ProtectData | DataProtectionPermissionFlags.UnprotectData | DataProtectionPermissionFlags.ProtectMemory | DataProtectionPermissionFlags.UnprotectMemory)) != DataProtectionPermissionFlags.NoFlags)
				{
					throw new ArgumentException(string.Format(Locale.GetText("Invalid enum {0}"), value), "DataProtectionPermissionFlags");
				}
				this._flags = value;
			}
		}

		public bool IsUnrestricted()
		{
			return this._flags == DataProtectionPermissionFlags.AllFlags;
		}

		public override IPermission Copy()
		{
			return new DataProtectionPermission(this._flags);
		}

		public override IPermission Intersect(IPermission target)
		{
			DataProtectionPermission dataProtectionPermission = this.Cast(target);
			if (dataProtectionPermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted() && dataProtectionPermission.IsUnrestricted())
			{
				return new DataProtectionPermission(PermissionState.Unrestricted);
			}
			if (this.IsUnrestricted())
			{
				return dataProtectionPermission.Copy();
			}
			if (dataProtectionPermission.IsUnrestricted())
			{
				return this.Copy();
			}
			return new DataProtectionPermission(this._flags & dataProtectionPermission._flags);
		}

		public override IPermission Union(IPermission target)
		{
			DataProtectionPermission dataProtectionPermission = this.Cast(target);
			if (dataProtectionPermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || dataProtectionPermission.IsUnrestricted())
			{
				return new SecurityPermission(PermissionState.Unrestricted);
			}
			return new DataProtectionPermission(this._flags | dataProtectionPermission._flags);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			DataProtectionPermission dataProtectionPermission = this.Cast(target);
			if (dataProtectionPermission == null)
			{
				return this._flags == DataProtectionPermissionFlags.NoFlags;
			}
			return dataProtectionPermission.IsUnrestricted() || (!this.IsUnrestricted() && (this._flags & ~dataProtectionPermission._flags) == DataProtectionPermissionFlags.NoFlags);
		}

		public override void FromXml(SecurityElement securityElement)
		{
			PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			this._flags = (DataProtectionPermissionFlags)Enum.Parse(typeof(DataProtectionPermissionFlags), securityElement.Attribute("Flags"));
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(typeof(DataProtectionPermission), 1);
			securityElement.AddAttribute("Flags", this._flags.ToString());
			return securityElement;
		}

		private DataProtectionPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			DataProtectionPermission dataProtectionPermission = target as DataProtectionPermission;
			if (dataProtectionPermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, typeof(DataProtectionPermission));
			}
			return dataProtectionPermission;
		}

		private const int version = 1;

		private DataProtectionPermissionFlags _flags;
	}
}
