using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net.Mail
{
	[Serializable]
	public sealed class SmtpPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public SmtpPermission(bool unrestricted)
		{
			this.unrestricted = unrestricted;
			this.access = (unrestricted ? SmtpAccess.ConnectToUnrestrictedPort : SmtpAccess.None);
		}

		public SmtpPermission(PermissionState state)
		{
			this.unrestricted = state == PermissionState.Unrestricted;
			this.access = (this.unrestricted ? SmtpAccess.ConnectToUnrestrictedPort : SmtpAccess.None);
		}

		public SmtpPermission(SmtpAccess access)
		{
			this.access = access;
		}

		public SmtpAccess Access
		{
			get
			{
				return this.access;
			}
		}

		public void AddPermission(SmtpAccess access)
		{
			if (!this.unrestricted && access > this.access)
			{
				this.access = access;
			}
		}

		public override IPermission Copy()
		{
			if (this.unrestricted)
			{
				return new SmtpPermission(true);
			}
			return new SmtpPermission(this.access);
		}

		public override IPermission Intersect(IPermission target)
		{
			SmtpPermission smtpPermission = this.Cast(target);
			if (smtpPermission == null)
			{
				return null;
			}
			if (this.unrestricted && smtpPermission.unrestricted)
			{
				return new SmtpPermission(true);
			}
			if (this.access > smtpPermission.access)
			{
				return new SmtpPermission(smtpPermission.access);
			}
			return new SmtpPermission(this.access);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			SmtpPermission smtpPermission = this.Cast(target);
			if (smtpPermission == null)
			{
				return this.IsEmpty();
			}
			if (this.unrestricted)
			{
				return smtpPermission.unrestricted;
			}
			return this.access <= smtpPermission.access;
		}

		public bool IsUnrestricted()
		{
			return this.unrestricted;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(typeof(SmtpPermission), 1);
			if (this.unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				SmtpAccess smtpAccess = this.access;
				if (smtpAccess != SmtpAccess.Connect)
				{
					if (smtpAccess == SmtpAccess.ConnectToUnrestrictedPort)
					{
						securityElement.AddAttribute("Access", "ConnectToUnrestrictedPort");
					}
				}
				else
				{
					securityElement.AddAttribute("Access", "Connect");
				}
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
			if (PermissionHelper.IsUnrestricted(securityElement))
			{
				this.access = SmtpAccess.Connect;
				return;
			}
			this.access = SmtpAccess.None;
		}

		public override IPermission Union(IPermission target)
		{
			SmtpPermission smtpPermission = this.Cast(target);
			if (smtpPermission == null)
			{
				return this.Copy();
			}
			if (this.unrestricted || smtpPermission.unrestricted)
			{
				return new SmtpPermission(true);
			}
			if (this.access > smtpPermission.access)
			{
				return new SmtpPermission(this.access);
			}
			return new SmtpPermission(smtpPermission.access);
		}

		private bool IsEmpty()
		{
			return !this.unrestricted && this.access == SmtpAccess.None;
		}

		private SmtpPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			SmtpPermission smtpPermission = target as SmtpPermission;
			if (smtpPermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, typeof(SmtpPermission));
			}
			return smtpPermission;
		}

		private const int version = 1;

		private bool unrestricted;

		private SmtpAccess access;
	}
}
