using System;
using System.Collections;
using System.Net;

namespace Mono.Http
{
	internal class NtlmClient : global::System.Net.IAuthenticationModule
	{
		public global::System.Net.Authorization Authenticate(string challenge, global::System.Net.WebRequest webRequest, global::System.Net.ICredentials credentials)
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
			global::System.Net.HttpWebRequest httpWebRequest = webRequest as global::System.Net.HttpWebRequest;
			if (httpWebRequest == null)
			{
				return null;
			}
			Hashtable hashtable = Mono.Http.NtlmClient.cache;
			global::System.Net.Authorization authorization;
			lock (hashtable)
			{
				NtlmSession ntlmSession = (NtlmSession)Mono.Http.NtlmClient.cache[httpWebRequest.RequestUri];
				if (ntlmSession == null)
				{
					ntlmSession = new NtlmSession();
					Mono.Http.NtlmClient.cache.Add(httpWebRequest.RequestUri, ntlmSession);
				}
				authorization = ntlmSession.Authenticate(text, webRequest, credentials);
			}
			return authorization;
		}

		public global::System.Net.Authorization PreAuthenticate(global::System.Net.WebRequest webRequest, global::System.Net.ICredentials credentials)
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

		private static Hashtable cache = new Hashtable();
	}
}
