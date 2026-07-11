using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	[ComVisible(false)]
	public sealed class SemaphoreAuditRule : AuditRule
	{
		public SemaphoreAuditRule(IdentityReference identity, SemaphoreRights semaphoreRights, AuditFlags flags)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
			this.semaphoreRights = semaphoreRights;
		}

		public SemaphoreRights SemaphoreRights
		{
			get
			{
				return this.semaphoreRights;
			}
		}

		private SemaphoreRights semaphoreRights;
	}
}
