using System;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Microsoft.Win32
{
	public class IntranetZoneCredentialPolicy : global::System.Net.ICredentialPolicy
	{
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		public IntranetZoneCredentialPolicy()
		{
		}

		public virtual bool ShouldSendCredential(global::System.Uri challengeUri, global::System.Net.WebRequest request, global::System.Net.NetworkCredential credential, global::System.Net.IAuthenticationModule authenticationModule)
		{
			Zone zone = Zone.CreateFromUrl(challengeUri.AbsoluteUri);
			return zone.SecurityZone == SecurityZone.Intranet;
		}
	}
}
