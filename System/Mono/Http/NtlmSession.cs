using System;
using System.Net;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Http
{
	internal class NtlmSession
	{
		public global::System.Net.Authorization Authenticate(string challenge, global::System.Net.WebRequest webRequest, global::System.Net.ICredentials credentials)
		{
			global::System.Net.HttpWebRequest httpWebRequest = webRequest as global::System.Net.HttpWebRequest;
			if (httpWebRequest == null)
			{
				return null;
			}
			global::System.Net.NetworkCredential credential = credentials.GetCredential(httpWebRequest.RequestUri, "NTLM");
			if (credential == null)
			{
				return null;
			}
			string userName = credential.UserName;
			string text = credential.Domain;
			string text2 = credential.Password;
			if (userName == null || userName == string.Empty)
			{
				return null;
			}
			text = ((text == null || text.Length <= 0) ? httpWebRequest.Headers["Host"] : text);
			bool flag = false;
			if (this.message == null)
			{
				this.message = new Type1Message
				{
					Domain = text
				};
			}
			else if (this.message.Type == 1)
			{
				if (challenge == null)
				{
					this.message = null;
					return null;
				}
				Type2Message type2Message = new Type2Message(Convert.FromBase64String(challenge));
				if (text2 == null)
				{
					text2 = string.Empty;
				}
				this.message = new Type3Message
				{
					Domain = text,
					Username = userName,
					Challenge = type2Message.Nonce,
					Password = text2
				};
				flag = true;
			}
			else if (challenge == null || challenge == string.Empty)
			{
				this.message = new Type1Message
				{
					Domain = text
				};
			}
			else
			{
				flag = true;
			}
			string text3 = "NTLM " + Convert.ToBase64String(this.message.GetBytes());
			return new global::System.Net.Authorization(text3, flag);
		}

		private MessageBase message;
	}
}
