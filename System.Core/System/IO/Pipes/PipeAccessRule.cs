using System;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.IO.Pipes
{
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public sealed class PipeAccessRule : AccessRule
	{
		[MonoNotSupported("ACL is not supported in Mono")]
		public PipeAccessRule(IdentityReference identity, PipeAccessRights rights, AccessControlType type)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, type)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public PipeAccessRule(string identity, PipeAccessRights rights, AccessControlType type)
			: this(null, rights, type)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public PipeAccessRights PipeAccessRights { get; private set; }
	}
}
