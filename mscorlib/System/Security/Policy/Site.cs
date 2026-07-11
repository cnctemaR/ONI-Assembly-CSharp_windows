using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Mono.Security;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Site : EvidenceBase, IIdentityPermissionFactory, IBuiltInEvidence
	{
		public Site(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("url");
			}
			if (!Site.IsValid(name))
			{
				throw new ArgumentException(Locale.GetText("name is not valid"));
			}
			this.origin_site = name;
		}

		public static Site CreateFromUrl(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			if (url.Length == 0)
			{
				throw new FormatException(Locale.GetText("Empty URL."));
			}
			string text = Site.UrlToSite(url);
			if (text == null)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Invalid URL '{0}'."), url), "url");
			}
			return new Site(text);
		}

		public object Copy()
		{
			return new Site(this.origin_site);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new SiteIdentityPermission(this.origin_site);
		}

		public override bool Equals(object o)
		{
			Site site = o as Site;
			return site != null && string.Compare(site.Name, this.origin_site, true, CultureInfo.InvariantCulture) == 0;
		}

		public override int GetHashCode()
		{
			return this.origin_site.GetHashCode();
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Site");
			securityElement.AddAttribute("version", "1");
			securityElement.AddChild(new SecurityElement("Name", this.origin_site));
			return securityElement.ToString();
		}

		public string Name
		{
			get
			{
				return this.origin_site;
			}
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return (verbose ? 3 : 1) + this.origin_site.Length;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		internal static bool IsValid(string name)
		{
			if (name == string.Empty)
			{
				return false;
			}
			if (name.Length == 1 && name == ".")
			{
				return false;
			}
			string[] array = name.Split(new char[] { '.' });
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (i != 0 || !(text == "*"))
				{
					string text2 = text;
					for (int j = 0; j < text2.Length; j++)
					{
						int num = Convert.ToInt32(text2[j]);
						if (num != 33 && num != 45 && (num < 35 || num > 41) && (num < 48 || num > 57) && (num < 64 || num > 90) && (num < 94 || num > 95) && (num < 97 || num > 123) && (num < 125 || num > 126))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal static string UrlToSite(string url)
		{
			if (url == null)
			{
				return null;
			}
			Uri uri = new Uri(url);
			if (uri.Scheme == Uri.UriSchemeFile)
			{
				return null;
			}
			string host = uri.Host;
			if (!Site.IsValid(host))
			{
				return null;
			}
			return host;
		}

		internal string origin_site;
	}
}
