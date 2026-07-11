using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RegistryAccessRule : AccessRule
	{
		public RegistryAccessRule(IdentityReference identity, RegistryRights registryRights, AccessControlType type)
			: this(identity, registryRights, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public RegistryAccessRule(string identity, RegistryRights registryRights, AccessControlType type)
			: this(new SecurityIdentifier(identity), registryRights, type)
		{
		}

		public RegistryAccessRule(IdentityReference identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: base(identity, 0, false, inheritanceFlags, propagationFlags, type)
		{
			this.rights = registryRights;
		}

		public RegistryAccessRule(string identity, RegistryRights registryRights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
			: this(new SecurityIdentifier(identity), registryRights, inheritanceFlags, propagationFlags, type)
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
