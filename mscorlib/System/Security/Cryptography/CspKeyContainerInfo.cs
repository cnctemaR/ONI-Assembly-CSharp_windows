using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CspKeyContainerInfo
	{
		public CspKeyContainerInfo(CspParameters parameters)
		{
			this._params = parameters;
			this._random = true;
		}

		public bool Accessible
		{
			get
			{
				return true;
			}
		}

		public CryptoKeySecurity CryptoKeySecurity
		{
			get
			{
				return null;
			}
		}

		public bool Exportable
		{
			get
			{
				return true;
			}
		}

		public bool HardwareDevice
		{
			get
			{
				return false;
			}
		}

		public string KeyContainerName
		{
			get
			{
				return this._params.KeyContainerName;
			}
		}

		public KeyNumber KeyNumber
		{
			get
			{
				return (KeyNumber)this._params.KeyNumber;
			}
		}

		public bool MachineKeyStore
		{
			get
			{
				return false;
			}
		}

		public bool Protected
		{
			get
			{
				return false;
			}
		}

		public string ProviderName
		{
			get
			{
				return this._params.ProviderName;
			}
		}

		public int ProviderType
		{
			get
			{
				return this._params.ProviderType;
			}
		}

		public bool RandomlyGenerated
		{
			get
			{
				return this._random;
			}
		}

		public bool Removable
		{
			get
			{
				return false;
			}
		}

		public string UniqueKeyContainerName
		{
			get
			{
				return this._params.ProviderName + "\\" + this._params.KeyContainerName;
			}
		}

		private CspParameters _params;

		internal bool _random;
	}
}
