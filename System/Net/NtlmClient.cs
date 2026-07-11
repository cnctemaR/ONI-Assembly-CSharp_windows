using System;
using Mono.Http;

namespace System.Net
{
	internal class NtlmClient : IAuthenticationModule
	{
		public NtlmClient()
		{
			this.authObject = new NtlmClient();
		}

		public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
		{
			if (this.authObject == null)
			{
				return null;
			}
			return this.authObject.Authenticate(challenge, webRequest, credentials);
		}

		public Authorization PreAuthenticate(WebRequest webRequest, ICredentials credentials)
		{
			return null;
		}

		public string AuthenticationType
		{
			get
			{
				return "NTLM";
			}
		}

		public bool CanPreAuthenticate
		{
			get
			{
				return false;
			}
		}

		private IAuthenticationModule authObject;
	}
}
