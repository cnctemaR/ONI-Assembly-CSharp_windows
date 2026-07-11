using System;
using System.Globalization;

namespace System.Net
{
	internal class CredentialHostKey
	{
		internal CredentialHostKey(string host, int port, string authenticationType)
		{
			this.Host = host;
			this.Port = port;
			this.AuthenticationType = authenticationType;
		}

		internal bool Match(string host, int port, string authenticationType)
		{
			return host != null && authenticationType != null && string.Compare(authenticationType, this.AuthenticationType, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(this.Host, host, StringComparison.OrdinalIgnoreCase) == 0 && port == this.Port;
		}

		public override int GetHashCode()
		{
			if (!this.m_ComputedHashCode)
			{
				this.m_HashCode = this.AuthenticationType.ToUpperInvariant().GetHashCode() + this.Host.ToUpperInvariant().GetHashCode() + this.Port.GetHashCode();
				this.m_ComputedHashCode = true;
			}
			return this.m_HashCode;
		}

		public override bool Equals(object comparand)
		{
			CredentialHostKey credentialHostKey = comparand as CredentialHostKey;
			return comparand != null && (string.Compare(this.AuthenticationType, credentialHostKey.AuthenticationType, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(this.Host, credentialHostKey.Host, StringComparison.OrdinalIgnoreCase) == 0) && this.Port == credentialHostKey.Port;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"[",
				this.Host.Length.ToString(NumberFormatInfo.InvariantInfo),
				"]:",
				this.Host,
				":",
				this.Port.ToString(NumberFormatInfo.InvariantInfo),
				":",
				ValidationHelper.ToString(this.AuthenticationType)
			});
		}

		internal string Host;

		internal string AuthenticationType;

		internal int Port;

		private int m_HashCode;

		private bool m_ComputedHashCode;
	}
}
