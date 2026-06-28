using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RegistryAuditRule : AuditRule
	{
		public RegistryAuditRule(IdentityReference identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: base(identity, 0, false, inheritanceFlags, propagationFlags, flags)
		{
			this.rights = registryRights;
		}

		public RegistryAuditRule(string identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(new SecurityIdentifier(identity), registryRights, inheritanceFlags, propagationFlags, flags)
		{
		}

		public RegistryRights RegistryRights
		{
			get
			{
				return this.rights;
			}
		}

		private RegistryRights rights;
	}
}
