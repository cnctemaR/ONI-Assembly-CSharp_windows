using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace System.IO.Pipes
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class PipeSecurity : NativeObjectSecurity
	{
		public PipeSecurity()
			: base(false, ResourceType.FileObject)
		{
		}

		internal PipeSecurity(SafeHandle handle, AccessControlSections includeSections)
			: base(false, ResourceType.FileObject, handle, includeSections)
		{
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

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new PipeAccessRule(identityReference, (PipeAccessRights)accessMask, type);
		}

		public void AddAccessRule(PipeAccessRule rule)
		{
			base.AddAccessRule(rule);
		}

		public void AddAuditRule(PipeAuditRule rule)
		{
			base.AddAuditRule(rule);
		}

		public sealed override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new PipeAuditRule(identityReference, (PipeAccessRights)accessMask, flags);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		protected internal void Persist(SafeHandle handle)
		{
			base.WriteLock();
			try
			{
				base.Persist(handle, base.AccessControlSectionsModified, null);
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		protected internal void Persist(string name)
		{
			base.WriteLock();
			try
			{
				base.Persist(name, base.AccessControlSectionsModified, null);
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		public bool RemoveAccessRule(PipeAccessRule rule)
		{
			return base.RemoveAccessRule(rule);
		}

		public void RemoveAccessRuleSpecific(PipeAccessRule rule)
		{
			base.RemoveAccessRuleSpecific(rule);
		}

		public bool RemoveAuditRule(PipeAuditRule rule)
		{
			return base.RemoveAuditRule(rule);
		}

		public void RemoveAuditRuleAll(PipeAuditRule rule)
		{
			base.RemoveAuditRuleAll(rule);
		}

		public void RemoveAuditRuleSpecific(PipeAuditRule rule)
		{
			base.RemoveAuditRuleSpecific(rule);
		}

		public void ResetAccessRule(PipeAccessRule rule)
		{
			base.ResetAccessRule(rule);
		}

		public void SetAccessRule(PipeAccessRule rule)
		{
			base.SetAccessRule(rule);
		}

		public void SetAuditRule(PipeAuditRule rule)
		{
			base.SetAuditRule(rule);
		}
	}
}
