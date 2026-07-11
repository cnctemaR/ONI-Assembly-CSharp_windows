using System;
using System.Net;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Microsoft.Win32
{
	public class IntranetZoneCredentialPolicy : ICredentialPolicy
	{
		[SecurityPermission(SecurityAction.Demand, ControlPolicy = true)]
		public IntranetZoneCredentialPolicy()
		{
		}

		public virtual bool ShouldSendCredential(Uri challengeUri, WebRequest request, NetworkCredential credential, IAuthenticationModule authModule)
		{
			return Zone.CreateFromUrl(challengeUri.AbsoluteUri).SecurityZone == SecurityZone.Intranet;
		}
	}
}
