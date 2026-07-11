using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class MutexAuditRule : AuditRule
	{
		public MutexAuditRule(IdentityReference identity, MutexRights eventRights, AuditFlags flags)
			: base(identity, (int)eventRights, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
		}

		public MutexRights MutexRights
		{
			get
			{
				return (MutexRights)base.AccessMask;
			}
		}
	}
}
