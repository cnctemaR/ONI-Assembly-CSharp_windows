using System;

namespace System.Net
{
	public interface ICredentialPolicy
	{
		bool ShouldSendCredential(global::System.Uri challengeUri, WebRequest request, NetworkCredential credential, IAuthenticationModule authenticationModule);
	}
}
