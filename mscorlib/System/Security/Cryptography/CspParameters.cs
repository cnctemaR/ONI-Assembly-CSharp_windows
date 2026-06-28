using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CspParameters
	{
		public CspParameters()
			: this(1)
		{
		}

		public CspParameters(int dwTypeIn)
			: this(dwTypeIn, null)
		{
		}

		public CspParameters(int dwTypeIn, string strProviderNameIn)
			: this(dwTypeIn, null, null)
		{
		}

		public CspParameters(int dwTypeIn, string strProviderNameIn, string strContainerNameIn)
		{
			this.ProviderType = dwTypeIn;
			this.ProviderName = strProviderNameIn;
			this.KeyContainerName = strContainerNameIn;
			this.KeyNumber = -1;
		}

		public CspParameters(int providerType, string providerName, string keyContainerName, CryptoKeySecurity cryptoKeySecurity, IntPtr parentWindowHandle)
			: this(providerType, providerName, keyContainerName)
		{
			if (cryptoKeySecurity != null)
			{
				this.CryptoKeySecurity = cryptoKeySecurity;
			}
			this._windowHandle = parentWindowHandle;
		}

		public CspParameters(int providerType, string providerName, string keyContainerName, CryptoKeySecurity cryptoKeySecurity, SecureString keyPassword)
			: this(providerType, providerName, keyContainerName)
		{
			if (cryptoKeySecurity != null)
			{
				this.CryptoKeySecurity = cryptoKeySecurity;
			}
			this._password = keyPassword;
		}

		public CspProviderFlags Flags
		{
			get
			{
				return this._Flags;
			}
			set
			{
				this._Flags = value;
			}
		}

		[MonoTODO("access control isn't implemented")]
		public CryptoKeySecurity CryptoKeySecurity
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SecureString KeyPassword
		{
			get
			{
				return this._password;
			}
			set
			{
				this._password = value;
			}
		}

		public IntPtr ParentWindowHandle
		{
			get
			{
				return this._windowHandle;
			}
			set
			{
				this._windowHandle = value;
			}
		}

		private CspProviderFlags _Flags;

		public string KeyContainerName;

		public int KeyNumber;

		public string ProviderName;

		public int ProviderType;

		private SecureString _password;

		private IntPtr _windowHandle;
	}
}
