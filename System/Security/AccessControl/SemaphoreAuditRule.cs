using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	[ComVisible(false)]
	public sealed class SemaphoreAuditRule : AuditRule
	{
		public SemaphoreAuditRule(IdentityReference identity, SemaphoreRights eventRights, AuditFlags flags)
			: base(identity, (int)eventRights, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
		}

		public SemaphoreRights SemaphoreRights
		{
			get
			{
				return (SemaphoreRights)base.AccessMask;
			}
		}
	}
}
