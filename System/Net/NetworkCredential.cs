using System;

namespace System.Net
{
	public class NetworkCredential : ICredentials, ICredentialsByHost
	{
		public NetworkCredential()
		{
		}

		public NetworkCredential(string userName, string password)
		{
			this.userName = userName;
			this.password = password;
		}

		public NetworkCredential(string userName, string password, string domain)
		{
			this.userName = userName;
			this.password = password;
			this.domain = domain;
		}

		public string Domain
		{
			get
			{
				return (this.domain != null) ? this.domain : string.Empty;
			}
			set
			{
				this.domain = value;
			}
		}

		public string UserName
		{
			get
			{
				return (this.userName != null) ? this.userName : string.Empty;
			}
			set
			{
				this.userName = value;
			}
		}

		public string Password
		{
			get
			{
				return (this.password != null) ? this.password : string.Empty;
			}
			set
			{
				this.password = value;
			}
		}

		public NetworkCredential GetCredential(global::System.Uri uri, string authType)
		{
			return this;
		}

		public NetworkCredential GetCredential(string host, int port, string authenticationType)
		{
			return this;
		}

		private string userName;

		private string password;

		private string domain;
	}
}
