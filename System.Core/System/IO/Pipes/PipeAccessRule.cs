using System;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.IO.Pipes
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class PipeAccessRule : AccessRule
	{
		public PipeAccessRule(IdentityReference identity, PipeAccessRights rights, AccessControlType type)
			: base(identity, (int)rights, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		public PipeAccessRule(string identity, PipeAccessRights rights, AccessControlType type)
			: this(new NTAccount(identity), rights, type)
		{
		}

		public PipeAccessRights PipeAccessRights
		{
			get
			{
				return (PipeAccessRights)base.AccessMask;
			}
		}
	}
}
