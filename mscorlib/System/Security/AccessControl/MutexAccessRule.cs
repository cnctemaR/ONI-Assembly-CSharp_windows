using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class MutexAccessRule : AccessRule
	{
		public MutexAccessRule(IdentityReference identity, MutexRights eventRights, AccessControlType type)
			: base(identity, (int)eventRights, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public MutexAccessRule(string identity, MutexRights eventRights, AccessControlType type)
			: this(new NTAccount(identity), eventRights, type)
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
