using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class SiteIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		public SiteIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public string Site
		{
			get
			{
				return this.site;
			}
			set
			{
				this.site = value;
			}
		}

		public override IPermission CreatePermission()
		{
			SiteIdentityPermission siteIdentityPermission;
			if (base.Unrestricted)
			{
				siteIdentityPermission = new SiteIdentityPermission(PermissionState.Unrestricted);
			}
			else if (this.site == null)
			{
				siteIdentityPermission = new SiteIdentityPermission(PermissionState.None);
			}
			else
			{
				siteIdentityPermission = new SiteIdentityPermission(this.site);
			}
			return siteIdentityPermission;
		}

		private string site;
	}
}
