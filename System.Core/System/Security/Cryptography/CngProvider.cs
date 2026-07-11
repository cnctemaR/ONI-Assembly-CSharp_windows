using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class CngProvider : IEquatable<CngProvider>
	{
		public CngProvider(string provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (provider.Length == 0)
			{
				throw new ArgumentException(global::SR.GetString("The provider name '{0}' is invalid.", new object[] { provider }), "provider");
			}
			this.m_provider = provider;
		}

		public string Provider
		{
			get
			{
				return this.m_provider;
			}
		}

		public static bool operator ==(CngProvider left, CngProvider right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(CngProvider left, CngProvider right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngProvider);
		}

		public bool Equals(CngProvider other)
		{
			return other != null && this.m_provider.Equals(other.Provider);
		}

		public override int GetHashCode()
		{
			return this.m_provider.GetHashCode();
		}

		public override string ToString()
		{
			return this.m_provider.ToString();
		}

		public static CngProvider MicrosoftSmartCardKeyStorageProvider
		{
			get
			{
				if (CngProvider.s_msSmartCardKsp == null)
				{
					CngProvider.s_msSmartCardKsp = new CngProvider("Microsoft Smart Card Key Storage Provider");
				}
				return CngProvider.s_msSmartCardKsp;
			}
		}

		public static CngProvider MicrosoftSoftwareKeyStorageProvider
		{
			get
			{
				if (CngProvider.s_msSoftwareKsp == null)
				{
					CngProvider.s_msSoftwareKsp = new CngProvider("Microsoft Software Key Storage Provider");
				}
				return CngProvider.s_msSoftwareKsp;
			}
		}

		private static volatile CngProvider s_msSmartCardKsp;

		private static volatile CngProvider s_msSoftwareKsp;

		private string m_provider;
	}
}
