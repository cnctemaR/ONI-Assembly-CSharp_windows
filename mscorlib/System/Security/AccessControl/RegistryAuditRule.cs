using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RegistryAuditRule : AuditRule
	{
		public RegistryAuditRule(IdentityReference identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(identity, registryRights, false, inheritanceFlags, propagationFlags, flags)
		{
		}

		internal RegistryAuditRule(IdentityReference identity, RegistryRights registryRights, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: base(identity, (int)registryRights, isInherited, inheritanceFlags, propagationFlags, flags)
		{
		}

		public RegistryAuditRule(string identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
			: this(new NTAccount(identity), registryRights, inheritanceFlags, propagationFlags, flags)
		{
		}

		public RegistryRights RegistryRights
		{
			get
			{
				return (RegistryRights)base.AccessMask;
			}
		}
	}
}
