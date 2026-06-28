using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CryptoKeyAccessRule : AccessRule
	{
		public CryptoKeyAccessRule(IdentityReference identity, CryptoKeyRights cryptoKeyRights, AccessControlType type)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow)
		{
			this.rights = cryptoKeyRights;
		}

		public CryptoKeyAccessRule(string identity, CryptoKeyRights cryptoKeyRights, AccessControlType type)
			: this(new SecurityIdentifier(identity), cryptoKeyRights, type)
		{
		}

		public CryptoKeyRights CryptoKeyRights
		{
			get
			{
				return this.rights;
			}
		}

		private CryptoKeyRights rights;
	}
}
