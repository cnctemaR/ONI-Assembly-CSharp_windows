using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class UrlIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		public UrlIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
			this.url = string.Empty;
		}

		public UrlIdentityPermission(string site)
		{
			if (site == null)
			{
				throw new ArgumentNullException("site");
			}
			this.url = site;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 13;
		}

		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				this.url = ((value != null) ? value : string.Empty);
			}
		}

		public override IPermission Copy()
		{
			if (this.url == null)
			{
				return new UrlIdentityPermission(PermissionState.None);
			}
			return new UrlIdentityPermission(this.url);
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			string text = esd.Attribute("Url");
			if (text == null)
			{
				this.url = string.Empty;
			}
			else
			{
				this.Url = text;
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			UrlIdentityPermission urlIdentityPermission = this.Cast(target);
			if (urlIdentityPermission == null || this.IsEmpty())
			{
				return null;
			}
			if (!this.Match(urlIdentityPermission.url))
			{
				return null;
			}
			if (this.url.Length > urlIdentityPermission.url.Length)
			{
				return this.Copy();
			}
			return urlIdentityPermission.Copy();
		}

		public override bool IsSubsetOf(IPermission target)
		{
			UrlIdentityPermission urlIdentityPermission = this.Cast(target);
			if (urlIdentityPermission == null)
			{
				return this.IsEmpty();
			}
			if (this.IsEmpty())
			{
				return true;
			}
			if (urlIdentityPermission.url == null)
			{
				return false;
			}
			int num = urlIdentityPermission.url.LastIndexOf('*');
			if (num == -1)
			{
				num = urlIdentityPermission.url.Length;
			}
			return string.Compare(this.url, 0, urlIdentityPermission.url, 0, num, true, CultureInfo.InvariantCulture) == 0;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.Element(1);
			if (!this.IsEmpty())
			{
				securityElement.AddAttribute("Url", this.url);
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			UrlIdentityPermission urlIdentityPermission = this.Cast(target);
			if (urlIdentityPermission == null)
			{
				return this.Copy();
			}
			if (this.IsEmpty() && urlIdentityPermission.IsEmpty())
			{
				return null;
			}
			if (urlIdentityPermission.IsEmpty())
			{
				return this.Copy();
			}
			if (this.IsEmpty())
			{
				return urlIdentityPermission.Copy();
			}
			if (!this.Match(urlIdentityPermission.url))
			{
				throw new ArgumentException(Locale.GetText("Cannot union two different urls."), "target");
			}
			if (this.url.Length < urlIdentityPermission.url.Length)
			{
				return this.Copy();
			}
			return urlIdentityPermission.Copy();
		}

		private bool IsEmpty()
		{
			return this.url == null || this.url.Length == 0;
		}

		private UrlIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			UrlIdentityPermission urlIdentityPermission = target as UrlIdentityPermission;
			if (urlIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(UrlIdentityPermission));
			}
			return urlIdentityPermission;
		}

		private bool Match(string target)
		{
			if (this.url == null || target == null)
			{
				return false;
			}
			int num = this.url.LastIndexOf('*');
			int num2 = target.LastIndexOf('*');
			int num3;
			if (num == -1 && num2 == -1)
			{
				num3 = Math.Max(this.url.Length, target.Length);
			}
			else if (num == -1)
			{
				num3 = num2;
			}
			else if (num2 == -1)
			{
				num3 = num;
			}
			else
			{
				num3 = Math.Min(num, num2);
			}
			return string.Compare(this.url, 0, target, 0, num3, true, CultureInfo.InvariantCulture) == 0;
		}

		private const int version = 1;

		private string url;
	}
}
