using System;
using System.Net;
using System.Security;
using System.Security.Policy;

namespace System.Xml
{
	public class XmlSecureResolver : XmlResolver
	{
		public XmlSecureResolver(XmlResolver resolver, Evidence evidence)
		{
			this.resolver = resolver;
			if (SecurityManager.SecurityEnabled)
			{
				this.permissionSet = SecurityManager.ResolvePolicy(evidence);
			}
		}

		public XmlSecureResolver(XmlResolver resolver, PermissionSet permissionSet)
		{
			this.resolver = resolver;
			this.permissionSet = permissionSet;
		}

		public XmlSecureResolver(XmlResolver resolver, string securityUrl)
		{
			this.resolver = resolver;
			if (SecurityManager.SecurityEnabled)
			{
				this.permissionSet = SecurityManager.ResolvePolicy(XmlSecureResolver.CreateEvidenceForUrl(securityUrl));
			}
		}

		public static Evidence CreateEvidenceForUrl(string securityUrl)
		{
			Evidence evidence = new Evidence();
			if (securityUrl != null && securityUrl.Length > 0)
			{
				try
				{
					Url url = new Url(securityUrl);
					evidence.AddHost(url);
				}
				catch (ArgumentException)
				{
				}
				try
				{
					Zone zone = Zone.CreateFromUrl(securityUrl);
					evidence.AddHost(zone);
				}
				catch (ArgumentException)
				{
				}
				try
				{
					Site site = Site.CreateFromUrl(securityUrl);
					evidence.AddHost(site);
				}
				catch (ArgumentException)
				{
				}
			}
			return evidence;
		}

		public override ICredentials Credentials
		{
			set
			{
				this.resolver.Credentials = value;
			}
		}

		[MonoTODO]
		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			if (SecurityManager.SecurityEnabled)
			{
				if (this.permissionSet == null)
				{
					throw new SecurityException(Locale.GetText("Security Manager wasn't active when instance was created."));
				}
				this.permissionSet.PermitOnly();
			}
			return this.resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			return this.resolver.ResolveUri(baseUri, relativeUri);
		}

		private XmlResolver resolver;

		private PermissionSet permissionSet;
	}
}
