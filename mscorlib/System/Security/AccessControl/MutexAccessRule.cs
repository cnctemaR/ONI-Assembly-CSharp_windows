using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class MutexAccessRule : AccessRule
	{
		public MutexAccessRule(IdentityReference identity, MutexRights eventRights, AccessControlType type)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
			this.rights = eventRights;
		}

		public MutexAccessRule(string identity, MutexRights eventRights, AccessControlType type)
			: this(new SecurityIdentifier(identity), eventRights, type)
		{
		}

		public MutexRights MutexRights
		{
			get
			{
				return this.rights;
			}
		}

		private MutexRights rights;
	}
}
