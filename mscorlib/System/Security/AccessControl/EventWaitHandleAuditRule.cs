using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class EventWaitHandleAuditRule : AuditRule
	{
		public EventWaitHandleAuditRule(IdentityReference identity, EventWaitHandleRights eventRights, AuditFlags flags)
			: base(identity, (int)eventRights, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
			if (eventRights < EventWaitHandleRights.Modify || eventRights > EventWaitHandleRights.FullControl)
			{
				throw new ArgumentOutOfRangeException("eventRights");
			}
		}

		public EventWaitHandleRights EventWaitHandleRights
		{
			get
			{
				return (EventWaitHandleRights)base.AccessMask;
			}
		}
	}
}
