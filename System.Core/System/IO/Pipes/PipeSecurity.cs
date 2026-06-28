using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.IO.Pipes
{
	[MonoNotSupported("ACL is not supported in Mono")]
	[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.HostProtectionPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nResources=\"None\"/>\n</PermissionSet>\n")]
	public class PipeSecurity : NativeObjectSecurity
	{
		[MonoNotSupported("ACL is not supported in Mono")]
		public PipeSecurity()
			: base(false, ResourceType.FileObject)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		public override Type AccessRightType
		{
			get
			{
				return typeof(PipeAccessRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(PipeAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(PipeAuditRule);
			}
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void AddAccessRule(PipeAccessRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void AddAuditRule(PipeAuditRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public sealed override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		protected internal void Persist(SafeHandle handle)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		protected internal void Persist(string name)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public bool RemoveAccessRule(PipeAccessRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void RemoveAccessRuleSpecific(PipeAccessRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public bool RemoveAuditRule(PipeAuditRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void RemoveAuditRuleAll(PipeAuditRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void RemoveAuditRuleSpecific(PipeAuditRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void ResetAccessRule(PipeAccessRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void SetAccessRule(PipeAccessRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}

		[MonoNotSupported("ACL is not supported in Mono")]
		public void SetAuditRule(PipeAuditRule rule)
		{
			throw new NotImplementedException("ACL is not supported in Mono");
		}
	}
}
