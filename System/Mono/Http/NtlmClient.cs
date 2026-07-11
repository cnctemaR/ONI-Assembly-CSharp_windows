using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Mono.Http
{
	internal class NtlmClient : IAuthenticationModule
	{
		public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
		{
			if (credentials == null || challenge == null)
			{
				return null;
			}
			string text = challenge.Trim();
			int num = text.ToLower().IndexOf("ntlm");
			if (num == -1)
			{
				return null;
			}
			num = text.IndexOfAny(new char[] { ' ', '\t' });
			if (num != -1)
			{
				text = text.Substring(num).Trim();
			}
			else
			{
				text = null;
			}
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return null;
			}
			ConditionalWeakTable<HttpWebRequest, NtlmSession> conditionalWeakTable = NtlmClient.cache;
			Authorization authorization;
			lock (conditionalWeakTable)
			{
				authorization = NtlmClient.cache.GetValue(httpWebRequest, (HttpWebRequest x) => new NtlmSession()).Authenticate(text, webRequest, credentials);
			}
			return authorization;
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

		private static readonly ConditionalWeakTable<HttpWebRequest, NtlmSession> cache = new ConditionalWeakTable<HttpWebRequest, NtlmSession>();
	}
}
