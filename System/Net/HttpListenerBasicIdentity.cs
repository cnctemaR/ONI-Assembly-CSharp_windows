using System;
using System.Security.Principal;

namespace System.Net
{
	public class HttpListenerBasicIdentity : GenericIdentity
	{
		public HttpListenerBasicIdentity(string username, string password)
			: base(username, "Basic")
		{
			this.password = password;
		}

		public virtual string Password
		{
			get
			{
				return this.password;
			}
		}

		private string password;
	}
}
