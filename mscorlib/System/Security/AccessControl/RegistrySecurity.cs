using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RegistrySecurity : NativeObjectSecurity
	{
		public RegistrySecurity()
			: base(true, ResourceType.RegistryKey)
		{
		}

		internal RegistrySecurity(string name, AccessControlSections includeSections)
			: base(true, ResourceType.RegistryKey, name, includeSections)
		{
		}

		public override Type AccessRightType
		{
			get
			{
				return typeof(RegistryRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(RegistryAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(RegistryAuditRule);
			}
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new RegistryAccessRule(identityReference, (RegistryRights)accessMask, isInherited, inheritanceFlags, propagationFlags, type);
		}

		public void AddAccessRule(RegistryAccessRule rule)
		{
			base.AddAccessRule(rule);
		}

		public bool RemoveAccessRule(RegistryAccessRule rule)
		{
			return base.RemoveAccessRule(rule);
		}

		public void RemoveAccessRuleAll(RegistryAccessRule rule)
		{
			base.RemoveAccessRuleAll(rule);
		}

		public void RemoveAccessRuleSpecific(RegistryAccessRule rule)
		{
			base.RemoveAccessRuleSpecific(rule);
		}

		public void ResetAccessRule(RegistryAccessRule rule)
		{
			base.ResetAccessRule(rule);
		}

		public void SetAccessRule(RegistryAccessRule rule)
		{
			base.SetAccessRule(rule);
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new RegistryAuditRule(identityReference, (RegistryRights)accessMask, isInherited, inheritanceFlags, propagationFlags, flags);
		}

		public void AddAuditRule(RegistryAuditRule rule)
		{
			base.AddAuditRule(rule);
		}

		public bool RemoveAuditRule(RegistryAuditRule rule)
		{
			return base.RemoveAuditRule(rule);
		}

		public void RemoveAuditRuleAll(RegistryAuditRule rule)
		{
			base.RemoveAuditRuleAll(rule);
		}

		public void RemoveAuditRuleSpecific(RegistryAuditRule rule)
		{
			base.RemoveAuditRuleSpecific(rule);
		}

		public void SetAuditRule(RegistryAuditRule rule)
		{
			base.SetAuditRule(rule);
		}
	}
}
