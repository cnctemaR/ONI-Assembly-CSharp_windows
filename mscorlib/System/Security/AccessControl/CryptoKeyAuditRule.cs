using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CryptoKeyAuditRule : AuditRule
	{
		public CryptoKeyAuditRule(IdentityReference identity, CryptoKeyRights cryptoKeyRights, AuditFlags flags)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
			this.rights = cryptoKeyRights;
		}

		public CryptoKeyAuditRule(string identity, CryptoKeyRights cryptoKeyRights, AuditFlags flags)
			: this(new SecurityIdentifier(identity), cryptoKeyRights, flags)
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
