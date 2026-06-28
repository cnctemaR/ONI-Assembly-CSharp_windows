using System;
using System.Security;
using System.Security.Permissions;

namespace System.Web
{
	[Serializable]
	public sealed class AspNetHostingPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public AspNetHostingPermission(AspNetHostingPermissionLevel level)
		{
			this.Level = level;
		}

		public AspNetHostingPermission(PermissionState state)
		{
			if (global::System.Security.Permissions.PermissionHelper.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this._level = AspNetHostingPermissionLevel.Unrestricted;
			}
			else
			{
				this._level = AspNetHostingPermissionLevel.None;
			}
		}

		public AspNetHostingPermissionLevel Level
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value < AspNetHostingPermissionLevel.None || value > AspNetHostingPermissionLevel.Unrestricted)
				{
					string text = global::Locale.GetText("Invalid enum {0}.");
					throw new ArgumentException(string.Format(text, value), "Level");
				}
				this._level = value;
			}
		}

		public bool IsUnrestricted()
		{
			return this._level == AspNetHostingPermissionLevel.Unrestricted;
		}

		public override IPermission Copy()
		{
			return new AspNetHostingPermission(this._level);
		}

		public override void FromXml(SecurityElement securityElement)
		{
			global::System.Security.Permissions.PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			if (securityElement.Tag != "IPermission")
			{
				string text = global::Locale.GetText("Invalid tag '{0}' for permission.");
				throw new ArgumentException(string.Format(text, securityElement.Tag), "securityElement");
			}
			if (securityElement.Attribute("version") == null)
			{
				string text2 = global::Locale.GetText("Missing version attribute.");
				throw new ArgumentException(text2, "securityElement");
			}
			if (global::System.Security.Permissions.PermissionHelper.IsUnrestricted(securityElement))
			{
				this._level = AspNetHostingPermissionLevel.Unrestricted;
			}
			else
			{
				string text3 = securityElement.Attribute("Level");
				if (text3 != null)
				{
					this._level = (AspNetHostingPermissionLevel)((int)Enum.Parse(typeof(AspNetHostingPermissionLevel), text3));
				}
				else
				{
					this._level = AspNetHostingPermissionLevel.None;
				}
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = global::System.Security.Permissions.PermissionHelper.Element(typeof(AspNetHostingPermission), 1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			securityElement.AddAttribute("Level", this._level.ToString());
			return securityElement;
		}

		public override IPermission Intersect(IPermission target)
		{
			AspNetHostingPermission aspNetHostingPermission = this.Cast(target);
			if (aspNetHostingPermission == null)
			{
				return null;
			}
			return new AspNetHostingPermission((this._level > aspNetHostingPermission.Level) ? aspNetHostingPermission.Level : this._level);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			AspNetHostingPermission aspNetHostingPermission = this.Cast(target);
			if (aspNetHostingPermission == null)
			{
				return this.IsEmpty();
			}
			return this._level <= aspNetHostingPermission._level;
		}

		public override IPermission Union(IPermission target)
		{
			AspNetHostingPermission aspNetHostingPermission = this.Cast(target);
			if (aspNetHostingPermission == null)
			{
				return this.Copy();
			}
			return new AspNetHostingPermission((this._level <= aspNetHostingPermission.Level) ? aspNetHostingPermission.Level : this._level);
		}

		private bool IsEmpty()
		{
			return this._level == AspNetHostingPermissionLevel.None;
		}

		private AspNetHostingPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			AspNetHostingPermission aspNetHostingPermission = target as AspNetHostingPermission;
			if (aspNetHostingPermission == null)
			{
				global::System.Security.Permissions.PermissionHelper.ThrowInvalidPermission(target, typeof(AspNetHostingPermission));
			}
			return aspNetHostingPermission;
		}

		private const int version = 1;

		private AspNetHostingPermissionLevel _level;
	}
}
