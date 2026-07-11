using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	[ComVisible(false)]
	public sealed class SemaphoreSecurity : NativeObjectSecurity
	{
		public SemaphoreSecurity()
			: base(false, ResourceType.KernelObject)
		{
		}

		public SemaphoreSecurity(string name, AccessControlSections includeSections)
			: base(false, ResourceType.KernelObject, name, includeSections)
		{
		}

		internal SemaphoreSecurity(SafeHandle handle, AccessControlSections includeSections)
			: base(false, ResourceType.KernelObject, handle, includeSections)
		{
		}

		public override Type AccessRightType
		{
			get
			{
				return typeof(SemaphoreRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(SemaphoreAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(SemaphoreAuditRule);
			}
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new SemaphoreAccessRule(identityReference, (SemaphoreRights)accessMask, type);
		}

		public void AddAccessRule(SemaphoreAccessRule rule)
		{
			base.AddAccessRule(rule);
		}

		public bool RemoveAccessRule(SemaphoreAccessRule rule)
		{
			return base.RemoveAccessRule(rule);
		}

		public void RemoveAccessRuleAll(SemaphoreAccessRule rule)
		{
			base.RemoveAccessRuleAll(rule);
		}

		public void RemoveAccessRuleSpecific(SemaphoreAccessRule rule)
		{
			base.RemoveAccessRuleSpecific(rule);
		}

		public void ResetAccessRule(SemaphoreAccessRule rule)
		{
			base.ResetAccessRule(rule);
		}

		public void SetAccessRule(SemaphoreAccessRule rule)
		{
			base.SetAccessRule(rule);
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new SemaphoreAuditRule(identityReference, (SemaphoreRights)accessMask, flags);
		}

		public void AddAuditRule(SemaphoreAuditRule rule)
		{
			base.AddAuditRule(rule);
		}

		public bool RemoveAuditRule(SemaphoreAuditRule rule)
		{
			return base.RemoveAuditRule(rule);
		}

		public void RemoveAuditRuleAll(SemaphoreAuditRule rule)
		{
			base.RemoveAuditRuleAll(rule);
		}

		public void RemoveAuditRuleSpecific(SemaphoreAuditRule rule)
		{
			base.RemoveAuditRuleSpecific(rule);
		}

		public void SetAuditRule(SemaphoreAuditRule rule)
		{
			base.SetAuditRule(rule);
		}

		internal void Persist(SafeHandle handle)
		{
			base.WriteLock();
			try
			{
				base.Persist(handle, (base.AccessRulesModified ? AccessControlSections.Access : AccessControlSections.None) | (base.AuditRulesModified ? AccessControlSections.Audit : AccessControlSections.None) | (base.OwnerModified ? AccessControlSections.Owner : AccessControlSections.None) | (base.GroupModified ? AccessControlSections.Group : AccessControlSections.None), null);
			}
			finally
			{
				base.WriteUnlock();
			}
		}
	}
}
