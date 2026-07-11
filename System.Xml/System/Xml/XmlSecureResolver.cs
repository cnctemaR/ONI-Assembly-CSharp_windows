using System;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading.Tasks;

namespace System.Xml
{
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class XmlSecureResolver : XmlResolver
	{
		public XmlSecureResolver(XmlResolver resolver, string securityUrl)
			: this(resolver, null)
		{
		}

		public XmlSecureResolver(XmlResolver resolver, Evidence evidence)
			: this(resolver, null)
		{
		}

		public XmlSecureResolver(XmlResolver resolver, PermissionSet permissionSet)
		{
			this.resolver = resolver;
		}

		public override ICredentials Credentials
		{
			set
			{
				this.resolver.Credentials = value;
			}
		}

		public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			return this.resolver.GetEntity(absoluteUri, role, ofObjectToReturn);
		}

		public override Uri ResolveUri(Uri baseUri, string relativeUri)
		{
			return this.resolver.ResolveUri(baseUri, relativeUri);
		}

		public static Evidence CreateEvidenceForUrl(string securityUrl)
		{
			return null;
		}

		public override Task<object> GetEntityAsync(Uri absoluteUri, string role, Type ofObjectToReturn)
		{
			return this.resolver.GetEntityAsync(absoluteUri, role, ofObjectToReturn);
		}

		private XmlResolver resolver;
	}
}
