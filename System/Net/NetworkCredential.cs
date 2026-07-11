using System;
using System.Security;

namespace System.Net
{
	public class NetworkCredential : ICredentials, ICredentialsByHost
	{
		public NetworkCredential()
			: this(string.Empty, string.Empty, string.Empty)
		{
		}

		public NetworkCredential(string userName, string password)
			: this(userName, password, string.Empty)
		{
		}

		public NetworkCredential(string userName, SecureString password)
			: this(userName, password, string.Empty)
		{
		}

		public NetworkCredential(string userName, string password, string domain)
		{
			this.UserName = userName;
			this.Password = password;
			this.Domain = domain;
		}

		public NetworkCredential(string userName, SecureString password, string domain)
		{
			this.UserName = userName;
			this.SecurePassword = password;
			this.Domain = domain;
		}

		public string UserName
		{
			get
			{
				return this.InternalGetUserName();
			}
			set
			{
				if (value == null)
				{
					this.m_userName = string.Empty;
					return;
				}
				this.m_userName = value;
			}
		}

		public string Password
		{
			get
			{
				return this.InternalGetPassword();
			}
			set
			{
				this.m_password = UnsafeNclNativeMethods.SecureStringHelper.CreateSecureString(value);
			}
		}

		public SecureString SecurePassword
		{
			get
			{
				return this.InternalGetSecurePassword().Copy();
			}
			set
			{
				if (value == null)
				{
					this.m_password = new SecureString();
					return;
				}
				this.m_password = value.Copy();
			}
		}

		public string Domain
		{
			get
			{
				return this.InternalGetDomain();
			}
			set
			{
				if (value == null)
				{
					this.m_domain = string.Empty;
					return;
				}
				this.m_domain = value;
			}
		}

		internal string InternalGetUserName()
		{
			return this.m_userName;
		}

		internal string InternalGetPassword()
		{
			return UnsafeNclNativeMethods.SecureStringHelper.CreateString(this.m_password);
		}

		internal SecureString InternalGetSecurePassword()
		{
			return this.m_password;
		}

		internal string InternalGetDomain()
		{
			return this.m_domain;
		}

		internal string InternalGetDomainUserName()
		{
			string text = this.InternalGetDomain();
			if (text.Length != 0)
			{
				text += "\\";
			}
			return text + this.InternalGetUserName();
		}

		public NetworkCredential GetCredential(Uri uri, string authType)
		{
			return this;
		}

		public NetworkCredential GetCredential(string host, int port, string authenticationType)
		{
			return this;
		}

		private string m_domain;

		private string m_userName;

		private SecureString m_password;
	}
}
