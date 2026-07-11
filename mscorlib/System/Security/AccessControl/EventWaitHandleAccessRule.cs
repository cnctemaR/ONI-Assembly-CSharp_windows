using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class EventWaitHandleAccessRule : AccessRule
	{
		public EventWaitHandleAccessRule(IdentityReference identity, EventWaitHandleRights eventRights, AccessControlType type)
			: base(identity, (int)eventRights, false, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow)
		{
		}

		public EventWaitHandleAccessRule(string identity, EventWaitHandleRights eventRights, AccessControlType type)
			: this(new NTAccount(identity), eventRights, type)
		{
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
