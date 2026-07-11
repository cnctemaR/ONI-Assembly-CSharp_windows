using System;

namespace System.Security.Permissions
{
	[Serializable]
	public sealed class StorePermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public StorePermission(PermissionState state)
		{
			if (PermissionHelper.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this._flags = StorePermissionFlags.AllFlags;
				return;
			}
			this._flags = StorePermissionFlags.NoFlags;
		}

		public StorePermission(StorePermissionFlags flag)
		{
			this.Flags = flag;
		}

		public StorePermissionFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if (value != StorePermissionFlags.NoFlags && (value & StorePermissionFlags.AllFlags) == StorePermissionFlags.NoFlags)
				{
					throw new ArgumentException(string.Format(global::Locale.GetText("Invalid enum {0}"), value), "StorePermissionFlags");
				}
				this._flags = value;
			}
		}

		public bool IsUnrestricted()
		{
			return this._flags == StorePermissionFlags.AllFlags;
		}

		public override IPermission Copy()
		{
			if (this._flags == StorePermissionFlags.NoFlags)
			{
				return null;
			}
			return new StorePermission(this._flags);
		}

		public override IPermission Intersect(IPermission target)
		{
			StorePermission storePermission = this.Cast(target);
			if (storePermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted() && storePermission.IsUnrestricted())
			{
				return new StorePermission(PermissionState.Unrestricted);
			}
			if (this.IsUnrestricted())
			{
				return storePermission.Copy();
			}
			if (storePermission.IsUnrestricted())
			{
				return this.Copy();
			}
			StorePermissionFlags storePermissionFlags = this._flags & storePermission._flags;
			if (storePermissionFlags == StorePermissionFlags.NoFlags)
			{
				return null;
			}
			return new StorePermission(storePermissionFlags);
		}

		public override IPermission Union(IPermission target)
		{
			StorePermission storePermission = this.Cast(target);
			if (storePermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || storePermission.IsUnrestricted())
			{
				return new StorePermission(PermissionState.Unrestricted);
			}
			StorePermissionFlags storePermissionFlags = this._flags | storePermission._flags;
			if (storePermissionFlags == StorePermissionFlags.NoFlags)
			{
				return null;
			}
			return new StorePermission(storePermissionFlags);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			StorePermission storePermission = this.Cast(target);
			if (storePermission == null)
			{
				return this._flags == StorePermissionFlags.NoFlags;
			}
			return storePermission.IsUnrestricted() || (!this.IsUnrestricted() && (this._flags & ~storePermission._flags) == StorePermissionFlags.NoFlags);
		}

		public override void FromXml(SecurityElement securityElement)
		{
			PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			string text = securityElement.Attribute("Flags");
			if (text == null)
			{
				this._flags = StorePermissionFlags.NoFlags;
				return;
			}
			this._flags = (StorePermissionFlags)Enum.Parse(typeof(StorePermissionFlags), text);
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(typeof(StorePermission), 1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", bool.TrueString);
			}
			else
			{
				securityElement.AddAttribute("Flags", this._flags.ToString());
			}
			return securityElement;
		}

		private StorePermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			StorePermission storePermission = target as StorePermission;
			if (storePermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, typeof(StorePermission));
			}
			return storePermission;
		}

		private const int version = 1;

		private StorePermissionFlags _flags;
	}
}
