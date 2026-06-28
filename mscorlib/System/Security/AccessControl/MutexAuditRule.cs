using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class MutexAuditRule : AuditRule
	{
		public MutexAuditRule(IdentityReference identity, MutexRights eventRights, AuditFlags flags)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
			this.rights = eventRights;
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
