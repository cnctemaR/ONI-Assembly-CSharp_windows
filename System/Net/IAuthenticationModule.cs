using System;

namespace System.Net
{
	public interface IAuthenticationModule
	{
		Authorization Authenticate(string challenge, WebRequest request, ICredentials credentials);

		Authorization PreAuthenticate(WebRequest request, ICredentials credentials);

		bool CanPreAuthenticate { get; }

		string AuthenticationType { get; }
	}
}
