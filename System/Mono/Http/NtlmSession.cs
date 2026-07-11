using System;
using System.Net;
using Mono.Security.Protocol.Ntlm;

namespace Mono.Http
{
	internal class NtlmSession
	{
		public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials)
		{
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest == null)
			{
				return null;
			}
			NetworkCredential credential = credentials.GetCredential(httpWebRequest.RequestUri, "NTLM");
			if (credential == null)
			{
				return null;
			}
			string text = credential.UserName;
			string text2 = credential.Domain;
			string text3 = credential.Password;
			if (text == null || text == "")
			{
				return null;
			}
			if (string.IsNullOrEmpty(text2))
			{
				int num = text.IndexOf('\\');
				if (num == -1)
				{
					num = text.IndexOf('/');
				}
				if (num >= 0)
				{
					text2 = text.Substring(0, num);
					text = text.Substring(num + 1);
				}
			}
			bool flag = false;
			if (this.message == null)
			{
				Type1Message type1Message = new Type1Message();
				type1Message.Domain = text2;
				type1Message.Host = "";
				type1Message.Flags |= NtlmFlags.NegotiateNtlm2Key;
				this.message = type1Message;
			}
			else if (this.message.Type == 1)
			{
				if (challenge == null)
				{
					this.message = null;
					return null;
				}
				Type2Message type2Message = new Type2Message(Convert.FromBase64String(challenge));
				if (text3 == null)
				{
					text3 = "";
				}
				this.message = new Type3Message(type2Message)
				{
					Username = text,
					Password = text3,
					Domain = text2
				};
				flag = true;
			}
			else if (challenge == null || challenge == string.Empty)
			{
				this.message = new Type1Message
				{
					Domain = text2,
					Host = ""
				};
			}
			else
			{
				flag = true;
			}
			return new Authorization("NTLM " + Convert.ToBase64String(this.message.GetBytes()), flag);
		}

		private MessageBase message;
	}
}
