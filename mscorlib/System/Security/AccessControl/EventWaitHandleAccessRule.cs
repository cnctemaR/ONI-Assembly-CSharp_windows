using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class EventWaitHandleAccessRule : AccessRule
	{
		public EventWaitHandleAccessRule(IdentityReference identity, EventWaitHandleRights eventRights, AccessControlType type)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow)
		{
			this.rights = eventRights;
		}

		public EventWaitHandleAccessRule(string identity, EventWaitHandleRights eventRights, AccessControlType type)
			: this(new SecurityIdentifier(identity), eventRights, type)
		{
		}

		public EventWaitHandleRights EventWaitHandleRights
		{
			get
			{
				return this.rights;
			}
		}

		private EventWaitHandleRights rights;
	}
}
